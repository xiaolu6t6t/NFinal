<%@ Page Language="C#" AutoEventWireup="true"%>
<%@ Import Namespace="Microsoft.CSharp" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Register Src="~/App/Views/Default/Common/Public/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/App/Views/Default/Common/Public/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>


<!doctype html>
<html>
<head>
    <script src="/App/Content/js/jquery-1.11.2.min.js"></script>
    <link href="/App/Content/css/frame.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <uc1:Header runat="server" />   
    <article>
        Hello,NFinal!

    </article>
    <uc1:Footer runat="server" ID="footer"/>
</body>
</html>