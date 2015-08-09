using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class Issue
    {
        public string timeCounter{ get; set; }
        public int IssueId { get; set; }
        public int ProductId { get; set; }
        public int ClientId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public int TagId { get; set; }
          [DisplayName("Task Code")]
        public string IssueCode { get; set; }
        public string UploadedFileName { get; set; }
        public string Description { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
       
        [DisplayName("Assigned To")]
        public string ApplicationUserId { get; set; }
         [DisplayName("Created On")]
        public System.DateTime CreatedOn { get; set; }
         [DisplayName("Assigned By")]
         public string AssignedBy { get; set; }
        [DisplayName("Assigned On")]
         public System.DateTime AssignedOn { get; set; }    
       
        [DisplayName("Last Updated By")]
         public string UpdatedBy { get; set; }
        [DisplayName("Last Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }    
        public virtual ICollection<AssignedTask> AssignedTasks { get; set; }
        public virtual Client Client { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Product Product { get; set; }
        public virtual Status Status { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}