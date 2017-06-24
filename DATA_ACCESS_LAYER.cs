using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DAL
{
   public class DATA_ACCESS_LAYER
    {
        #region FILL DATA TABLE
        public static void Fill(DataTable dataTable, String procedureName, String connString)
        {
            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
            oCommand.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter oAdapter = new SqlDataAdapter();


            oAdapter.SelectCommand = oCommand;
            oConnection.Open();

            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    oAdapter.SelectCommand.Transaction = oTransaction;
                    oAdapter.Fill(dataTable);
                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oAdapter.Dispose();
                }
            }
        }

        public static void Fill(DataTable dataTable, String procedureName, SqlParameter[] parameters, String connString)
        {
            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
            oCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                oCommand.Parameters.AddRange(parameters);

            SqlDataAdapter oAdapter = new SqlDataAdapter();
            oAdapter.SelectCommand = oCommand;

            oConnection.Open();

            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    oAdapter.SelectCommand.Transaction = oTransaction;
                    oAdapter.Fill(dataTable);
                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oAdapter.Dispose();
                }
            }
        }
        #endregion

        #region FILL DATASET
        public static void Fill(DataSet dataSet, String procedureName, String connString)
        {

            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
            oCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter oAdapter = new SqlDataAdapter();

            oAdapter.SelectCommand = oCommand;

            oConnection.Open();
            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    oAdapter.SelectCommand.Transaction = oTransaction;
                    oAdapter.Fill(dataSet);

                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oAdapter.Dispose();
                }
            }
        }

        public static void Fill(DataSet dataSet, String procedureName, SqlParameter[] parameters, String connString)
        {
            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);

            oCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                oCommand.Parameters.AddRange(parameters);

            SqlDataAdapter oAdapter = new SqlDataAdapter();
            oAdapter.SelectCommand = oCommand;

            oConnection.Open();
            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    oAdapter.SelectCommand.Transaction = oTransaction;
                    oAdapter.Fill(dataSet);

                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oAdapter.Dispose();
                }
            }
        }
        #endregion

        #region EXECUTE SCALAR
        public static object ExecuteScalar(String procedureName, String connString)
        {
            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);

            oCommand.CommandType = CommandType.StoredProcedure;
            object oReturnValue;
            oConnection.Open();
            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    oCommand.Transaction = oTransaction;
                    oReturnValue = oCommand.ExecuteScalar();
                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oCommand.Dispose();
                }
            }
            return oReturnValue;
        }

        public static object ExecuteScalar(String procedureName, SqlParameter[] parameters, String connString)
        {

            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);

            oCommand.CommandType = CommandType.StoredProcedure;
            object oReturnValue;
            oConnection.Open();
            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    if (parameters != null)
                        oCommand.Parameters.AddRange(parameters);

                    oCommand.Transaction = oTransaction;
                    oReturnValue = oCommand.ExecuteScalar();

                    oTransaction.Commit();

                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oCommand.Dispose();
                }
            }
            return oReturnValue;
        }
        #endregion

        #region EXECUTE NON QUERY
        public static int ExecuteNonQuery(string procedureName, string connString)
        {

            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);

            oCommand.CommandType = CommandType.StoredProcedure;
            int iReturnValue;

            oConnection.Open();
            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    oCommand.Transaction = oTransaction;
                    iReturnValue = oCommand.ExecuteNonQuery();
                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oCommand.Dispose();
                }
            }
            return iReturnValue;
        }
        public static int ExecuteNonQuery(string procedureName, SqlParameter[] parameters, string connString)
        {
            SqlConnection oConnection = new SqlConnection(connString);
            SqlCommand oCommand = new SqlCommand(procedureName, oConnection);

            oCommand.CommandType = CommandType.StoredProcedure;
            int iReturnValue;

            oConnection.Open();
            using (SqlTransaction oTransaction = oConnection.BeginTransaction())
            {
                try
                {
                    if (parameters != null)
                        oCommand.Parameters.AddRange(parameters);

                    oCommand.Transaction = oTransaction;
                    iReturnValue = oCommand.ExecuteNonQuery();

                    oTransaction.Commit();
                }
                catch
                {
                    oTransaction.Rollback();
                    throw;
                }
                finally
                {
                    if (oConnection.State == ConnectionState.Open)
                        oConnection.Close();

                    oConnection.Dispose();
                    oCommand.Dispose();
                }
            }
            return iReturnValue;
        }
        #endregion
    }
}
