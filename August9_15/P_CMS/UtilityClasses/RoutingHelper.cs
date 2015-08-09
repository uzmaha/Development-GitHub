using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace P_CMS.UtilityClasses
{
    public class RoutingHelper
    {
        private readonly string[] validOptions;
        public RoutingHelper(string options)
        {
            validOptions = options.Split('|');
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                return validOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}