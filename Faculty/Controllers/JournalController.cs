using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
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
        public ActionResult ManageJournal(int courseId)
        {
            JournalsManager journalsManager = new JournalsManager();
            CoursesManager coursesManager = new CoursesManager();
            var journalsList = journalsManager.GetMarksForUsers(courseId);
            var course = coursesManager.GetSpecificCourse(courseId);
            List<JournalViewModel> journal = new List<JournalViewModel>();
            if(journalsList != null)
            {
                foreach (var item in journalsList)
                {
                    journal.Add(new JournalViewModel(item.Journals.First().Id, item.FirstName, item.LastName, item.Journals.First().Mark,
                        course.CourseName, courseId));
                }
            }
            

            return View(journal);
        }

        // GET: Journal/ManageUserMark
        public ActionResult ManageUserMark(int journalId, int courseId)
        {
            JournalsManager journalsManager = new JournalsManager();
            var journal = journalsManager.GetJournal(journalId);
            ViewBag.CourseId = courseId;
            ViewBag.JournalId = journalId;

            return View(journal);
        }

        // POST: Journal/ManageUserMark
        [HttpPost]
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