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

namespace Faculty.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            UsersManager usersManager = new UsersManager();
            ViewBag.UserData = usersManager.GetSpecificUser(userId);
            ViewBag.Lector = usersManager.GetUserRole(userId);
            var model = new IndexViewModel
            {
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
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
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

        // GET: /Manage/DisplayCoursesForLector
        [Authorize(Roles = "Lector")]
        public ActionResult DisplayCoursesForLector(string userId, string statusFilter, string themeFilter, string courseNameFilter, int? page)
        {
            CoursesManager coursesManager = new CoursesManager();
            ViewBag.UserId = userId;
            ViewBag.CurrentStatusFilter = statusFilter;
            ViewBag.CurrentThemeFilter = themeFilter;
            ViewBag.CourseNameFilter = courseNameFilter;
            ViewBag.Themes = new SelectList(coursesManager.GetAllThemes(themeFilter));
            ViewBag.Status = new SelectList(new List<string> { "All", "Unknown", "Upcoming", "Active", "Ended" });
            int pageSize = 1;
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

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
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

        // GET: /Manage/UserCourses
        public ActionResult DisplayUserCourses(string userId, string courseNameFilter, string courseStatusFilter, int? page)
        {
            JournalsManager journalsManager = new JournalsManager();
            CoursesManager coursesManager = new CoursesManager();
            UsersManager usersManager = new UsersManager();
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
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}