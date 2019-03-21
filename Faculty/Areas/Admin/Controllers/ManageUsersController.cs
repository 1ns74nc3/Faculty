using Faculty.Logic.DB;
using Faculty.Logic.Models;
using Faculty.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        private UsersManager usersManager;
        private JournalsManager journalsManager;
        private LogManager logManager;

        public ManageUsersController()
        {
            usersManager = new UsersManager();
            journalsManager = new JournalsManager();
            logManager = new LogManager();
        }

        //Add new user
        // GET: /Admin/ManageUsers/AddUser
        public ActionResult AddUser()
        {
            logManager.AddEventLog("ManageUsersController(Admin area) => AddUser ActionResult called(GET)", "ActionResult");
            ViewBag.Roles = new List<string>{ "Admin", "Lector", "Student" };
            return View();
        }

        // POST: /Admin/ManageUsers/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(RegisterViewModel user, string role)
        {
            logManager.AddEventLog("ManageUsersController(Admin area) => AddUser ActionResult called(POST)", "ActionResult");
            if (ModelState.IsValid)
            {
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
                return RedirectToAction("DisplayUsers" , new { statusMessage = "You succesfully added new user!" });
            }
            else
            {
                ViewBag.Roles = new List<string> { "Admin", "Lector", "Student" };
                return View();
            }
        }


        //Display all users
        // GET: /Admin/ManageUsers/DisplayUsers
        public ActionResult DisplayUsers(string userFirstNameFilter, string userLastNameFilter, string roleFilter, int? page, string statusMessage)
        {
            logManager.AddEventLog("ManageUsersController(Admin area) => DisplayUsers ActionResult called(GET)", "ActionResult");
            ViewBag.StatusMessage = statusMessage;
            ViewBag.FirstNameFilter = userFirstNameFilter;
            ViewBag.LastNameFilter = userLastNameFilter;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.Roles = new SelectList(new List<string> { "All", "Admin", "Lector", "Student" });
            int pageSize = 10;
            int pageNumber = (page ?? 1);
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

        //Edit specific user
        // GET: /Admin/ManageUsers/EditUser
        public ActionResult EditUser(string userId)
        {
            logManager.AddEventLog("ManageUsersController(Admin area) => EditUser ActionResult called(GET)", "ActionResult");
            ViewBag.UserId = userId ?? throw new ArgumentNullException();
            var user = usersManager.GetSpecificUser(userId);
            ViewBag.Roles = usersManager.GetRolesListWithActiveRole(userId);
            

            return View(user);
        }

        // POST: /Admin/ManageUsers/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser user, string role, string userId)
        {
            logManager.AddEventLog("ManageUsersController(Admin area) => EditUser ActionResult called(POST)", "ActionResult");
            user.Id = userId ?? throw new ArgumentNullException();
            if (ModelState.IsValid)
            {
                usersManager.EditUser(user, role);
                return RedirectToAction("DisplayUsers", new { statusMessage = "You succesfully edited"+user.FirstName+" "+user.LastName+"user!" });
            }
            else
            {
                ViewBag.CurrentRole = "Current role - " + usersManager.GetUserRole(user.Id);
                ViewBag.Roles = new List<string> { "Admin", "Lector", "Student" };
                return View(user);
            }
        }

        //Delete specific user
        // GET: /Admin/ManageUsers/RemoveUser
        public ActionResult RemoveUser(string userId)
        {
            logManager.AddEventLog("ManageUsersController(Admin area) => RemoveUser ActionResult called(GET)", "ActionResult");
            if (userId == null)
                throw new ArgumentNullException();
            ViewBag.Username = usersManager.GetSpecificUser(userId).UserName;
            journalsManager.RemoveJournalForUser(userId);
            usersManager.RemoveUser(userId);

            return View();
        }
    }
}