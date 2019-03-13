﻿using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class CoursesController : Controller
    {
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

        // GET: Courses/DisplayCourses
        public ActionResult DisplayCourses()
        {
            CoursesManager coursesManager = new CoursesManager();
            var courses = coursesManager.GetCourses();
            return View(courses);
        }

        // GET: Courses/SignToCourse
        [Authorize]
        public ActionResult SignOrQuitCourse(int id, bool userIsOnCourse)
        {
            CoursesManager coursesManager = new CoursesManager();
            JournalsManager journalsManager = new JournalsManager();
            string currentUserId = User.Identity.GetUserId();
            ViewBag.RegistrationResult = "";
            
            if (userIsOnCourse)
            {
                ViewBag.RegistrationResult = coursesManager.RemoveUserFromCourse(id, currentUserId);
                journalsManager.RemoveJournalForUser(currentUserId);
            }
            else
            {
                ViewBag.RegistrationResult = coursesManager.AddUserToCourse(id, currentUserId);
                journalsManager.AddJournalForUser(id, currentUserId);
            }

            return View();
        }
    }
}