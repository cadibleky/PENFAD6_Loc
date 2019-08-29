
using Oracle.ManagedDataAccess.Client;
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



namespace PenFad6Starter.DbContext
{

    public class AppSettings : IDisposable
    {
        //private IDbCommand cmd;
        private IDbConnection cn;
        DataProvider dp;

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
            //string proStr = ConfigurationManager.AppSettings["Prime5ClientProvider"];
            //string cnStr = ConfigurationManager.AppSettings["Prime5DBConnectionString"];

            string proStr = ConfigurationManager.ConnectionStrings["DbConnection"].ProviderName;
            string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

            //if (proStr == "System.Data.SqlClient")
            //{
            //dp = (DataProvider)Enum.Parse(typeof(DataProvider), "SqlServer");
            // cn = new SqlConnection(cnStr);
            cn = new OracleConnection(cnStr);
            //}
            //else
            //{
            //    dp = (DataProvider)Enum.Parse(typeof(DataProvider), proStr);
            //    // Get a specific connection.
            //    cn = GetConnection(dp);
            //    cn.ConnectionString = cnStr;
            //}

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

        public string conPrime()
        {
            // Get provider and connection string
            //string proStr = ConfigurationManager.AppSettings["provider"];
            //string cnStr = ConfigurationManager.AppSettings["DbSqlstring"];

            string proStr = ConfigurationManager.ConnectionStrings["DbConnection"].ProviderName;
            string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

            //if (proStr == "System.Data.SqlClient")
            //{
            //    dp = (DataProvider) Enum.Parse(typeof(DataProvider), "SqlServer");
            //    cn = new SqlConnection(cnStr);
            //}
            //else
            //{
            //    dp = (DataProvider) Enum.Parse(typeof(DataProvider), proStr);
            //    // Get a specific connection.
            //    cn = GetConnection(dp);
            //    cn.ConnectionString = cnStr;
            //}


            return cnStr;


        }

        public static string conString()
        {
            // Get provider and connection string
            //string proStr = ConfigurationManager.AppSettings["provider"];
            //string cnStr = ConfigurationManager.AppSettings["DbSqlstring"];

            string proStr = ConfigurationManager.ConnectionStrings["DbConnection"].ProviderName;
            string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
 


            return cnStr;


        }

        public   string conStringSQL()
        {
            string cnStr = ConfigurationManager.ConnectionStrings["sqlcon"].ConnectionString;
            return cnStr;


        }




    }

}
