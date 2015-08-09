namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {     
    
        public int ProductId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
       
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
