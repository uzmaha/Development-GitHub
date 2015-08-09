using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace P_CMS.UtilityClasses
{
    public static class AppConstant
    {
        public const string MANAGER="manager";
        public const string SENDERMAILADDRESS="SenderEmailAddress";
        public const string APPLICATIONPATH = "ApplicationPath";
      
    }
    public static class AppRoles
    {
        public const string MANAGER = "Manager";
        public const string TUSER = "TUser";
        public const string ADMINISTRATOR = "Admin";
        public const string SUPERUSER = "SuperUser";
        public const string SYSTEMUSER = "SystemUser";
        public const string MTS = "Manager,TUser,SuperUser";
        public const string AS = "Admin,SuperUser";
    }
    public static class AppStatus
    {
        public const string ASSIGNED = "assigned";
        public const string UNASSIGNED = "unassigned";
        public const string REASSIGNEDBYMANAGER= "reassigned by manager";
        public const string REASSIGNEDBYTUSER= "reassigned by tuser";
        public const string REASSIGNEDBYSUPERUSER= "reassigned by superuser";
        public const string CLOSED = "closed";
       
    }
}