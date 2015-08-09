using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using P_CMS.Models;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using P_CMS.BAL;
using P_CMS.UtilityClasses;
using System.Web.Helpers;
using System.Web.Security;
using P_CMS.ViewModels;
using System.Collections.Generic;

namespace P_CMS.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string id = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            if (id != null)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var currentUser = db.Users.Find(id);
                LoginViewModel objLoginViewModel = new LoginViewModel();
                objLoginViewModel.Email = currentUser.Email;
                objLoginViewModel.Password = "";
                return View(objLoginViewModel);
            }
            else { return View(); }
        }
        public ActionResult ChangedPassword(string userId = null, string resetPw = null)
        {
            ChangePasswordModel objChangePasswordViewModel = new ChangePasswordModel();
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Find(userId);
            if (resetPw == "true")
            {
                if (Session["CurrentUser"] == null)
                {
                    Session["CurrentUser"] = User.Identity.Name;
                }
            }
            objChangePasswordViewModel.OldPassword = user.Password;
            return View(objChangePasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangedPassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var user = db.Users.Find(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                user.Password = model.NewPassword;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                user.PasswordHash = DBHandler.getHashPW(user.Id);
                DBHandler.SaveUserLogged(user, null, null, UserLoggedActions.PasswordChanged);
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ApplicationUser user = null;
            if (!ModelState.IsValid)
            {
                user = await UserManager.FindAsync(model.Email, model.Password);
            }
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    ApplicationDbContext db = new ApplicationDbContext();
                    user = await UserManager.FindAsync(model.Email, model.Password);
                    var loginUser = db.Users.Find(user.Id);
                    if (loginUser.LoginCounter + 1 == 1)
                    {
                        AuthenticationManager.SignOut();
                        if (User.Identity.IsAuthenticated)
                        {
                            return RedirectToLocal("/");
                        }
                        else
                        {
                            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                            return RedirectToAction("ResetPassword", new { code = code, userId = user.Id, resetPw = "true" });
                        }
                    }
                    else { return RedirectToLocal(returnUrl); }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        [Authorize(Roles = AppRoles.AS)]
        public ActionResult Register()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            // ViewData["AllProduct"] = DBHandler.GetProducts();
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");
            ViewBag.DesignationId = new SelectList(db.Designations, "DesignationId", "DesignationName");
            RegisterViewModel objRegisterViewModel = new RegisterViewModel();
            objRegisterViewModel.Products = DBHandler.GetProducts();
            return View(objRegisterViewModel);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AS)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                string userFullName = DataHelper.ToPascalConvention(model.FirstName + " " + model.LastName);
                var user = new ApplicationUser { LoginCounter = 0, UserName = model.Email, FullName = userFullName, FirstName = model.FirstName, Password = model.Password, LastName = model.LastName, IsActive = true, Email = model.Email, PhoneNumber = model.ContactNo, PasswordHash = model.Password };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    List<Product> listProducts = new List<Product>();
                    if (!string.IsNullOrEmpty(model.selectedProduct))
                    {  
                            ManagerProduct objManagerProduct = new ManagerProduct();
                            objManagerProduct.ProductIds = model.selectedProduct;
                            string currentAddedUserId = DBHandler.GetUserByEmail(model.Email).Id;
                            objManagerProduct.ApplicationUserId = currentAddedUserId;
                            objManagerProduct.CreatedOn = DateTime.Now;
                            objManagerProduct.UpdatedOn= DateTime.Now;
                            db.ManagerProducts.Add(objManagerProduct);
                            db.SaveChanges();
                        
                    }
                    var role = db.Roles.Find(model.RoleId);
                    if (role != null)
                    {
                        UserManager.AddToRole(user.Id, role.Name);
                    }
                    DBHandler.SaveUserLogged(user, model.RoleId, role.Name, UserLoggedActions.UserRegisteredRoleAssigned);
                    return RedirectToAction("Index", "User");
                }
                AddErrors(result);
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", model.RoleId);
            return View(model);
        }

        [Authorize(Roles = AppRoles.AS)]
        public ActionResult Edit(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var u_manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string role_name = u_manager.GetRoles(id).FirstOrDefault() == null ? "" : u_manager.GetRoles(id).FirstOrDefault();
            ApplicationUser applicationUser = db.Users.Find(id);
            EditViewModel appUser = new EditViewModel();
            appUser.ContactNo = applicationUser.PhoneNumber;
            appUser.Email = applicationUser.Email;
            appUser.FirstName = DataHelper.ToPascalConvention(applicationUser.FirstName);
            appUser.LastName = DataHelper.ToPascalConvention(applicationUser.LastName);
            appUser.Password = applicationUser.Password;
            appUser.RoleName = role_name;
            appUser.UserId = applicationUser.Id;
            appUser.ConfirmPassword = applicationUser.Password;           
            appUser.selectedProduct = DBHandler.getUserProductsByUserId(applicationUser.Id);
            appUser.Products = DBHandler.GetProducts();
            appUser.RoleId = db.Roles.Where(r => r.Name.ToLower() == role_name.ToLower()).First().Id;
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", appUser.RoleId);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AS)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                try
                {
                    var u_manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var currentUser = u_manager.FindById(model.UserId);
                    if (currentUser != null)
                    {
                        string userFullName = DataHelper.ToPascalConvention(model.FirstName + " " + model.LastName);
                        currentUser.UserName = model.Email;
                        currentUser.FirstName = model.FirstName;
                        currentUser.LastName = model.LastName;
                        currentUser.FullName = userFullName;
                        currentUser.PhoneNumber = model.ContactNo;
                        currentUser.Email = model.Email;
                        //  currentUser.PasswordHash = Crypto.HashPassword(model.Password);                      
                        db.Entry(currentUser).State = EntityState.Modified;
                        db.SaveChanges();
                        
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                        var role = roleManager.FindById(model.RoleId);
                        if (role != null)
                        {
                            u_manager.RemoveFromRole(model.UserId, model.RoleName);
                            u_manager.AddToRole(model.UserId, role.Name);
                            db.SaveChanges();
                        }
                        List<Product> listProducts = new List<Product>();
                        int userProductId = DBHandler.UserProductId(model.UserId);
                        ManagerProduct objManagerProduct = new ManagerProduct();
                          
                        if (userProductId > 0)
                        {                          
                                         
                            objManagerProduct = db.ManagerProducts.Find(userProductId);
                            objManagerProduct.ProductIds = model.selectedProduct;         
                            objManagerProduct.ApplicationUserId = objManagerProduct.ApplicationUserId;
                            objManagerProduct.UpdatedOn = DateTime.Now;
                            objManagerProduct.CreatedOn = objManagerProduct.CreatedOn;
                            db.Entry(objManagerProduct).State = EntityState.Modified;
                            db.SaveChanges();  
                        }
                        else
                        {
                            objManagerProduct.UpdatedOn = DateTime.Now;
                            objManagerProduct.ProductIds = model.selectedProduct;         
                            objManagerProduct.ApplicationUserId = model.UserId;
                            objManagerProduct.CreatedOn = DateTime.Now;
                            db.ManagerProducts.Add(objManagerProduct);
                            db.SaveChanges();
                        }
                       
                        DBHandler.SaveUserLogged(currentUser, model.RoleId, role.Name, UserLoggedActions.UserUpadated);
                        return RedirectToAction("Index", "User");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", model.RoleId);
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string id = null)
        {
            ForgotPasswordViewModel objForgotPasswordViewModel = new ForgotPasswordViewModel();
            if (id != null)
            {
                objForgotPasswordViewModel.Email = id;
                return View(objForgotPasswordViewModel);
            }
            else { return View(); }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null && !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    try
                    {
                        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        MailManager.sendPasswordResetEmail("Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>", user.Email);
                        return View("ForgotPasswordConfirmation");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Please! Try again.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User does not exist.");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation(string id = null)
        {
            if (id != null)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var user = db.Users.Where(u => u.Email == id).First();
                return View(user);
            }
            else { return View(); }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId = null, string resetPw = null)
        {
            ResetPasswordViewModel objResetPasswordViewModel = new ResetPasswordViewModel();
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Find(userId);
            try
            {
                if (user != null)
                {
                    if (resetPw == "true")
                    {
                        if (Session["CurrentUser"] == null)
                        {
                            Session["CurrentUser"] = User.Identity.Name;
                        }
                    }
                    objResetPasswordViewModel.NewPassword = string.Empty;
                    objResetPasswordViewModel.Email = user.Email;
                    return code == null ? View("Error") : View(objResetPasswordViewModel);
                }
                return View("Error");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User does not exist.");
                return View();
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.NewPassword);
            if (Session["CurrentUser"] != null)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var currentuser = db.Users.Find(user.Id);
                if (currentuser.LoginCounter + 1 == 1)
                {
                    currentuser.Password = model.Password;
                    currentuser.LoginCounter = currentuser.LoginCounter + 1;
                    db.Entry(currentuser).State = EntityState.Modified;
                    db.SaveChanges();
                    currentuser.PasswordHash = DBHandler.getHashPW(currentuser.Id);
                    var UserRole = UserManager.GetRoles(user.Id);
                    DBHandler.SaveUserLogged(user, DBHandler.GetRoleIdByRoleName(UserRole[0].ToString()), UserRole[0].ToString(), UserLoggedActions.PasswordReset);
                    if (result.Succeeded)
                    {
                        Session.Remove("CurrentUser");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                if (result.Succeeded)
                {
                    string Id = @model.Email;
                    return RedirectToAction("ResetPasswordConfirmation", "Account", user);
                }
                else { return View("Error"); }
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation(ApplicationUser user)
        {
            if (user != null)
            {
                ViewBag.Email = user.Email;
                return View(user);
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            if (ModelState.IsValid)
            {
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                AuthenticationManager.SignOut();
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Account", "Login");
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            return callbackUrl;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}