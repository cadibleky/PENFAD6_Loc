using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PenFad6Starter.DbContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;


namespace PenFad6Starter.Modules
{
   public class addController
    {


        public void add_Module_oracle(int p_ModuleId, string p_ModuleName, int p_ParentId, int p_ModuleCode, string p_url, int p_Position, string p_NodeLeaf, string p_Desc, string p_NavType)
        {
            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

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
                        cmd.CommandText = "add_sec_controller";
                        cmd.Parameters.Add("p_ModuleId", OracleDbType.Int32).Value = p_ModuleId;
                        cmd.Parameters.Add("p_ModuleName", OracleDbType.Varchar2).Value = p_ModuleName;
                        cmd.Parameters.Add("p_ParentId", OracleDbType.Varchar2).Value = p_ParentId;
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Int32).Value = p_ModuleCode;
                        cmd.Parameters.Add("p_url", OracleDbType.Varchar2).Value = p_url;
                        cmd.Parameters.Add("p_Position", OracleDbType.Int32).Value = p_Position;
                        cmd.Parameters.Add("p_NodeLeaf", OracleDbType.Varchar2).Value = p_NodeLeaf;
                        cmd.Parameters.Add("p_Desc", OracleDbType.Varchar2).Value = p_Desc;
                        cmd.Parameters.Add("p_NavType", OracleDbType.Varchar2).Value = p_NavType;
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







    }
}
