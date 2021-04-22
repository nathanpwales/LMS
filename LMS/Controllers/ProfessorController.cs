using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            // Query Enrolled students
            // Join students to get student info
            // Join class, course, and semester to identify this class based on input parameters.
            var query =
                from e in db.Enrolled
                join s in db.Student on e.SId equals s.SId
                join cl in db.Class on e.ClassId equals cl.ClassId
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year
                select new
                {
                    fname = s.FName,
                    lname = s.LName,
                    uid = s.SId,
                    dob = s.Dob,
                    grade = e.Grade
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            // Query Assignments in this category
            // Joins necessary because Assignments belong to AssignmentCategory belong to Class identified by Semester and Course.
            var query =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join ag in db.Assignment on ac.AssnCategoryId equals ag.AssnCategoryId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year && (ac.Name == category || category == null)
                select new
                {
                    aname = ag.Name,
                    cname = ac.Name,
                    due = ag.DueDate,
                    submissions = ag.Submission.Count()
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            // Query the assignment Categories for this class.
            // AssignmentCategory belongs to a class identified by Semester (season/year) and Course (subject/num).
            var query =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year
                select new
                {
                    name = ac.Name,
                    weight = ac.Weight
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            // Query table to check if assignment category with the same name already exists in the same class.
            var query =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year && ac.Name == category
                select ac.Name;

            // If the above query found any results, then there already exists an AssignmentCategory
            // with the same name in the same class.
            if (query.ToArray().Length > 0)
            {
                return Json(new { success = false });
            }

            // Grab the class ID for the class specified by the input parameters.
            var queryForClassID =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year
                select cl.ClassId;

            uint classID = queryForClassID.First();

            // Create the new Assignment category with the specified name/weight and the ClassID from above.
            AssignmentCategory newAssnCat = new AssignmentCategory();
            newAssnCat.Name = category;
            newAssnCat.Weight = (uint)catweight;
            newAssnCat.ClassId = classID;

            // only return true if the addition saves successfully.
            try
            {
                db.AssignmentCategory.Add(newAssnCat);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            // return false if the try failed. Changes were not saved successfully to db.
            return Json(new { success = false });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            // Query to check if an assignment with the same name already exists in the same assignment category for this class
            var query =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join ag in db.Assignment on ac.AssnCategoryId equals ag.AssnCategoryId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year && ac.Name == category && ag.Name == asgname
                select ac.Name;

            // if the above query found a duplicate assignment, return false.
            if (query.ToArray().Length > 0)
            {
                return Json(new { success = false });
            }

            // query for the correct Assignment Category ID with the given class input parameters
            var queryForAssnCatID =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year && ac.Name == category
                select new
                {
                    category = ac.AssnCategoryId,
                    classID = ac.ClassId
                };

            uint categoryID = queryForAssnCatID.First().category;
            
            // Create the new assignment with the name/maxpoints/dueDate given as input parameters.
            // CategoryID from above query.
            Assignment newAssn = new Assignment();
            newAssn.Name = asgname;
            newAssn.MaxPoints = (uint)asgpoints;
            newAssn.DueDate = asgdue;
            newAssn.Contents = asgcontents;
            newAssn.AssnCategoryId = categoryID;
            db.Assignment.Add(newAssn);

            

            // only return true if the addition saves successfully.
            try
            {
                
                db.SaveChanges();
                UpdateAllGradesForClass(queryForAssnCatID.First().classID);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            // return false if the try failed.Changes were not saved successfully to db.
            return Json(new { success = false });
        }

        


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            // Query Submissions
            // Join Students to get student info for submission
            // Join Assignment, AssignmentCategory, Class, Semester, Course
            // to identify class and Assignment given by input parameters
            var query =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join ag in db.Assignment on ac.AssnCategoryId equals ag.AssnCategoryId
                join sb in db.Submission on ag.AssnId equals sb.AssnId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                join st in db.Student on sb.SId equals st.SId
                where cr.DeptAbbr == subject & cr.Number == num & sm.Season == season & sm.Year == year & ac.Name == category & ag.Name == asgname
                select new
                {
                    fname = st.FName,
                    lname = st.LName,
                    uid = st.SId,
                    time = sb.Time,
                    score = sb.Score
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            // Query to select this submission specified by input parameters.
            var query =
                from st in db.Student
                join e in db.Enrolled on st.SId equals e.SId
                join cl in db.Class on e.ClassId equals cl.ClassId
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join ag in db.Assignment on ac.AssnCategoryId equals ag.AssnCategoryId
                join sb in db.Submission on ag.AssnId equals sb.AssnId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cr.DeptAbbr == subject && cr.Number == num && sm.Season == season && sm.Year == year && ac.Name == category && ag.Name == asgname && st.SId == uid
                select new
                {
                    subm = sb,
                    cID = cl.ClassId
                };



            // if this submission was not found, or if duplicate submissions are found, return false.
            if (query.ToArray().Length != 1)
            {
                return Json(new { success = false });
            }

            

            // only return true if changes save successfully.
            try
            {
                // Update score to score given as method parameter
                query.First().subm.Score = (uint)score;
                db.SaveChanges();
                UpdateOverallGrade(query.First().cID, uid);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error Grading assignment {" + asgname + "} with error: " + e.Message);
            }

            // return false if the try failed. The changes were not saved successfully.
            return Json(new { success = false });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            // Query the db for classes taught by this professor.
            // Join Course and Semester for output values.
            var query =
                from cl in db.Class
                join cr in db.Course on cl.CourseId equals cr.CourseId
                join sm in db.Semester on cl.SemesterId equals sm.SemesterId
                where cl.Professor == uid
                select new
                {
                    subject = cr.DeptAbbr,
                    number = cr.Number,
                    name = cr.Name,
                    season = sm.Season,
                    year = sm.Year
                };

            return Json(query.ToArray());
        }

        private void UpdateOverallGrade(uint classId, string uid)
        {
            var query =

                from a in db.Assignment
                join sb in db.Submission on a.AssnId equals sb.AssnId into sub

                from q1 in sub.DefaultIfEmpty()
                join ac in db.AssignmentCategory on a.AssnCategoryId equals ac.AssnCategoryId
                join e in db.Enrolled on ac.ClassId equals e.ClassId
                where e.SId == uid & ac.ClassId == classId
                select new
                {
                    enrollment = e,
                    catName = ac.Name,
                    catWeight = ac.Weight,
                    maxPoints = a.MaxPoints,
                    points = q1 == null ? null : (uint?)q1.Score,

                };

            Dictionary<string, uint> totalCatPoints = new Dictionary<string, uint>();
            Dictionary<string, uint?> earnedCatPoints = new Dictionary<string, uint?>();
            Dictionary<string, uint> catWeights = new Dictionary<string, uint>();

            uint catSum = 0;

            foreach (var v in query)
            {
                if (!catWeights.ContainsKey(v.catName))
                {
                    catWeights.Add(v.catName, v.catWeight);
                    catSum += v.catWeight;
                }

                if (totalCatPoints.ContainsKey(v.catName))
                {
                    totalCatPoints[v.catName] += v.maxPoints;
                }
                else
                {
                    totalCatPoints.Add(v.catName, v.maxPoints);
                }

                if (earnedCatPoints.ContainsKey(v.catName))
                {
                    if (v.points != null)
                        earnedCatPoints[v.catName] += v.points;
                }
                else
                {
                    if (v.points != null)
                        earnedCatPoints.Add(v.catName, v.points);
                    else
                        earnedCatPoints.Add(v.catName, 0);

                }

            }

            Dictionary<string, decimal?> catPercents = new Dictionary<string, decimal?>();

            foreach (KeyValuePair<string, uint> v in totalCatPoints)
            {
                
                catPercents.Add(v.Key, (decimal?)earnedCatPoints[v.Key] / (decimal?)totalCatPoints[v.Key]);

            }

            decimal? sum = 0;

            foreach (KeyValuePair<string, decimal?> v in catPercents)
            {
                
                sum += v.Value * (catWeights[v.Key] / (decimal?)catSum);
                    
            }

            sum *= 100;
            string letterGrade = "E";

            System.Diagnostics.Debug.WriteLine("yo");
            System.Diagnostics.Debug.WriteLine(sum);
            if (sum >= 93)
                letterGrade = "A";
            else if (sum >= 90)
                letterGrade = "A-";
            else if (sum >= 87)
                letterGrade = "B+";
            else if (sum >= 83)
                letterGrade = "B";
            else if (sum >= 80)
                letterGrade = "B-";
            else if (sum >= 77)
                letterGrade = "C+";
            else if (sum >= 73)
                letterGrade = "C";
            else if (sum >= 70)
                letterGrade = "C-";
            else if (sum >= 67)
                letterGrade = "D+";
            else if (sum >= 63)
                letterGrade = "D";
            else if (sum >= 60)
                letterGrade = "D-";
            else
                letterGrade = "E";

            Enrolled newEnroll = query.First().enrollment;

            newEnroll.Grade = letterGrade;

            // db.SaveChanges called after helper method is called.


        }

        private void UpdateAllGradesForClass(uint classID)
        {
            var query =
                from e in db.Enrolled
                where e.ClassId == classID
                select e.SId;

            foreach (string uid in query)
            {
                UpdateOverallGrade(classID, uid);
            }
        }

        /*******End code to modify********/

    }
}