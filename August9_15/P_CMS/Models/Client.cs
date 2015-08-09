
namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Client
    {     
    
        public int ClientId { get; set; }
        public Nullable<int> ClientCompanyId { get; set; }
        public Nullable<int> CityId { get; set; }
        public string ApplicationUserId { get; set; }
         [Display(Name = "Client")]
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Description { get; set; }
        public string color { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual City City { get; set; }
        public virtual ClientCompany ClientCompany { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
