using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P_CMS.Models;
using P_CMS.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using P_CMS.UtilityClasses;
using P_CMS.BAL;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Configuration;
using P_CMS.ViewModels;

namespace P_CMS.Controllers
{
    // [RoutePrefix("pronetCMS")]

    public class IssueController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        List<Issue> allIssues = DBHandler.GetIssues();
        List<ApplicationUser> allUsers = DBHandler.GetUsers();
        // List<Status> allStatus = DBHandler.GetStatus();
        List<Client> allClients = DBHandler.GetClients();

        [Authorize]
        //  [Route("users/{id:int:min(100)}")]

        public ActionResult Index(int id = 0, int statusId = 0, int clientid = 0, int priorityid = 0, int productid = 0, string date = null)
        {
            string currentRole = "";
            string currentUserId = User.Identity.GetUserId();
            string assingedRoles = string.Empty;
            List<TaskModels> taskModel = DBHandler.getAllTasks();
            if (User.IsInRole(AppRoles.TUSER))
            {
                currentRole = AppRoles.TUSER;
                taskModel = taskModel.Where(i => i.AssignedToId == currentUserId).ToList();
            }
            if (id == -3)
            { id = -2;
                taskModel = taskModel.Where(i => i.StatusType.ToLower() !=AppStatus.CLOSED).ToList();
            }
            if (id == -2)
            {
                try
                {                   
                    string todate = string.Empty;
                    string fromdate = string.Empty;
                    if ((!string.IsNullOrEmpty(date)) && (date != "null"))
                    {
                        DataHelper.filterDates(date, out fromdate, out todate);
                        if (!string.IsNullOrEmpty(todate) || !string.IsNullOrEmpty(fromdate))
                        {
                            if (!string.IsNullOrEmpty(todate) && string.IsNullOrEmpty(fromdate))
                            {
                                todate = Convert.ToDateTime(todate).AddDays(1).ToString();
                                taskModel = taskModel.Where(i => i.CreatedOn <= Convert.ToDateTime(todate)).ToList();
                            }
                            if (!string.IsNullOrEmpty(fromdate) && string.IsNullOrEmpty(todate))
                            {
                               // fromdate = Convert.ToDateTime(fromdate).AddDays(-1).ToString();
                                taskModel = taskModel.Where(i => i.CreatedOn >= Convert.ToDateTime(fromdate)).ToList();
                            }

                            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
                            {
                                if (fromdate == todate)
                                {
                                    taskModel = taskModel.Where(i => i.CreatedOn.ToShortDateString() == Convert.ToDateTime(todate).ToShortDateString()).ToList();
                                }
                                else
                                {
                                    fromdate = Convert.ToDateTime(fromdate).AddDays(-1).ToString();
                                    todate = Convert.ToDateTime(todate).AddDays(1).ToString();
                                    taskModel = taskModel.Where(i => i.CreatedOn <= Convert.ToDateTime(todate) && i.CreatedOn >= Convert.ToDateTime(fromdate)).ToList();
                                }
                            }
                        }
                    }
                    if (statusId > 0)
                    {
                        taskModel = (from t in taskModel.Where(t => t.StatusId == statusId) select t).ToList();
                    }
                    if (priorityid > 0)
                    {
                        taskModel = (from t in taskModel.Where(t => t.PriorityId == priorityid) select t).ToList();
                    }
                    if (productid > 0)
                    {
                        taskModel = (from t in taskModel.Where(t => t.ProductId == productid) select t).ToList();
                    }
                    if (clientid > 0)
                    {
                        taskModel = (from t in taskModel.Where(t => t.ClientId == clientid) select t).ToList();
                    }
                }
                catch (Exception)
                {
                    taskModel = null;
                }
            }
            else
            {
                if (statusId > 0 && (id != 0))
                {
                    taskModel = (from t in taskModel.Where(t => t.StatusId == statusId) select t).ToList();
                }
                else if (User.IsInRole(AppRoles.TUSER))
                {
                    taskModel = (from t in taskModel.Where(t => t.StatusType.ToLower() != AppStatus.CLOSED) select t).ToList();
                }
                else
                {
                    taskModel = (from t in taskModel.Where(t => t.StatusType.ToLower() == AppStatus.UNASSIGNED) select t).ToList();
                }
            } 
           
            if (id == 0)
            {
                if (User.IsInRole(AppRoles.TUSER))
                {

                   // statusId = DBHandler.GetStatusIdByType(AppStatus.ASSIGNED);
                }
                else { statusId = DBHandler.GetStatusIdByType(AppStatus.UNASSIGNED); }
            }
           // else if (id != -2 && id != -1) { taskModel = taskModel.Where(t => t.IssueId == id).ToList(); }
            ViewBag.StatusType = DBHandler.GetStatusTypeById(statusId) == "" ? DBHandler.GetClientById(clientid) : DBHandler.GetStatusTypeById(statusId);
            List<Status> allUserStatus = DBHandler.GetStatus(currentRole);
            ViewBag.StatusId = new SelectList(allUserStatus, "StatusId", "StatusType", statusId);
            return View(taskModel);
        }

        [Authorize(Roles = AppRoles.MTS)]
        public ActionResult AssignedTask(int id = 0, int mode = 0)
        {
            string currentUserId = User.Identity.GetUserId();
            Issue assignedtask = DBHandler.GetIssueById(id);
            if (assignedtask == null)
            {
                return HttpNotFound();
            }
            AssignedTasks taskAssigned = new AssignedTasks();
            taskAssigned.TaskTag = assignedtask.Tag.TagValue;
            taskAssigned.statusType = assignedtask.Status.StatusType;
            taskAssigned.StatusId = assignedtask.Status.StatusId;
            taskAssigned.IssueId = assignedtask.IssueId;
            taskAssigned.Client = assignedtask.Client;
            taskAssigned.ClientId = assignedtask.ClientId;
            taskAssigned.PrevDescription = DBHandler.removeStringFromDescription(assignedtask.Description);
            taskAssigned.ProductId = assignedtask.ProductId;
            taskAssigned.Priority = assignedtask.Priority;
            taskAssigned.PriorityId = assignedtask.PriorityId;
            taskAssigned.ApplicationUser = assignedtask.ApplicationUser;
            taskAssigned.userid = assignedtask.ApplicationUserId;
            taskAssigned.IssueTagId = assignedtask.TagId;
            taskAssigned.IssueCode = assignedtask.IssueCode;
            taskAssigned.AssignedTo = assignedtask.ApplicationUser == null ? "Non" : assignedtask.ApplicationUser.FullName;
            taskAssigned.timeCounter = "";
            taskAssigned.Description = "";
          
           
            taskAssigned.UploadedFileName = assignedtask.UploadedFileName;
            string[] filesUploaded = string.IsNullOrEmpty(assignedtask.UploadedFileName) ? null : assignedtask.UploadedFileName.Contains('$') ? assignedtask.UploadedFileName.Split('$') : null;
            List<FilesUploaded> objFilesUploaded = new List<FilesUploaded>();
            if (filesUploaded != null)
            {
                foreach (var item in filesUploaded)
                {
                    string fileUploaded = string.Empty;
                    string fileUserId = string.Empty;
                    string fileName = DataHelper.getFileName(item, out fileUserId, out fileUploaded);
                    objFilesUploaded.Add(new FilesUploaded { name = fileName });
                }
            }
            else if (!string.IsNullOrEmpty(assignedtask.UploadedFileName))
            {
                string fileUploaded = string.Empty;
                string fileUserId = string.Empty;
                string fileName = DataHelper.getFileName(assignedtask.UploadedFileName, out fileUserId, out fileUploaded);
                objFilesUploaded.Add(new FilesUploaded { name = fileName,dbFileName=assignedtask.UploadedFileName });
            }
            if (objFilesUploaded.Count() > 0)
            {
                ViewData["UploadedFiles"] = objFilesUploaded;
            }
            taskAssigned.CreatedBy = DataHelper.ToPascalConvention(assignedtask.CreatedBy);
            taskAssigned.UpdatedBy = DataHelper.ToPascalConvention(assignedtask.UpdatedBy);
            taskAssigned.CreatedOn = assignedtask.CreatedOn;
            taskAssigned.UpdatedOn = assignedtask.UpdatedOn;
            List<ApplicationUser> usersNotInAdminAndMangerRole = new List<ApplicationUser>();
            var roles = db.Roles.Where(m => m.Name.ToLower() != AppRoles.ADMINISTRATOR.ToLower() && m.Name.ToLower() != AppRoles.MANAGER.ToLower() && m.Name.ToLower().ToLower() != AppRoles.SYSTEMUSER.ToLower() && m.Name != AppRoles.SUPERUSER.ToLower()).ToList();
            foreach (var role in roles)
            {
                var users = allUsers.Where(m => m.Roles.Any(r => r.RoleId == role.Id) && m.Id != currentUserId).ToList();
                foreach (var u in users)
                {
                    usersNotInAdminAndMangerRole.Add(new ApplicationUser { Id = u.Id, FullName = u.FullName });
                }
            }
            var allusers = (from u in usersNotInAdminAndMangerRole select new { Id = u.Id, FullName = u.FullName });
            if (id > 0 && (taskAssigned.statusType.ToLower() == AppStatus.ASSIGNED || taskAssigned.statusType.ToLower() == AppStatus.REASSIGNEDBYTUSER || taskAssigned.statusType.ToLower() == AppStatus.REASSIGNEDBYMANAGER || taskAssigned.statusType.ToLower() == AppStatus.REASSIGNEDBYSUPERUSER))
            {
                ViewBag.userid = new SelectList(allusers.Where(u => u.Id != assignedtask.ApplicationUserId).ToList(), "Id", "FullName", assignedtask.ApplicationUserId);
            }
            else if (taskAssigned.statusType.ToLower() == AppStatus.CLOSED)
            {
                ViewBag.userid = new SelectList(allusers, "Id", "FullName", assignedtask.ApplicationUserId);
            }
            else
            {
                ViewBag.userid = new SelectList(allusers, "Id", "FullName");
            } 
            if (User.IsInRole(AppRoles.MANAGER))
            {
                taskAssigned.selectedProduct = DBHandler.getUserProductsByUserId(currentUserId);
            }
            else { taskAssigned.selectedProduct = DBHandler.getUserProductsByUserId(taskAssigned.AssignedBy); }

            //List<Product> listProducts = new List<Product>();
            //if (!string.IsNullOrEmpty(taskAssigned.selectedProduct))
            //{
            //    string[] selectedProducts = taskAssigned.selectedProduct.Split(',');            
            //    listProducts = (from item in db.Products where selectedProducts.Contains(item.ProductId.ToString()) select item).ToList();
            //}
        //    ViewBag.ProductId = new SelectList(listProducts, "ProductId", "ProductName", taskAssigned.ProductId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", taskAssigned.ProductId);
            return View(taskAssigned);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.MTS)]
        // [ValidateAntiForgeryToken]
        public ActionResult AssignedTask(AssignedTasks taskAssigned)
        {
          //  assignedtask.ApplicationUser == null ? "Non" : assignedtask.ApplicationUser.FullName;
            if (ModelState.IsValid)
            {
                AssignedTask obj_assignedtask = new AssignedTask();
                Issue objTask = db.Issues.Find(taskAssigned.IssueId);
                objTask.ProductId = taskAssigned.ProductId;
                objTask.UploadedFileName = taskAssigned.UploadedFileName;
                List<Status> list_Status = db.Status.Select(s => s).ToList();
                string statusType = db.Status.Find(objTask.StatusId).StatusType.ToString().ToLower();
                var user = db.Users.Find(User.Identity.GetUserId());
                string hrLine = "_hrLine_";
                string divStartGreen = "_dsg_";
                string nextLIne = "_newline_";
                string boldStart = "_bs_";
                string boldEnd = "_be_";
                string divStart = "_ds_";
                string divEnd = "_de_";
                string dvStart = "_dstart_";
                string newkDescription = System.Net.WebUtility.HtmlDecode(String.IsNullOrEmpty(taskAssigned.Description) ? "" : taskAssigned.Description);
                string preDescription = objTask.Description;
                int statusId = 0;
                string mailSubject = "";
                var assignedUserName = db.Users.Find(taskAssigned.userid);
                string u_Name = assignedUserName.FullName;
                if (statusType == AppStatus.UNASSIGNED)
                {
                    objTask.Description = divStart + dvStart + hrLine + dvStart + "Task " + objTask.IssueCode + " is assigned by " + DataHelper.ToPascalConvention(user.FullName) + "." + divEnd + hrLine + divEnd + boldStart + "Commented By:" + boldEnd + " " + DataHelper.ToPascalConvention(db.Users.Where(u => u.Email == User.Identity.Name).First().FullName) + nextLIne + boldStart + "Date:" + boldEnd + " " + objTask.UpdatedOn.ToString() + nextLIne + boldStart + "Comments:" + boldEnd + nextLIne + newkDescription + divEnd + preDescription;
                    statusId = list_Status.First(s => s.StatusType.ToLower() == AppStatus.ASSIGNED).StatusId;
                    mailSubject = objTask.IssueCode + " has been assigned to " + u_Name + ".";
                }
                else
                {
                    mailSubject = objTask.IssueCode + " has been re-assigned to " + u_Name + ".";
                    if (User.IsInRole(AppRoles.TUSER))
                    {
                        statusId = list_Status.First(s => s.StatusType.ToLower() == AppStatus.REASSIGNEDBYTUSER).StatusId;
                    }
                    else if (User.IsInRole(AppRoles.MANAGER))
                    {
                        statusId = list_Status.First(s => s.StatusType.ToLower() == AppStatus.REASSIGNEDBYMANAGER).StatusId;
                    }
                    else if (User.IsInRole(AppRoles.SUPERUSER))
                    {
                        statusId = list_Status.First(s => s.StatusType.ToLower() == AppStatus.REASSIGNEDBYSUPERUSER).StatusId;
                    }
                    objTask.Description = divStart + divStartGreen + hrLine + dvStart + "Task " + objTask.IssueCode + " is re-assigned by " + DataHelper.ToPascalConvention(user.FullName) + "." + divEnd + hrLine + divEnd + boldStart + "Commented By:" + boldEnd + " " + DataHelper.ToPascalConvention(db.Users.Where(u => u.Email == User.Identity.Name).First().FullName) + nextLIne + boldStart + "Date:" + boldEnd + " " + objTask.UpdatedOn.ToString() + nextLIne + boldStart + "Comments:" + boldEnd + nextLIne + newkDescription + divEnd + preDescription;
                }
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                taskAssigned.timeCounter = stopwatch.ToString();
                objTask.StatusId = statusId;
                objTask.ApplicationUserId = taskAssigned.userid;
                //  objTask.UploadedFileName = taskAssigned.UploadedFileName;
                objTask.CreatedBy = objTask.CreatedBy;
                objTask.UpdatedBy = User.Identity.GetUserId();
                objTask.CreatedOn = objTask.CreatedOn;
                objTask.UpdatedOn = DateTime.Now;
                db.Issues.Add(objTask);
                //  db.Entry(objTask).State = EntityState.Modified;
                db.SaveChanges();
                var all_issues = db.Issues.ToList();
                var allFilteredIssues = (from p in all_issues
                                         group p by objTask.IssueCode into g
                                         select g.OrderBy(p => p.UpdatedOn).Last()).ToList();
                int? statusID = allFilteredIssues[0].StatusId;
                int issueID = allFilteredIssues[0].IssueId;
                //obj_assignedtask.statusType = db.Status.Find(statusId).Status;
                obj_assignedtask.IssueId = issueID;
                obj_assignedtask.ApplicationUserId = taskAssigned.userid;
                obj_assignedtask.CreatedBy = objTask.CreatedBy;
                obj_assignedtask.UpdatedBy = User.Identity.GetUserId();
                obj_assignedtask.CreatedOn = objTask.CreatedOn;
                obj_assignedtask.UpdatedOn = DateTime.Now;
                db.AssignedTasks.Add(obj_assignedtask);
                db.SaveChanges();

                ////Email Sending------                  
                var pagelink = Url.Action("Details", "Issue", new { id = issueID }, protocol: Request.Url.Scheme);
                string anchorHtml = "<a href=\"" + pagelink + "\">Click here to change status</a>";
                var uri = new Uri(pagelink);
                string logoPath = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port + "/Images/pronetemaillogo.png";
                string imageHtml = "<img height='55' width='112' src='" + logoPath + "'/>";
                string body;
                string filepath = Server.MapPath(ConfigurationManager.AppSettings["TaskCreationEmailTemplate"].ToString());
                using (var sr = new StreamReader(filepath))
                {
                    body = sr.ReadToEnd();
                }
                string description = System.Net.WebUtility.HtmlDecode(newkDescription);
                // body = System.IO.File.ReadAllText(filepath);
                var assignedUser = db.Users.Find(taskAssigned.userid);
                string userName = assignedUser.FirstName + " " + assignedUser.LastName;
                string messageBody = string.Format(body.ToString(), mailSubject, objTask.IssueCode, objTask.Tag.TagValue, objTask.Client.Name, objTask.Priority.PriorityType, objTask.Product.ProductName, objTask.Status.StatusType, DataHelper.ToPascalConvention((user.FirstName + " " + user.LastName)), objTask.UpdatedOn, description, anchorHtml, DateTime.Now.Year, imageHtml, "", userName, "Assigned To", "'border: solid 1px black;'");//
                string sender = ConfigurationManager.AppSettings["SenderEmailAddress"].ToString();
                try
                {
                    MailManager.sendEmailToManagerRoles(sender, messageBody, mailSubject, "assignTask", obj_assignedtask.ApplicationUserId, objTask.UploadedFileName);
                    return RedirectToAction("Index", new { id = issueID, statusId = statusID });
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", new { id = issueID, statusId = statusID });
                }
                ////Email Sending End-----
            }
            Issue issue = db.Issues.Find(taskAssigned.IssueId);
            taskAssigned.TaskTag = issue.Tag.TagValue;
            taskAssigned.statusType = issue.Status.StatusType;
            taskAssigned.IssueId = issue.IssueId;
            taskAssigned.userid = issue.ApplicationUserId;
            taskAssigned.Client = issue.Client;
            taskAssigned.ClientId = issue.ClientId;
            taskAssigned.AssignedTo = issue.ApplicationUser == null ? "Non" : issue.ApplicationUser.FullName;
            taskAssigned.PrevDescription = DBHandler.removeStringFromDescription(issue.Description);
            taskAssigned.ProductId = issue.ProductId;
            taskAssigned.Priority = issue.Priority;
            taskAssigned.PriorityId = issue.PriorityId;
            taskAssigned.ApplicationUser = issue.ApplicationUser;
            taskAssigned.userid = issue.ApplicationUserId;
            taskAssigned.IssueTagId = issue.TagId;
            taskAssigned.IssueCode = issue.IssueCode;
            taskAssigned.Description = "";
            taskAssigned.CreatedBy = issue.CreatedBy;
            taskAssigned.UpdatedBy = issue.UpdatedBy;
            taskAssigned.CreatedOn = issue.CreatedOn;
            taskAssigned.UpdatedOn = issue.UpdatedOn;
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", taskAssigned.ProductId);
            List<ApplicationUser> usersNotInAdminAndMangerRole = new List<ApplicationUser>();
            var roles = db.Roles.Where(m => m.Name.ToLower() != AppRoles.ADMINISTRATOR.ToLower() && m.Name.ToLower() != AppRoles.MANAGER.ToLower() && m.Name.ToLower() != AppRoles.SUPERUSER.ToLower());
            foreach (var role in roles)
            {
                var users = allUsers.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).ToList();
                foreach (var u in users)
                {
                    usersNotInAdminAndMangerRole.Add(new ApplicationUser { Id = u.Id, FullName = u.FullName });
                }
            }
            if (taskAssigned.userid != null)
            {
                ViewBag.userid = new SelectList(usersNotInAdminAndMangerRole, "Id", "FullName", taskAssigned.userid);
            }
            else
            {
                ViewBag.userid = new SelectList(usersNotInAdminAndMangerRole, "Id", "FullName");
            }
            return View(taskAssigned);
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            string[] filesUploaded = null;
            if (!string.IsNullOrEmpty(issue.UploadedFileName) && issue.UploadedFileName.Contains('$'))
            {
                filesUploaded = issue.UploadedFileName.Split('$');
            }
            List<FilesUploaded> objFilesUploaded = new List<FilesUploaded>();
            if (filesUploaded != null)
            {
                foreach (var item in filesUploaded)
                {
                    string fileUploaded = string.Empty;
                    string fileUserId = string.Empty;
                    if (!string.IsNullOrEmpty(item))
                    {
                        string fileName = DataHelper.getFileName(item, out fileUserId, out fileUploaded);
                        objFilesUploaded.Add(new FilesUploaded { name = fileName,dbFileName=item });
                    }
                }
            }
            else if (!string.IsNullOrEmpty(issue.UploadedFileName))
            {
                string fileUploaded = string.Empty;
                string fileUserId = string.Empty;
                string fileName = DataHelper.getFileName(issue.UploadedFileName, out fileUserId, out fileUploaded);
                objFilesUploaded.Add(new FilesUploaded { name = fileName, dbFileName = issue.UploadedFileName });
            }
            if (objFilesUploaded.Count() > 0)
            {
                ViewData["UploadedFiles"] = objFilesUploaded;
            }
            if (issue.ApplicationUser != null)
            {
                issue.ApplicationUser.FullName = DataHelper.ToPascalConvention(issue.ApplicationUser.FullName);
            }
            issue.Description = DBHandler.removeStringFromDescription(issue.Description);
            issue.CreatedBy = db.Users.Find(issue.CreatedBy).FullName;
            issue.UpdatedBy = db.Users.Find(issue.UpdatedBy).FullName;
            return View(issue);
        }

        [Authorize(Roles = AppRoles.AS)]
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name");
            ViewBag.PriorityId = new SelectList(db.Priorities, "PriorityId", "PriorityType");
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName");
            ViewBag.TagId = new SelectList(db.Tags, "TagId", "TagValue");
            TaskModels issue = new TaskModels();
            return View(issue);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AS)]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(TaskModels issue)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                var u_manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                string currentUserEmailAddress = u_manager.GetEmail(currentUserId);
                int count_lenght = 4;
                Issue objIssue = new Issue();
                objIssue.ClientId = issue.ClientId;
                objIssue.ProductId = issue.ProductId;
                objIssue.PriorityId = issue.PriorityId;
                objIssue.StatusId = db.Status.Select(s => s).ToList().First(s => s.StatusType.ToLower() == AppStatus.UNASSIGNED).StatusId;
                objIssue.TagId = issue.TagId;
                objIssue.UploadedFileName = issue.UploadedFileName;
                Client objClient = db.Clients.First(c => c.ClientId == issue.ClientId);
                int clientIssuesCount = 0;
                //  string clientIssuesYear =Convert.ToDateTime(db.Issues.Where(t => t.ClientId == issue.ClientId).ToList().First().CreatedOn).Year.ToString();
                //issue code
                List<Issue> all_issues = db.Issues.ToList();
                var clientIssuesYear = (from p in all_issues
                                        group p by p.ClientId into g
                                        select g.OrderBy(p => p.CreatedOn).Last()).ToList();
                List<Issue> currentYearIssuesByClient = new List<Issue>();
                foreach (Issue item in all_issues)
                {
                    if ((item.ClientId == issue.ClientId) && (Convert.ToDateTime(item.CreatedOn).Year.ToString() == DateTime.Now.Year.ToString()))
                    {
                        currentYearIssuesByClient.Add(item);
                    }
                }
                //  currentYearIssuesByClient = all_issues.Where(item => (item.ClientId == issue.ClientId) && ( item.CreatedOn !=null && item.CreatedOn.Value.Year == DateTime.Now.Year)).ToList();
                clientIssuesCount = currentYearIssuesByClient.Count() + 1;
                var task_count_format = clientIssuesCount.ToString().PadLeft(count_lenght, '0');
                var datestring = DateTime.Now.Day.ToString().PadLeft(2, '0') + "" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "" + DateTime.Now.Year.ToString();
                objIssue.IssueCode = String.Format("{0}-{1}-{2}", objClient.Name, datestring, task_count_format.ToString());
                //
                string nextLIne = "_newline_";
                string hrLine = "_hrLine_";
                string boldStart = "_bs_";
                string boldEnd = "_be_";
                string divStart = "_ds_";
                string divEnd = "_de_";
                string dvStart = "_dstart_";
                objIssue.Tag = db.Tags.Find(objIssue.TagId);
                objIssue.Status = db.Status.Find(objIssue.StatusId);
                objIssue.Priority = db.Priorities.Find(issue.PriorityId);
                objIssue.Product = db.Products.Find(issue.ProductId);
                var user = db.Users.Find(currentUserId);
                objIssue.UpdatedBy = currentUserId;
                objIssue.CreatedOn = DateTime.Now;
                objIssue.CreatedBy = currentUserId;
                objIssue.AssignedBy = currentUserId;
                objIssue.AssignedOn = DateTime.Now;
                objIssue.UpdatedOn = DateTime.Now;
                string taskDescription = System.Net.WebUtility.HtmlDecode(issue.Description);
                objIssue.Description = divStart + dvStart + hrLine + dvStart + "New Task " + objIssue.IssueCode + " is added." + divEnd + hrLine + divEnd + boldStart + "Commented By:" + boldEnd + " " + DataHelper.ToPascalConvention(db.Users.Where(u => u.Email == User.Identity.Name).First().FullName) + nextLIne + boldStart + "Date:" + boldEnd + " " + objIssue.CreatedOn.ToString() + nextLIne + boldStart + "Comments:" + boldEnd + nextLIne + taskDescription + divEnd;
                db.Issues.Add(objIssue);
                db.SaveChanges();
                int issueID = db.Issues.Where(i => i.IssueCode == objIssue.IssueCode).ToList().First().IssueId;
                //Email Sending------   ttp://localhost:12950/Images/pronetemaillogo.png <img src="ttp://localhost:12950/Images/pronetemaillogo.png"/>
                var pagelink = Url.Action("AssignedTask", "Issue", new { id = issueID }, protocol: Request.Url.Scheme);
                var uri = new Uri(pagelink);
                string logoPath = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port + "/Images/pronetemaillogo.png";
                string imageHtml = "<img height='55' width='112' src='" + logoPath + "'/>";
                string anchorHtml = "<a href=\"" + pagelink + "\">Click here to assign this task.</a>";
                string body;
                string filepath = Server.MapPath("~/App_Data/EmailTemplates/TaskCreationEmail.html");
                using (var sr = new StreamReader(filepath))
                {
                    body = sr.ReadToEnd();
                }
                string mailSubject = objIssue.IssueCode + " has been created";
                string description = System.Net.WebUtility.HtmlDecode(issue.Description);
                string messageBody = string.Format(body.ToString(), mailSubject, objIssue.IssueCode, objIssue.Tag.TagValue, objClient.Name, objIssue.Priority.PriorityType, objIssue.Product.ProductName, objIssue.Status.StatusType, (user.FirstName + " " + user.LastName).ToString(), objIssue.CreatedOn, description, anchorHtml, DateTime.Now.Year, imageHtml, "", "", "", "none");//
                //  string sender = currentUserEmailAddress;
                string sender = ConfigurationManager.AppSettings["SenderEmailAddress"].ToString();
                try
                {
                    MailManager.sendEmailToManagerRoles(sender, messageBody, mailSubject, "createTask", "", issue.UploadedFileName);
                    return RedirectToAction("Index", new { id = issueID });
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", new { id = issueID });
                }
                //Email Sending End-----
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name", issue.ClientId);
            ViewBag.PriorityId = new SelectList(db.Priorities, "PriorityId", "PriorityType", issue.PriorityId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", issue.ProductId);
            ViewBag.TagId = new SelectList(db.Tags, "TagId", "TagValue", issue.TagId);
            return View(issue);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = AppRoles.MTS)]
        public ActionResult TaskStatus(int id = 0)
        {
            string currentUserId = User.Identity.GetUserId();
            var all_tasks = (from i in (db.Issues) select i).ToList();
            var task = (from p in all_tasks
                        group p by p.IssueCode into g
                        select g.OrderByDescending(p => p.UpdatedOn).First()).ToList();
            if ((User.IsInRole(AppRoles.ADMINISTRATOR)) || (User.IsInRole(AppRoles.MANAGER)) || (User.IsInRole(AppRoles.SUPERUSER)))
            {
                var taskModel = (from t in task.Where(i => i.Status.StatusType.ToLower() != AppStatus.CLOSED && i.Status.StatusType.ToLower() != AppStatus.UNASSIGNED) select (new { IssueId = t.IssueId, IssueCode = t.IssueCode })).ToList();
                ViewBag.IssueId = new SelectList(taskModel, "IssueId", "IssueCode", id);
            }
            else
            {
                var all_assignedTasks = db.AssignedTasks.Where(at => at.ApplicationUserId == currentUserId).ToList();
                var assignedTask = (from at in all_assignedTasks select new AssignedTask { ApplicationUserId = at.ApplicationUserId, IssueId = at.IssueId }).ToList();
                var taskModel = (from t in task.Where(i => i.Status.StatusType.ToLower() != AppStatus.CLOSED && i.Status.StatusType.ToLower() != AppStatus.UNASSIGNED) join at in assignedTask on t.IssueId equals at.IssueId select (new { IssueId = t.IssueId, IssueCode = t.IssueCode })).ToList();
                ViewBag.IssueId = new SelectList(taskModel, "IssueId", "IssueCode", id);
            }
            var status = new List<Status>();
            if (User.IsInRole(AppRoles.MANAGER) || User.IsInRole(AppRoles.SUPERUSER))
            {
                status = db.Status.Where(t => t.StatusType.ToLower() == AppStatus.CLOSED || t.StatusType.ToLower() == AppStatus.REASSIGNEDBYTUSER || t.StatusType.ToLower() == AppStatus.REASSIGNEDBYMANAGER || t.StatusType.ToLower() == AppStatus.REASSIGNEDBYSUPERUSER).ToList();
                ViewBag.StatusId = new SelectList(status, "StatusId", "StatusType");
            }
            else
            {
                status = db.Status.Where(t => t.StatusType.ToLower() == AppStatus.CLOSED).ToList();
                ViewBag.StatusId = new SelectList(status, "StatusId", "StatusType", 7);
            }
            if (id > 0)
            {
                ChangeStatusModel taskToChangeStatus = new ChangeStatusModel();
                Issue assignedtask = db.Issues.Find(id);
                if (assignedtask == null)
                {
                    return HttpNotFound();
                }
                taskToChangeStatus.AssignedTo = assignedtask.ApplicationUser.FirstName + " " + assignedtask.ApplicationUser.LastName;
                taskToChangeStatus.ClientName = assignedtask.Client.Name;
                taskToChangeStatus.id = assignedtask.IssueId;
                taskToChangeStatus.ProductName = assignedtask.Product.ProductName;
                taskToChangeStatus.PriorityType = assignedtask.Priority.PriorityType;
                taskToChangeStatus.Tag = assignedtask.Tag.TagValue;
                taskToChangeStatus.TaskTag = assignedtask.Tag.TagValue;
                taskToChangeStatus.StatusType = assignedtask.Status.StatusType;
                taskToChangeStatus.IssueId = assignedtask.IssueId;
                taskToChangeStatus.ClientId = assignedtask.ClientId;
                taskToChangeStatus.ProductId = assignedtask.ProductId;
                taskToChangeStatus.PriorityId = assignedtask.PriorityId;
                taskToChangeStatus.IssueTagId = assignedtask.TagId;
                taskToChangeStatus.IssueCode = assignedtask.IssueCode;
                taskToChangeStatus.PrevDescription = DBHandler.removeStringFromDescription(assignedtask.Description);
                taskToChangeStatus.Description = "";
                taskToChangeStatus.ApplicationUserId = assignedtask.ApplicationUserId;
                taskToChangeStatus.CreatedBy = assignedtask.CreatedBy;
                taskToChangeStatus.UpdatedBy = assignedtask.UpdatedBy;
                taskToChangeStatus.CreatedOn = assignedtask.CreatedOn;
                taskToChangeStatus.UpdatedOn = assignedtask.UpdatedOn;
                taskToChangeStatus.UploadedFileName = assignedtask.UploadedFileName;
                string[] filesUploaded = string.IsNullOrEmpty(assignedtask.UploadedFileName) ? null : assignedtask.UploadedFileName.Contains('$') ? assignedtask.UploadedFileName.Split('$') : null;
                List<FilesUploaded> objFilesUploaded = new List<FilesUploaded>();
                if (filesUploaded != null)
                {
                    foreach (var item in filesUploaded)
                    {
                        string fileUploaded = string.Empty;
                        string fileUserId = string.Empty;
                        string fileName = DataHelper.getFileName(item, out fileUserId, out fileUploaded);
                        objFilesUploaded.Add(new FilesUploaded { name = fileName });
                    }
                }
                else if (!string.IsNullOrEmpty(assignedtask.UploadedFileName))
                {
                    string fileUploaded = string.Empty;
                    string fileUserId = string.Empty;
                    string fileName = DataHelper.getFileName(assignedtask.UploadedFileName, out fileUserId, out fileUploaded);
                    objFilesUploaded.Add(new FilesUploaded { name = fileName });
                }
                if (objFilesUploaded.Count() > 0)
                {
                    ViewData["UploadedFiles"] = objFilesUploaded;
                }
                return View(taskToChangeStatus);
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.MTS)]
        [ValidateAntiForgeryToken]
        public ActionResult TaskStatus(ChangeStatusModel taskAssigned)
        {
           
                ChangeStatusModel taskToChangeStatus = new ChangeStatusModel();
                string currentUserId = User.Identity.GetUserId().ToString();
                var user = db.Users.Find(currentUserId);
                Issue objTask = db.Issues.Find(taskAssigned.id);
                List<Status> list_Status = db.Status.Select(s => s).ToList();
                string statusType = db.Status.Find(objTask.StatusId).StatusType.ToString().ToLower();
                taskToChangeStatus.StatusId = objTask.StatusId;
                taskToChangeStatus.UpdatedBy = objTask.UpdatedBy;
                taskToChangeStatus.UpdatedOn = objTask.UpdatedOn;
                taskToChangeStatus.ClientId = objTask.ClientId;
                taskToChangeStatus.ProductName = objTask.Product.ProductName;
                taskToChangeStatus.AssignedTo = objTask.ApplicationUser.FirstName + " " + objTask.ApplicationUser.LastName;
                taskToChangeStatus.StatusType = objTask.Status.StatusType;
                taskToChangeStatus.PriorityId = objTask.PriorityId;
                taskToChangeStatus.IssueCode = objTask.IssueCode;
                taskToChangeStatus.TaskTag = objTask.Tag.TagValue;
                taskToChangeStatus.CreatedBy = objTask.CreatedBy;
                taskToChangeStatus.PrevDescription = DBHandler.removeStringFromDescription(objTask.Description);
                taskToChangeStatus.Description = "";
                taskToChangeStatus.ApplicationUserId = objTask.ApplicationUserId;
                taskToChangeStatus.CreatedOn = objTask.CreatedOn;
                taskToChangeStatus.UpdatedBy = objTask.UpdatedBy;
                taskToChangeStatus.UpdatedOn = objTask.UpdatedOn;
                objTask.UploadedFileName = taskAssigned.UploadedFileName;
                if (ModelState.IsValid)
                {
                    int statusId;
                    statusId = list_Status.First(s => s.StatusType.ToLower() == AppStatus.CLOSED).StatusId;
                    objTask.UpdatedBy = currentUserId;
                    objTask.UpdatedOn = DateTime.Now;
                    string hrLine = "_hrLine_";
                    string divStartRed = "_dsr_";
                    string nextLIne = "_newline_";
                    string boldStart = "_bs_";
                    string boldEnd = "_be_";
                    string divStart = "_ds_";
                    string divEnd = "_de_";
                    string dvStart = "_dstart_";
                    string newkDescription = System.Net.WebUtility.HtmlDecode(String.IsNullOrEmpty(taskAssigned.Description) ? "" : taskAssigned.Description.ToString());
                    string preDescription = objTask.Description;
                    int? statusID = 0;
                    int issueID = 0;
                    try
                    {
                        string body;
                        string mailSubject = string.Empty;
                        string messageBody = string.Empty;
                        string filepath = Server.MapPath("~/App_Data/EmailTemplates/TaskCreationEmail.html");
                        using (var sr = new StreamReader(filepath))
                        {
                            body = sr.ReadToEnd();
                        }
                        var pagelink = Url.Action("Detail", "Issue", new { id = issueID }, protocol: Request.Url.Scheme);
                        string anchorHtml = "<a href=\"" + pagelink + "\">Click here to close status</a>";
                        var uri = new Uri(pagelink);
                        string logoPath = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port + "/Images/pronetemaillogo.png";
                        string imageHtml = "<img height='55' width='112' src='" + logoPath + "'/>";
                        var assigendUser = db.Users.Find(objTask.ApplicationUserId);
                        string userName = DataHelper.ToPascalConvention(assigendUser.FullName);
                        string description = System.Net.WebUtility.HtmlDecode(newkDescription);
                        string sender = ConfigurationManager.AppSettings["SenderEmailAddress"].ToString();
                        if (taskAssigned.isClosed == "on")
                        {
                            objTask.Description = divStart + divStartRed + hrLine + dvStart + "Task " + objTask.IssueCode + " is closed by " + DataHelper.ToPascalConvention(user.FullName) + "." + divEnd + hrLine + divEnd + boldStart + "Commented By:" + boldEnd + " " + DataHelper.ToPascalConvention(db.Users.Where(u => u.Email == User.Identity.Name).First().FullName) + nextLIne + boldStart + "Date:" + boldEnd + " " + objTask.UpdatedOn.ToString() + nextLIne + boldStart + "Comments:" + boldEnd + nextLIne + newkDescription + divEnd + preDescription;
                            objTask.StatusId = statusId;
                            db.Issues.Add(objTask);
                            db.SaveChanges();
                            var all_issues = db.Issues.ToList();
                            var allFilteredIssues = (from p in all_issues
                                                     group p by objTask.IssueCode into g
                                                     select g.OrderBy(p => p.UpdatedOn).Last()).ToList();
                            statusID = allFilteredIssues[0].StatusId;
                            issueID = allFilteredIssues[0].IssueId;
                            ////Email Sending------                               
                             mailSubject = objTask.IssueCode + " has been closed";                            
                            // body = System.IO.File.ReadAllText(filepath);                           
                             messageBody = string.Format(body.ToString(), mailSubject, objTask.IssueCode, objTask.Tag.TagValue, objTask.Client.Name, objTask.Priority.PriorityType, objTask.Product.ProductName, objTask.Status.StatusType, (user.FirstName + " " + user.LastName).ToString(), objTask.UpdatedOn, description, anchorHtml, DateTime.Now.Year, imageHtml, "", userName, "Assigned To", "'border: solid 1px black;'");
                        }
                        else
                        {                           
                            mailSubject = objTask.IssueCode + " has been updated";
                            messageBody = string.Format(body.ToString(), mailSubject, objTask.IssueCode, objTask.Tag.TagValue, objTask.Client.Name, objTask.Priority.PriorityType, objTask.Product.ProductName, objTask.Status.StatusType, (user.FirstName + " " + user.LastName).ToString(), objTask.UpdatedOn, description, anchorHtml, DateTime.Now.Year, imageHtml, "", userName, "Assigned To", "'border: solid 1px black;'");
                            //  
                            objTask.Description = divStart + dvStart + hrLine + dvStart + "Task " + objTask.IssueCode + "'s comments are updated by " + user.FirstName + " " + user.LastName + "." + divEnd + hrLine + divEnd + boldStart + "Commented By:" + boldEnd + " " + db.Users.Where(u => u.Email == User.Identity.Name).First().FirstName + " " + db.Users.Where(u => u.Email == User.Identity.Name).First().LastName + nextLIne + boldStart + "Date:" + boldEnd + " " + objTask.UpdatedOn.ToString() + nextLIne + boldStart + "Comments:" + boldEnd + nextLIne + newkDescription + divEnd + preDescription;
                            db.Entry(objTask).State = EntityState.Modified;
                            db.SaveChanges();
                            statusID = objTask.StatusId;
                            issueID = objTask.IssueId;
                        }
                        MailManager.sendEmailToManagerRoles(sender, messageBody, mailSubject, "changeStatus", objTask.ApplicationUserId, objTask.UploadedFileName);
                        return RedirectToAction("Index", new { id = issueID, statusId = statusID });
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Index", new { id = issueID, statusId = statusID });
                    }
                    ////Email Sending End-----
                }
                return View(taskToChangeStatus);
           
        }
    }
}
