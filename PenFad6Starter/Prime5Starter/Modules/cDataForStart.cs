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
    public class cDataForStart
    {

        public void Add_Initial_Data(DateTime transdate )
        {
            try
            {
                string maker_id = "teksol.admin";
                Add_GL_MajorAccounts(transdate);

                //add admin role
                addUserRoles("R10", "TekSol Admin", "TekSol Administrator", "Active", "teksol.admin", DateTime.Now);
                addUserRoles("R100", "System Admin", "System Administrator", "Active", "teksol.admin", DateTime.Now);
                //add user group
                addUseGroup("G10", "R10", "TekSol Administrator", "TekSol User Group", "Active", "teksol.admin", transdate);
                addUseGroup("G100", "R100", "System Administrator", "System User Group", "Active", "teksol.admin", transdate);

                //persimmions for security 
                string groupmajorPermission = string.Empty;
                groupmajorPermission = groupmajorPermission + "|" + "100|" + "101|";
                string GroupPermission = string.Empty;
                GroupPermission = (GroupPermission + "|" + "10001|" + "1000101|" + "1000102|" + "1000103|" + "1000104|" + "1000105|" + "1000106|" + "10002|" + "1000201|" + "1000202|" + "1000203|" + "10101|" + "1010101|" + "1010102|" + "10102|").ToString();

                groupmajorPermission = AES_Encrypt(groupmajorPermission, "teksolencrypt$@teksol.com987908123");
                GroupPermission = AES_Encrypt(GroupPermission, "teksolencrypt$@teksol.com987908123");

                AssignRightsToUseGroup(100, "G10", GroupPermission, groupmajorPermission);
                AssignRightsToUseGroup(100, "G100", GroupPermission, groupmajorPermission);

                string GroupPermission_setup = string.Empty;
                GroupPermission_setup = (GroupPermission_setup + "10101|" + "1010101|" + "1010102|" + "1010103|" + "1010104|" + "1010105|" + "10102|").ToString();
                GroupPermission_setup = AES_Encrypt(GroupPermission_setup, "teksolencrypt$@teksol.com987908123");
                AssignRightsToUseGroup(101, "G10", GroupPermission_setup, groupmajorPermission);
                AssignRightsToUseGroup(101, "G100", GroupPermission_setup, groupmajorPermission);

                //add user for teksol
                string teksolpassword = "teksol@ABCabc123";
                string syspassword = "teksol@administrator";
                teksolpassword = AES_Encrypt(teksolpassword, "teksolencrypt$@teksol.com987908123");
                syspassword = AES_Encrypt(syspassword, "teksolencrypt$@teksol.com987908123");

                //dc.add_EmployeeForAdminUser("SYSTEM", "For_System_Use", "TekSol", "Admin", "User");


                Add_Admin_Users_Dapper("teksol.admin", teksolpassword, "TEKSOL USER", "G10", transdate, "teksol.admin", transdate);
                Add_Admin_Users_Dapper("sys.admin", syspassword, "SYSTEM ADMIN", "G100", transdate, "teksol.admin", transdate);

                //DEFUALT FUNDS
               
                Add_Default_fund("SYSTEM FUND", "", maker_id, transdate); // code is 01

                Add_Default_District("NA", maker_id); // code is 01


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

        public void Add_GL_MajorAccounts(DateTime makedate)
        {
            try
            {
                string makerid = "teksol.user";
                //addGL_MajorAccounts(1, "ASSETS", makerid, makedate);
                //addGL_MajorAccounts(2, "LIABILITIES", makerid, makedate);
                //addGL_MajorAccounts(3, "DEALING WITH MEMERS", makerid, makedate);
                //addGL_MajorAccounts(4, "EXPENSES", makerid, makedate);

                //addGL_MajorAccounts(101, "BANK", makerid, makedate);
                //addGL_MajorAccounts(101, "BANK", makerid, makedate);


                //addGL_MajorAccounts(2, "LIABILITIES", makerid, makedate);
                //addGL_MajorAccounts(3, "DEALING WITH MEMERS", makerid, makedate);
                //addGL_MajorAccounts(4, "EXPENSES", makerid, makedate);



            }
            catch (Exception)
            {
                throw;
            }
        }
        //private void addGL_MajorAccounts(decimal gl_major_code, string desc, string makerid, DateTime makedate)
        //{


        //    //TransactionOptions tsOp = new TransactionOptions();
        //    //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
        //    //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

        //    var app = new AppSettings();

        //    try
        //    {
        //        using (OracleConnection conp = new OracleConnection(app.conPrime()))
        //        {
        //            conp.Open();
        //            using (OracleCommand cmd = new OracleCommand())
        //            {
        //                cmd.Connection = conp;
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.CommandText = "add_gl_major_acct";
        //                cmd.Parameters.Add("p_glcode", OracleDbType.Decimal, ParameterDirection.Input).Value = gl_major_code;
        //                cmd.Parameters.Add("p_description", OracleDbType.Varchar2, ParameterDirection.Input).Value = desc;
        //                cmd.Parameters.Add("p_makerid", OracleDbType.Varchar2, ParameterDirection.Input).Value = makerid;
        //                cmd.Parameters.Add("p_makerdate", OracleDbType.Date, ParameterDirection.Input).Value = makedate;
        //                cmd.ExecuteNonQuery();
        //                //conp.Close();
        //            }
        //        }

        //        // ts.Complete();
        //    }
        //    catch (Exception ex)
        //    {
        //        string sss = ex.ToString();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //if (conp.State==ConnectionState.Open)
        //        //{
        //        //    conp.Close();
        //        //}

        //        // ts.Dispose();
        //    }

        //}
        private void addUserRoles(string Roleid, string RoleName, string Desc, string status, string makerid, DateTime makedate)
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
                        cmd.CommandText = "mix_sec_UserRole";
                        cmd.Parameters.Add("P_UserRoleId", OracleDbType.Varchar2).Value = Roleid;
                        cmd.Parameters.Add("p_UserRoleName", OracleDbType.Varchar2).Value = RoleName;
                        cmd.Parameters.Add("p_UserRoleDesc", OracleDbType.Varchar2).Value = Desc;
                        cmd.Parameters.Add("p_UserRoleStatus", OracleDbType.Varchar2).Value = status;
                        cmd.Parameters.Add("p_MakerId", OracleDbType.Varchar2).Value = makerid;
                        cmd.Parameters.Add("p_MakeDate", OracleDbType.Date).Value = makedate;
                        cmd.ExecuteNonQuery();
                        conp.Close();
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

        private void addUseGroup(string GroupId, string Roleid, string GroupName, string Desc, string status, string makerid, DateTime makedate)
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
                        cmd.CommandText = "mix_sec_UserRoleGroup";
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Varchar2).Value = GroupId;
                        cmd.Parameters.Add("P_UserRoleId", OracleDbType.Varchar2).Value = Roleid;
                        cmd.Parameters.Add("p_UserGroupName", OracleDbType.Varchar2).Value = GroupName;
                        cmd.Parameters.Add("p_UserGroupDesc", OracleDbType.Varchar2).Value = Desc;
                        cmd.Parameters.Add("p_UserGroupStatus", OracleDbType.Varchar2).Value = status;
                        cmd.Parameters.Add("p_MakerId", OracleDbType.Varchar2).Value = makerid;
                        cmd.Parameters.Add("p_MakeDate", OracleDbType.Date).Value = makedate;
                        cmd.ExecuteNonQuery();
                        conp.Close();
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

        private void AssignRightsToUseGroup(int p_modulecode, string p_UserGroupId, string p_acessrights, string p_acessrightsmajormodules) //, string status, string makerid, DateTime makedate)
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
                        cmd.CommandText = "upd_sec_UserGroupRights";
                        cmd.Parameters.Add("p_modulecode", OracleDbType.Int32).Value = p_modulecode;
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Varchar2).Value = p_UserGroupId;
                        cmd.Parameters.Add("p_acessrights", OracleDbType.Varchar2).Value = p_acessrights;
                        cmd.Parameters.Add("p_acessrightsmajormodules", OracleDbType.Varchar2).Value = p_acessrightsmajormodules;
                        cmd.ExecuteNonQuery();
                        conp.Close();
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

        private void Add_Admin_Users_ADO(string p_UserId, string p_Password, string p_fullname, string p_UserGroupId, DateTime p_PasswordExpiryDate, string p_MakerId, DateTime p_MakeDate) //, string status, string makerid, DateTime makedate)
        {

            string format = "yyyy-MM-dd HH:mm:ss";
            //DateTime dt = DateTime.ParseExact("2010-01-01 23:00:00", format, CultureInfo.InvariantCulture);

            DateTime dd = DateTime.Now;
            //string date = "01/APR/2016";
            string date = "2016/APR/01";
            string timenow = DateTime.Now.TimeOfDay.ToString();

            string date_time = (date + " " + timenow);

            DateTime transdate = Convert.ToDateTime(date + " " + timenow);
            string new_date = transdate.ToString("yyyy-MM-dd HH:mm:ss");
            //DateTime new_date_Now = Convert.ToDateTime(new_date);


            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

            var app = new AppSettings();
            OracleConnection conp;
            try
            {
                using (conp = new OracleConnection(app.conPrime()))
                {
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "add_sec_user";
                        cmd.Parameters.Add("p_UserId", OracleDbType.Varchar2, 100).Value = p_UserId;
                        cmd.Parameters.Add("p_Password", OracleDbType.Varchar2, 1000).Value = p_Password;

                        cmd.Parameters.Add("p_EmployeeId", OracleDbType.Varchar2, 100).Value = p_fullname;
                        cmd.Parameters.Add("p_UserGroupId", OracleDbType.Varchar2, 100).Value = p_UserGroupId;
                        cmd.Parameters.Add("p_UserStatus", OracleDbType.Varchar2, 100).Value = "ACTIVE";


                        cmd.Parameters.Add("p_FullName", OracleDbType.Varchar2, 100).Value = p_fullname;
                        cmd.Parameters.Add("p_Email", OracleDbType.Varchar2, 100).Value = "sys.support@teksol.com.gh";
                        cmd.Parameters.Add("p_MobileNo", OracleDbType.Varchar2, 100).Value = "0";
                        cmd.Parameters.Add("p_PhoneNo", OracleDbType.Varchar2, 100).Value = "0";

                        cmd.Parameters.Add("p_MakeId", OracleDbType.Varchar2, 100).Value = p_MakerId;
                        cmd.Parameters.Add("p_MakeDate", OracleDbType.Varchar2).Value = new_date;
                        cmd.Parameters.Add("p_AuthId", OracleDbType.Varchar2, 100).Value = "teksol.admin";
                        cmd.Parameters.Add("p_AuthStatus", OracleDbType.Varchar2, 100).Value = "AUTHORIZED";

                        cmd.ExecuteNonQuery();
                        conp.Close();
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
                //if (conp.State == ConnectionState.Open)
                //{
                //    conp.Close();
                //}
              //  ts.Dispose();
            }

        }

        private void Add_Admin_Users_Dapper(string p_UserId, string p_Password, string p_fullname, string p_UserGroupId, DateTime p_PasswordExpiryDate, string p_MakerId, DateTime p_MakeDate) //, string status, string makerid, DateTime makedate)
        {

            string format = "yyyy-MM-dd HH:mm:ss";
            //DateTime dt = DateTime.ParseExact("2010-01-01 23:00:00", format, CultureInfo.InvariantCulture);

            DateTime dd = DateTime.Now;
            //string date = "01/APR/2016";
            string date = "2016/APR/01";
            string timenow = DateTime.Now.TimeOfDay.ToString();

            string date_time = (date + " " + timenow);

            DateTime transdate =  Convert.ToDateTime(date + " " + timenow);
            string new_date = transdate.ToString("yyyy-MM-dd HH:mm:ss");
            //DateTime new_date_Now = Convert.ToDateTime(new_date);


            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

            var db = new AppSettings();
            OracleConnection conp;

            var param = new DynamicParameters();
            try
            {

                // param.Add(name: "p_date", value: transdate, dbType: DbType.Date, direction: ParameterDirection.Input);

                param.Add(name: "p_UserId", value: p_UserId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Password", value: p_Password, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeId", value: p_fullname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserGroupId", value: p_UserGroupId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserStatus", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_AuthStatus", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeId", value: p_MakerId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeDate", value: transdate, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_AuthId", value: p_MakerId, dbType: DbType.String, direction: ParameterDirection.Input);


                param.Add(name: "p_FullName", value: p_fullname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Email", value: "sys.support@teksol.com.gh", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MobileNo", value: "0", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_PhoneNo", value: "0", dbType: DbType.String, direction: ParameterDirection.Input);
 

                db.GetConnection().Execute(sql: "add_sec_User", param: param, commandType: CommandType.StoredProcedure);
            

               // ts.Complete();
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

        private void Add_Default_fund_old(string fund_name, string fund_desc, string makerid, DateTime makedate)
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
                        cmd.CommandText = "SETUP_PROCEDURES.ADD_PFM_FUND";
                        cmd.Parameters.Add("P_FUND_NAME", OracleDbType.Varchar2).Value = fund_name;
                        cmd.Parameters.Add("P_FUND_DESCRIPTION", OracleDbType.Varchar2).Value = fund_desc;
                        cmd.Parameters.Add("P_AUTH_STATUS", OracleDbType.Varchar2).Value = "AUTHORIZED";
                        cmd.Parameters.Add("P_MAKER_ID", OracleDbType.Varchar2).Value = makerid;
                        cmd.Parameters.Add("P_MAKE_DATE", OracleDbType.Date).Value = makedate;
                        cmd.ExecuteNonQuery();
                        conp.Close();
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


        public void Add_Default_fund(string fund_name, string fund_desc, string makerid, DateTime makedate)
        {
            AppSettings db = new AppSettings();
            try
            {
                var param = new DynamicParameters();

                    param.Add(name: "P_FUND_NAME", value: fund_name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FUND_DESCRIPTION", value: fund_desc, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: makerid, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKE_DATE", value: makedate, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_PFM_FUND", param: param, commandType: CommandType.StoredProcedure);
               
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                db.Dispose();
            }

        }


        public void Add_Default_District(string District_Name, string makerid)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                //con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_DISTRICT_ID", value: "", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_DISTRICT_NAME", value: District_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MAKER_ID", value: makerid, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: "", dbType: DbType.String, direction: ParameterDirection.Input);
                app.GetConnection().Execute("MIX_SETUP_DISTRICT", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
              
            }
        }
        public static string AES_Encrypt(string input_text, string security_Code)
        {
            // ''' security_Code= teksolencrypt$@teksol.com987908123
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
            }
            return encrypted;
        }




    }
}
