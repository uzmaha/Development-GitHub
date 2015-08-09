namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tag
    {     
        public int TagId { get; set; }
        public string TagValue { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }
    }
}
