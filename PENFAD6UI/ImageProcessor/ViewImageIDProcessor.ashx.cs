using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace PENFAD6UI.ImageProcessor
{
    /// <summary>
    /// Summary description for ViewImageIDProcessor
    /// </summary>
    public class ViewImageIDProcessor : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string cust_no;
                if (context.Request.QueryString["Cust_No"] != null)
                {
                    cust_no = (context.Request.QueryString["Cust_No"]).ToString();
                }
                else
                    throw new ArgumentException("No parameter specified");

                context.Response.ContentType = "image/jpeg";

                Stream strm = DisplayKYCImage(cust_no);

                byte[] buffer = new byte[4096];
                int byteSeq = strm.Read(buffer, 0, 4096);

                while (byteSeq > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, byteSeq);
                    byteSeq = strm.Read(buffer, 0, 4096);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public Stream DisplayKYCImage(string Cust_No)
        {
            try
            {

                OracleConnection con;

                //Get connection
                string cnStr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                con = new OracleConnection(cnStr);


                con.Open();
                //var app = new AppSettings();
                //con = app.GetConnection();

                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "Select Employee_Photo_ID from crm_Employee where Cust_No=  '" + Cust_No + "'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;

                cmd.ExecuteScalar();

                return new MemoryStream((byte[])cmd.ExecuteScalar());
                //con.Close();
            }
            catch
            {
                throw;
            }
            //finally
            //{
            //    if (con.State == ConnectionState.Open)
            //    {
            //        con.Close();
            //        if (con != null) { con = null; }
            //    }
            //}

        }

       

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}