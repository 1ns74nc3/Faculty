using Faculty.Logic.DB;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Faculty
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private LogManager logManager = new LogManager();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            logManager.RemoveAllLogs();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                logManager.AddExcaptionLog(exception.Message);
                Response.Redirect("/ErrorHandler/Error");
            }
        }
    }
}
