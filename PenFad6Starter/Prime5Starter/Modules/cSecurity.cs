using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PenFad6Starter.DbContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Transactions;
using System.Data.SqlClient;

namespace PenFad6Starter.Modules
{
    public class cSecurity
    {

        private addController adcon = new addController();

        // system security code is 100 


        public void add_Security_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 100;
                int posi = 0;
                //foldername/controllername/action

                //Users -Parent level : starts with = 10001 -----0-----100
                parentid = 0;
                posi = 10001;
                adcon.add_Module_oracle(10001, "Users", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                     //Users - node level 1: start with : 1000101----10001----100
                parentid = 10001;
                posi = 1000101;
                adcon.add_Module_oracle(1000101, "New User", parentid, modulecode, "Security/User/AddUserTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000102, "Approve User", parentid, modulecode, "Security/User/AddUserPendingTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000103, "Reset User Password", parentid, modulecode, "Security/User/AddUserResetTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000104, "Reassign User Group", parentid, modulecode, "Security/User/AddUserReassignTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000105, "Suspend User", parentid, modulecode, "Security/User/AddUserSuspendTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000106, "Activate User", parentid, modulecode, "Security/User/AddUserActivateTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000107, "View Users", parentid, modulecode, "Security/User/AddUserViewTab", posi += 1, "L", "description", tree);//L
                

                //Roles -Parent level : starts with = 10002 -----0-----100
                parentid = 0;
                posi = 10002;
                adcon.add_Module_oracle(10002, "Roles And Group", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                               //Roles - node level 1: start with : 1000201----10002----100
                parentid = 10002;
                posi = 1000201;
                adcon.add_Module_oracle(1000201, "User Roles", parentid, modulecode, "Security/UserRole/AddRoleTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000202, "User Group", parentid, modulecode, "Security/UserGroup/AddUserGroupTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1000203, "Group Permissions", parentid, modulecode, "Security/UserGroupPermission/AddUserGroupPermissionTab", posi += 1, "L", "description", tree);//L

            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }


        }






        private void add_Module_oracle_NotInUse(int p_ModuleId, string p_ModuleName, int p_ParentId, int p_ModuleCode, string p_url, int p_Position, string p_NodeLeaf, string p_Desc, string p_NavType)
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

        private void add_Module_sql(int p_ModuleId, string p_ModuleName, int p_ParentId, int p_ModuleCode, string p_url, int p_Position, string p_NodeLeaf, string p_Desc, string p_NavType)
        {
            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

            var app = new AppSettings();
            try
            {
                using (SqlConnection conp = new SqlConnection(app.conStringSQL()))
                {
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "ad_controller";
                        cmd.Parameters.Add("@ModuleId", SqlDbType.Decimal).Value = p_ModuleId;
                        cmd.Parameters.Add("@ModuleName", SqlDbType.NVarChar).Value = p_ModuleName;
                        cmd.Parameters.Add("@ParentId", SqlDbType.Decimal).Value = p_ParentId;
                        cmd.Parameters.Add("@Url", SqlDbType.NVarChar).Value = p_url;
                        
                        cmd.Parameters.Add("@ModuleCode", SqlDbType.Decimal).Value = p_ModuleCode;
                        cmd.Parameters.Add("@LeafNode", SqlDbType.VarChar).Value = p_NodeLeaf;
                        //cmd.Parameters.Add("p_Desc", OracleDbType.Varchar2).Value = p_Desc;
                        //cmd.Parameters.Add("p_NavType", OracleDbType.Varchar2).Value = p_NavType;
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
