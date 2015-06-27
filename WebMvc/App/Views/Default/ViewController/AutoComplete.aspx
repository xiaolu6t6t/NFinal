<%@ Page Language="C#" AutoEventWireup="truev" CodeBehind="AutoComplete.aspx.cs" Inherits="WebMvc.App.Views.Default.ViewController.AutoComplete" %>
<%@ Register Src="~/NFinal/Controll/KindEditorLibrary.ascx" TagPrefix="uc1" TagName="KindEditorLibrary" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server" >
    <div>
    </div>
    </form>
    <if condition="<%=ViewBag.users.Count>0 %>" >
        
    <else/>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </if>
    <foreach enumerator="<%var user = ViewBag.users.GetEnumerator(); %>">
        <div>
            <%=user.Current.uid %>
        </div>
    </foreach>
</body>
</html>