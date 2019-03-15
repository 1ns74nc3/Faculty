using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    [Authorize(Roles = "Lector")]
    public class JournalController : Controller
    {
        // GET: Journal/ManageJournal
        public ActionResult ManageJournal(int courseId)
        {
            JournalsManager journalsManager = new JournalsManager();
            CoursesManager coursesManager = new CoursesManager();
            UsersManager usersManager = new UsersManager();
            var journalsList = journalsManager.GetMarksForUsers(courseId);
            var course = coursesManager.GetSpecificCourse(courseId);
            List<JournalViewModel> journal = new List<JournalViewModel>();

            var courseLectorId = course.LectorId;

            var currentUserId = User.Identity.GetUserId();

            if (courseLectorId == currentUserId && course.CourseStatus == Course.Status.Ended)
            {
                if (journalsList != null)
                {
                    foreach (var item in journalsList)
                    {
                        journal.Add(new JournalViewModel(
                            item.Journals.First().Id,
                            item.FirstName,
                            item.LastName,
                            item.Journals.First().Mark,
                            course.CourseName));
                    }
                }

                return View(journal);
            }
            return RedirectToAction("Error");
            
        }

        // GET: Journal/ManageUserMark
        public ActionResult ManageUserMark(int journalId, int courseId)
        {
            CoursesManager coursesManager = new CoursesManager();
            UsersManager usersManager = new UsersManager();
            var course = coursesManager.GetSpecificCourse(courseId);
            
            JournalsManager journalsManager = new JournalsManager();
            var journal = journalsManager.GetJournal(journalId);

            var currentUserId = User.Identity.GetUserId();

            if (course.LectorId == currentUserId && course.CourseStatus == Course.Status.Ended) {
                ViewBag.CourseId = courseId;
                ViewBag.JournalId = journalId;
                return View(journal);
            }

            return RedirectToAction("Error");
        }

        // POST: Journal/ManageUserMark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserMark(Journal journal, int courseId, int journalId)
        {
            JournalsManager journalsManager = new JournalsManager();
            journal.Id = journalId;
            if (ModelState.IsValid)
            {
                journalsManager.EditJournal(journal);
                return RedirectToAction("CourseInfo", "Courses", new { id = courseId });
            }
            else
            {
                ModelState.AddModelError("error", "You entered invalid data!" );
                var defaultJournal = journalsManager.GetJournal(journal.Id);
                ViewBag.CourseId = courseId;
                ViewBag.JournalId = journalId;
                return View(defaultJournal);
            }          
        }
    }
}