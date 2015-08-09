using P_CMS.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace P_CMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
           
            //var constraintsResolver = new DefaultInlineConstraintResolver();
            //constraintsResolver.ConstraintMap.Add("values", typeof(RoutingHelper));
            //routes.MapMvcAttributeRoutes(constraintsResolver);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "downloads",
               url: "{controller}/{action}/{fileName}/{fileDBName}",
               defaults: new { controller = "WebHelper", action = "Download", fileName = UrlParameter.Optional, fileDBName = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Issues",
               url: "{controller}/{action}/{id}/{statusId}/{clientid}/{priorityid}/{productid}/{date}",
               defaults: new { controller = "Issue", action = "Index", id = UrlParameter.Optional, statusId = UrlParameter.Optional, clientid = UrlParameter.Optional, priorityid = UrlParameter.Optional, productid = UrlParameter.Optional, date = UrlParameter.Optional}
           );
           // routes.MapRoute(
           //    name: "pronetfCMS",
           //    url: "{controller}/{action}/{id}",
           //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           //);
        }
    }
}
