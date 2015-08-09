using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using P_CMS.Models;
using P_CMS.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_CMS.BAL
{
    public class AppUserManager
    {
       // private ApplicationDbContext db = new ApplicationDbContext();
        internal static void SeedUser()
        {
            ApplicationDbContext context = new ApplicationDbContext();
           
                if (!context.Users.Any(u => u.UserName == "admin@pronet-tech.net"))
                {
                    var manager = GetUserManager();
                    var user = new ApplicationUser { UserName = "admin@pronet-tech.net", FirstName = "admin", LastName = "admin",IsActive=true, Password = "admin123", Email = "admin@pronet-tech.net", PhoneNumber = "0333333434", PasswordHash = "admin123",LoginCounter=1 };
                    var result = manager.Create(user, user.Password);
                    if (!context.Roles.Any(r => r.Name == AppRoles.ADMINISTRATOR))
                    {
                        var roleStore = new RoleStore<IdentityRole>(context);
                        var roleManager = new RoleManager<IdentityRole>(roleStore);
                        roleManager.Create(new IdentityRole { Name = AppRoles.ADMINISTRATOR });
                    }
                    manager.AddToRole(user.Id, AppRoles.ADMINISTRATOR);
                }
            
        }

        private static ApplicationUserManager GetUserManager()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            return manager;
        }

        internal static void SeedApplication()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            // seeding status
            if (db.Status != null && db.Status.Count() == 0)
            {
                db.Status.AddRange(new List<Status>() { 
            new Status{StatusType ="Process"},
            new Status{StatusType ="Pending"},
            new Status{StatusType ="Assigned"},
            new Status{StatusType ="ReAssigned By Manager"},
            new Status{StatusType ="UnAssigned"},
            new Status{StatusType ="Completed"},
            new Status{StatusType ="Closed"},  
             new Status{StatusType ="ReAssigned By TUser"},
              new Status{StatusType ="ReAssigned By SuperUser"}
            });
                db.SaveChanges();
            }

            if (db.Countries != null && db.Countries.Count() == 0)
            {
                db.Countries.AddRange(new List<Country>() { 
            new Country{CountryName ="Pakistan"},            
            });
                db.SaveChanges();
            }

            if (db.Cities != null && db.Cities.Count() == 0)
            {
                db.Cities.AddRange(new List<City>() { 
            new City{CityName ="Karachi"},            
            new City{CityName ="Islamabad"},            
            new City{CityName ="Lahore"},            
            });
                db.SaveChanges();
            }

            if (db.Priorities != null && db.Priorities.Count() == 0)
            {
                db.Priorities.AddRange(new List<Priority>() { 
            new Priority{PriorityType ="High"},            
            new Priority{PriorityType ="Medium"},            
            new Priority{PriorityType ="Low"},            
            });
                db.SaveChanges();
            }

            if (db.ClientCompanies != null && db.ClientCompanies.Count() == 0)
            {
                db.ClientCompanies.AddRange(new List<ClientCompany>() { 
            new ClientCompany{Name ="Faisal Bank",CompanyType="Bank",ContactNo="03534564",CreatedOn=DateTime.Now,Description="It is a bank"},            
            new ClientCompany{Name ="Alfalah Bank",CompanyType="Bank",ContactNo="03534564",CreatedOn=DateTime.Now,Description="It is a bank"},            
            new ClientCompany{Name ="HBL Bank",CompanyType="Bank",ContactNo="03534564",CreatedOn=DateTime.Now,Description="It is a bank"},            
            });
                db.SaveChanges();
            }

            if (db.Clients != null && db.Clients.Count() == 0)
            {
                int cityId=db.Cities.Select(c => c).ToList()[0].CityId;
                db.Clients.AddRange(new List<Client>() { 
            new Client{Name ="HBL",ApplicationUserId=null, CityId=cityId,ClientCompanyId=3,ContactNo="03534564",CreatedOn=DateTime.Now,Description="It is a bank"},            
            new Client{Name ="FBL",ApplicationUserId=null, CityId=cityId,ClientCompanyId=1,ContactNo="03534564",CreatedOn=DateTime.Now,Description="It is a bank"},            
            new Client{Name ="AFL",ApplicationUserId=null, CityId=cityId,ClientCompanyId=2,ContactNo="03534564",CreatedOn=DateTime.Now,Description="It is a bank"},            
            });
                db.SaveChanges();
            }

            if (db.Tags != null && db.Tags.Count() == 0)
            {
                db.Tags.AddRange(new List<Tag>() { 
            new Tag{TagValue ="IVR Setup"},            
            new Tag{TagValue ="Avaya Support"},
            });
                db.SaveChanges();
            }
            if (db.Products != null && db.Products.Count() == 0)
            {
                db.Products.AddRange(new List<Product>() { 
            new Product{ProductName ="Avaya Blue"},
            new Product{ProductName ="EMC"},
            new Product{ProductName ="Oracle Applications"},
            new Product{ProductName ="FIS"},
            new Product{ProductName ="Barracuda"},
            new Product{ProductName ="Juniper"},
            new Product{ProductName ="Fortinet"}, 
            new Product{ProductName ="Kaspersky"},
            new Product{ProductName ="SAFNet"}
            });
                db.SaveChanges();
            }
        }
    }
}