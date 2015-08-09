
namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AssignedTask
    {      
        public int AssignedTaskId { get; set; }       
        public Nullable<int> IssueId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Description { get; set; }
        public  string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string statusType { get; set; }       
        public virtual Issue Issue{ get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
