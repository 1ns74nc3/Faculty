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
                Journal journal = new Journal(courseId, userId);
                db.Journals.Add(journal);
                db.SaveChanges();
            }
        }

        //Get all related to course data 
        public ICollection<object> GetMarksForUsers(int courseId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                UsersManager usersManager = new UsersManager();
                var journals = db.Journals.Where(j => j.CourseId == courseId).ToList();
                List<object> result = new List<object>();
                if (journals.Any())
                {
                    foreach (var item in journals)
                    {
                        var users = usersManager.GetSpecificUser(item.UserId);
                        result.Add(new { users.FirstName, users.LastName, item.Mark, item.Id });
                    }
                }
                
                return result;
            }
        }

        //Remove Mark for user in course
        public void RemoveJournalForUser(int courseId, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var journal = db.Journals.SingleOrDefault(j => j.UserId == userId && j.UserId == userId);
                db.Journals.Remove(journal);
                db.SaveChanges();
            }
        }

    }
}
