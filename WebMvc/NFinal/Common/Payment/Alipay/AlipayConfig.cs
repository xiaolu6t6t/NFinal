using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace NFinal.Common.Payment.Alipay
{
    public class Config
    {
        private static string connectionString = @"Data Source=|DataDirectory|\Common.db";
        public string partner = "";
        public string seller_email = "";
        public string key = "";
        public static string input_charset = "utf-8";
        public static string sign_type = "MD5";

        private int id = 0;
        public Config(int id)
        {
            this.id = id;
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            string sql = "";
            if (id > 0)
            {
                sql = "select * from alipay_config where id=" + id.ToString();
            }
            else
            {
                sql = "select * from alipay_config limit 1";
            }
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                this.partner = reader["partner"].ToString();
                this.seller_email = reader["seller_email"].ToString();
                this.key = reader["key"].ToString();
            }
            reader.Close();
            connection.Close();
        }
    }
}