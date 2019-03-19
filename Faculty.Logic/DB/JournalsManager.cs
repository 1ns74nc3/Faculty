using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    public class JournalsManager
    {
       
        //Adding db record to set users marks
        public void AddJournalForUser(int? courseId, string userId)
        {
            if (courseId != null)
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

        public void DeleteJournalsWhenRemovingCourse(int? courseId)
        {
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

        public void EditJournal(Journal journal)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Journals.SingleOrDefault(j => j.Id == journal.Id).Mark = journal.Mark;
                db.SaveChanges();
            }
        }

        public Journal GetJournal(int? journalId)
        {
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

        public ICollection<Journal> GetAllJournals()
        {
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

        public ICollection<Journal> GetSortedJournalsList(string firstName, string lastName, ICollection<Journal> journals)
        {
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
