using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using P_CMS.Models;
using P_CMS.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace P_CMS.Views.Report
{
    public partial class reporttest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string clientName = "";
            string statusType = "";
            string priorityType = "";  
            string ProductName = "";  
            string fromDate = "";  
            string toDate = "";  
            DataSet ds = new DataSet();
             ds =(DataSet)Session["ReportSource"];         
          // CrystalReportViewer1.LogOnInfo.Add(new ConnectionInfo { ServerName = "MOHTSHM\\SQLEXPRES", DatabaseName = "pronetCMSDB2", UserID = "sa", Password = "sa123", Type = ConnectionInfoType.SQL, IntegratedSecurity = false });
            
            if (Request.QueryString.Count>0)
             {
                    clientName =string.IsNullOrEmpty(Request.QueryString["client"])? "" : ("Client Name: "+Request.QueryString["client"].ToString());
                     statusType = string.IsNullOrEmpty(Request.QueryString["status"])? "" : ("Status Type: "+Request.QueryString["status"].ToString());
                     priorityType = string.IsNullOrEmpty(Request.QueryString["priority"])? "" : ("Priority Type: "+Request.QueryString["priority"].ToString());
                     ProductName = string.IsNullOrEmpty(Request.QueryString["product"])? "" : ("Product Name: "+Request.QueryString["product"].ToString());
                     fromDate = string.IsNullOrEmpty(Request.QueryString["fromDate"])? "" : ("From Date: "+Request.QueryString["fromDate"].ToString());
                     toDate = string.IsNullOrEmpty(Request.QueryString["toDate"])? "" : ("To Date: "+Request.QueryString["toDate"].ToString());
             }
            try
            {
                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(Server.MapPath("~/Report/CrystalReport2.rpt"));
                if (ds != null)
                {
                    rptDoc.SetDataSource(ds.Tables[0]);
                }

                TextObject txtClient = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[0].ReportObjects["txtClient"]);
                txtClient.Text = clientName;
                TextObject txtStatus = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[0].ReportObjects["txtStatus"]);
                txtStatus.Text = statusType;
                TextObject txtProduct = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[0].ReportObjects["txtProduct"]);
                txtProduct.Text = ProductName;
                TextObject txtPriority = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[0].ReportObjects["txtPriority"]);
                txtPriority.Text = priorityType;
                TextObject txtFromDate = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[0].ReportObjects["txtFromDate"]);
                txtFromDate.Text = fromDate;
                TextObject txtToDate = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[0].ReportObjects["txtToDate"]);
                txtToDate.Text = toDate;

                // TextObject txtPOS = (CrystalDecisions.CrystalReports.Engine.TextObject)(rptDoc.ReportDefinition.Sections[2].ReportObjects[0]);
                // txtPOS.Text ="uzma";'
                //CRRedist2008_x86
                CrystalReportViewer1.DisplayGroupTree = false;
                CrystalReportViewer1.Width = 900;
                CrystalReportViewer1.HasToggleGroupTreeButton = false;
                CrystalReportViewer1.ReportSource = rptDoc; 
            }
            catch (Exception)
            {

                CrystalReportViewer1.ReportSource = null; 
            }
          
         }        
    }
}