using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Areas.Admin.Controllers
{
    public class ManageJournalsController : Controller
    {
        private UsersManager usersManager;
        private JournalsManager journalsManager;
        private CoursesManager coursesManager;

        public ManageJournalsController()
        {
            journalsManager = new JournalsManager();
            usersManager = new UsersManager();
            coursesManager = new CoursesManager();
        }

        // GET: /Admin/ManageJournals/DisplayJournals
        public ActionResult DisplayJournals(string userFirstNameFilter, string userLastNameFilter, int? page, string statusMessage)
        {
            ViewBag.StatusMessage = statusMessage;
            ViewBag.FirstNameFilter = userFirstNameFilter;
            ViewBag.LastNameFilter = userLastNameFilter;
            
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            
            var journalsList = journalsManager.GetAllJournals();
            if (Request.HttpMethod == "POST")
            {
                journalsList = journalsManager.GetSortedJournalsList(userFirstNameFilter, userLastNameFilter, journalsList);
                List<JournalViewModel> journalsPost = JournalViewModel.GetJournalsList(journalsList, usersManager, coursesManager);

                return View(journalsPost.ToPagedList(pageNumber, pageSize));
            }
            journalsList = journalsManager.GetSortedJournalsList(userFirstNameFilter, userLastNameFilter, journalsList);
            List<JournalViewModel> journals = JournalViewModel.GetJournalsList(journalsList, usersManager, coursesManager);

            return View(journals.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Admin/ManageJournals/EditMark
        public ActionResult EditMark(int? journalId)
        {
            if (journalId == null)
                return View("Error");
            var journal = journalsManager.GetJournal(journalId);
            ViewBag.JournalId = journalId;

            return View(journal);
        }

        // POST: /Admin/ManageJournals/EditMark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMark(Journal journal, int? journalId)
        {
            if (journalId == null)
                return View("Error");
            journal.Id = journalId;
            if (ModelState.IsValid)
            {
                journalsManager.EditJournal(journal);
                return RedirectToAction("DisplayJournals", new { statusMessage = "You succesfully edited mark!" });
            }
            else
            {
                ModelState.AddModelError("error", "You entered invalid data!");
                var defaultJournal = journalsManager.GetJournal(journal.Id);
                ViewBag.JournalId = journalId;

                return View(defaultJournal);
            }
        }
    }
}