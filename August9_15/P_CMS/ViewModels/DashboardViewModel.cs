using P_CMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P_CMS.ViewModels
{
    public class DashboardViewModel
    {
        // public int ClientId { get; set; }
        //public string ClientName { get; set; }

      

















        //////

       
       
       
        
        
      
       
        public int ProductId { get; set; }
       
      
       
        public int PriorityId { get; set; }
     
      
        public int TagId { get; set; }
       
        public string TaskCode { get; set; }
       
        public string StatusType { get; set; }
        
     
       
         
        
       
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Created On")]
        public System.DateTime CreatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }
        [Display(Name = "Updated On")]
        public System.DateTime? UpdatedOn { get; set; }       
     
        public string ProductName { get; set; }      

        public string PriorityType { get; set; }

        public string IssueTag1 { get; set; }

        public string Status { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }

        public String IssueCode { get; set; }
        public IList<TaskModels> Issues { get; set; }
        public IList<IssuesSearchedClient> IssuesSearchClientCounter { get; set; }
        
        public IList<IssuesSearched> IssuesSearchCounter { get; set; }
        public IList<IssuesSearchedCount> IssuesSearchCounts { get; set; }
       
    }
}