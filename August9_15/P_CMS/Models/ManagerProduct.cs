using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class ManagerProduct
    {
        public int ManagerProductId { get; set; }
        public string ProductIds { get; set; }
        public string ApplicationUserId { get; set; }
      
        public System.DateTime CreatedOn { get; set; }
      
        public System.DateTime UpdatedOn { get; set; }
       
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}