using Faculty.Logic.DB;
using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        // GET: Admin/UsersController/DisplayUsers
        public ActionResult DisplayUsers()
        {
            UsersManager usersManager = new UsersManager();
            var users = usersManager.GetUsers();
            return View(users);
        }

        // GET: Admin/UsersController/EditUser
        public ActionResult EditUser(string id)
        {
            UsersManager usersManager = new UsersManager();
            var user = usersManager.GetSpecificUser(id);
            ViewBag.CurrentRole = "Current role - "+ usersManager.GetUserRole(id);
            ViewBag.Roles = new List<string>{ "Admin", "Lector", "Student" };
            return View(user);
        }

        // POST: Admin/UsersController/EditUser
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
    }
}