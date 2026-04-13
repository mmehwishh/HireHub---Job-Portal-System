using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Job_Portal_System
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            using (var db = new JobDBEntities3())
            {
                db.Database.ExecuteSqlCommand("EXEC sp_UpdateExpiredJobs");
            }
        }
    }
}
