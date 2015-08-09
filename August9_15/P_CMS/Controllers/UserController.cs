using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P_CMS.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using P_CMS.ViewModel;
using System.Web.Services;
using P_CMS.UtilityClasses;

namespace P_CMS.Controllers
{
    [Authorize(Roles = AppRoles.AS)]
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var all_users = db.Users.Where(u => u.Email != "admin@pronet-tech.net" && u.IsActive==true).ToList();
            var appuers = from u in all_users.OrderBy(u=>u.FirstName) select  new AppUser { Id = u.Id, ContactNo = u.PhoneNumber, FirstName =DataHelper.ToPascalConvention(u.FirstName+" "+u.LastName), EmailAddress = u.Email };
            return View(appuers);
        }

        [Authorize(Roles = AppRoles.AS)]       
        public ActionResult Details(string id)
        {
            RegisterViewModel objRegisterViewModel = new RegisterViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var store = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser applicationUser = userManager.FindById(id);            
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            string role_name = userManager.GetRoles(id)[0];
            objRegisterViewModel.ContactNo = applicationUser.PhoneNumber;
            objRegisterViewModel.FirstName =DataHelper.ToPascalConvention(applicationUser.FirstName);
            objRegisterViewModel.LastName =DataHelper.ToPascalConvention(applicationUser.LastName);
            objRegisterViewModel.Email = applicationUser.Email;
            objRegisterViewModel.Password = applicationUser.Password;
            objRegisterViewModel.selectedProduct = DBHandler.getUserProductsByUserId(applicationUser.Id);
            objRegisterViewModel.Products = DBHandler.GetProducts();
            objRegisterViewModel.RoleId = applicationUser.Id;
            objRegisterViewModel.RoleName = role_name;
            return View(objRegisterViewModel);
        }
     

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var store = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser applicationUser = userManager.FindById(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            RegisterViewModel objRegisterViewModel = new RegisterViewModel();
            objRegisterViewModel.ContactNo = applicationUser.PhoneNumber;
            objRegisterViewModel.FirstName = DataHelper.ToPascalConvention(applicationUser.FirstName);
            objRegisterViewModel.LastName = DataHelper.ToPascalConvention(applicationUser.LastName);
            objRegisterViewModel.selectedProduct = DBHandler.getUserProductsByUserId(applicationUser.Id);
            objRegisterViewModel.Products = DBHandler.GetProducts();
            objRegisterViewModel.Email = applicationUser.Email;
            objRegisterViewModel.Password = applicationUser.Password;
            objRegisterViewModel.RoleId = applicationUser.Id;
            return View(objRegisterViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var store = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser applicationUser = userManager.FindById(id);
            applicationUser.IsActive = false;
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();
           // userManager.Delete(applicationUser);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [WebMethod]
        public bool AddRole(string role)
        {
            if (!string.IsNullOrEmpty(role.Trim()))
            {
                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                roleManager.Create(new IdentityRole { Name = role.Trim() });

                return true;
            }
            return false;
        }
    }
}
