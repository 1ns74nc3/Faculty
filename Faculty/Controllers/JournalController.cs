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
            var journalsList = journalsManager.GetMarksForUsers(courseId);
            var course = coursesManager.GetSpecificCourse(courseId);
            List<JournalViewModel> journals = new List<JournalViewModel>();

            var courseLectorId = course.LectorId;
            var currentUserId = User.Identity.GetUserId();

            if (courseLectorId == currentUserId && course.CourseStatus == Course.Status.Ended)
            {
                journals = JournalViewModel.GetJournalsList(journalsList, course);

                return View(journals);
            }
            return View("Error");
            
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

            return View("Error");
        }

        // POST: Journal/ManageUserMark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserMark(Journal journal, int currentcourseId, int journalId)
        {
            JournalsManager journalsManager = new JournalsManager();
            journal.Id = journalId;
            if (ModelState.IsValid)
            {
                journalsManager.EditJournal(journal);
                return RedirectToAction("ManageJournal", "Journal", new { courseId = currentcourseId });
            }
            else
            {
                ModelState.AddModelError("Error", "You entered invalid data!" );
                var defaultJournal = journalsManager.GetJournal(journal.Id);
                ViewBag.CourseId = currentcourseId;
                ViewBag.JournalId = journalId;
                return View(defaultJournal);
            }          
        }
    }
}