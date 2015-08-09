using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P_CMS.ViewModels
{
    public class IssuesSearchedCount
    {
       
        public int ClientId{ get; set; }
        public string ClientName { get; set; }
        public int HighCount { get; set; }
        public int MediumCount { get; set; }
        public int LowCount { get; set; }
        public int UnassignedCount { get; set; }
        public int AssignedCount { get; set; }
        public int ReassignedByTUserCount { get; set; }
        public int ReassignedByManagerCount { get; set; }
        public int ReassignedBySuperUserCount { get; set; }
        public int ClosedCount { get; set; }

        public int AssignedId { get; set; }

        public int ClosedId { get; set; }

        public int UnAssignedId { get; set; }

        public int ReassignedByTUserId { get; set; }

        public int ReassignedBySuperUserId { get; set; }

        public int ReassignedByManagerId { get; set; }

        public int HighId { get; set; }

        public int MediumId { get; set; }

        public int LowId { get; set; }

        public int ProductId { get; set; }

      

        public string Date { get; set; }
    }
}
