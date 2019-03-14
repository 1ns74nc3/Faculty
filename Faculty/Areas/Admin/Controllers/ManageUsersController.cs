using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        // GET: Admin/ManageUsers/DisplayUsers
        public ActionResult DisplayUsers()
        {
            UsersManager usersManager = new UsersManager();
            var usersList = usersManager.GetUsers();
            List<UserViewModel> users = new List<UserViewModel>();
            if (usersList != null)
            {
                foreach (var item in usersList)
                {
                    users.Add(new UserViewModel(item.Id, item.FirstName, item.LastName, item.Age, item.Email, usersManager.GetUserRole(item.Id)));
                }
            }
                
            return View(users);
        }

        // GET: Admin/ManageUsers/EditUser
        public ActionResult EditUser(string userId)
        {
            UsersManager usersManager = new UsersManager();
            var user = usersManager.GetSpecificUser(userId);
            ViewBag.CurrentRole = "Current role - "+ usersManager.GetUserRole(userId);
            ViewBag.Roles = new List<string>{ "Admin", "Lector", "Student" };
            return View(user);
        }

        // POST: Admin/ManageUsers/EditUser
        [HttpPost]
        public ActionResult EditUser(ApplicationUser user, string role)
        {
            if (ModelState.IsValid)
            {
                UsersManager userManager = new UsersManager();
                userManager.EditUser(user, role);
                return RedirectToAction("DisplayUsers");
            }
            else
            {
                UsersManager usersManager = new UsersManager();
                ViewBag.CurrentRole = "Current role - " + usersManager.GetUserRole(user.Id);
                ViewBag.Roles = new List<string> { "Admin", "Lector", "Student" };
                return View(user);
            }
        }

        // GET: Admin/ManageUsers/RemoveUser
        public ActionResult RemoveUser(string userId)
        {
            JournalsManager journalsManager = new JournalsManager();
            UsersManager usersManager = new UsersManager();
            ViewBag.Username = usersManager.GetSpecificUser(userId).UserName;
            journalsManager.RemoveJournalForUser(userId);
            usersManager.RemoveUser(userId);

            return View();
        }
    }
}