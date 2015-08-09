using P_CMS.Models;
using P_CMS.UtilityClasses;
using P_CMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using P_CMS.BAL;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Configuration;
using System.Web.Services;


namespace P_CMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        List<Issue> allIssues = DBHandler.GetIssues();
        List<ApplicationUser> allUsers = DBHandler.GetUsers();
        List<Product> allProducts = DBHandler.GetProducts();
        // List<Status> allStatus = DBHandler.GetStatus();
        List<Priority> allPriority = DBHandler.GetPriorities();
        List<Client> allClients = DBHandler.GetClients();
        List<Tag> allTags = DBHandler.GetTags();
        [Authorize]

        public ActionResult Index(string ClientId = null, string PriorityId = null, string ProductId = null, string StatusId = null, string DateTo = null, string DateFrom = null)
        {
            string currentRole = "";
            if (User.IsInRole(AppRoles.TUSER))
            {
                currentRole = AppRoles.TUSER;
            }
            List<Status> allUserStatus = DBHandler.GetStatus(currentRole);
            DashboardViewModel objDashboardViewModel = new DashboardViewModel();
            ViewBag.ClientId = new SelectList(allClients, "ClientId", "Name");
            ViewBag.Statusid = new SelectList(allUserStatus, "StatusId", "StatusType");
            ViewBag.PriorityId = new SelectList(allPriority, "PriorityId", "PriorityType");
            ViewBag.ProductId = new SelectList(allProducts, "ProductId", "ProductName");
            ViewBag.TagId = new SelectList(allProducts, "TagId", "TagValue");
            objDashboardViewModel = DashBoard(ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom);
            return View(Json(objDashboardViewModel));
        }

        public DashboardViewModel DashBoard(string ClientId = null, string PriorityId = null, string ProductId = null, string StatusId = null, string DateTo = null, string DateFrom = null)
        {
            string currentRole = "";
            DashboardViewModel objDashboardViewModel = new DashboardViewModel();
            string currentUserId = User.Identity.GetUserId();
            string assingedRoles = string.Empty;
            List<TaskModels> taskModel = DBHandler.getAllTasks();
            if (User.IsInRole(AppRoles.TUSER))
            {
                currentRole = AppRoles.TUSER;
                taskModel = taskModel.Where(i => i.AssignedToId == currentUserId).ToList();
                // allStatus = allStatus.Where(s => s.StatusType.ToLower() == AppStatus.ASSIGNED || s.StatusType.ToLower() == AppStatus.CLOSED || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYTUSER || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYMANAGER || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYSUPERUSER).ToList();
            }
            //else if (User.IsInRole(AppRoles.MANAGER))
            //{
            //    taskModel = taskModel.Where(i => i.CreatedOn.Date<DateTime.Now.Date).ToList();
            //}
            List<Status> allUserStatus = DBHandler.GetStatus(currentRole);
            taskModel = DBHandler.getAllFilteredTasks(taskModel, ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom);
            objDashboardViewModel.IssuesSearchCounts = DBHandler.getSearchIssuesCount(taskModel, ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom);
            objDashboardViewModel.IssuesSearchClientCounter = DBHandler.getSearchClientIssuesCounts(taskModel, ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom);
            IssuesSearched objIssuesSearched = new IssuesSearched();
            List<IssuesSearched> lst_IssuesSearched = new List<IssuesSearched>();
            List<IssuesSearchedClient> lst_IssuesSearchedClient = new List<IssuesSearchedClient>();
            int unassignedcount = 0;
            int assignedcount = 0;
            int closedcount = 0;
            int reassignedbyTUcount = 0;
            int reassignedbyMcount = 0;
            int reassignedbySUcount = 0;
            foreach (var item in objDashboardViewModel.IssuesSearchCounts)
            {
                unassignedcount = unassignedcount + item.UnassignedCount;
                assignedcount = assignedcount + item.AssignedCount;
                closedcount = closedcount + item.ClosedCount;
                reassignedbyTUcount = reassignedbyTUcount + item.ReassignedByTUserCount;
                reassignedbyMcount = reassignedbyMcount + item.ReassignedByManagerCount;
                reassignedbySUcount = reassignedbySUcount + item.ReassignedBySuperUserCount;
            }
            string projectName = ((unassignedcount == 0) && (assignedcount == 0) && (closedcount == 0) && (reassignedbyTUcount == 0) && (reassignedbyMcount == 0) && (reassignedbySUcount == 0)) ? "False" : objDashboardViewModel.IssuesSearchCounts[0].ClientName;
            int assignedId = DBHandler.GetStatusIdByType(AppStatus.ASSIGNED);
            int closedId = DBHandler.GetStatusIdByType(AppStatus.CLOSED);
            int unAssignedId = DBHandler.GetStatusIdByType(AppStatus.UNASSIGNED);
            int reassignedByTUserId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYTUSER);
            int reassignedBySuperUserId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYSUPERUSER);
            int reassignedByManagerId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYMANAGER);
            lst_IssuesSearched.Add(new IssuesSearched { IssueTitle = "Unassigned", IssueCount = unassignedcount == 0 ? "" : unassignedcount.ToString(), projectType = projectName, statusId = unAssignedId, color = "#ff0000" });
            lst_IssuesSearched.Add(new IssuesSearched { IssueTitle = "Assigned", IssueCount = assignedcount == 0 ? "" : assignedcount.ToString(), projectType = "", statusId = assignedId, color = "#9de219" });
            lst_IssuesSearched.Add(new IssuesSearched { IssueTitle = "Closed", IssueCount = closedcount == 0 ? "" : closedcount.ToString(), projectType = "", statusId = closedId, color = "#006634" });
            lst_IssuesSearched.Add(new IssuesSearched { IssueTitle = "Tech-User Reassigned", IssueCount = reassignedbyTUcount == 0 ? "" : reassignedbyTUcount.ToString(), projectType = "", statusId = reassignedByTUserId, color = "#7c7c7c" });
            lst_IssuesSearched.Add(new IssuesSearched { IssueTitle = "Manager Reassigned", IssueCount = reassignedbyMcount == 0 ? "" : reassignedbyMcount.ToString(), projectType = "", statusId = reassignedByManagerId, color = "yellow" });
            lst_IssuesSearched.Add(new IssuesSearched { IssueTitle = "SuperUser Reassigned", IssueCount = reassignedbySUcount == 0 ? "" : reassignedbySUcount.ToString(), projectType = "", statusId = reassignedBySuperUserId, color = "black" });
            objDashboardViewModel.IssuesSearchCounter = lst_IssuesSearched;
            return objDashboardViewModel;
        }

        public JsonResult DashBoardPieChart(string ClientId = null, string PriorityId = null, string ProductId = null, string StatusId = null, string DateTo = null, string DateFrom = null)
        {
            List<DashboardViewModel> listDashboardViewModel = new List<DashboardViewModel>();
            listDashboardViewModel.Add(DashBoard(ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom));
            IList<DashboardViewModel> list = listDashboardViewModel;
            return Json(list);
        }

        public ActionResult PageNotFound()
        {
            ViewBag.Message = "Page Not Found!!";
            return View();
        }
    }
}