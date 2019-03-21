using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    //Managing journals
    public class JournalsManager
    {
        private LogManager logManager = new LogManager();

        //Adding journal for user when signing to the course
        public void AddJournalForUser(int? courseId, string userId)
        {
            logManager.AddEventLog("JournalsManager => AddJournalForUser method called", "Method");
            if (courseId != null && userId !=null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var user = db.Users.SingleOrDefault(u => u.Id == userId);
                    Journal journal = new Journal(courseId);
                    db.Journals.Add(journal);
                    db.SaveChanges();
                    db.Journals.SingleOrDefault(j => j.Id == journal.Id).Users.Add(user);
                    db.SaveChanges();
                }
            }
        }

        //Delete all journals connected to the course when course was removed by admin
        public void DeleteJournalsWhenRemovingCourse(int? courseId)
        {
            logManager.AddEventLog("JournalsManager => DeleteJournalsWhenRemovingCourse method called", "Method");
            if (courseId != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var journals = db.Journals.Where(j => j.CourseId == courseId).ToList();
                    foreach (var item in journals)
                    {
                        db.Journals.Remove(item);
                    }
                    db.SaveChanges();
                }
            }
        }

        //Edit Mark in journal
        public void EditJournal(Journal journal)
        {
            logManager.AddEventLog("JournalsManager => EditJournal method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Journals.SingleOrDefault(j => j.Id == journal.Id).Mark = journal.Mark;
                db.SaveChanges();
            }
        }

        //Get specific journal by ID
        public Journal GetJournal(int? journalId)
        {
            logManager.AddEventLog("JournalsManager => GetJournal method called", "Method");
            if (journalId != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var result = db.Journals.Where(j => j.Id == journalId).Include("Users").SingleOrDefault();

                    return result;
                }
            }
            else
            {
                return null;
            }
        }

        //Get list of all Journals
        public ICollection<Journal> GetAllJournals()
        {
            logManager.AddEventLog("JournalsManager => GetAllJournals method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var result = db.Journals
                    .Include(j => j.Users)
                    .ToList();
                return result;
            }
        }

        public ICollection<Journal> GetAllJournalsForUser(string userId)
        {
            logManager.AddEventLog("JournalsManager => GetAllJournalsForUser method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var result = db.Journals.Where(j => j.Users.FirstOrDefault(user => user.Id == userId).Id == userId)
                    .Include(j => j.Users)
                    .ToList();
                return result;
            }
        }


        //Get all marks related to course and users 
        public ICollection<ApplicationUser> GetMarksForUsers(int? courseId)
        {
            logManager.AddEventLog("JournalsManager => GetMarksForUsers method called", "Method");
            if (courseId != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var result = db.Users.Where(u => u.Courses.FirstOrDefault(c => c.Id == courseId).Id == courseId)
                        .Include(j => j.Journals)
                        .ToList();

                    return result;
                }
            }
            else
            {
                return null;
            }
        }

        //Sort journals using filters
        public ICollection<Journal> GetSortedJournalsList(string firstName, string lastName, ICollection<Journal> journals)
        {
            logManager.AddEventLog("JournalsManager => GetSortedJournalsList method called", "Method");
            if (firstName != null && firstName != "")
            {
                journals = journals
                    .Where(j => j.Users.First().FirstName.Length >= firstName.Length)
                    .Where(j => j.Users.First().FirstName.ToLower().Substring(0, firstName.Length) == firstName)
                    .ToList();
            }

            if (lastName != null && lastName != "")
            {
                journals = journals
                   .Where(j => j.Users.First().LastName.Length >= lastName.Length)
                   .Where(j => j.Users.First().LastName.ToLower().Substring(0, lastName.Length) == lastName)
                   .ToList();
            }

            return journals;
        }

        //Remove Mark for user in course
        public void RemoveJournalForUser(string userId)
        {
            logManager.AddEventLog("JournalsManager => RemoveJournalForUser method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var journal = db.Journals.Where(j=>j.Users.Select(u=>u.Id).FirstOrDefault() == userId).FirstOrDefault();
                if (journal != null) { 
                    db.Journals.Remove(journal);
                    db.SaveChanges();
                }
            }
        }

    }
}
