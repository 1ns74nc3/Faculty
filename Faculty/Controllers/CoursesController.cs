using Faculty.Logic.DB;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses/DisplayCourses
        public ActionResult DisplayCourses()
        {
            CoursesManager coursesManager = new CoursesManager();
            var courses = coursesManager.GetCourses();
            return View(courses);
        }

        // GET: Courses/CourseInfo
        public ActionResult CourseInfo(int id)
        {
            CoursesManager coursesManager = new CoursesManager();
            var course = coursesManager.GetSpecificCourse(id);
            ViewBag.Lector = coursesManager.GetLectorInfo(course);
            string currentUserId = User.Identity.GetUserId();
            ViewBag.UserIsSignedToCourse = coursesManager.UserIsSignedToCourse(id, currentUserId);
            return View(course);
        }

        // GET: Courses/SignToCourse
        public ActionResult SignOrQuitCourse(int id, bool userIsOnCourse)
        {
            CoursesManager coursesManager = new CoursesManager();
            string currentUserId = User.Identity.GetUserId();
            ViewBag.RegistrationResult = "";
            if (userIsOnCourse)
            {
                ViewBag.RegistrationResult = coursesManager.RemoveUserFromCourse(id, currentUserId);
            }
            else
            {
                ViewBag.RegistrationResult = coursesManager.AddUserToCourse(id, currentUserId);
            }

            return View();
        }
    }
}