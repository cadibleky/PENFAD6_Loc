
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;

using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PENFAD6DAL.DbContext
{

    public class AppSettings : IDisposable
    {
        //private IDbCommand cmd;
        private IDbConnection cn;
        //DataProvider dp;

        public void Dispose()
        {
            if (cn != null)
            {
                cn.Dispose();
                cn = null;
            }
        }
        public IDbConnection GetConnection()
        {
            // Get provider and connection string

            string proStr = ConfigurationManager.ConnectionStrings["DbConnection"].ProviderName;
            string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

            cn = new OracleConnection(cnStr);


            cn.Open();
            return cn;


        }
        static IDbConnection GetConnection(DataProvider dp)
        {
            IDbConnection conn = null;
            switch (dp)
            {
                case DataProvider.SqlServer:
                    conn = new SqlConnection();
                    break;
                case DataProvider.OleDb:
                    conn = new OleDbConnection();
                    break;
                case DataProvider.Odbc:
                    conn = new OdbcConnection();
                    break;
                //case DataProvider.Oracle:
                //    conn = new OracleConnection();
                //    break;
            }
            return conn;
        }

        public static void CloseConnection(IDbConnection cnn)
        {
            cnn.Close();
        }


        //public string conPrime()
        //{
        //    string proStr = ConfigurationManager.ConnectionStrings["DbConnection"].ProviderName;
        //    string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            
        //    return cnStr;
            
        //}
        public string conString()
        {
            // Get provider and connection string
            //string proStr = ConfigurationManager.AppSettings["provider"];
            //string cnStr = ConfigurationManager.AppSettings["DbSqlstring"];

            string proStr = ConfigurationManager.ConnectionStrings["DbConnection"].ProviderName;
            string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;



            return cnStr;


        }




    }

}
