using P_CMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Http;
using System.Net.Http.Headers;

namespace P_CMS.Controllers
{
    public class WebHelperController : Controller
    {
        // GET: WebHelper
        public string Index()
        {
            string userid = User.Identity.GetUserId();
            string CKEditorFuncNum = Request["CKEditorFuncNum"];
            string FileFolderPath = "/UploadedFiles/" + userid + "/";
            var file = Request.Files["upload"];
            string filename = Path.GetFileName(file.FileName);
            string fileExtension = Path.GetExtension(filename);

            int posA = filename.IndexOf(fileExtension);
            if (posA == -1)
            {
                filename = "";
            }
            string fileNameWOExt = filename.Substring(0, posA);
          
            var dateString = DateTime.Now.ToString().Replace(" ", "_");
            dateString = dateString.Replace("/", "");
            filename = fileNameWOExt + "_" + dateString.Replace(":", "") + "_" + "" + getIssueCode();
            string ImagePath =FileFolderPath + filename + fileExtension;
            string serverPath = Server.MapPath(ImagePath);
            Directory.CreateDirectory(Server.MapPath("/UploadedFiles/" + userid + "/"));
            file.SaveAs(serverPath);
            string OnlyFileName=filename + fileExtension;
            var pagelink = Url.Action("Download", "WebHelper", new { id = "cclientId" }, protocol: Request.Url.Scheme);
            pagelink = pagelink.Replace("cclientId", "{selectedclientid}/" + filename + fileExtension + "/" + userid + "'");
            string fileLink = "<a href=\"" + pagelink + "\" target='_parent'>" + fileNameWOExt + fileExtension + " (Press Control + Click, to download)</a>";
           // string fileLink = "<a id='aDownloadFile' href='/WebHelper/Download/{selectedclientid}/" + filename + fileExtension + "/" + userid + "'  >" + fileNameWOExt + fileExtension + " (Press Control + Click, to download)</a>";
            string uploadedUrlPath = @"<script>window.parent.setCkeditorText('" + fileLink.Replace('\'', '\"') + "','" + userid+"&"+ filename + fileExtension + "'); </script>";
            return uploadedUrlPath;
        }

      
        [HttpPost]
        public virtual ActionResult UploadFile()
        {
           
            string userid = User.Identity.GetUserId();
            string FileFolderPath = "/UploadedFiles/" + userid + "/";
            string pathForSaving = string.Empty;
            HttpPostedFileBase myFile = Request.Files["FileToUpload"];
            //
            string filename = Path.GetFileName(myFile.FileName);
            string fileExtension = Path.GetExtension(filename);

            int posA = filename.IndexOf(fileExtension);
            if (posA == -1)
            {
                filename = "";
            }
            string fileNameWOExt = filename.Substring(0, posA);

            var dateString = DateTime.Now.ToString().Replace(" ", "_");
            dateString = dateString.Replace("/", "");
            filename = fileNameWOExt + "_" + dateString.Replace(":", "");
            string ImagePath = FileFolderPath + filename + fileExtension;
          //  string serverPath = Server.MapPath(ImagePath);
            bool isUploaded = false;
            string message = "File upload failed";
            if (myFile != null && myFile.ContentLength != 0)
            {
                pathForSaving = Server.MapPath(FileFolderPath);
                if (this.CreateFolderIfNeeded(pathForSaving))
                {
                    try
                    {
                        myFile.SaveAs(Path.Combine(pathForSaving, filename + fileExtension));
                        isUploaded = true;
                        message = "File uploaded successfully!";
                    }
                    catch (Exception ex)
                    {
                        message = string.Format("File upload failed: {0}", ex.Message);
                    }
                }
            }
            return Json(new { isUploaded = isUploaded, message = message, filepath = userid + "&" + filename + fileExtension, file = fileNameWOExt + fileExtension }, "text/html");
        }
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    /*TODO: You must process this exception.*/
                    result = false;
                }
            }
            return result;
        }

        public ActionResult Download(string fileName = null, string fileDBName = null)
        {
            string userid = fileDBName.Split('&')[0];
            string filename = fileName.Split('_')[0];
            string fileExtension = Path.GetExtension(fileName);
            var filePath = Server.MapPath("\\UploadedFiles\\" + userid + "\\" + fileDBName.Split('&')[1]);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileNameToDisplay = filename + fileExtension;
      
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet);
        }
        private string getIssueCode()
        {
            int count_lenght = 4;
            string clientIssuesCount = "";
            ApplicationDbContext db = new ApplicationDbContext();
            List<Issue> all_issues = db.Issues.ToList();
            List<Issue> currentYearIssuesByClient = new List<Issue>();
            foreach (Issue item in all_issues)
            {
                if (Convert.ToDateTime(item.CreatedOn).Year.ToString() == DateTime.Now.Year.ToString())
                {
                    currentYearIssuesByClient.Add(item);
                }
            }
            clientIssuesCount = (currentYearIssuesByClient.Count() + 1).ToString();
            clientIssuesCount = clientIssuesCount.PadLeft(count_lenght, '0');
            return clientIssuesCount;
        }



    }
}
