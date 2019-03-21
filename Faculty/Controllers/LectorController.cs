using Faculty.Logic.DB;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class LectorController : Controller
    {
        private UsersManager usersManager;
        private CoursesManager coursesManager;
        private LogManager logManager;

        public LectorController()
        {
            usersManager = new UsersManager();
            coursesManager = new CoursesManager();
            logManager = new LogManager();
        }

        // GET: Lector
        public ActionResult LectorInfo(int courseId)
        {
            logManager.AddEventLog("LectorController => LectorInfo ActionResult called(GET)", "ActionResult");
            if(courseId==null)
                return View("Error");
            var lectorId = coursesManager.GetSpecificCourse(courseId).LectorId;
            var lector = usersManager.GetSpecificUser(lectorId);
            ViewBag.CourseId = courseId;

            return View(lector);
        }
    }
}