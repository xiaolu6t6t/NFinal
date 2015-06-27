﻿using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$App}.Models
{
    public class {$database}:NFinal.DB.DBBase
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