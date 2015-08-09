using P_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace P_CMS.UtilityClasses
{
    public class MailManager
    {
        internal static void sendPasswordResetEmail(string mailSubject, string messageBody, string sendTo)
        {
            StringBuilder str = new StringBuilder();
            SmtpClient client = new SmtpClient();
            MailAddress address = new MailAddress(ConfigurationManager.AppSettings["SenderEmailAddress"].ToString(), "Uzma Hamid");
            // MailAddress address = new MailAddress("noreply@yahoo.com");
            MailMessage message = new MailMessage(ConfigurationManager.AppSettings["SenderEmailAddress"].ToString(), sendTo);
          //  message.From = address;
            message.Subject = mailSubject;
            message.Body = str.Append(messageBody).ToString();
            message.IsBodyHtml = true;
            client.DeliveryFormat = SmtpDeliveryFormat.International;
           // client.Send(message);
        }

        internal static void sendEmail(string messageBody)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                SmtpClient client = new SmtpClient();
                MailAddress address = new MailAddress(ConfigurationManager.AppSettings["SenderEmailAddress"].ToString());
                MailMessage message = new MailMessage(ConfigurationManager.AppSettings["SenderEmailAddress"].ToString(), ConfigurationManager.AppSettings["SendTo"].ToString());
                str.Append(messageBody);
                message.From = address;
                message.Subject = "test";
                message.Body = str.ToString();
                message.IsBodyHtml = true;
                client.DeliveryFormat = SmtpDeliveryFormat.International;
             //   client.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static void sendEmailToManagerRoles(string sender, string messageBody, string mailSubject, string actionName, string AssignedTo, string UploadedFileName)
        {
            var users = getAllManagerUsers(actionName, AssignedTo);
            foreach (ApplicationUser user in users)
            {                
                ApplicationDbContext db = new ApplicationDbContext();
                StringBuilder str = new StringBuilder();
                MailAddress address = new MailAddress(sender);
                SmtpClient client = new SmtpClient();
                client.DeliveryFormat = SmtpDeliveryFormat.International;
                MailMessage message = new MailMessage(ConfigurationManager.AppSettings[AppConstant.SENDERMAILADDRESS].ToString(), user.Email);                
                 Attachment attachmentItem = null;
                if (UploadedFileName !=null)
                {
                    string[] fileSUploaded = UploadedFileName.Split('$');                   
                    foreach (var item in fileSUploaded)
                    {
                        string fileUpload = string.Empty;
                        string fileUserId = string.Empty;
                        string fileName = DataHelper.getFileName(item, out fileUserId, out fileUpload);
                        string filePath = String.Format("{0}/UploadedFiles/{1}/{2}", ConfigurationManager.AppSettings[AppConstant.APPLICATIONPATH].ToString(), fileUserId, fileUpload);
                        if (File.Exists(filePath))
                        {                           
                            attachmentItem = new Attachment(new System.IO.FileStream(filePath, FileMode.Open), fileName);
                            message.Attachments.Add(attachmentItem);                          
                        }
                    } 
                }
                message.Subject = mailSubject;
                message.Body = str.Append(messageBody).ToString();
                message.IsBodyHtml = true;
              //  client.Send(message);
                attachmentItem.Dispose();
            }
        }

        private static List<ApplicationUser> getAllManagerUsers(string actionName, string AssignedTo)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var allUsers = db.Users.ToList();
            List<ApplicationUser> usersNotInAdminAndMangerRole = new List<ApplicationUser>();
            var roles = db.Roles.Where(m => m.Name == AppConstant.MANAGER).ToList();
            foreach (var role in roles)
            {
                var users = allUsers.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).ToList();
                foreach (var u in users)
                {
                    usersNotInAdminAndMangerRole.Add(new ApplicationUser { Email = u.Email, FirstName = u.FirstName, LastName = u.LastName });
                }
            }
            if (actionName == "assignTask")
            {
                ApplicationUser tusers = db.Users.Find(AssignedTo);
                usersNotInAdminAndMangerRole.Add(tusers);
            }
            List<ApplicationUser> allusers = (from u in usersNotInAdminAndMangerRole select new ApplicationUser { Email = u.Email, FirstName = u.FirstName, LastName = u.LastName }).ToList();
            return allusers;
        }
    }
}