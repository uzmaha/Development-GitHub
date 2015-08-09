using P_CMS.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public StatusType Status{ get; set; }
    }
}