<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navigator.ascx.cs" Inherits="NFinal.Controll.Navigator" %>
<%__pageNavigatorSize__ = 5; %>
当前页:<%=__pageIndex__ %> / <%=__pageCount__ %>
<a href="<%=string.Format(__pageUrl__, 1) %>">首页</a>
<a href="<%=string.Format(__pageUrl__, __pageIndex__ == 1 ? 1 : __pageIndex__ - 1) %>">上一页</a>
<%for (int i = ((__pageIndex__ - 1) / __pageNavigatorSize__) * __pageNavigatorSize__ + 1; i <= __pageCount__ && i <= ((__pageIndex__ - 1) / __pageNavigatorSize__ + 1) * __pageNavigatorSize__; i++)
            {%>
    <%if (i == __pageIndex__)
                {%>
    <a style="color:red;" href="<%=string.Format(__pageUrl__, i) %>"><%=i %></a>
    <%}else {%>
    <a href="<%=string.Format(__pageUrl__, i) %>"><%=i%></a>
    <%} %>
<%} %>
<a href="<%=string.Format(__pageUrl__, __pageIndex__ == __pageCount__ ? __pageCount__ : __pageIndex__ + 1) %>">下一页</a>
<a href="<%=string.Format(__pageUrl__, __pageCount__) %>">末页</a>