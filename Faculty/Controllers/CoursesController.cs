using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class CoursesController : Controller
    {
        private UsersManager usersManager;
        private JournalsManager journalsManager;
        private CoursesManager coursesManager;

        public CoursesController()
        {
            usersManager = new UsersManager();
            journalsManager = new JournalsManager();
            coursesManager = new CoursesManager();
        }

        //Display course information
        // GET: /Courses/CourseInfo
        public ActionResult CourseInfo(int? courseId)
        {
            if (courseId == null)
                return View("Error");
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
            else
                ViewBag.UserIsLectorOfThisCourse = false;

            return View(course);
        }

        //Display all courses with filters
        // GET: /Courses/DisplayCourses
        public ActionResult DisplayCourses(string currentFilter, string statusFilter, string themeFilter, string lectorFilter, int? page)
        {
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentStatusFilter = statusFilter;
            ViewBag.CurrentThemeFilter = themeFilter;
            ViewBag.CurrentLectorFilter = lectorFilter;
            ViewBag.Themes = new SelectList(coursesManager.GetAllThemes(themeFilter));
            ViewBag.Status = new SelectList(new List<string> { "All", "Unknown", "Upcoming", "Active", "Ended" });
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var coursesList = coursesManager.GetCourses();
            if (Request.HttpMethod == "POST")
            {
                coursesList = coursesManager.GetSortedCourses(currentFilter, statusFilter, themeFilter, lectorFilter, null, coursesList);
                var coursesPost = CourseViewModel.GetCoursesList(coursesList, 1);
                
                ViewBag.Lectors = new SelectList(usersManager.GetAllLectors(
                    coursesPost.Select(c => c.Lector).ToList(), lectorFilter), lectorFilter);
                return View(coursesPost.ToPagedList(pageNumber, pageSize));
            }
            coursesList = coursesManager.GetSortedCourses(currentFilter, statusFilter, themeFilter, lectorFilter, null, coursesList);
            var courses = CourseViewModel.GetCoursesList(coursesList, 1);
            ViewBag.Lectors = new SelectList(usersManager.GetAllLectors(
                    courses.Select(c => c.Lector).ToList(), lectorFilter), lectorFilter);

            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        //Sign to course or quit the course
        // GET: /Courses/SignOrQuitCourse
        [Authorize]
        public ActionResult SignOrQuitCourse(int? courseId, bool userIsOnCourse = false)
        {
            if (courseId == null)
                return View("Error");
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