using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {

            var query =
                from e in db.Enrolled
                join cl in db.Class on e.ClassId equals cl.ClassId
                join c in db.Course on cl.CourseId equals c.CourseId
                join s in db.Semester on cl.SemesterId equals s.SemesterId
                where e.SId == uid
                select new
                { 
                subject = c.DeptAbbr,
                number = c.Number,
                name = c.Name,
                season = s.Season,
                year = s.Year,
                grade = e.Grade
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {

            var query =

               
                from a in db.Assignment
                join sb in db.Submission on a.AssnId equals sb.AssnId into sub

                from q1 in sub.DefaultIfEmpty()
                join ac in db.AssignmentCategory on a.AssnCategoryId equals ac.AssnCategoryId
                join cl in db.Class on ac.ClassId equals cl.ClassId
                join c in db.Course on cl.CourseId equals c.CourseId
                join e in db.Enrolled on cl.ClassId equals e.ClassId
                join s in db.Semester on cl.SemesterId equals s.SemesterId

                where s.Year == year & s.Season == season & c.DeptAbbr == subject & c.Number == num & e.SId == uid
                select new
                {
                    aname = a.Name,
                    cname = ac.Name,
                    due = a.DueDate,
                    score = q1 == null ? null : (decimal?)q1.Score
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {


            var query =
                from s in db.Semester
                join cl in db.Class on s.SemesterId equals cl.SemesterId
                join c in db.Course on cl.CourseId equals c.CourseId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join a in db.Assignment on ac.AssnCategoryId equals a.AssnCategoryId
                where s.Season == season & s.Year == year & c.DeptAbbr == subject & c.Number == num & ac.Name == category & a.Name == asgname
                select a.AssnId;

            

            //doesn't work if the is a '#' sign in the assignment name....
            Submission newSub = new Submission();
            newSub.SId = uid;
            newSub.Time = DateTime.Now;
            newSub.Score = 0;
            newSub.Contents = contents;
            newSub.AssnId = query.ToArray()[0];

            

            try
            {
                db.Submission.Add(newSub);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                db.Submission.Remove(newSub);
                var query2 =
                      from s in db.Semester
                      join cl in db.Class on s.SemesterId equals cl.SemesterId
                      join c in db.Course on cl.CourseId equals c.CourseId
                      join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                      join a in db.Assignment on ac.AssnCategoryId equals a.AssnCategoryId
                      join sb in db.Submission on a.AssnId equals sb.AssnId
                      where s.Season == season & s.Year == year & c.DeptAbbr == subject & c.Number == num & ac.Name == category & a.Name == asgname & sb.SId == uid
                      select sb;

                foreach(Submission sub in query2)
                {
                    sub.Time = DateTime.Now;
                    sub.Contents = contents;
                }

                db.SaveChanges();

            }




            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {

            return Json(new { success = false });
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {

            return Json(null);
        }

        /*******End code to modify********/

    }
}