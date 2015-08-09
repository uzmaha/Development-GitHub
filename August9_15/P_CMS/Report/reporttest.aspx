﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reporttest.aspx.cs" Inherits="P_CMS.Views.Report.reporttest" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>.lblHeader {
        margin-top:2px !important;
    }</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
     <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" />
    </div>
      

    </form>
</body>
</html>


