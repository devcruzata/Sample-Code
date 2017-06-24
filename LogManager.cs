using DAL;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BAL.Common
{
   public class LogManager
    {
        /// <summary>
        /// Save Error Log
        /// </summary>
        /// <param name="obj"></param>        

        public static void LogError(string EventName, int Priority, string Source, string Message, string StackTrace)
        {
            objResponse Response = new objResponse();
            try
            {

                SqlParameter[] sqlParameter = new SqlParameter[5];

                sqlParameter[0] = new SqlParameter("@EventName", SqlDbType.NVarChar, 500);
                sqlParameter[0].Value = EventName;

                sqlParameter[1] = new SqlParameter("@Priority", SqlDbType.Int, 1);
                sqlParameter[1].Value = Priority;

                sqlParameter[2] = new SqlParameter("@Source", SqlDbType.NVarChar, 1000);
                sqlParameter[2].Value = Source;

                sqlParameter[3] = new SqlParameter("@Message", SqlDbType.NVarChar, 4000);
                sqlParameter[3].Value = Message;

                sqlParameter[4] = new SqlParameter("@StackTrace", SqlDbType.NVarChar, 4000);
                sqlParameter[4].Value = StackTrace;

                DATA_ACCESS_LAYER.Fill(Response.ResponseData, "usp_LogError", sqlParameter, DB_CONSTANTS.ConnectionString_Easy_Save);


                Response.ErrorCode = 0;
                Response.ErrorMessage = "Success";
            }
            catch (Exception ex)
            {
                Response.ErrorCode = 2001;
                Response.ErrorMessage = "Error while processing: " + ex.Message;
            }
        }
    }
}
