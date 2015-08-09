using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P_CMS.ViewModels
{
    public class IssuesSearched
    {
        public string IssueTitle { get; set; }

        public string IssueCount { get; set; }

        public string projectType { get; set; }
        public string color { get; set; }
        public int statusId { get; set; }
    }
    public class IssuesSearchedClient
    {
        public string IssueTitle { get; set; }

        public string IssueCount { get; set; }

        public string projectType { get; set; }
        public string color { get; set; }
        public int statusId { get; set; }

        public int clientid { get; set; }
    }
}
