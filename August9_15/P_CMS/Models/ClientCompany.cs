

namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class ClientCompany
    {
      
        [Required]
        public int ClientCompanyId { get; set; }
      
        public string Name { get; set; }
        public string CompanyType { get; set; }
        public string ContactNo { get; set; }
        public string EmailAddress { get; set; }
        public string Fax { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
      
        public virtual ICollection<Client> Clients { get; set; }
    }
}
