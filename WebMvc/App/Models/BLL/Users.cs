using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Models.BLL
{
    public class Users
    {
        public List<dynamic> GetUsers()
        {
            Models.Common.OpenConnection();
            var users = Models.Common.QueryAll("select * from users");
            Models.Common.CloseConnection();
            return users;
        }
    }
}