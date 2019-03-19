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

        public JournalController()
        {
            usersManager = new UsersManager();
            journalsManager = new JournalsManager();
            coursesManager = new CoursesManager();
        }

        // GET: /Journal/ManageJournal
        public ActionResult ManageJournal(int? courseId, string userFirstNameFilter, string userLastNameFilter, int? page, string statusMessage )
        {
            if (courseId == null)
                return View("Error");
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

        // GET: /Journal/ManageUserMark
        public ActionResult ManageUserMark(int? journalId, int? courseId)
        {
            if (courseId == null || journalId == null)
                return View("Error");
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
        public ActionResult ManageUserMark(Journal journal, int? courseId, int? journalId)
        {
            if (courseId == null || journalId == null)
                return View("Error");
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