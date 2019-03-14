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
            var lectors = usersManager.GetUsersWithSpecificRole("Lector");
            lectors.Insert(0, null);
            ViewBag.LectorsList = new SelectList(lectors, "Id", "LastName");
            return View();
        }

        // POST: Admin/ManageCourses/AddCourse
        [HttpPost]
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
                var lectors = usersManager.GetUsersWithSpecificRole("Lector");
                lectors.Insert(0, null);
                ViewBag.LectorsList = new SelectList(lectors, "Id", "LastName");
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
        public ActionResult DeleteCourse(int id)
        {
            CoursesManager coursesManager = new CoursesManager();
            ViewBag.Course = coursesManager.GetSpecificCourse(id).CourseName;
            coursesManager.DeleteCourse(id);
            return View();
        }

        // GET: Admin/ManageCourses/EditCourse
        public ActionResult EditCourse(int id)
        {
            CoursesManager coursesManager = new CoursesManager();
            var course = coursesManager.GetSpecificCourse(id);
            UsersManager usersManager = new UsersManager();
            ViewBag.CurrentLector = coursesManager.GetLectorInfo(course);
            var lectors = usersManager.GetUsersWithSpecificRole("Lector");
            lectors.Insert(0, null);
            ViewBag.LectorsList = new SelectList(lectors, "Id", "LastName");
            return View(course);
        }

        // POST: Admin/ManageCourses/EditCourse
        [HttpPost]
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
                UsersManager userManager = new UsersManager();
                ViewBag.CurrentLector = coursesManager.GetLectorInfo(course);
                var lectors = userManager.GetUsersWithSpecificRole("Lector");
                lectors.Insert(0, null);
                ViewBag.LectorsList = new SelectList(lectors, "Id", "LastName");
                return View(course);
            }
        }


    }
}