using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P_CMS.Models
{
    public class EmailModel
    {


        public string EmailFrom { get; set; }

        public string EmailSubject { get; set; }
        [Required]
        public string EmailBody { get; set; }
    }
}