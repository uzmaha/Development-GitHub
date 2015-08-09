using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;

namespace P_CMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
     
      
      
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public int LoginCounter{ get; set; }
        public Nullable<int> DesignationId { get; set; }
       
        public virtual Designation Designation { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("CS", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<AssignedTask> AssignedTasks{ get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<ClientCompany> ClientCompanies { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ManagerProduct> ManagerProducts{ get; set; }
        public DbSet<Project> Project { get; set; }

       public DbSet<UserLogged> UserLogged { get; set; }
        
        public DbSet<Designation> Designations { get; set; }
  
    }
}