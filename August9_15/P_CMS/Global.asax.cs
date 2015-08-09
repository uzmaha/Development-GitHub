using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using P_CMS.BAL;
using P_CMS.Models;
using P_CMS.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace P_CMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer<ApplicationDbContext>(
            //    null
            //    );

            // MailManager.sendEmail("this is a testing mail, thanks");        
            AppUserManager.SeedUser();
            AppUserManager.SeedApplication();          
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        void Session_Start(object sender, EventArgs e)
        {
            string sessionId = Session.SessionID;
        }

    }
}
