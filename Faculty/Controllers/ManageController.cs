using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Faculty.Models;
using Faculty.Logic.DB;
using System.Collections.Generic;
using PagedList;
using Faculty.Logic.Models;

namespace Faculty.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private UsersManager usersManager;
        private JournalsManager journalsManager;
        private CoursesManager coursesManager;
        private LogManager logManager;

        public ManageController()
        {
            usersManager = new UsersManager();
            journalsManager = new JournalsManager();
            coursesManager = new CoursesManager();
            logManager = new LogManager();
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            usersManager = new UsersManager();
            journalsManager = new JournalsManager();
            coursesManager = new CoursesManager();
            logManager = new LogManager();
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //Display user data and links to see user courses, lectors functions, admin functions, edit account
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            logManager.AddEventLog("ManageController => Index ActionResult called(GET)", "ActionResult");
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : message == ManageMessageId.ProfileEdited ? "You succesfully edited your account data!"
                : "";

            var userId = User.Identity.GetUserId();
            var user = usersManager.GetSpecificUser(userId);
            ViewBag.UserRole = usersManager.GetUserRole(userId);

            var model = new IndexViewModel
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                UserInfo = user.UserInformation,
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            logManager.AddEventLog("ManageController => ChangePassword ActionResult called(GET)", "ActionResult");
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            logManager.AddEventLog("ManageController => ChangePassword ActionResult called(POST)", "ActionResult");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //Display courses for Lector where he's signed as Lector
        // GET: /Manage/DisplayCoursesForLector
        [Authorize(Roles = "Lector")]
        public ActionResult DisplayCoursesForLector(string userId, string statusFilter, string themeFilter, string courseNameFilter, int? page)
        {
            logManager.AddEventLog("ManageController => DisplayCoursesForLector ActionResult called(GET)", "ActionResult");
            if (usersManager.GetUserRole(userId) != "Lector" || userId == null)
                return View("Error");
            ViewBag.UserId = userId;
            ViewBag.CurrentStatusFilter = statusFilter;
            ViewBag.CurrentThemeFilter = themeFilter;
            ViewBag.CourseNameFilter = courseNameFilter;
            ViewBag.Themes = new SelectList(coursesManager.GetAllThemes(themeFilter));
            ViewBag.Status = new SelectList(new List<string> { "All", "Unknown", "Upcoming", "Active", "Ended" });
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var coursesList = coursesManager.GetCoursesForLector(userId);
            if (Request.HttpMethod == "POST")
            {
                coursesList = coursesManager.GetSortedCourses(null, statusFilter, themeFilter, null, courseNameFilter, coursesList);
                List<CourseViewModel> coursesPost = CourseViewModel.GetCoursesList(coursesList, 2);

                return View(coursesPost.ToPagedList(pageNumber, pageSize));
            }

            coursesList = coursesManager.GetSortedCourses(null, statusFilter, themeFilter, null, courseNameFilter, coursesList);
            List<CourseViewModel> courses = CourseViewModel.GetCoursesList(coursesList, 2);


            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        //Display all courses where user is signed
        // GET: /Manage/DisplayUserCourses
        public ActionResult DisplayUserCourses(string userId, string courseNameFilter, string courseStatusFilter, int? page)
        {
            logManager.AddEventLog("ManageController => DisplayUserCourses ActionResult called(GET)", "ActionResult");
            if (userId == null)
                return View("Error");
            ViewBag.UserId = userId;
            ViewBag.CourseName = courseNameFilter;
            ViewBag.CourseStatus = courseStatusFilter;
            ViewBag.Status = new SelectList(new List<string> { "All", "Unknown", "Upcoming", "Active", "Ended" });
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var journalsList = journalsManager.GetAllJournalsForUser(userId);
            if (Request.HttpMethod == "POST")
            {
                List<JournalViewModel> journalsPost = JournalViewModel.GerSortedJournalsList(
                courseNameFilter,
                courseStatusFilter,
                JournalViewModel.GetJournalsList(journalsList, usersManager, coursesManager
                ));
                return View(journalsPost.ToPagedList(pageNumber, pageSize));
            }
            List<JournalViewModel> journals = JournalViewModel.GerSortedJournalsList(
                courseNameFilter,
                courseStatusFilter,
                JournalViewModel.GetJournalsList(journalsList, usersManager, coursesManager
                ));


            return View(journals.ToPagedList(pageNumber, pageSize));
        }

        //Edit user data
        // GET: /Manage/EditUserData
        public ActionResult EditUserData(string userId)
        {
            logManager.AddEventLog("ManageController => EditUserData ActionResult called(GET)", "ActionResult");
            if (userId == null)
                return View("Error");
            var user = usersManager.GetSpecificUser(userId);
            ViewBag.UserId = userId;

            return View(user);
        }

        // POST: /Manage/EditUserData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserData(ApplicationUser user, string role, string userId)
        {
            logManager.AddEventLog("ManageController => EditUserData ActionResult called(POST)", "ActionResult");
            if (userId == null)
                return View("Error");
            user.Id = userId;
            if (ModelState.IsValid)
            {
                usersManager.EditUser(user, null);
                return RedirectToAction("Index", new { Message = ManageMessageId.ProfileEdited });
            }
            else
            {
                ViewBag.UserId = userId;
                return View(user);
            }
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            logManager.AddEventLog("ManageController => SetPassword ActionResult called(GET)", "ActionResult");
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            logManager.AddEventLog("ManageController => SetPassword ActionResult called(POST)", "ActionResult");
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            ProfileEdited,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}