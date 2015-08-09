using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WebForms;
using P_CMS.Models;
using P_CMS.UtilityClasses;
using P_CMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace P_CMS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        [Authorize]
        [Authorize(Roles = AppRoles.AS)]
        public ActionResult AllIssueReport()
        {
            string currentRole = "";
            if (User.IsInRole(AppRoles.TUSER))
            {
                currentRole = AppRoles.TUSER;
            }
            List<Product> allProducts = DBHandler.GetProducts();
            List<Status> allStatus = DBHandler.GetStatus(currentRole);
            List<Priority> allPriority = DBHandler.GetPriorities();
            List<Client> allClients = DBHandler.GetClients();
            ViewBag.ClientId = new SelectList(allClients, "ClientId", "Name");
            ViewBag.Statusid = new SelectList(allStatus, "StatusId", "StatusType");
            ViewBag.PriorityId = new SelectList(allPriority, "PriorityId", "PriorityType");
            ViewBag.ProductId = new SelectList(allProducts, "ProductId", "ProductName");           
            List<TaskModels> rptSource = DBHandler.getAllTasks();
            DataSet ds = DBHandler.getDataSetFrfomTaskModelList(rptSource);
            Session["ReportSource"] = ds;
            return View();
        }

        [Authorize(Roles = AppRoles.AS)]
        public List<TaskModels> GeneralReport(string ClientId = null, string PriorityId = null, string ProductId = null, string StatusId = null, string DateTo = null, string DateFrom = null)
        {
          //  Session["ReportSource"]=null;
            string currentUserId = "";
            bool isTUSerRole = User.IsInRole(AppRoles.TUSER);         
            List<TaskModels> list_Issues = new List<TaskModels>();
            list_Issues = DBHandler.ReportIssues(isTUSerRole, currentUserId, ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom);
            if (list_Issues.Count() > 0)
            {
                DataSet ds = DBHandler.getDataSetFrfomTaskModelList(list_Issues);
                Session["ReportSource"] = ds;
                return list_Issues;
            }
            else { return null; }           
        }       
    }
}
