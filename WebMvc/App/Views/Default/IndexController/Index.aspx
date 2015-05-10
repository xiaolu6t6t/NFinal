<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs"  Inherits="WebMvc.App.Views.Default.IndexController.Index"  Buffer="true"%>
<%@ Register Src="~/App/Views/Default/Public/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/App/Views/Default/Public/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>
<!doctype html>
<html>
<head>
    <uc1:KindEditorLibrary runat="server" id="KindEditorLibrary" />
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
    <uc1:Header runat="server" id="Header" />
    <article>
        <%=text %>/<%=text %>
    </article>
    <uc1:Footer runat="server" id="Footer" />
</body>
</html>