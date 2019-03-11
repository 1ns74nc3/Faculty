using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    public class JournalsManager
    {
        public ICollection<Journal> GetUsers()
        {
            List<Journal> users = new List<Journal>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Journals
                    .Include(j => j.Users)
                    .Include(j => j.Courses)
                    .OrderByDescending(j => j.Mark)
                    .ToList();
            }
            return users;
        }
    }
}
