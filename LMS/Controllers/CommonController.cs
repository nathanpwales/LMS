using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        /*******Begin code to modify********/

        // TODO: Uncomment and change 'X' after you have scaffoled


        protected Team78LMSContext db;

        public CommonController()
        {
            db = new Team78LMSContext();
        }


        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */

        // TODO: Uncomment and change 'X' after you have scaffoled

        public void UseLMSContext(Team78LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            // TODO: Do not return this hard-coded array.


            //basic query to get all the departments
            var query =
            from dep in db.Department
            select new
            {
                name = dep.Name,
                subject = dep.Abbr
            };



            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {

            // get every department
            var query =
                from d in db.Department
                select d;
            
            // list to be turned into json
            List<object> courses = new List<object>();

            foreach (Department d in query)
            {
                //for each department, grab all of the classes in it
                var query2 =
                    from c in db.Course
                    where c.DeptAbbr == d.Abbr
                    select new
                    {
                        number = c.Number,
                        cname = c.Name
                    };

                //add to result list the abbr, the name, and the list of courses
                courses.Add(new { subject = d.Abbr, dname = d.Name, courses = query2.ToArray() });

            }

            //return the result
            return Json(courses.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            //simple query to get offerings of the classes
            var query =
                from c in db.Course
                join cl in db.Class on c.CourseId equals cl.CourseId
                join s in db.Semester on cl.SemesterId equals s.SemesterId
                join p in db.Professor on cl.Professor equals p.UId
                where c.DeptAbbr == subject & c.Number == number
                select new
                {
                    season = s.Season,
                    year = s.Year,
                    location = cl.Location,
                    start = cl.Start,
                    end = cl.End,
                    fname = p.FName,
                    lname = p.LName
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            //simple query to get the assignment contents
            var query =
                from s in db.Semester
                join cl in db.Class on s.SemesterId equals cl.SemesterId
                join c in db.Course on cl.CourseId equals c.CourseId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join a in db.Assignment on ac.AssnCategoryId equals a.AssnCategoryId
                where s.Season == season & s.Year == year & c.DeptAbbr == subject & c.Number == num & ac.Name == category & a.Name == asgname
                select a.Contents;
            return Content(query.FirstOrDefault());
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            //simple query to get contents of submission
            var query =
                from sb in db.Submission
                join a in db.Assignment on sb.AssnId equals a.AssnId
                join ac in db.AssignmentCategory on a.AssnCategoryId equals ac.AssnCategoryId
                join cl in db.Class on ac.ClassId equals cl.ClassId
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year && ac.Name == category && a.Name == asgname && sb.SId == uid
                select sb.Contents;

            string content = "";

            // if we have content grab it
            if (query.Count() == 1)
            {
                content = query.First();
            }

            return Content(content);
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {

            //query to see if it is a student
            var queryStudents =
                from s in db.Student
                join d in db.Department on s.Major equals d.Abbr
                where s.SId == uid
                select new
                {
                    fname = s.FName,
                    lname = s.LName,
                    uid = s.SId,
                    department = d.Name
                };
            //if student, return
            if (queryStudents.Count() > 0)
                return Json(queryStudents.First());

            else
            {
                //query to see if it is a professor
                var queryProfessor =
                    from p in db.Professor
                    join d in db.Department on p.DeptAbbr equals d.Abbr
                    where p.UId == uid
                    select new
                    {
                        fname = p.FName,
                        lname = p.LName,
                        uid = p.UId,
                        department = d.Name
                    };
                //if its a professor, retuurn
                if (queryProfessor.Count() > 0)
                    return Json(queryProfessor.First());

                //query to see if admin
                else
                {
                    var queryAdmin =
                        from a in db.Admin
                        where a.UId == uid
                        select new
                        {
                            fname = a.FName,
                            lname = a.LName,
                            uid = a.UId
                        };
                    //return if admin
                    if (queryAdmin.Count() > 0)
                        return Json(queryAdmin.First());
                }
            }

            //it failed
            return Json(new { success = false });
        }


        /*******End code to modify********/

    }
}