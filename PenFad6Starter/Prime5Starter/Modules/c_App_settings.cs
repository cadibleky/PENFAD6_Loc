using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PenFad6Starter.DbContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Transactions;
using Dapper;

namespace PenFad6Starter.Modules
{
   public class c_App_settings
    {


        private void add_App_Settings_values(string code, string desc, string value) //, int p_ModuleCode, string p_url, int p_Position, string p_NodeLeaf, string p_Desc, string p_NavType)
        {
           
            var app = new AppSettings();
            try
            {
                using (OracleConnection conp = new OracleConnection(app.conPrime()))
                {
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "ADD_APP_SETTINGS";
                        cmd.Parameters.Add("p_code", OracleDbType.Varchar2).Value = code;
                        cmd.Parameters.Add("p_desc", OracleDbType.Varchar2).Value = desc;
                        cmd.Parameters.Add("p_value", OracleDbType.Varchar2).Value = value;
                        cmd.ExecuteNonQuery();
                        //conp.Close();
                    }
                }

                // ts.Complete();
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }
            finally
            {
                //if (conp.State==ConnectionState.Open)
                //{
                //    conp.Close();
                //}

                // ts.Dispose();
            }

        }


        public void Add_App_settings(DateTime transdate, string company_name)
        {
            try
            {

                add_App_Settings_values("10", "ENFORCE USER PASSWORD EXPIRY", "YES");
                add_App_Settings_values("11", "NUMBER OF DAYS TO PASSWORD EXPIRY", "90");
                add_App_Settings_values("12", "ALLOW PASSWORD FAILURE COUNT", "YES");
                add_App_Settings_values("13", "PASSWORD FAILURE COUNT(NUMEBER OF TIMES)", "5");
  

            }
            catch (Exception ex)
            {
                string ss = ex.ToString();
                throw ex;
            }
            finally
            {

            }
        }






















    }
}
