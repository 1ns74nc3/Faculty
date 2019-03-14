using Faculty.Logic.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    //Get, edit, delete data from Identity Users table
    public class UsersManager
    {
        //Edit course in database
        public void EditUser(ApplicationUser user, string role)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userToChange = db.Users.Where(u => u.Id == user.Id).FirstOrDefault();
                db.Entry(userToChange).CurrentValues.SetValues(user);
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                userManager.RemoveFromRole(user.Id, userManager.GetRoles(user.Id).SingleOrDefault());
                userManager.AddToRole(user.Id, role);
                db.SaveChanges();
            }
        }

        //Get all courses for specific user
        public List<Course> GetCoursesForSpecificUser(string userId)
        {
            List<Course> courses = new List<Course>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                courses = db.Users.Where(u => u.Id == userId).Include(c => c.Courses).Select(item => item.Courses).ToList().First().ToList();
            }
            return courses;
        }

        //Get all users
        public ICollection<ApplicationUser> GetUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.Include(u => u.Roles).OrderBy(u => u.LastName).ToList();
            }
            return users;
        }

        //Get specific user by User ID
        public ApplicationUser GetSpecificUser(string userId)
        {
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                user = db.Users.Where(u => u.Id == userId).Include(u => u.Roles).SingleOrDefault();
            }
            return user;
        }

        //Get role for specific user by User ID
        public string GetUserRole(string id)
        {
            string result = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                result = userManager.GetRoles(id).SingleOrDefault();
            }
            return result;
        }

        //Get list of users with specific role
        public List<ApplicationUser> GetUsersWithSpecificRole(string role)
        {
            List<ApplicationUser> result = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var roleId = db.Roles.Where(r => r.Name == role).SingleOrDefault().Id;
                result = db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId)).ToList();
            }
            return result;
        }

        public void RemoveUser(string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                JournalsManager journalsManager = new JournalsManager();
                var user = db.Users.SingleOrDefault(c => c.Id == userId);
                journalsManager.RemoveJournalForUser(userId);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }
    }
}
