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
        private JournalsManager journalsManager = new JournalsManager();

        //Add new user
        public void AddUser(ApplicationUser user, string password, string role)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                userManager.Create(user, password);
                userManager.AddToRole(user.Id, role);
            }
        }

        //Edit course in database
        public void EditUser(ApplicationUser user, string role)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userToChange = db.Users.FirstOrDefault(u => u.Id == user.Id);
                userToChange.UserName = user.UserName;
                userToChange.Age = user.Age;
                userToChange.Email = user.Email;
                userToChange.FirstName = user.FirstName;
                userToChange.LastName = user.LastName;
                userToChange.UserInformation = user.UserInformation;
                userToChange.UserIsBlocked = user.UserIsBlocked;
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                if (role != null)
                {
                    userManager.RemoveFromRole(user.Id, userManager.GetRoles(user.Id).SingleOrDefault());
                    userManager.AddToRole(user.Id, role);
                }
                db.SaveChanges();
            }
        }

        public List<string> GetAllLectors(List<string> courses, string currentLector)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<string> result = new List<string>();
                result.Add(currentLector);
                foreach (var item in courses)
                {
                    if (!result.Contains(item) && !item.Equals("None"))
                    {
                        result.Add(item);
                    }
                }
                if (currentLector != null && currentLector != "")
                    result.Add(null);
                return result;
            }
        }

        //Get all courses for specific user
        public List<ApplicationUser> GetCoursesForSpecificUser(string userId)
        {
            List<ApplicationUser> courses = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                courses = db.Users.Where(u => u.Id == userId).Include(c => c.Courses).Include(j => j.Journals).ToList();
            }
            return courses;
        }

        //return list of users with Lector role, 1st element is null or current lector on the course
        public IList<string> GetRolesListWithActiveRole(string userId)
        {
            var currentRole = GetUserRole(userId);
            List<string> rolesList = new List<string> { "Admin", "Lector", "Student" };
            List<string> roles = new List<string> { currentRole };
            foreach (var item in rolesList)
            {
                if (!roles.Contains(item))
                    roles.Add(item);
            }
            return roles;
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

        public ICollection<ApplicationUser> GetSortedUsersList(string firstName, string lastName, string role, ICollection<ApplicationUser> users)
        {
            if (firstName != null && firstName != "")
            {
                users = users
                    .Where(u => u.FirstName.Length >= firstName.Length)
                    .Where(u => u.FirstName.ToLower().Substring(0, firstName.Length) == firstName)
                    .ToList();
            }

            if (lastName != null && lastName != "")
            {
                users = users
                    .Where(u => u.LastName.Length >= lastName.Length)
                    .Where(u => u.LastName.ToLower().Substring(0, lastName.Length) == lastName)
                    .ToList();
            }

            if(role!=null && role != "" && role!="All")
            {
                users = users
                    .Where(u => GetUserRole(u.Id) == role)
                    .ToList();
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
        public string GetUserRole(string userId)
        {
            string result = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                result = userManager.GetRoles(userId).SingleOrDefault();
            }
            return result;
        }

        //Get list of users with specific role
        public List<ApplicationUser> GetLectorsForCourseEdit(string userId)
        {
            List<ApplicationUser> result = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (userId != "" && userId != null)
                {
                    var currentUser = db.Users.SingleOrDefault(u => u.Id == userId);
                    result.Add(currentUser);
                }

                var lectorsList = db.Users.Where(u => u.Roles.Any(r => r.RoleId == db.Roles.FirstOrDefault(role => role.Name == "Lector").Id)).ToList();
                result.Add(null);
                if (lectorsList.Any())
                {
                    foreach (var item in lectorsList)
                    {
                        if(!result.Contains(item))
                            result.Add(item);
                    }
                }
                

            }
            return result;
        }

        //delete user, connected journals and if user was lector - remove this lector for courses and update course status to unknown
        public void RemoveUser(string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var user = db.Users.SingleOrDefault(c => c.Id == userId);
                var courses = db.Courses.Where(c => c.LectorId == userId).ToList();
                foreach(var items in courses)
                {
                    items.LectorId = null;
                    items.SetStatus();
                }
                journalsManager.RemoveJournalForUser(userId);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public bool UserIsBlocked(string userName)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Users.SingleOrDefault(u => u.UserName == userName).UserIsBlocked;
            }
        }

    }
}
