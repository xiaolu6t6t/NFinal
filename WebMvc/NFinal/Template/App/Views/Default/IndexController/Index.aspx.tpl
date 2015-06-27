<%@ Page Language="C#" AutoEventWireup="true"%>
<%@ Import Namespace="Microsoft.CSharp" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Register Src="~/App/Views/Default/Common/Public/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/App/Views/Default/Common/Public/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>
<!doctype html>
<html>
<head>
    <uc1:kindeditorlibrary runat="server" id="KindEditorLibrary" />
    <script src="/App/Content/js/jquery-1.11.2.min.js"></script>
    <link href="/App/Content/css/frame.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        .auto-style1 {
            width: 140px;
        }
        .auto-style2 {
            width: 197px;
        }
    </style>
</head>
<body>
    <uc1:Header runat="server" />   
    <article>
        Hello,NFinal!
    </article>
    <uc1:Footer runat="server" />
</body>
</html>