using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }
        public string DesignationName{ get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers{ get; set; }
    }
}