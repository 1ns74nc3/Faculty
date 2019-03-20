using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageCoursesController : Controller
    {
        private UsersManager usersManager;
        private CoursesManager coursesManager;

        public ManageCoursesController()
        {
            usersManager = new UsersManager();
            coursesManager = new CoursesManager();
        }

        //Add new course
        // GET: /Admin/ManageCourses/AddCourse
        public ActionResult AddCourse()
        {

            ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(null), "Id", "LastName");
            return View();
        }

        // POST: /Admin/ManageCourses/AddCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(Course course, string lector)
        {
            if (ModelState.IsValid)
            {
                if (lector != null && lector != "")
                {
                    course.LectorId = lector;
                }
                coursesManager.AddCourse(course);
                return RedirectToAction("DisplayCourses", new { statusMessage = "You succesfully edited " + course.CourseName + " course!" });
            }
            else
            {
                ModelState.AddModelError("error", "You entered invalid data!" );
                ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(null), "Id", "LastName");
                return View();
            }
        }

        //Display all courses
        // GET: /Admin/ManageCourses/DisplayCourses
        public ActionResult DisplayCourses(string statusFilter, string themeFilter, string lectorFilter, string courseNameFilter, int? page, string statusMessage)
        {
            ViewBag.StatusMessage = statusMessage;
            ViewBag.CurrentStatusFilter = statusFilter;
            ViewBag.CurrentThemeFilter = themeFilter;
            ViewBag.CurrentLectorFilter = lectorFilter;
            ViewBag.CourseNameFilter = courseNameFilter;
            ViewBag.Themes = new SelectList(coursesManager.GetAllThemes(themeFilter));
            ViewBag.Status = new SelectList(new List<string> { "All", "Unknown", "Upcoming", "Active", "Ended" });
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var coursesList = coursesManager.GetCourses();

            if (Request.HttpMethod == "POST")
            {
                coursesList = coursesManager.GetSortedCourses(null, statusFilter, themeFilter, lectorFilter, courseNameFilter, coursesList);
                var coursesPost = CourseViewModel.GetCoursesList(coursesList, 1);
                
                ViewBag.Lectors = new SelectList(usersManager.GetAllLectors(
                    coursesPost.Select(c => c.Lector).ToList(), lectorFilter), lectorFilter);
                return View(coursesPost.ToPagedList(pageNumber, pageSize));
            }

            coursesList = coursesManager.GetSortedCourses(null, statusFilter, themeFilter, lectorFilter, courseNameFilter, coursesList);
            var courses = CourseViewModel.GetCoursesList(coursesList, 1);
            ViewBag.Lectors = new SelectList(usersManager.GetAllLectors(
                courses.Select(c => c.Lector).ToList(), lectorFilter), lectorFilter);

            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        //Delete specific course
        // GET: /Admin/ManageCourses/DeleteCourse
        public ActionResult DeleteCourse(int? courseId)
        {
            if(courseId == null)
                return View("Error");

            ViewBag.Course = coursesManager.GetSpecificCourse(courseId).CourseName;
            coursesManager.DeleteCourse(courseId);
            return View();
            
        }

        //Edit specific course
        // GET: /Admin/ManageCourses/EditCourse
        public ActionResult EditCourse(int? courseId)
        {
            if (courseId == null)
                return View("Error");

            var course = coursesManager.GetSpecificCourse(courseId);
            ViewBag.CurrentLector = coursesManager.GetLectorInfo(course);
            ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(course.LectorId), "Id", "LastName");
            return View(course);
        }

        // POST: /Admin/ManageCourses/EditCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(Course course, string lector, int? courseId)
        {
            if (courseId == null)
                return View("Error");

            course.Id = courseId;
            if (ModelState.IsValid)
            {
                if (lector != null)
                {
                    course.LectorId = lector;
                    coursesManager.EditCourse(course);
                }

                return RedirectToAction("DisplayCourses", new { statusMessage = "You succesfully edited " + course.CourseName+" course!"});
            }
            else
            {
                ModelState.AddModelError("error", "You entered invalid data!" );
                ViewBag.CurrentLector = coursesManager.GetLectorInfo(course);
                ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(course.LectorId), "Id", "LastName");
                return View(course);
            }
        }


    }
}