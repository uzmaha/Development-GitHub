using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P_CMS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
       
        [Display(Name = "First Name")]
        
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        //[DataType(DataType.Password)]

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel : IdentityUser
    {
        public string selectedProduct { get; set; }
        public string UserId { get; set; }
       
        [Display(Name = "User Role")]
        [Required]
        public string RoleId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "First Name")]
        
        public string FirstName { get; set; }
         [Required]
         [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Enter valid contact no.!")]
        
         [Display(Name = "Contact No")]
        public string ContactNo { get; set; }
       
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
           [Display(Name = "Assigned Role")]
        public string RoleName { get; set; }
        [Required]
        [EmailAddress]
       // [RegularExpression(@"^[_+a-z0-9-]+(\.[_+a-z0-9-]+)*@pronet-tech.net$",ErrorMessage="Invalid email address.")]
       [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@pronet-tech.net$", ErrorMessage = "Invalid email address.")]
      
        [Display(Name = "Email Address")]
                public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<Product> Products { get; set; }
    }
    public class EditViewModel
    {
      
        public string selectedProduct { get; set; }
        public List<Product> Products { get; set; }
        public string UserId { get; set; }

        [Display(Name = "User Role")]
        [Required]
        public string RoleId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "First Name")]

        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"\d+", ErrorMessage = "Enter valid contact no.!")]

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@pronet-tech.net$", ErrorMessage = "Invalid email address.")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

       
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

      
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
