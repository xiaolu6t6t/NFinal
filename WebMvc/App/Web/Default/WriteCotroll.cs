using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.App.Web.Default
{
    public class WriteCotroll:Controller
    {
        public WriteCotroll(System.IO.TextWriter tw):base(tw){}
        public WriteCotroll(string fileName) : base(fileName) { }
        

        public void Index()
        {
            //Navigator UserControl1 = new Navigator();
            //UserControl1.__render__ = (ViewBag) =>
            //{
            //    //生成的Write函数代码
            //    View("Navigator.aspx");
            //};
            //UserControl1.__render__(UserControl1);
        }
        //<uc1:Navigator ID="UserControl1"/>

        //<%Navigator {$id} = new Navigator();%>
        //<%{$id}.__render__ =(ViewBag)=>{%>
        //用户控件中的代码
        //<%=__x__.pageIndex%>
        //.......................
        //<%};%>
        //{$id}.__render__(UserControll);

        //<%Navigator UserControl1 = new Navigator();%>
        //<%UserControl1.__render__ =(ViewBag)=>{%>
        //用户控件中的代码
        //<%=__x__.pageIndex%>
        //.......................
        //<%};%>
        //UserControl1.__render__(UserControll);
    }
}