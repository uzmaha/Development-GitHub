
using P_CMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class ChangeStatusModel
    {
        public String TaskTag { get; set; }
       
        public int IssueId { get; set; }
      
        public int CategoryId { get; set; }

        public string UploadedFileName { get; set; }
        public string UserId { get; set; }
        public string ApplicationUserId { get; set; }
          
        public string StatusType { get; set; }
        public string Tag { get; set; }


        public int id { get; set; }
        
        public int ProductId { get; set; }
        [Display(Name="Close Task")]
        public string isClosed{ get; set; }
        public int ClientId { get; set; }

        public int PriorityId { get; set; }
        public string status { get; set; }
        public string ClientName { get; set; }
        public string AssignedTo { get; set; }
       
       
        public int StatusId { get; set; }
        
        public int IssueTagId { get; set; }
        public string ProductName { get; set; }
        public string PriorityType { get; set; }
        public string IssueCode { get; set; }
       
        [Required(ErrorMessage = "Description is required")]
        [Display(Name="New Comments")]
        public string Description { get; set; }
       
        public string PrevDescription { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime? UpdatedOn { get; set; }
      
    }
}