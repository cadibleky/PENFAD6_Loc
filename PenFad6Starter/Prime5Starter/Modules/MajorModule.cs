using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PenFad6Starter.DbContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace PenFad6Starter.Modules
{
  public  class MajorModule
    {
        public void add_Majormodules()
        {
            add_MajorModule(11, "CRM ", 11, "Active");
            add_MajorModule(12, "Remittance Management", 12, "Active");
            add_MajorModule(13, "Portfolio Management", 13, "Active");
            add_MajorModule(14, "Scheme Accounting", 14, "Active");
            add_MajorModule(15, "Utilities", 15, "Active");

            add_MajorModule(100, "Security", 1000, "Active");
            add_MajorModule(101, "Setup", 1001, "Active");
           
        }

        private void add_MajorModule(int p_ModuleCode, string p_ModuleName, int p_ModulePosition, string p_ModuleStatus) //, string makerid, DateTime makedate)
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
                        cmd.CommandText = "add_sec_ControllerMajor";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Int32).Value = p_ModuleCode;
                        cmd.Parameters.Add("p_ModuleName", OracleDbType.Varchar2).Value = p_ModuleName;
                        cmd.Parameters.Add("p_ModulePosition", OracleDbType.Int32).Value = p_ModulePosition;
                        cmd.Parameters.Add("p_ModuleStatus", OracleDbType.Varchar2).Value = p_ModuleStatus;
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
               
            }

        }

        //end of class
    }
}
