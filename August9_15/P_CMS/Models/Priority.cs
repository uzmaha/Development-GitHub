

namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Priority
    {     
        public int PriorityId { get; set; }
        public string PriorityType { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }
    }
}
