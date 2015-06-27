using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Controllers
{
    public class DataBaseController : Controller
    {
        #region 查询
        //查询首行首列
        public void QueryObject()
        {
            int id = 1;
            Models.Common.OpenConnection();
            int count = Models.Common.QueryObject("select count(*) from users where id=@id").ToInt();
            Models.Common.CloseConnection();
            Write(count);
        }
        //查询所有行
        public void QueryAll()
        {
            Models.Common.OpenConnection();
            var users = Models.Common.QueryAll("select u.id as uid,count(*) from users as u where u.id<0");
            Models.Common.CloseConnection();
            //foreach (var user in users)
            //{
            //    Write(user.id);
            //    Write(user.name);
            //    Write(user.pwd);
            //}
        }
        //查询随机行
        public void QueryRandom()
        {
            Models.Common.OpenConnection();
            var users = Models.Common.QueryRandom("select * from users",2);
            Models.Common.CloseConnection();
            //foreach (var user in users)
            //{
            //    Write(user.id);
            //    Write(user.name);
            //    Write(user.pwd);
            //}
        }
        //查询前多少行
        public void QueryTop()
        {
            Models.Common.OpenConnection();
            var users = Models.Common.QueryTop("select * from users", 2);
            Models.Common.CloseConnection();
            //foreach (var user in users)
            //{
            //    Write(user.id);
            //    Write(user.name);
            //    Write(user.pwd);
            //}
        }
        //查询一行
        public void QueryRow()
        {
            Models.Common.OpenConnection();
            var user = Models.Common.QueryRow("select * from users where id=1");
            Models.Common.CloseConnection();
            //Write(user.id);
            //Write(user.name);
            //Write(user.pwd);
        }

        #endregion
        #region 增删改
        public void Page()
        { 
        
        }
        public void Insert()
        { 
        
        }
        public void Delete()
        { 
        
        }
        public void Update()
        { 
        
        }
        #endregion
        #region 父子表,多表关系
        public void Tree()
        {

        }
        #endregion
    }
}