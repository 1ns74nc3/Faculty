using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageCoursesController : Controller
    {
        // GET: Admin/ManageCourses/AddCourse
        public ActionResult AddCourse()
        {
            UsersManager usersManager = new UsersManager();
            ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(null), "Id", "LastName");
            return View();
        }

        // POST: Admin/ManageCourses/AddCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(Course course, string lector)
        {
            if (ModelState.IsValid)
            {
                CoursesManager coursesManager = new CoursesManager();
                if (lector != null)
                {
                    course.LectorId = lector;
                }
                coursesManager.AddCourse(course);
                return RedirectToAction("DisplayCourses");
            }
            else
            {
                ModelState.AddModelError("error", "You entered invalid data!" );
                UsersManager usersManager = new UsersManager();
                ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(null), "Id", "LastName");
                return View();
            }
        }

        // GET: Admin/ManageCourses/DisplayCourses
        public ActionResult DisplayCourses()
        {
            CoursesManager coursesManager = new CoursesManager();
            var coursesList = coursesManager.GetCourses();
            List<CourseViewModel> courses = new List<CourseViewModel>();
            if (coursesList != null)
            {
                foreach (var item in coursesList)
                {
                    courses.Add(new CourseViewModel(item.Id, item.CourseName, item.StartDate.ToShortDateString(), item.EndDate.ToShortDateString(),
                        item.Theme, item.CourseStatus.ToString(), item.Users.Count));
                }
            }

            return View(courses);
        }

        // GET: Admin/ManageCourses/DeleteCourse
        public ActionResult DeleteCourse(int courseId)
        {
            CoursesManager coursesManager = new CoursesManager();
            ViewBag.Course = coursesManager.GetSpecificCourse(courseId).CourseName;
            coursesManager.DeleteCourse(courseId);
            return View();
        }

        // GET: Admin/ManageCourses/EditCourse
        public ActionResult EditCourse(int courseId)
        {
            CoursesManager coursesManager = new CoursesManager();
            var course = coursesManager.GetSpecificCourse(courseId);
            UsersManager usersManager = new UsersManager();
            ViewBag.CurrentLector = coursesManager.GetLectorInfo(course);
            ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(course.LectorId), "Id", "LastName");
            return View(course);
        }

        // POST: Admin/ManageCourses/EditCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(Course course, string lector)
        {
            if (ModelState.IsValid)
            {
                CoursesManager coursesManager = new CoursesManager();
                if (lector != null)
                {
                    course.LectorId = lector;
                    coursesManager.EditCourse(course);
                }

                return RedirectToAction("DisplayCourses");
            }
            else
            {
                ModelState.AddModelError("error", "You entered invalid data!" );
                CoursesManager coursesManager = new CoursesManager();
                UsersManager usersManager = new UsersManager();
                ViewBag.CurrentLector = coursesManager.GetLectorInfo(course);
                ViewBag.LectorsList = new SelectList(usersManager.GetLectorsForCourseEdit(course.LectorId), "Id", "LastName");
                return View(course);
            }
        }


    }
}