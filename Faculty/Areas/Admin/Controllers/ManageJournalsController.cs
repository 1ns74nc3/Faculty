﻿using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Areas.Admin.Controllers
{
    public class ManageJournalsController : Controller
    {
        // GET: Admin/ManageJournals/DisplayJournals
        public ActionResult DisplayJournals()
        {
            JournalsManager journalsManager = new JournalsManager();
            CoursesManager coursesManager = new CoursesManager();
            var journalsList = journalsManager.GetAllJournals();
            List<JournalViewModel> journals = new List<JournalViewModel>();

            if (journalsList != null)
            {
                foreach(var item in journalsList)
                {
                    var courseName = coursesManager.GetSpecificCourse(item.CourseId).CourseName;
                    journals.Add(new JournalViewModel(
                        item.Id, 
                        item.Users.First().FirstName, 
                        item.Users.First().LastName, 
                        item.Mark,
                        courseName
                        ));
                }
            }

            return View(journals);
        }

        // GET: Admin/ManageJournals/EditMark
        public ActionResult EditMark(int journalId)
        {
            JournalsManager journalsManager = new JournalsManager();
            var journal = journalsManager.GetJournal(journalId);
            ViewBag.JournalId = journalId;

            return View(journal);
        }

        // POST: Admin/ManageJournals/EditMark
        [HttpPost]
        public ActionResult EditMark(Journal journal, int journalId)
        {
            JournalsManager journalsManager = new JournalsManager();
            journal.Id = journalId;
            if (ModelState.IsValid)
            {
                journalsManager.EditJournal(journal);
                return RedirectToAction("DisplayJournals");
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