using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DAL
{
    public class DB_CONSTANTS
    {
        private static string connString_Easy_Save = ConfigurationManager.AppSettings["connection_Easy_Save"].ToString();       ;


        public static string ConnectionString_Easy_Save
        {
            get { return connString_Easy_Save; }
        }       
    }
}
