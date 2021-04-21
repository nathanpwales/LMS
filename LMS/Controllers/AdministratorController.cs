using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {

            var query =
                  from cor in db.Course
                  where cor.DeptAbbr == subject
                  select new
                  {
                      number = cor.Number,
                      name = cor.Name
                  };



            return Json(query.ToArray());
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var query =
                from p in db.Professor
                where p.DeptAbbr == subject
                select new
                {
                    lname = p.LName,
                    fname = p.FName,
                    uid = p.UId
                };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            var query =
                from c in db.Course
                where c.DeptAbbr == subject & c.Number == number
                select c.Name;

            if(query.ToArray().Length > 0)
            {
                return Json(new { success = false });
            }

            Course newCourse = new Course();
            newCourse.DeptAbbr = subject;
            newCourse.Number = (uint)number;
            newCourse.Name = name;
            db.Course.Add(newCourse);
            db.SaveChanges();

            return Json(new { success = true });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {

            //Check if the course already exists that semester
            var query =
                from c in db.Course
                join cl in db.Class on c.CourseId equals cl.CourseId
                join s in db.Semester on cl.SemesterId equals s.SemesterId
                where c.DeptAbbr == subject & c.Number == number & s.Season == season & s.Year == year
                select c.Name;


            if (query.ToArray().Length > 0)
            {
                return Json(new { success = false });
            }

            //check if the course overlaps with another course
           var query2 =
                from cl in db.Class
                join s in db.Semester on cl.SemesterId equals s.SemesterId
                where s.Season == season & s.Year == year & cl.Location == location & 
                ((TimeSpan.Compare(cl.Start, end.TimeOfDay) <= 0 & TimeSpan.Compare(cl.Start, start.TimeOfDay) >= 0) | (TimeSpan.Compare(cl.End, end.TimeOfDay) <= 0 & TimeSpan.Compare(cl.End, start.TimeOfDay) >= 0))
                select cl.Course;


            if (query2.ToArray().Length > 0)
            {
                return Json(new { success = false });
            }

            //find the semester id
            uint sem = 0;

            var query3 =
                 (from s in db.Semester
                  where s.Season == season & s.Year == year
                  select s.SemesterId).Take(1);

            if (query3.ToArray().Length == 0)
            {
                // if semester id doesn't exist, create it (this was due to our poor implementation of the database)
                Semester nSem = new Semester();
                nSem.Season = season;
                nSem.Year = (uint)year;
                db.Semester.Add(nSem);
                db.SaveChanges();

                var query3_5 =
                    (from s in db.Semester
                    where s.Season == season & s.Year == year
                    select s.SemesterId).Take(1);


                sem = query3_5.ToArray()[0];

            }
            else
            {
                sem = query3.ToArray()[0];
            }

            //find the courseID (this assumes it exists)
            var query4 =
                from c in db.Course
                where c.DeptAbbr == subject & c.Number == number
                select c.CourseId;


            //create and add the class
            Class newClass = new Class();
            newClass.Start = start.TimeOfDay;
            newClass.End = end.TimeOfDay;
            newClass.Location = location;
            newClass.SemesterId = sem;
            newClass.CourseId = query4.ToArray()[0];
            newClass.Professor = instructor;
            db.Class.Add(newClass);
            db.SaveChanges();

            return Json(new { success = true });
        }


        /*******End code to modify********/

    }
}