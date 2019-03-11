using System.Web.Mvc;

namespace Faculty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: Admin/AdminPanel
        public ActionResult AdminPanel()
        {
            return View();
        }
    }
}