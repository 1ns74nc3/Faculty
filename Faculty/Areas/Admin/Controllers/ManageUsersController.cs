using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        // GET: Admin/ManageUsers/AddUser
        public ActionResult AddUser()
        {
            ViewBag.Roles = new List<string>{ "Admin", "Lector", "Student" };
            return View();
        }

        // POST: Admin/ManageUsers/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(RegisterViewModel user, string role)
        {
            if (ModelState.IsValid)
            {
                UsersManager usersManager = new UsersManager();
                var newUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Age = user.Age,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserInformation = user.UserInformation
                };
                usersManager.AddUser(newUser, user.Password, role);
                return RedirectToAction("DisplayUsers");
            }
            else
            {
                ViewBag.Roles = new List<string> { "Admin", "Lector", "Student" };
                return View();
            }
        }



        // GET: Admin/ManageUsers/DisplayUsers
        public ActionResult DisplayUsers(string userFirstNameFilter, string userLastNameFilter, string roleFilter, int? page)
        {
            ViewBag.FirstNameFilter = userFirstNameFilter;
            ViewBag.LastNameFilter = userLastNameFilter;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.Roles = new SelectList(new List<string> { "All", "Admin", "Lector", "Student" });
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            UsersManager usersManager = new UsersManager();
            var usersList = usersManager.GetUsers();
            if (Request.HttpMethod == "POST")
            {
                usersList = usersManager.GetSortedUsersList(userFirstNameFilter, userLastNameFilter, roleFilter, usersList);
                var usersPost = UserViewModel.GetUsersList(usersList, usersManager);

                return View(usersPost.ToPagedList(pageNumber, pageSize));
            }

            usersList = usersManager.GetSortedUsersList(userFirstNameFilter, userLastNameFilter, roleFilter, usersList);
            var users = UserViewModel.GetUsersList(usersList, usersManager);

            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/ManageUsers/EditUser
        public ActionResult EditUser(string userId)
        {
            UsersManager usersManager = new UsersManager();
            var user = usersManager.GetSpecificUser(userId);
            ViewBag.Roles = usersManager.GetRolesListWithActiveRole(userId);
            ViewBag.UserId = userId;

            return View(user);
        }

        // POST: Admin/ManageUsers/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser user, string role, string userId)
        {
            user.Id = userId;
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