namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Status
    {    
    
        public int StatusId { get; set; }
        public string StatusType { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }
    }
}
