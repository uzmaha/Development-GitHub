
using P_CMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class TaskModels
    {


        public int IssueId { get; set; }
        [Required(ErrorMessage="Select Client")]
        [Display(Name = "By Client")]
        public int ClientId { get; set; }
        
        [Required(ErrorMessage = "Select Product")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
       
        [Required(ErrorMessage = "Select Priority")]
        [Display(Name = "Priority")]
        public int PriorityId { get; set; }
       
        [Required(ErrorMessage = "Select Issue Tag")]
        [Display(Name = "Issue Tag")]
        public int TagId { get; set; }
        [Display(Name = "Task Code")]
        public string TaskCode { get; set; }
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        public string StatusType { get; set; }
        public string FullName { get; set; }
        public string UploadedFileName { get; set; }
        public string ClientName { get; set; }
          [Display(Name = "Description")]
         [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Created On")]
        public System.DateTime CreatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }
        [Display(Name = "Last Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public virtual Client Client { get; set; }
        public virtual Product Product { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual ICollection<AssignedTasks> AssignedTasks { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ProductName { get; set; }
      

        public string PriorityType { get; set; }

        public string IssueTag1 { get; set; }

        public string Status { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }

        public String IssueCode { get; set; }
        public String AssignedToId { get; set; }
    }
}