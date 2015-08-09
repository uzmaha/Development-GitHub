using P_CMS.Models;
using P_CMS.ViewModels;
using POS.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace P_CMS.UtilityClasses
{
    public static class DBHandler
    {
        // private static  ApplicationDbContext db = new ApplicationDbContext();
        internal static List<Status> GetStatus(string UserRole=null)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Status> listStatus = db.Status.Where(s => s.StatusType.ToLower() == AppStatus.UNASSIGNED || s.StatusType.ToLower() == AppStatus.ASSIGNED || s.StatusType.ToLower() == AppStatus.CLOSED|| s.StatusType.ToLower() == AppStatus.REASSIGNEDBYTUSER || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYMANAGER || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYSUPERUSER).ToList();
            if (UserRole == AppRoles.TUSER)
            {
                listStatus = listStatus.Where(s => s.StatusType.ToLower() != AppStatus.UNASSIGNED).ToList();
            }
            return listStatus;
        }
        internal static List<Priority> GetPriorities()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Priorities.ToList();
        }
        private static int GetPriorityByType(string priorityType)
        {
            if (!string.IsNullOrEmpty(priorityType))
            {
                return GetPriorities().Where(s => s.PriorityType.ToLower() == priorityType).ToList().First().PriorityId;
            }
            return 0;
        }
        internal static List<Tag> GetTags()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Tags.ToList();
        }
        internal static List<ApplicationUser> GetUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Users.Where(u=>u.IsActive==true).ToList();
        }
        internal static List<AssignedTask> GetAssigendIssues()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.AssignedTasks.ToList();
        }
        internal static List<Client> GetClients()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Clients.ToList();
        }
        internal static string GetClientById(int clientId)
        {
            if (clientId > 0)
            {
                return GetClients().Where(c =>c.ClientId == clientId).ToList()[0].Name.ToString();
            }
            return "";
        }
        internal static string GetClientColorById(int clientId)
        {
            if (clientId > 0)
            {
                return GetClients().Where(c => c.ClientId == clientId).ToList()[0].color.ToString();
            }
            return "";
        }
        internal static List<Product> GetProducts()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Products.ToList();
        }
        internal static List<Issue> GetIssues()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Issues.ToList();
        }
        internal static Issue GetIssueById(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Issues.Find(id);
        }

        internal static ApplicationUser GetUserById(string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Users.Find(userId);
        }
        internal static ApplicationUser GetUserByEmail(string userEmailAddress)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Users.Where(u=>u.Email==userEmailAddress).ToList().First();
        }

        internal static List<AssignedTask> GetAssignedTasksByUserId(string userId)
        {
            return GetAssigendIssues().Where(at => at.ApplicationUserId == userId).ToList();
        }

        internal static string GetStatusTypeById(int statusId)
        {
            if (statusId > 0)
            {
                return GetStatus().Where(s => s.StatusId == statusId).ToList()[0].StatusType.ToString();
            }
            return "";
        }
        internal static int GetStatusIdByType(string statusType)
        {
            if (!string.IsNullOrEmpty(statusType))
            {
                return GetStatus().Where(s => s.StatusType.ToLower() == statusType).ToList().First().StatusId;
            }
            return 0;
        }

        internal static void SaveUserLogged(ApplicationUser currentUser, string roleId, string roleName, UserLoggedActions userLoggedActions)
        {
            //  ApplicationDbContext db = new ApplicationDbContext();
            var objUser_Logged = new UserLogged();
            objUser_Logged.UserName = currentUser.UserName;
            objUser_Logged.FirstName = currentUser.FirstName;
            objUser_Logged.LastName = currentUser.LastName;
            objUser_Logged.FullName = currentUser.FullName;
            objUser_Logged.Password = currentUser.Password;
            objUser_Logged.Email = currentUser.Email;
            objUser_Logged.PhoneNumber = currentUser.PhoneNumber;
            objUser_Logged.PasswordHash = currentUser.PasswordHash;
            objUser_Logged.Address = currentUser.Address;
            objUser_Logged.DesignationId = currentUser.DesignationId;
            objUser_Logged.EmailConfirmed = currentUser.EmailConfirmed;
            objUser_Logged.SecurityStamp = currentUser.SecurityStamp;
            objUser_Logged.AccessFailedCount = currentUser.AccessFailedCount;
            objUser_Logged.PhoneNumberConfirmed = currentUser.PhoneNumberConfirmed;
            objUser_Logged.selectedProducts = getUserProductsByUserId(currentUser.Id);
            objUser_Logged.TwoFactorEnabled = currentUser.TwoFactorEnabled;
            objUser_Logged.LockoutEnabled = currentUser.LockoutEnabled;
            objUser_Logged.LockoutEndDateUtc = currentUser.LockoutEndDateUtc;
            objUser_Logged.Password = currentUser.Password;
            objUser_Logged.UserId = currentUser.Id == null ? "" : currentUser.Id;
            objUser_Logged.UpdatedOn = DateTime.Now;
            objUser_Logged.LoggedAction = userLoggedActions;
            if (string.IsNullOrEmpty(currentUser.Id))
            {
                objUser_Logged.CreatedOn = DateTime.Now;
            }
            ApplicationDbContext db = new ApplicationDbContext();
            if (String.IsNullOrEmpty(roleId))
            {
                var allLoggedUsers = (from p in db.UserLogged.ToList()
                                      group p by currentUser.Id into g
                                      select g.OrderBy(p => p.UpdatedOn).Last()).ToList();
                if (allLoggedUsers.Count() > 0)
                {
                    roleId = allLoggedUsers[0].RoleId;
                    roleName = allLoggedUsers[0].RoleName;
                }
            }
            objUser_Logged.RoleId = roleId;
            objUser_Logged.RoleName = roleName;
            db.UserLogged.Add(objUser_Logged);
            db.SaveChanges();
        }

        internal static void UpdateUser(ApplicationUser user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        internal static string getHashPW(string userId)
        {
            ApplicationUser user = GetUserById(userId);
            return user.PasswordHash.ToString();
        }

        internal static List<TaskModels> getAllTasks()
        {
            var taskModel = new List<TaskModels>();
            var all_tasks = GetIssues();
            var task = (from p in all_tasks
                        group p by p.IssueCode into g
                        select g.OrderByDescending(p => p.UpdatedOn).First()).ToList();
            if (task != null)
            {
                taskModel = (from t in task select new TaskModels { AssignedToId = t.ApplicationUserId, AssignedTo = t.ApplicationUser == null ? "Non" : (DataHelper.ToPascalConvention(t.ApplicationUser.FirstName) + " " + DataHelper.ToPascalConvention(t.ApplicationUser.LastName)), ApplicationUser = t.ApplicationUser, Client = t.Client, Product = t.Product, Priority = t.Priority, Tag = t.Tag, ProductId = t.ProductId, ClientId = t.ClientId, ClientName = t.Client.Name, CreatedBy =DataHelper.ToPascalConvention(DBHandler.GetUserById(t.CreatedBy).FullName), CreatedOn = t.CreatedOn, Description = t.Description.Replace("_newline_", "<br/>").Replace("_bs_", "<strong>").Replace("_be_", "</strong>").Replace("_dsr_", "<div class='closedText'>").Replace("_dsg_", "<div class='reassignedText'>").Replace("_ds_", "<div class='dvDescription'>").Replace("_de_", "</div>").Replace("_dstart_", "<div>").Replace("_hrLine_", "<hr class='newLine'/>"), IssueTag1 = t.Tag.TagValue, PriorityType = t.Priority.PriorityType, ProductName = t.Product.ProductName, StatusId = t.StatusId, TaskCode = t.IssueCode, IssueId = t.IssueId, UpdatedBy = DBHandler.GetUserById(t.CreatedBy).FullName, UpdatedOn = t.UpdatedOn, StatusType = t.Status.StatusType, PriorityId = t.PriorityId }).ToList();
                //(from t in task select new TaskModels { AssignedToId = t.ApplicationUserId, AssignedTo = t.ApplicationUser == null ? "Non" : (t.ApplicationUser.FirstName + " " + t.ApplicationUser.LastName), ClientId = t.ClientId, ClientName = t.Client.Name, CreatedBy = DBHandler.UserById(t.CreatedBy).FirstName + " " + DBHandler.UserById(t.CreatedBy).LastName, CreatedOn = t.CreatedOn, IssueTag1 = t.Tag.TagValue, PriorityType = t.Priority.PriorityType, ProductName = t.Product.ProductName, StatusId = t.StatusId, TaskCode = t.IssueCode, IssueId = t.IssueId, UpdatedBy = DBHandler.UserById(t.CreatedBy).UserName, UpdatedOn = t.UpdatedOn, StatusType = t.Status.StatusType, ProductId = t.ProductId, PriorityId = t.PriorityId }).ToList();
            }
            return taskModel;
        }
        internal static List<Issue> getAllIssues()
        {
            List<Issue> issues = new List<Issue>();
            var all_tasks = GetIssues();
            issues = (from p in all_tasks
                      group p by p.IssueCode into g
                      select g.OrderByDescending(p => p.UpdatedOn).First()).ToList();

            return issues;
        }
   
        internal static string removeStringFromDescription(string desc)
        {
            return desc.Replace("_newline_", "<br/>").Replace("_hrLine_", "<hr class='newLine'/>").Replace("_bs_", "<strong>").Replace("_be_", "</strong>").Replace("_dsr_", "<div class='closedText'>").Replace("_dsg_", "<div class='reassignedText'>").Replace("_ds_", "<div class='dvDescription'>").Replace("_de_", "</div>").Replace("_dstart_", "<div>");
        }

        internal static DataSet getDataSetFrfomList(List<Issue> rptSource)
        {
            DataTable tb = new DataTable();
            DataSet ds = new DataSet();
            tb.Columns.Add("ProductId", typeof(int));
            tb.Columns.Add("ClientId", typeof(int));
            tb.Columns.Add("PriorityId", typeof(int));
            tb.Columns.Add("StatusId", typeof(int));
            tb.Columns.Add("TagId", typeof(int));
            tb.Columns.Add("IssueCode", typeof(string));
            tb.Columns.Add("Description", typeof(string));
            tb.Columns.Add("ApplicationUserId", typeof(string));
            tb.Columns.Add("CreatedBy", typeof(string));
            tb.Columns.Add("UpdatedBy", typeof(string));
            tb.Columns.Add("CreatedOn", typeof(DateTime));
            foreach (var item in rptSource)
            {
                tb.Rows.Add(item.ProductId, item.ClientId, item.PriorityId, item.StatusId, item.TagId, item.IssueCode, "uzma", string.IsNullOrEmpty(item.ApplicationUserId) ? "Non" : item.ApplicationUserId, item.CreatedBy, item.UpdatedBy, item.CreatedOn);
            }
            ds.Tables.Add(tb);
            return ds;
        }

        internal static DataSet getDataSetFrfomTaskModelList(List<TaskModels> rptSource)
        {
            DataTable tb = new DataTable();
            DataSet ds = new DataSet();
            tb.Columns.Add("AssignedTo", typeof(string));
            tb.Columns.Add("PriorityType", typeof(string));
            tb.Columns.Add("ProductName", typeof(string));
            tb.Columns.Add("IssueTag1", typeof(string));
            tb.Columns.Add("UpdatedOn", typeof(string));
            tb.Columns.Add("TaskCode", typeof(string));
            tb.Columns.Add("StatusType", typeof(string));
            tb.Columns.Add("ClientName", typeof(string));
            tb.Columns.Add("ProductId", typeof(int));
            tb.Columns.Add("ClientId", typeof(int));
            tb.Columns.Add("PriorityId", typeof(int));
            tb.Columns.Add("StatusId", typeof(int));
            tb.Columns.Add("TagId", typeof(int));
            tb.Columns.Add("IssueCode", typeof(string));
            tb.Columns.Add("Description", typeof(string));
            tb.Columns.Add("ApplicationUserId", typeof(string));
            tb.Columns.Add("CreatedBy", typeof(string));
            tb.Columns.Add("UpdatedBy", typeof(string));
            tb.Columns.Add("CreatedOn", typeof(string));
            foreach (var item in rptSource)
            {
                tb.Rows.Add(item.AssignedTo, item.PriorityType, item.ProductName, item.IssueTag1, Convert.ToDateTime(item.UpdatedOn).ToString(), item.TaskCode, item.StatusType, item.ClientName, item.ProductId, item.ClientId, item.PriorityId, item.StatusId, item.TagId, item.IssueCode, "uzma", string.IsNullOrEmpty(item.AssignedToId) ? "Non" : item.AssignedToId, item.CreatedBy, item.UpdatedBy, item.CreatedOn);
            }
            ds.Tables.Add(tb);
            return ds;
        }

        internal static string GetRoleIdByRoleName(string roleName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Roles.Where(r => r.Name.ToLower() == roleName.ToLower()).First().Name;
        }

        internal static List<TaskModels> ReportIssues(bool isTUSerRole, string currentUserId, string ClientId = null, string PriorityId = null, string ProductId = null, string StatusId = null, string DateTo = null, string DateFrom = null)
        {
            List<Status> allStatus = DBHandler.GetStatus();
            string assingedRoles = string.Empty;
            List<TaskModels> taskModel = DBHandler.getAllTasks();
            if (isTUSerRole)
            {
                taskModel = taskModel.Where(i => i.AssignedToId == currentUserId).ToList();
                allStatus = allStatus.Where(s => s.StatusType.ToLower() == AppStatus.ASSIGNED || s.StatusType.ToLower() == AppStatus.CLOSED || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYTUSER || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYMANAGER || s.StatusType.ToLower() == AppStatus.REASSIGNEDBYSUPERUSER).ToList();
            }
            taskModel = DBHandler.getReportSearchedIssues(taskModel, ClientId, PriorityId, ProductId, StatusId, DateTo, DateFrom);
            return taskModel;
        }

        private static List<TaskModels> getReportSearchedIssues(List<TaskModels> taskModel, string ClientId, string PriorityId, string ProductId, string StatusId, string DateTo, string DateFrom)
        {
            if (!string.IsNullOrEmpty(DateTo))
            {
                string[] dtTo = DateTo.Split('/');
                DateTo = DateTime.Parse(dtTo[1] + '/' + dtTo[0] + '/' + dtTo[2]).ToString();
            }
            if (!string.IsNullOrEmpty(DateFrom))
            {
                string[] dtFrom = DateFrom.Split('/');
                DateFrom = DateTime.Parse(dtFrom[1] + '/' + dtFrom[0] + '/' + dtFrom[2]).ToString();
            }
            IssuesSearchedCount objIssuesSearchedCount = new IssuesSearchedCount();
            if (!string.IsNullOrEmpty(ProductId) && Convert.ToInt32(ProductId) > 0)
            {
                taskModel = taskModel.Where(i => i.ProductId.ToString() == ProductId).ToList();
            }
            if (!string.IsNullOrEmpty(PriorityId) && Convert.ToInt32(PriorityId) > 0)
            {
                taskModel = taskModel.Where(i => i.PriorityId.ToString() == PriorityId).ToList();
            }
            if (!string.IsNullOrEmpty(StatusId) && Convert.ToInt32(StatusId) > 0)
            {
                taskModel = taskModel.Where(i => i.StatusId.ToString() == StatusId).ToList();
            }
            if (!string.IsNullOrEmpty(ClientId) && Convert.ToInt32(ClientId) > 0)
            {
                taskModel = taskModel.Where(i => i.ClientId.ToString() == ClientId).ToList();
            }
            if (!string.IsNullOrEmpty(DateTo) || !string.IsNullOrEmpty(DateFrom))
            {
                if (!string.IsNullOrEmpty(DateTo) && string.IsNullOrEmpty(DateFrom))
                {
                    DateTo = Convert.ToDateTime(DateTo).AddDays(1).ToString();
                    taskModel = taskModel.Where(i => i.CreatedOn <= Convert.ToDateTime(DateTo)).ToList();
                }
                if (!string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {                   
                  //  DateFrom = Convert.ToDateTime(DateFrom).AddDays(-1).ToString();
                    taskModel = taskModel.Where(i => i.CreatedOn >= Convert.ToDateTime(DateFrom)).ToList();
                }
                if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
                {
                    if (DateFrom == DateTo)
                    {
                        taskModel = taskModel.Where(i => i.CreatedOn.ToShortDateString() == Convert.ToDateTime(DateTo).ToShortDateString()).ToList();
                    }
                    else
                    {
                        DateFrom = Convert.ToDateTime(DateFrom).AddDays(-1).ToString();
                        DateTo = Convert.ToDateTime(DateTo).AddDays(1).ToString();
                        taskModel = taskModel.Where(i => i.CreatedOn <= Convert.ToDateTime(DateTo) && i.CreatedOn >= Convert.ToDateTime(DateFrom)).ToList();
                    }
                }
            }
            return taskModel;
        }

        internal static List<TaskModels> getAllFilteredTasks(List<TaskModels> taskModel, string ClientId, string PriorityId, string ProductId, string StatusId, string DateTo, string DateFrom)
        {
            if (!string.IsNullOrEmpty(DateTo))
            {
                string[] dtTo = DateTo.Split('/');
                DateTo = DateTime.Parse(dtTo[1] + '/' + dtTo[0] + '/' + dtTo[2]).ToString();
            }
            if (!string.IsNullOrEmpty(DateFrom))
            {
                string[] dtFrom = DateFrom.Split('/');
                DateFrom = DateTime.Parse(dtFrom[1] + '/' + dtFrom[0] + '/' + dtFrom[2]).ToString();
            }
            IssuesSearchedCount objIssuesSearchedCount = new IssuesSearchedCount();
            if (!string.IsNullOrEmpty(ProductId) && Convert.ToInt32(ProductId) > 0)
            {
                taskModel = taskModel.Where(i => i.ProductId.ToString() == ProductId).ToList();
            }
            if (!string.IsNullOrEmpty(PriorityId) && Convert.ToInt32(PriorityId) > 0)
            {
                taskModel = taskModel.Where(i => i.PriorityId.ToString() == PriorityId).ToList();
            }
            if (!string.IsNullOrEmpty(StatusId) && Convert.ToInt32(StatusId) > 0)
            {
                taskModel = taskModel.Where(i => i.StatusId.ToString() == StatusId).ToList();
            }
            if (!string.IsNullOrEmpty(DateTo) || !string.IsNullOrEmpty(DateFrom))
            {
                if (!string.IsNullOrEmpty(DateTo) && string.IsNullOrEmpty(DateFrom))
                {
                    DateTo = Convert.ToDateTime(DateTo).AddDays(1).ToString();
                    taskModel = taskModel.Where(i => i.CreatedOn <= Convert.ToDateTime(DateTo)).ToList();
                }
                if (!string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateFrom = Convert.ToDateTime(DateFrom).AddDays(-1).ToString();
                    taskModel = taskModel.Where(i => i.CreatedOn >= Convert.ToDateTime(DateFrom)).ToList();
                }
                if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
                {
                    if (DateFrom == DateTo)
                    {
                        taskModel = taskModel.Where(i => i.CreatedOn.ToShortDateString() == Convert.ToDateTime(DateTo).ToShortDateString()).ToList();
                    }
                    else
                    {
                        DateFrom = Convert.ToDateTime(DateFrom).AddDays(-1).ToString();
                        DateTo = Convert.ToDateTime(DateTo).AddDays(1).ToString();
                        taskModel = taskModel.Where(i => i.CreatedOn <= Convert.ToDateTime(DateTo) && i.CreatedOn >= Convert.ToDateTime(DateFrom)).ToList();
                    }
                }
            }
            return taskModel;
        }

        internal static IList<IssuesSearchedCount> getSearchIssuesCount(List<TaskModels> taskModel, string ClientId, string PriorityId, string ProductId, string StatusId, string DateTo, string DateFrom)
        {
            string toDate = string.IsNullOrEmpty(DateTo) ? "none" : DateTo;
            string fromDate = string.IsNullOrEmpty(DateFrom) ? "none" : DateFrom;
            string date = CryptorEngine.ConvertStringToHex(fromDate + "@" + toDate, System.Text.Encoding.Unicode);
            List<IssuesSearchedCount> objlistIssuesCount = new List<IssuesSearchedCount>();
            if (taskModel != null && taskModel.Count() > 0)
            {
                var clients = GetClients();//client check
                if (!string.IsNullOrEmpty(ClientId) && Convert.ToInt32(ClientId) > 0)
                {
                    clients = clients.Where(c => c.ClientId == Convert.ToInt32(ClientId)).ToList();
                }
                if (clients != null && clients.Count() > 0)
                {
                    int assignedId = DBHandler.GetStatusIdByType(AppStatus.ASSIGNED);
                    int closedId = DBHandler.GetStatusIdByType(AppStatus.CLOSED);
                    int unAssignedId = DBHandler.GetStatusIdByType(AppStatus.UNASSIGNED);
                    int reassignedByTUserId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYTUSER);
                    int reassignedBySuperUserId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYSUPERUSER);
                    int reassignedByManagerId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYMANAGER);
                    int mediumId = DBHandler.GetPriorityByType("medium");
                    int lowId = DBHandler.GetPriorityByType("low");
                    int highId = DBHandler.GetPriorityByType("high");
                    foreach (var item in clients)
                    {
                        objlistIssuesCount.Add(new IssuesSearchedCount
                        {
                            ProductId = string.IsNullOrEmpty(ProductId) ? 0 : Convert.ToInt32(ProductId),
                            Date = date,
                            HighId = highId,
                            MediumId = mediumId,
                            LowId = lowId,
                            AssignedId = assignedId,
                            ClosedId = closedId,
                            UnAssignedId = unAssignedId,
                            ReassignedByTUserId = reassignedByTUserId,
                            ReassignedBySuperUserId = reassignedBySuperUserId,
                            ReassignedByManagerId = reassignedByManagerId,
                            AssignedCount = (from t in (taskModel.Where(t => t.StatusId == assignedId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            ClosedCount = (from t in (taskModel.Where(t => t.StatusId == closedId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            UnassignedCount = (from t in (taskModel.Where(t => t.StatusId == unAssignedId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            ReassignedByTUserCount = (from t in (taskModel.Where(t => t.StatusId == reassignedByTUserId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            ReassignedBySuperUserCount = (from t in (taskModel.Where(t => t.StatusId == reassignedBySuperUserId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            ReassignedByManagerCount = (from t in (taskModel.Where(t => t.StatusId == reassignedByManagerId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            HighCount = (from t in (taskModel.Where(t => t.PriorityId == highId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            MediumCount = (from t in (taskModel.Where(t => t.PriorityId == mediumId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            LowCount = (from t in (taskModel.Where(t => t.PriorityId == lowId && t.ClientId == item.ClientId).ToList()) select t).Count(),
                            ClientName = item.Name,
                            ClientId = item.ClientId
                        });
                    }
                }
            }
            return objlistIssuesCount;
        }

        internal static IList<IssuesSearchedClient> getSearchClientIssuesCounts(List<TaskModels> taskModel, string ClientId, string PriorityId, string ProductId, string StatusId, string DateTo, string DateFrom)
        {
            taskModel = taskModel.Where(i => i.StatusType.ToLower() != "closed").ToList();
            string toDate = string.IsNullOrEmpty(DateTo) ? "none" : DateTo;
            string fromDate = string.IsNullOrEmpty(DateFrom) ? "none" : DateFrom;
            string date = CryptorEngine.ConvertStringToHex(fromDate + "@" + toDate, System.Text.Encoding.Unicode);
            List<IssuesSearchedClient> objlistIssuesCount = new List<IssuesSearchedClient>();
            if (taskModel != null && taskModel.Count() > 0)
            {
                var clients = GetClients();//client check
                if (!string.IsNullOrEmpty(ClientId) && Convert.ToInt32(ClientId) > 0)
                {
                    clients = clients.Where(c => c.ClientId == Convert.ToInt32(ClientId)).ToList();
                }
                if (clients != null && clients.Count() > 0)
                {
                    int assignedId = DBHandler.GetStatusIdByType(AppStatus.ASSIGNED);
                    int closedId = DBHandler.GetStatusIdByType(AppStatus.CLOSED);
                    int unAssignedId = DBHandler.GetStatusIdByType(AppStatus.UNASSIGNED);
                    int reassignedByTUserId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYTUSER);
                    int reassignedBySuperUserId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYSUPERUSER);
                    int reassignedByManagerId = DBHandler.GetStatusIdByType(AppStatus.REASSIGNEDBYMANAGER);
                    int mediumId = DBHandler.GetPriorityByType("medium");
                    int lowId = DBHandler.GetPriorityByType("low");
                    int highId = DBHandler.GetPriorityByType("high");
                    for (int i = 0; i < clients.Count(); i++)
                    {
                        int issuesCount = taskModel.Where(t => t.ClientId == clients[i].ClientId).ToList().Count();
                        if (issuesCount > 0)
                        {


                            objlistIssuesCount.Add(new IssuesSearchedClient
                            {
                                IssueTitle = clients[i].Name,
                                IssueCount = issuesCount == 0 ? "" : issuesCount.ToString(),
                                color = GetClientColorById(clients[i].ClientId),
                                clientid = clients[i].ClientId
                            });
                        }
                    }
                  
                }
            }
            return objlistIssuesCount;
        }

        internal static string getUserProductsByUserId(string userid)
        {
            string productString = string.Empty;
            ApplicationDbContext db = new ApplicationDbContext();
            var userProduct = db.ManagerProducts.Where(u => u.ApplicationUserId == userid).ToList();
            if (userProduct!=null && userProduct.Count()>0)
            {
                productString=userProduct.First().ProductIds;
            }
            return productString;
        }

        internal static int UserProductId(string userid)
        {
            int managerProductId = 0;
            ApplicationDbContext db = new ApplicationDbContext();
            List<ManagerProduct> listMAnagerProduct = db.ManagerProducts.Where(u => u.ApplicationUserId == userid).ToList();
            if (listMAnagerProduct!=null & listMAnagerProduct .Count()>0)
            {
                managerProductId =listMAnagerProduct.First().ManagerProductId;      
            }
            return managerProductId;
        }
    }
}