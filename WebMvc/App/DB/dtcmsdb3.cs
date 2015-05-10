using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.DB
{
    public class dtcmsdb3:NFinal.DB.DBBase
    {
        public static NFinal.DB.Transation GetTransation()
        {
            return new NFinal.DB.Transation();
        }

        public static NFinal.DB.Connection GetConnection()
        {
            return new NFinal.DB.Connection();
        }
    }
}