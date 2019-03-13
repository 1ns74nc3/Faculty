using Faculty.Logic.DB;
using Faculty.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    [Authorize(Roles = "Admin, Lector")]
    public class JournalController : Controller
    {
        // GET: Journal/ManageJournal
        public ActionResult ManageJournal(int id)
        {
            JournalsManager journalsManager = new JournalsManager();
            CoursesManager coursesManager = new CoursesManager();
            var data = journalsManager.GetMarksForUsers(id);
            ViewBag.Course = coursesManager.GetSpecificCourse(id);

            return View(data);
        }

        // GET: Journal/ManageUserMark
        public ActionResult ManageUserMark(byte journalId, int courseId)
        {
            JournalsManager journalsManager = new JournalsManager();
            var journal = journalsManager.GetJournal(journalId);
            ViewBag.CourseId = courseId;
            ViewBag.JournalId = journalId;

            return View(journal);
        }

        // POST: Journal/ManageUserMark
        [HttpPost]
        public ActionResult ManageUserMark(Journal journal, int courseId, byte journalId)
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