using Dapper;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using System.Collections;

namespace PENFAD6DAL.Repository.Security
{
    public class cSecurityRepo
    {
        //  -----Louis----

        private readonly Sec_UserGroupRightTemp secgrouprights_temp = new Sec_UserGroupRightTemp();
        public DataSet GetNodes_NotInUse(decimal ModuleCode)
        {
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
                        cmd.CommandText = "sel_sec_Controller";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor,ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "node");
                            return ds;
                        }
                    }
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
        public DataSet GetNodesChilds_NotInUse(decimal parentid)
        {
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
                        cmd.CommandText = "sel_sec_ControllerByParentId";
                        cmd.Parameters.Add("p_parentid",OracleDbType.Decimal,ParameterDirection.Input).Value = parentid;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "child");
                            return ds;
                        }
                    }
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
        public DataSet GetNodesTest()
        {
            var app = new AppSettings();
            using (SqlConnection conp = new SqlConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "aaTest";
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "node");
                            return ds;
                        }
                    }
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

        public DataSet GetNodesChildsTest(int parentid)
        {
            var app = new AppSettings();
            using (SqlConnection conp = new SqlConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "aaTestChild";
                        cmd.Parameters.AddWithValue("@parentid", parentid);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "child");
                            return ds;
                        }
                    }
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


        public int AddGroupPermissionDataTable(DataTable dtable, decimal Modulecode)
        {
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
               
                try
                {
                    int i = 0;
                    //using (SqlConnection conp = new SqlConnection(app.conString()))
                    //{
                    //    conp.Open();
                    //    using (SqlCommand cmd = new SqlCommand())
                    //    {
                    //        cmd.Connection = conp;
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.CommandText = "upd_sec_UserGroupPermission";
                    //        cmd.Parameters.AddWithValue("@dt", dtable);
                    //        cmd.Parameters.AddWithValue("@mainOrsub", "sub");
                    //        cmd.Parameters.AddWithValue("@UserId", GlobalValue.User_ID);
                    //        cmd.ExecuteNonQuery();
                    //        i = 1;
                    //    }
                    //}


                    conp.Open();

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
                        da.Update(dtable);
                    }

                    //update records for this user for sub module (i.e left treenode navigations)
                    using (OracleCommand cmdup = new OracleCommand())
                    {
                        cmdup.Connection = conp;
                        cmdup.CommandType = CommandType.StoredProcedure;
                        cmdup.CommandText = "upd_sec_UserGroupPermission";
                        cmdup.Parameters.Add("p_UserId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                        cmdup.Parameters.Add("p_mainOrsub", OracleDbType.Varchar2).Value = "sub";
                        cmdup.ExecuteNonQuery();

                    }

                    i += 1;
                    return i;
                }
                catch (Exception ex)
                {

                    string exss = ex.ToString();
                    return 0;
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

        public DataTable GetMajorModuleAcessForAUserGroup_NotInUse(string usergroupid, int modulecode)
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
                        cmd.Parameters.Add("p_modulecode", OracleDbType.Int32).Value = 1;////always 1 for loading major permission for a user group
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Int32).Value = usergroupid;

                        OracleParameter acessrights = new OracleParameter("@acessrights", OracleDbType.Varchar2);
                        acessrights.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(acessrights);
                        cmd.ExecuteNonQuery();

                        grouppermissions = acessrights.Value.ToString();
                        grouppermissions = AES_Decrypt(grouppermissions);
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
                        if (modulecode == 0)
                        {

                            //insert major module codes into temp
                            using (OracleCommand cmd1 = new OracleCommand())
                            {
                                cmd1.Connection = conp;
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.CommandText = "upd_sec_UserGroupPermission";

                                //cmd1.Parameters.Add("@dt", OracleDbType.Int32, dt.Select("")).Value = dt;
                                //cmd1.Parameters.Add("@mainOrsub", OracleDbType.Varchar2).Value = "main";
                                cmd1.Parameters.Add("@UserId", OracleDbType.Int32).Value = GlobalValue.User_ID;
                                cmd1.ExecuteNonQuery();
                            }

                            dt.Rows.Clear();
                            using (OracleCommand cmd2 = new OracleCommand())
                            {
                                cmd2.Connection = conp;
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.CommandText = "sel_sec_UserGroupRightTempForUser";
                                //cmd2.Parameters.AddWithValue("@UserId", GlobalValue.User_ID);
                                //cmd2.Parameters.AddWithValue("@ParentId", 0);
                                //cmd2.Parameters.AddWithValue("@MajorOrMenus", 0);
                                //cmd2.Parameters.AddWithValue("@ModuleCode", modulecode);
                                using (OracleDataAdapter da = new OracleDataAdapter(cmd2))
                                {
                                    da.Fill(dt);
                                    return dt;
                                }
                            }

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

        //GET GROUP ID
        public void Get_GroupId(string userid)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_USER_ID", userid, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GROUPID", param, commandType: CommandType.StoredProcedure);
                GlobalValue.User_GroupID = param.Get<string>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetMajorModuleAcessForAUserGroup(string usergroupid, decimal modulecode)
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
                        grouppermissions = AES_Decrypt(grouppermissions);
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
                        if (modulecode == 0)
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

                           
                            dt.Rows.Clear();
                            // generate all major modules for user
                            using (OracleCommand cmd2 = new OracleCommand())
                            {
                                cmd2.Connection = conp;
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.CommandText = "sel_sec_UserGrpRightForUser";
                                cmd2.Parameters.Add("p_UserId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                                //cmd2.Parameters.Add("p_ParentId", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
                                cmd2.Parameters.Add("p_MajorOrMenus", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
                                cmd2.Parameters.Add("p_ModuleCode", OracleDbType.Varchar2, ParameterDirection.Input).Value = modulecode;

                                cmd2.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                                using (OracleDataAdapter da = new OracleDataAdapter(cmd2))
                                {
                                    da.Fill(dt);
                                    return dt;
                                }
                            }

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


        public int UpdateGroupPermission(string usergroupid, string access, int ModuleCode)
        {
            string access_majormodules = "";
            var app = new AppSettings();
            try
            {
                DataTable majormodule_dt = GetMajorModuleAcessForAUserGroup(usergroupid, ModuleCode);
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

                


                access_majormodules = AES_Encrypt(access_majormodules);
                access = AES_Encrypt(access);
                int i = 0;
                using (SqlConnection conp = new SqlConnection(app.conString()))
                {
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "upd_sec_UserGroupRights";
                        cmd.Parameters.AddWithValue("@modulecode", ModuleCode);
                        cmd.Parameters.AddWithValue("@UserGroupId", usergroupid);
                        cmd.Parameters.AddWithValue("@acessrights", access);
                        cmd.Parameters.AddWithValue("@acessrightsmajormodules", access_majormodules);
                        cmd.ExecuteNonQuery();
                    }
                }
                return i;
            }
            catch (Exception ex)
            {

                string exss = ex.ToString();
                return 0;
            }
        }


        public static string AES_Encrypt(string input_text)
        {
            //Please dont change or EDIT ANY PART of THIS CODE
            // ''' security_Code= teksolencrypt$@teksol.com987908123
            string security_Code = "teksolencrypt$@teksol.com987908123";
            System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged();
            System.Security.Cryptography.MD5CryptoServiceProvider Hash_AES = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string encrypted = "";
            try
            {
                byte[] hash = new byte[32];
                byte[] temp = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(security_Code));
                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);
                AES.Key = hash;
                AES.Mode = System.Security.Cryptography.CipherMode.ECB;
                //'Security.Cryptography.CipherMode.ECB
                System.Security.Cryptography.ICryptoTransform DESEncrypter = AES.CreateEncryptor();
                byte[] Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(input_text);
                encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return encrypted;
        }
        public static string AES_Decrypt(string input_text)
        {
            //Please dont change or EDIT ANY PART of THIS CODE
            // ''' security_Code= teksolencrypt$@teksol.com987908123
            string securitycode = "teksolencrypt$@teksol.com987908123";
            System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged();
            System.Security.Cryptography.MD5CryptoServiceProvider Hash_AES = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string decrypted = "";
            try
            {
                byte[] hash = new byte[32];
                byte[] temp = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(securitycode));
                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);
                AES.Key = hash;
                AES.Mode = CipherMode.ECB;
                // Security.Cryptography.CipherMode.ECB
                System.Security.Cryptography.ICryptoTransform DESDecrypter = AES.CreateDecryptor();
                byte[] Buffer = Convert.FromBase64String(input_text);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return decrypted;
        }


        public DataTable GetGroupPermissions(decimal modulecode, string usergroupid)
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
                        grouppermissions = AES_Decrypt(grouppermissions);
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
        
        public DataSet GetNodesForAUserAFromTemp(int ParentId, decimal modulecode)
        {
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
                        cmd.CommandText = "sel_sec_UserGrpRightForUser";
                        cmd.Parameters.Add("p_UserId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                        //cmd.Parameters.Add("p_ParentId", OracleDbType.Decimal, ParameterDirection.Input).Value= ParentId;
                        cmd.Parameters.Add("p_MajorOrMenus", OracleDbType.Int32, ParameterDirection.Input).Value = 9; //9 is ok for this. dont change it
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal, ParameterDirection.Input).Value = modulecode;

                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "node");
                            return ds;
                        }
                    }
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


        public DataSet GetUrls(int Moduleid)
        {
            var app = new AppSettings();
            using (SqlConnection conp = new SqlConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_UserControllerLeafsByModuleCode";
                        cmd.Parameters.AddWithValue("@ModuleId", Moduleid);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "url"); //, "node");
                            ds.Tables["url"].PrimaryKey = new DataColumn[] { ds.Tables["url"].Columns["ModuleId"] };
                            return ds;
                        }
                    }
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

        public DataSet GetMajorModules()
        {
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
                        cmd.CommandText = "sel_sec_MajorControllers";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "node");
                            return ds;
                        }
                    }
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

        public DataSet GetMajorModulesForAUserGroup()
        {
            var app = new AppSettings();
            using (SqlConnection conp = new SqlConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ModLevel1";
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "node");
                            return ds;
                        }
                    }
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
        public DataSet GetMajorModulesGroupPermission()
        {
            var app = new AppSettings();
            using (SqlConnection conp = new SqlConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ModLevel1";
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "node");
                            return ds;
                        }
                    }
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
