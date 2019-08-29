using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;
using System.Collections;
using System.Data.SqlClient;
using System.Transactions;

namespace PENFAD6DAL.Repository.Security
{
   public class sec_UserGroupPermissionRepo
    {
        [Key]
        public string UserGroupId { get; set; }
        public string UserGroupName { get; set; }
        public string UserRoleId { get; set; }
        public string UserRoleName { get; set; }
        public string MajorModuleCode { get; set; }
        public string MajorModuleName { get; set; }


        public decimal ModuleId { get; set; }
        public string ModuleName { get; set; }
        public decimal ParentId { get; set; }
        public decimal ModuleCode { get; set; }
        public string Url { get; set; }
        public decimal Position { get; set; }
        public string NodeLeaf { get; set; }
        public string NavType { get; set; }






        private cSecurityRepo sec = new cSecurityRepo();

        public DataSet GetSubModules_ForMajorModuleForPermissions(decimal ModuleCode)
        {
            var data = new DataSet();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ControllerByModuleCode";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_UserId", OracleDbType.Varchar2).Value = GlobalValue.User_ID;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            // DataTable dt = new DataTable();
                            da.Fill(data, "nodeT");
                        }
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        //public DataSet GetModules_DataSetsql(decimal ModuleCode)
        //{

        //    DataSet data = new DataSet();
        //    var app = new AppSettings();

        //    using (SqlConnection conp = new SqlConnection(app.conPrimeSQL()))
        //    {
        //        try
        //        {
        //            //Get connection
        //            conp.Open();
        //            using (SqlCommand cmd = new SqlCommand())
        //            {
        //                cmd.Connection = conp;
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.CommandText = "sel_controllers"; //"sel_sec_ControllerByModuleCode";
        //                cmd.Parameters.Add("@ModuleCode", SqlDbType.Decimal).Value = ModuleCode;
        //                //cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
        //                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //                {
        //                    DataSet ds = new DataSet();
        //                    da.Fill(ds, "nodeT");
        //                    data = ds;


        //                }
        //            }

        //            return data;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (conp.State == ConnectionState.Open)
        //            {
        //                conp.Close();
        //                conp.Dispose();
        //            }
        //        }
        //    }

        //}


        public DataTable GetPermissionsForAUserGroup_ForAccessRights( string usergroupid, decimal modulecode)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RecId", typeof(string));
            dt.Columns.Add("ModuleId", typeof(int));
            dt.Columns.Add("UserGroupId", typeof(string));
            dt.Columns.Add("UserId", typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns["ModuleId"] };

            string grouppermissions = string.Empty;
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {

                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_UserGroupPermission";
                        cmd.Parameters.Add("p_modulecode", OracleDbType.Decimal, ParameterDirection.Input).Value = modulecode;////always 1 for loading major permission for a user group
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Varchar2, ParameterDirection.Input).Value = usergroupid;
                        //cmd.Parameters.add("p_acessrights", OracleDbType.Varchar2, ParameterDirection.Output);

                        OracleParameter acessrights = new OracleParameter("p_acessrights", OracleDbType.Varchar2, 4000);
                        acessrights.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(acessrights);

                        cmd.ExecuteNonQuery();

                        grouppermissions = acessrights.Value.ToString();
                        grouppermissions = cSecurityRepo.AES_Decrypt(grouppermissions);
                        //split the perssions
                        string[] separators = { "|" }; // { ",", ".", "!", "?", ";", ":", " " };
                        string[] grouprights = grouppermissions.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        //fill data table with rights
                        foreach (var ro in grouprights)
                        {
                            string recid = (GlobalValue.User_ID + usergroupid + ro);
                            int moduleidd = Convert.ToInt32(ro);
                            dt.Rows.Add(recid, moduleidd, usergroupid, GlobalValue.User_ID);
                        }
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    //throw ex;
                    string ss = ex.ToString();
                    return dt;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        public DataTable GetPermissionsForAUserGroup_ForAccessRights_ForMajor_NotInUse(string usergroupid, decimal modulecode)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("RecId", typeof(string));
            dt.Columns.Add("ModuleId", typeof(int));
            dt.Columns.Add("UserGroupId", typeof(string));
            dt.Columns.Add("UserId", typeof(string));
            //dt.Columns.Add("mainOrSub", typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns["p_ModuleId"] };



            string grouppermissions = string.Empty;
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();

                    using (OracleCommand cmd = new OracleCommand())
                    {
                        decimal oone = 1;
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_UserGroupPermission";
                        cmd.Parameters.Add("p_modulecode", OracleDbType.Decimal, ParameterDirection.Input).Value = oone;////always 1 for loading major permission for a user group
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Varchar2, ParameterDirection.Input).Value = usergroupid;
                        //cmd.Parameters.add("p_acessrights", OracleDbType.Varchar2, ParameterDirection.Output);

                        OracleParameter acessrights = new OracleParameter("p_acessrights", OracleDbType.Varchar2, 4000);
                        acessrights.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(acessrights);

                        cmd.ExecuteNonQuery();

                        grouppermissions = acessrights.Value.ToString();
                        grouppermissions = cSecurityRepo.AES_Decrypt(grouppermissions);
                        //split the perssions
                        string[] separators = { "|" }; // { ",", ".", "!", "?", ";", ":", " " };
                        string[] grouprights = grouppermissions.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        //fill data table with rights
                        foreach (var ro in grouprights)
                        {
                            string recid = (GlobalValue.User_ID + usergroupid + ro);
                            int moduleidd = Convert.ToInt32(ro);
                            dt.Rows.Add(recid, moduleidd, usergroupid, GlobalValue.User_ID);
                        }
                        // return existing permissions for menu links
                        if (modulecode > 0)
                        {
                            //----------------------starts-----the code here originally were in one procedure in sql- re-write if oracle support bulkcopy--------------------- 
                            // proc name is :  upd_sec_UserGroupPermission
                            //delete records for this user
                            using (OracleCommand cmddel = new OracleCommand())
                            {
                                cmddel.Connection = conp;
                                cmddel.CommandType = CommandType.Text;
                                cmddel.CommandText = "delete from SEC_USERGROUPRIGHTTEMP where USERID = :p_UserId";
                                cmddel.Parameters.Add(":p_UserId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                                cmddel.ExecuteNonQuery();
                            }
                            //insert major module codes into temp
                            using (OracleCommand cmd1 = new OracleCommand())
                            {
                                OracleDataAdapter da = new OracleDataAdapter();
                                da.InsertCommand = new OracleCommand("insert into SEC_USERGROUPRIGHTTEMP  (RECID,MODULEID,USERGROUPID,USERID) values ( :p_RecId,:p_ModuleId,:p_UserGroupId,:p_UserId)", conp);
                                da.InsertCommand.Parameters.Add(":p_RecId", OracleDbType.Varchar2, 1000, "RecId");
                                da.InsertCommand.Parameters.Add(":p_ModuleId", OracleDbType.Decimal, 100, "ModuleId");
                                da.InsertCommand.Parameters.Add(":p_UserGroupId", OracleDbType.Varchar2, 100, "UserGroupId");
                                da.InsertCommand.Parameters.Add(":p_UserId", OracleDbType.Varchar2, 100, "UserId");
                                ////da.InsertCommand.Parameters.Add("p_mainOrsub", OracleDbType.Varchar2).Value = "main";
                                da.Update(dt);
                            }

                            // update records for this user for sub module (i.e left treenode navigations)
                            //using (OracleCommand cmdup = new OracleCommand())
                            //{
                            //    cmdup.Connection = conp;
                            //    cmdup.CommandType = CommandType.StoredProcedure;
                            //    cmdup.CommandText = "upd_sec_UserGroupPermission";
                            //    cmdup.Parameters.Add("p_UserId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                            //    cmdup.Parameters.Add("p_mainOrsub", OracleDbType.Varchar2).Value = "main";
                            //    cmdup.ExecuteNonQuery();

                            //}
                            //------------------------------------end------

                            dt.Rows.Clear();
                            //// generate all major modules for user
                            //using (OracleCommand cmd2 = new OracleCommand())
                            //{
                            //    cmd2.Connection = conp;
                            //    cmd2.CommandType = CommandType.StoredProcedure;
                            //    cmd2.CommandText = "sel_sec_UserGrpRightForUser";
                            //    cmd2.Parameters.Add("p_UserId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                            //    //cmd2.Parameters.Add("p_ParentId", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
                            //    cmd2.Parameters.Add("p_MajorOrMenus", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
                            //    cmd2.Parameters.Add("p_ModuleCode", OracleDbType.Varchar2, ParameterDirection.Input).Value = modulecode;

                            //    cmd2.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                            //    using (OracleDataAdapter da = new OracleDataAdapter(cmd2))
                            //    {
                            //        da.Fill(dt);
                            //        return dt;
                            //    }
                            //}


                            return dt;


                        }
                        else
                        {
                            return dt;
                        }

                    }

                    //retrive details


                    // return dt;
                }
                catch (Exception ex)
                {
                    //throw ex;
                    string ss = ex.ToString();
                    dt.Rows.Clear();
                    return dt;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }


        public bool SaveRights_ForUseGroup(string usergroupid, string access, decimal ModuleCode) //, string status, string makerid, DateTime makedate)
        {

            DataTable majormodule_dt = sec.GetMajorModuleAcessForAUserGroup(usergroupid, ModuleCode);

            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

            var app = new AppSettings();
            string access_majormodules = "";
            try
            {
                
                if (majormodule_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in majormodule_dt.Rows)
                    {
                        string mod_code = dr["ModuleId"].ToString();
                        if (mod_code != ModuleCode.ToString())
                        {
                            access_majormodules = access_majormodules + '|' + mod_code.ToString();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(access))
                {
                    access_majormodules = access_majormodules + '|' + ModuleCode.ToString();
                }









                access_majormodules = cSecurityRepo.AES_Encrypt(access_majormodules);
                access = cSecurityRepo.AES_Encrypt(access);


                using (OracleConnection conp = new OracleConnection(app.conString()))
                {
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "upd_sec_UserGroupRights";
                        cmd.Parameters.Add("p_modulecode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Varchar2).Value = usergroupid;
                        cmd.Parameters.Add("p_acessrights", OracleDbType.Varchar2).Value = access;
                        cmd.Parameters.Add("p_acessrightsmajormodules", OracleDbType.Varchar2).Value = access_majormodules;
                        cmd.ExecuteNonQuery();
                        //conp.Close();
                    }
                }

                //ts.Complete();
                return true;
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }
            finally
            {
                //if (conp.State == ConnectionState.Open)
                //{
                //    conp.Close();
                //}
                //ts.Dispose();
            }

        }












        public List<sec_UserGroupPermissionRepo> GetModules(decimal ModuleCode)
        {

            var data = new List<sec_UserGroupPermissionRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ControllerByModuleCode";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            data = dt.AsEnumerable().Select(row => new sec_UserGroupPermissionRepo
                            {
                                //moduleId = row.Field<decimal>("MODULE_ID"),
                                //moduleName = row.Field<string>("MODULE_NAME"),
                                //ParentId = row.Field<decimal>("PARENT_ID"),
                                //moduleCode = row.Field<decimal>("MODULE_CODE"),
                                //Url = row.Field<string>("URL"),
                                ////Position = row.Field<decimal>("EMPLOYEE_ID"),
                                //NodeLeaf = row.Field<string>("NODE_LEAF"),
                                //NavType = row.Field<string>("NAV_TYPE")

                            }).ToList();

                        }
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }


        public IEnumerable<sec_UserGroupPermissionRepo> GetModules_new(decimal ModuleCode)
        {
            IEnumerable<sec_UserGroupPermissionRepo> data; // = new List<sec_TreeNode>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ControllerByModuleCode";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            data = dt.AsEnumerable().Select(row => new sec_UserGroupPermissionRepo
                            {
                                //moduleId = row.Field<decimal>("MODULE_ID"),
                                //moduleName = row.Field<string>("MODULE_NAME"),
                                //ParentId = row.Field<decimal>("PARENT_ID"),
                                //moduleCode = row.Field<decimal>("MODULE_CODE"),
                                //Url = row.Field<string>("URL"),
                                ////Position = row.Field<decimal>("EMPLOYEE_ID"),
                                //NodeLeaf = row.Field<string>("NODE_LEAF"),
                                //NavType = row.Field<string>("NAV_TYPE")

                            }).ToList();

                        }
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }



















    }
}
