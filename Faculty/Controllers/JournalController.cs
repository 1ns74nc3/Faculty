using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using Microsoft.AspNet.Identity;
using PagedList;
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
        private UsersManager usersManager;
        private JournalsManager journalsManager;
        private CoursesManager coursesManager;
        private LogManager logManager;

        public JournalController()
        {
            usersManager = new UsersManager();
            journalsManager = new JournalsManager();
            coursesManager = new CoursesManager();
            logManager = new LogManager();
        }

        //Manage journals when course is Ended
        // GET: /Journal/ManageJournal
        public ActionResult ManageJournal(int courseId, string userFirstNameFilter, string userLastNameFilter, int? page, string statusMessage )
        {
            logManager.AddEventLog("JournalController => ManageJournal ActionResult called(GET)", "ActionResult");
            ViewBag.StatusMessage = statusMessage;
            ViewBag.FirstNameFilter = userFirstNameFilter;
            ViewBag.LastNameFilter = userLastNameFilter;
            ViewBag.CourseId = courseId;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var journalsList = journalsManager.GetMarksForUsers(courseId);
            var course = coursesManager.GetSpecificCourse(courseId);

            var courseLectorId = course.LectorId;
            var currentUserId = User.Identity.GetUserId();

            if (courseLectorId == currentUserId && course.CourseStatus == Course.Status.Ended)
            {
                if (Request.HttpMethod == "POST")
                {
                    journalsList = usersManager.GetSortedUsersList(userFirstNameFilter, userLastNameFilter,null, journalsList);
                    List<JournalViewModel> journalsPost = JournalViewModel.GetJournalsList(journalsList, course);

                    return View(journalsPost.ToPagedList(pageNumber, pageSize));
                }
                journalsList = usersManager.GetSortedUsersList(userFirstNameFilter, userLastNameFilter, null, journalsList);
                List<JournalViewModel> journals = JournalViewModel.GetJournalsList(journalsList, course);

                return View(journals.ToPagedList(pageNumber, pageSize));
            }
            return View("Error");
            
        }

        //Edit mark
        // GET: /Journal/ManageUserMark
        public ActionResult ManageUserMark(int journalId, int courseId)
        {
            logManager.AddEventLog("JournalController => ManageUserMark ActionResult called(GET)", "ActionResult");
            var course = coursesManager.GetSpecificCourse(courseId);
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
        public ActionResult ManageUserMark(Journal journal, int courseId, int journalId)
        {
            logManager.AddEventLog("JournalController => ManageUserMark ActionResult called(POST)", "ActionResult");
            journal.Id = journalId;
            if (ModelState.IsValid)
            {
                journalsManager.EditJournal(journal);
                return RedirectToAction("ManageJournal", "Journal", new { courseId = courseId, statusMessage = "You succesfully edited mark!" });
            }
            else
            {
                ModelState.AddModelError("Error", "You entered invalid data!" );
                var defaultJournal = journalsManager.GetJournal(journal.Id);
                ViewBag.CourseId = courseId;
                ViewBag.JournalId = journalId;
                return View(defaultJournal);
            }          
        }
    }
}