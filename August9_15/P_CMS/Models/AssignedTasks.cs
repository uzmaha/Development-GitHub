using P_CMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class AssignedTasks
    {
        [DisplayName("Assigned By")]
        public string AssignedBy { get; set; }
        [DisplayName("Assigned On")]
        public System.DateTime AssignedOn { get; set; }  
        public string timeCounter { get; set; }
        public String TaskTag { get; set; }
        public Nullable<int> IssueId { get; set; }
      //  public string isProductChanged{ get; set; }
        public int CategoryId { get; set; }
       [Required(ErrorMessage = "Select resource")]
        public string userid { get; set; }
        
        public string Id { get; set; }

         [Required(ErrorMessage = "Select Product")]
        public int ProductId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Status status { get; set; }
        public List<Product> Products { get; set; }
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string UploadedFileName { get; set; }
        public Nullable<int> PriorityId { get; set; }
        public string statusType { get; set; }
        public string FirstName { get; set; }
        public int StatusId { get; set; }
        public Nullable<int> IssueTagId { get; set; }
        public string IssueCode { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public string PrevDescription { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public virtual Client Client { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual Priority Priority { get; set; }



        public string selectedProduct { get; set; }
    }
}