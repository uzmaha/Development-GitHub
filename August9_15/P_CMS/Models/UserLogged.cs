using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class UserLogged
    { 
        public string UserName { get; set; }
          public string FirstName { get; set; }
          public string FullName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
         public string Email { get; set; }
         public string PhoneNumber { get; set; }
          public string PasswordHash { get; set; }
          public int UserLoggedId { get; set; }

        
          public string UserId { get; set; }
          public string RoleId { get; set; }
          public string RoleName { get; set; }
        public string Address { get; set; }
        public int? DesignationId { get; set; }
       
        public bool EmailConfirmed { get; set; }
        public string selectedProducts { get; set; }
      
        public string SecurityStamp { get; set; }
       
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public System.DateTime? CreatedOn { get; set; }



        public System.DateTime? UpdatedOn { get; set; }


        public UtilityClasses.UserLoggedActions LoggedAction { get; set; }
    }
}