using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses/CourseInfo
        public ActionResult CourseInfo(int courseId)
        {
            CoursesManager coursesManager = new CoursesManager();
            var course = coursesManager.GetSpecificCourse(courseId);
            ViewBag.Lector = coursesManager.GetLectorInfo(course);
            string currentUserId = User.Identity.GetUserId();
            ViewBag.UserIsSignedToCourse = coursesManager.UserIsSignedToCourse(courseId, currentUserId);
            ViewBag.UserIsLector = false;
            ViewBag.CourseStatusEnded = false;

            if(course.CourseStatus==Course.Status.Ended)
                ViewBag.CourseStatusEnded = true;

            if (course.LectorId == currentUserId)
                ViewBag.UserIsLectorOfThisCourse = true;

            return View(course);
        }

        // GET: Courses/DisplayCourses
        public ActionResult DisplayCourses()
        {
            CoursesManager coursesManager = new CoursesManager();
            UsersManager usersManager = new UsersManager();
            var coursesList = coursesManager.GetCourses();
            List<CourseViewModel> courses = new List<CourseViewModel>();
            if (coursesList != null)
            {
                foreach (var item in coursesList)
                {
                    var lectorData = usersManager.GetSpecificUser(item.LectorId);
                    var lector = "None";
                    if (lectorData != null)
                        lector = string.Concat(lectorData.FirstName, " ", lectorData.LastName);
                    courses.Add(new CourseViewModel(item.Id, item.CourseName, item.StartDate.ToShortDateString(), item.EndDate.ToShortDateString(), 
                        item.Theme, item.CourseStatus.ToString(), item.Users.Count, lector));
                }
            }
            

            return View(courses);
        }

        // GET: Courses/SignToCourse
        [Authorize]
        public ActionResult SignOrQuitCourse(int courseId, bool userIsOnCourse)
        {
            CoursesManager coursesManager = new CoursesManager();
            JournalsManager journalsManager = new JournalsManager();
            string currentUserId = User.Identity.GetUserId();
            ViewBag.RegistrationResult = "";
            
            if (userIsOnCourse)
            {
                ViewBag.RegistrationResult = coursesManager.RemoveUserFromCourse(courseId, currentUserId);
                journalsManager.RemoveJournalForUser(currentUserId);
            }
            else
            {
                ViewBag.RegistrationResult = coursesManager.AddUserToCourse(courseId, currentUserId);
                journalsManager.AddJournalForUser(courseId, currentUserId);
            }

            return View();
        }
    }
}