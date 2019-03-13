using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    public class JournalsManager
    {
        //Adding db record to set users marks
        public void AddJournalForUser(int courseId, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                UsersManager coursesManager = new UsersManager();
                var user = db.Users.SingleOrDefault(u => u.Id == userId);
                Journal journal = new Journal(courseId);
                db.Journals.Add(journal);
                db.SaveChanges();
                db.Journals.SingleOrDefault(j => j.Id == journal.Id).Users.Add(user);
                db.SaveChanges();
            }
        }

        public void DeleteJournalsWhenRemovingCourse(int courseId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var journals = db.Journals.Where(j => j.CourseId == courseId).ToList();
                foreach (var item in journals)
                {
                    db.Journals.Remove(item);
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

        public Journal GetJournal(int journalId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var result = db.Journals.Where(j => j.Id == journalId).Include("Users").SingleOrDefault();

                return result;
            }
        }
        //Get all related to course data 
        public IList<ApplicationUser> GetMarksForUsers(int courseId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var result = db.Users.Where(u => u.Courses.FirstOrDefault(c => c.Id == courseId).Id == courseId)
                    .Include(j => j.Journals)
                    .ToList();
                
                return result;
            }
        }

        //Remove Mark for user in course
        public void RemoveJournalForUser(string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var journal = db.Journals.Where(j=>j.Users.Select(u=>u.Id).FirstOrDefault() == userId).First();
                db.Journals.Remove(journal);
                db.SaveChanges();
            }
        }

    }
}
