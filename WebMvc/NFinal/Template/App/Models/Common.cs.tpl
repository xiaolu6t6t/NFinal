using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$app}.Models
{
    public class Common:NFinal.DB.DBBase
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