using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace P_CMS.UtilityClasses
{
    public static class CustomExtension
    {
        public static string CurrentAction(this UrlHelper urlHelper)
        {
            var routeValueDictionary = urlHelper.RequestContext.RouteData.Values;
            // in case using virtual dirctory 
            var rootUrl = urlHelper.Content("~/");
            return string.Format("{0}{1}/{2}/", rootUrl, routeValueDictionary["controller"], routeValueDictionary["action"]);
        }
        public static string estringUpper(this string stringToUpper)
        {
            return string.Format("{0}", stringToUpper.ToUpper());
        }
    }
}