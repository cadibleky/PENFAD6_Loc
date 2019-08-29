
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;
using System.Net.Mail;
using System.Transactions;
using PENFAD6DAL.Repository.Setup.SystemSetup;

namespace PENFAD6DAL.Repository.Security
{
    //-----LOUIS ------
    public class sec_UserRepo
    {
        AppSettings db = new AppSettings();
        [Key]
        [Required(ErrorMessage = "Username required.", AllowEmptyStrings = false)]
        //[StringLength(maximumLength: 100)]
        public string User_Id { get; set; }
        // [Required(ErrorMessage = "Password required.", AllowEmptyStrings = false)]
        //[StringLength(maximumLength: 1000)]
        public string Password { get; set; }

        // [Required(ErrorMessage = " Employee ID is required.")]
        //[StringLength(maximumLength: 100)]
        public string Employee_Id { get; set; }

        [Required(ErrorMessage = " User Group ID is required.")]
        //[StringLength(maximumLength: 100)]
        public string User_Group_Id { get; set; }

        //[StringLength(maximumLength: 100)]
        public string User_Status { get; set; }

        //[StringLength(maximumLength: 100)]
        [Required]
        public string Employee_Name { get; set; }

        public DateTime? Password_Expiry_Date { get; set; }
        public DateTime Password_Change_Date { get; set; }

        //[StringLength(maximumLength: 100)]
        public int Password_Failure_Count { get; set; }

        //[StringLength(maximumLength: 100)]
        public string Token_Number { get; set; }
        public string Password_Change_Token { get; set; }

        //[StringLength(maximumLength: 100)]
        public string Maker_Id { get; set; }

        //[DataType(DataType.DateTime)]
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }

        //[DataType(DataType.DateTime)]
        public DateTime Update_Date { get; set; }

        //[StringLength(maximumLength: 100)]
        public string Auth_Id { get; set; }

        //[DataType(DataType.DateTime)]
        public DateTime Auth_Date { get; set; }

        //[StringLength(maximumLength: 100)]
        public string Auth_Status { get; set; }

        //[StringLength(maximumLength: 100)]
        public string User_Group_Name { get; set; }

        //[StringLength(maximumLength: 100)]
        public string User_Role_Id { get; set; }

        //[StringLength(maximumLength: 100)]
        public string User_Role_Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string User_Group_Name_old  { get; set; }


        IDbConnection con;

        //----------Methods begin

        //user flags= New,Active,Suspended,Droped
        readonly setup_InternetRepo internetRepo = new setup_InternetRepo();

        public void SendIt(string from, string pass, string subj, string msg, string to)
        {
            try
            {
                //var m = new MailAddress("","");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
     
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(from);
                mail.To.Add(to);
                mail.Subject = subj;
                mail.Body = msg;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(from, pass);
                SmtpServer.EnableSsl = true;
               
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateNewUser(sec_UserRepo user)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                string password_hash = cSecurityRepo.AES_Encrypt(user.Password);

                param.Add(name: "p_UserId", value: user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Password", value: password_hash, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeId", value: user.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FullName", value: user.Employee_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserGroupId", value: user.User_Group_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "p_Email", value: user.Email, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MobileNo", value: user.MobileNo, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_PhoneNo", value: user.PhoneNo, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "p_UserStatus", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_AuthStatus", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_AuthId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "add_sec_User", param: param, commandType: CommandType.StoredProcedure);

                if (result != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }


        //    public bool ApproveNewUser(sec_UserRepo repo_user)
        //    {
        //        //Get connectoin
        //        var app = new AppSettings();
        //        TransactionOptions tsOp = new TransactionOptions();
        //        tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
        //        TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
        //        tsOp.Timeout = TimeSpan.FromMinutes(20);

        //        using (OracleConnection conn = new OracleConnection(app.conString()))  //
        //        {

        //            try
        //            {

        //                string queryinternet = "select * from setup_company";
        //                #region get email properties
        //                {
        //                var paramuser = new DynamicParameters();
        //                paramuser.Add("P_USER_ID", repo_user.User_Id, DbType.String, ParameterDirection.Input);
        //                paramuser.Add("REMAIL", "", DbType.String, ParameterDirection.Output);
        //                paramuser.Add("RPASSWORD", "", DbType.String, ParameterDirection.Output);
        //                paramuser.Add("RFULLNAME", "", DbType.String, ParameterDirection.Output);
        //                con.Execute("SEL_EMAIL_PROP", paramuser, commandType: CommandType.StoredProcedure);
        //                repo_user.Email = paramuser.Get<string>("REMAIL");
        //                repo_user.Password = paramuser.Get<string>("RPASSWORD");
        //                repo_user.Employee_Name = paramuser.Get<string>("RFULLNAME");
        //                }
        //            #endregion

        //            #region send email

        //                OracleCommand commandinternet = new OracleCommand(queryinternet, conn);
        //                conn.Open();
        //                OracleDataReader readerinternet;
        //                readerinternet = commandinternet.ExecuteReader();
        //                // Always call Read before accessing data.
        //                while (readerinternet.Read())
        //                {
        //                    internetRepo.smtp = (string)readerinternet["smtp"];
        //                    internetRepo.email_from = (string)readerinternet["email_from"];
        //                    internetRepo.email_password = (string)readerinternet["email_password"];
        //                    internetRepo.port = Convert.ToInt16(readerinternet["port"]);
        //                    internetRepo.company_name = (string)readerinternet["company_name"];
        //                }


        //                string security_Code = "teksolencrypt$@teksol.com987908123";
        //                repo_user.Password = GlobalValue.AES_Decrypt(repo_user.Password, security_Code);

        //                var msg = $@"<b>Dear {repo_user.Employee_Name}</b> <br/> <br/><font color=blue>Your User Name is {repo_user.User_Id} and  Password is {repo_user.Password}</font>";
        //                string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Teksol Penfad authentication for " + repo_user.Employee_Name, to = repo_user.Email;

        //                string smtp = internetRepo.smtp;
        //                int port = internetRepo.port;
        //                internetRepo.SendIt(from, pass, subj, msg, to, smtp, port);

        //            #endregion

        //            #region Approve user
        //            var param = new DynamicParameters();
        //            param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
        //            param.Add(name: "p_UserStatus", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
        //            param.Add(name: "p_AuthStatus", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
        //            param.Add(name: "p_AuthDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
        //            param.Add(name: "p_AuthId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
        //            con.Execute(sql: "APP_SEC_USER", param: param, commandType: CommandType.StoredProcedure);
        //            #endregion

        //            //ts.Complete();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (con.State == ConnectionState.Open)
        //            {
        //                con.Close();
        //            }
        //        }

        //    }
        //}
        public void ApproveNewUser(sec_UserRepo repo_user)
        {
            var app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);


            string queryinternet = "select * from setup_company";

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //string password_hash = cSecurityRepo.AES_Encrypt(repo_user.Password);

                    #region get email properties
                    {
                        var paramuser = new DynamicParameters();
                        paramuser.Add("P_USER_ID", repo_user.User_Id, DbType.String, ParameterDirection.Input);
                        paramuser.Add("REMAIL", "", DbType.String, ParameterDirection.Output);
                        paramuser.Add("RPASSWORD", "", DbType.String, ParameterDirection.Output);
                        paramuser.Add("RFULLNAME", "", DbType.String, ParameterDirection.Output);
                        conn.Execute("SEL_EMAIL_PROP", paramuser, commandType: CommandType.StoredProcedure);
                        repo_user.Email = paramuser.Get<string>("REMAIL");
                        repo_user.Employee_Name = paramuser.Get<string>("RFULLNAME");
                    }
                    #endregion

                    #region reassign user

                    var param = new DynamicParameters();
                    param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_UserStatus", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_AuthStatus", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_AuthDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_AuthId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute(sql: "APP_SEC_USER", param: param, commandType: CommandType.StoredProcedure);
                    #endregion


                    #region send email

                    OracleCommand commandinternet = new OracleCommand(queryinternet, conn);
                    conn.Open();
                    OracleDataReader readerinternet;
                    readerinternet = commandinternet.ExecuteReader();
                    // Always call Read before accessing data.
                    while (readerinternet.Read())
                    {
                        internetRepo.smtp = (string)readerinternet["smtp"];
                        internetRepo.email_from = (string)readerinternet["email_from"];
                        internetRepo.email_password = (string)readerinternet["email_password"];
                        internetRepo.port = Convert.ToInt16(readerinternet["port"]);
                        internetRepo.company_name = (string)readerinternet["company_name"];
                    }

                    string security_Code = "teksolencrypt$@teksol.com987908123";
                    repo_user.Password = GlobalValue.AES_Decrypt(repo_user.Password, security_Code);

                    var msg = $@"<b>Dear {repo_user.Employee_Name}</b> <br/> <br/><font color=blue>Your User Name is {repo_user.User_Id} and  Password is {repo_user.Password}</font>";
                    string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Teksol Penfad authentication for " + repo_user.Employee_Name, to = repo_user.Email;

                    string smtp = internetRepo.smtp;
                    int port = internetRepo.port;
                    //                internetRepo.SendIt(from, pass, subj, msg, to, smtp, port);
                    internetRepo.SendIt(from, pass, subj, msg, to, smtp, port, internetRepo.company_name);

                    #endregion


                    ts.Complete();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ts.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        //    public void ResetNewUser(sec_UserRepo repo_user)
        //    {
        //        var app = new AppSettings();

        //        TransactionOptions tsOp = new TransactionOptions();
        //        tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
        //        TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
        //        tsOp.Timeout = TimeSpan.FromMinutes(20);

        //        using (OracleConnection conn = new OracleConnection(app.conString()))  //
        //        {

        //            try
        //            {
        //                string queryinternet = "select * from setup_company";
        //                string password_hash = cSecurityRepo.AES_Encrypt(repo_user.Password);

        //                #region get email properties
        //                {
        //                    var paramuser = new DynamicParameters();
        //                    paramuser.Add("P_USER_ID", repo_user.User_Id, DbType.String, ParameterDirection.Input);
        //                    paramuser.Add("REMAIL", "", DbType.String, ParameterDirection.Output);
        //                    paramuser.Add("RPASSWORD", "", DbType.String, ParameterDirection.Output);
        //                    paramuser.Add("RFULLNAME", "", DbType.String, ParameterDirection.Output);
        //                    conn.Execute("SEL_EMAIL_PROP", paramuser, commandType: CommandType.StoredProcedure);
        //                    repo_user.Email = paramuser.Get<string>("REMAIL");
        //                    repo_user.Employee_Name = paramuser.Get<string>("RFULLNAME");
        //                }
        //                #endregion

        //                #region Approve user
        //                var param = new DynamicParameters();
        //                param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
        //                param.Add(name: "p_Password", value: password_hash, dbType: DbType.String, direction: ParameterDirection.Input);
        //                param.Add(name: "p_MakerId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

        //                conn.Execute(sql: "RES_SEC_USER", param: param, commandType: CommandType.StoredProcedure);
        //                #endregion

        //                #region send email

        //                    OracleCommand commandinternet = new OracleCommand(queryinternet, conn);
        //                    conn.Open();
        //                    OracleDataReader readerinternet;
        //                    readerinternet = commandinternet.ExecuteReader();
        //                    // Always call Read before accessing data.
        //                    while (readerinternet.Read())
        //                    {
        //                        internetRepo.smtp = (string)readerinternet["smtp"];
        //                        internetRepo.email_from = (string)readerinternet["email_from"];
        //                        internetRepo.email_password = (string)readerinternet["email_password"];
        //                        internetRepo.port = Convert.ToInt16(readerinternet["port"]);
        //                        internetRepo.company_name = (string)readerinternet["company_name"];
        //                    }


        //                    var msg = $@"<b>Dear {repo_user.Employee_Name}</b> <br/> <br/><font color=red>Your User Name is {repo_user.User_Id} and new Password is {repo_user.Password}</font>";
        //                    string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Teksol Penfad password reset for " + repo_user.Employee_Name, to = repo_user.Email;

        //                    string smtp = internetRepo.smtp;
        //                    int port = internetRepo.port;
        //                    internetRepo.SendIt(from, pass, subj, msg, to, smtp, port);

        //                #endregion

        //                ts.Complete();

        //               }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                ts.Dispose();
        //                if (conn.State == ConnectionState.Open)
        //                {
        //                    conn.Close();
        //                }
        //            }

        //        }

        //    }
        public void ResetNewUser(sec_UserRepo repo_user)
        {
            var app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);


            string queryinternet = "select * from setup_company";

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //string password_hash = cSecurityRepo.AES_Encrypt(repo_user.Password);

                    #region get email properties
                    {
                        var paramuser = new DynamicParameters();
                        paramuser.Add("P_USER_ID", repo_user.User_Id, DbType.String, ParameterDirection.Input);
                        paramuser.Add("REMAIL", "", DbType.String, ParameterDirection.Output);
                        paramuser.Add("RPASSWORD", "", DbType.String, ParameterDirection.Output);
                        paramuser.Add("RFULLNAME", "", DbType.String, ParameterDirection.Output);
                        conn.Execute("SEL_EMAIL_PROP", paramuser, commandType: CommandType.StoredProcedure);
                        repo_user.Email = paramuser.Get<string>("REMAIL");
                        repo_user.Employee_Name = paramuser.Get<string>("RFULLNAME");
                    }
                    #endregion

                    #region send email
                    OracleCommand commandinternet = new OracleCommand(queryinternet, conn);
                    conn.Open();
                    OracleDataReader readerinternet;
                    readerinternet = commandinternet.ExecuteReader();
                    // Always call Read before accessing data.
                    while (readerinternet.Read())
                    {
                        internetRepo.smtp = (string)readerinternet["smtp"];
                        internetRepo.email_from = (string)readerinternet["email_from"];
                        internetRepo.email_password = (string)readerinternet["email_password"];
                        internetRepo.port = Convert.ToInt16(readerinternet["port"]);
                        internetRepo.company_name = (string)readerinternet["company_name"];
                    }
                 

                    var msg = $@"<b>Dear {repo_user.Employee_Name}</b> <br/> <br/><font color=red>Your User Name is {repo_user.User_Id} and new Password is {repo_user.Password}</font>";
                    string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Teksol Penfad password reset for " + repo_user.Employee_Name, to = repo_user.Email;
                    string smtp = internetRepo.smtp;
                    int port = internetRepo.port;
                    internetRepo.SendIt(from, pass, subj, msg, to, smtp, port, internetRepo.company_name);

                    #endregion

                    #region reset user
                    string password_hash = cSecurityRepo.AES_Encrypt(repo_user.Password);
                    var param = new DynamicParameters();
                    param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Password", value: password_hash, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MakerId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                    conn.Execute(sql: "RES_SEC_USER", param: param, commandType: CommandType.StoredProcedure);

                    #endregion

                   


                    ts.Complete();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ts.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        public void ReassignNewUser(sec_UserRepo repo_user)
        {
            var app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);


            string queryinternet = "select * from setup_company";

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //string password_hash = cSecurityRepo.AES_Encrypt(repo_user.Password);

                    #region get email properties
                    {
                        var paramuser = new DynamicParameters();
                        paramuser.Add("P_USER_ID", repo_user.User_Id, DbType.String, ParameterDirection.Input);
                        paramuser.Add("REMAIL", "", DbType.String, ParameterDirection.Output);
                        paramuser.Add("RPASSWORD", "", DbType.String, ParameterDirection.Output);
                        paramuser.Add("RFULLNAME", "", DbType.String, ParameterDirection.Output);
                        conn.Execute("SEL_EMAIL_PROP", paramuser, commandType: CommandType.StoredProcedure);
                        repo_user.Email = paramuser.Get<string>("REMAIL");
                        repo_user.Employee_Name = paramuser.Get<string>("RFULLNAME");
                    }
                    #endregion

                    #region reassign user
                    var param = new DynamicParameters();
                    param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_UserGroup", value: repo_user.User_Group_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MakerId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                    conn.Execute(sql: "REASSIGN_SEC_USER", param: param, commandType: CommandType.StoredProcedure);
                    #endregion

                    #region send email

                    OracleCommand commandinternet = new OracleCommand(queryinternet, conn);
                    conn.Open();
                    OracleDataReader readerinternet;
                    readerinternet = commandinternet.ExecuteReader();
                    // Always call Read before accessing data.
                    while (readerinternet.Read())
                    {
                        internetRepo.smtp = (string)readerinternet["smtp"];
                        internetRepo.email_from = (string)readerinternet["email_from"];
                        internetRepo.email_password = (string)readerinternet["email_password"];
                        internetRepo.port = Convert.ToInt16(readerinternet["port"]);
                        internetRepo.company_name = (string)readerinternet["company_name"];
                    }

                    var msg = $@"Dear {repo_user.Employee_Name},  Please be informed that your Teksol Penfad user group has been changed. Contact the system's aadministrator for clarification.</b> <br/> <br/>   Thank you. </b> <br/> <br/>{internetRepo.company_name}";
                    string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Change of User Group; Teksol Penfad", to = repo_user.Email;
                    string smtp = internetRepo.smtp;
                    int port = internetRepo.port;
                    //string attach = DocumentName;
                    internetRepo.SendIt(from, pass, subj, msg, to, smtp, port, internetRepo.company_name);

                    #endregion


                    ts.Complete();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ts.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }

        public bool SuspendNewUser(sec_UserRepo repo_user)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();             
               
                #region Approve user
                var param = new DynamicParameters();

                param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "SUS_SEC_USER", param: param, commandType: CommandType.StoredProcedure);
                #endregion

               
                if (result != 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }
        }

        public bool ActivateNewUser(sec_UserRepo repo_user)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                #region Approve user
                var param = new DynamicParameters();

                param.Add(name: "p_UserId", value: repo_user.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "ACT_SEC_USER", param: param, commandType: CommandType.StoredProcedure);
                #endregion


                if (result != 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }


        public List<sec_UserRepo> GetUserList(string searchtype, string searchvalue)
        {
            var user_list = new List<sec_UserRepo>();
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
                        cmd.CommandText = "sel_sec_UserByStatus";
                        cmd.Parameters.Add("p_searchtype", OracleDbType.Varchar2, ParameterDirection.Input).Value= searchtype;
                        cmd.Parameters.Add("p_searchvalue", OracleDbType.Varchar2, ParameterDirection.Input).Value = searchvalue;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            user_list = dt.AsEnumerable().Select(row => new sec_UserRepo
                            {
                                Employee_Id = row.Field<string>("EMPLOYEE_ID"),
                                User_Id = row.Field<string>("USER_ID"),
                                Employee_Name = row.Field<string>("EmployeeName"),
                                User_Group_Id = row.Field<string>("USER_ID"),
                                User_Group_Name = row.Field<string>("USER_GROUP_NAME"),
                                User_Role_Name = row.Field<string>("USER_ROLE_NAME"),
                                Maker_Id = row.Field<string>("MAKER_ID"),
                                Make_Date = row.Field<DateTime>("MAKE_DATE")
                            }).ToList();

                        }
                    }

                    return user_list;
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

        public List<sec_UserRepo> GetEmployeeToCreateUserList()
        {

            var user_list = new List<sec_UserRepo>();
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
                        cmd.CommandText = "sel_sec_EmployeeForUser";
                        //cmd.Parameters.Add("p_searchtype", OracleDbType.Varchar2, ParameterDirection.Input).Value = searchtype;
                        //cmd.Parameters.Add("p_searchvalue", OracleDbType.Varchar2, ParameterDirection.Input).Value = searchvalue;
                        //cmd.Parameters.Add("p_Employeestatus", OracleDbType.Varchar2, ParameterDirection.Input).Value = Employeestatus;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            user_list = dt.AsEnumerable().Select(row => new sec_UserRepo
                            {
                                Employee_Id = row.Field<string>("STAFF_ID"),
                                Employee_Name = row.Field<string>("STAFF_Name"),
                                Email = row.Field<string>("EMAIL"),
                                PhoneNo = row.Field<string>("PHONENO"),
                                MobileNo = row.Field<string>("MOBILENO"),
                            }).ToList();

                        }
                    }

                    return user_list;
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


        public List<sec_UserRepo> GetUserPendingList()
        {

            var user_list = new List<sec_UserRepo>();
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
                        cmd.CommandText = "sel_sec_PendingUser";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            user_list = dt.AsEnumerable().Select(row => new sec_UserRepo
                            {
                                User_Id = row.Field<string>("USER_ID"),
                                Employee_Name = row.Field<string>("FULLNAME"),
                                User_Group_Id = row.Field<string>("USER_GROUP_ID"),
                                User_Group_Name = row.Field<string>("USER_GROUP_NAME"),
                                Email = row.Field<string>("EMAIL"),
                                Password = row.Field<string>("Password"),
                            }).ToList();

                        }
                    }

                    return user_list;
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

        public List<sec_UserRepo> GetUserActiveList()
        {

            var user_list = new List<sec_UserRepo>();
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
                        cmd.CommandText = "sel_sec_ActiveUser";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            user_list = dt.AsEnumerable().Select(row => new sec_UserRepo
                            {
                                User_Id = row.Field<string>("USER_ID"),
                                Employee_Name = row.Field<string>("FULLNAME"),
                                User_Group_Id = row.Field<string>("USER_GROUP_ID"),
                                User_Group_Name = row.Field<string>("USER_GROUP_NAME"),
                                User_Group_Name_old = row.Field<string>("USER_GROUP_NAME"),
                                Email = row.Field<string>("EMAIL"),
                            }).ToList();

                        }
                    }

                    return user_list;
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
        public void ChangePassword(string user_id, string password)
        {
            try
            {

                var param = new DynamicParameters();
                password = cSecurityRepo.AES_Encrypt(password);

                param.Add(name: "P_USER_ID", value: user_id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PASSWORD", value: password, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                db.GetConnection().Execute(sql: "CHG_SEC_USER_PASSWORD", param: param, commandType: CommandType.StoredProcedure);
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
        public List<sec_UserRepo> GetUserSuspendList()
        {

            var user_list = new List<sec_UserRepo>();
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
                        cmd.CommandText = "sel_sec_SuspendedUser";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            user_list = dt.AsEnumerable().Select(row => new sec_UserRepo
                            {
                                User_Id = row.Field<string>("USER_ID"),
                                Employee_Name = row.Field<string>("FULLNAME"),
                                User_Group_Id = row.Field<string>("USER_GROUP_ID"),
                                User_Group_Name = row.Field<string>("USER_GROUP_NAME"),
                                Email = row.Field<string>("EMAIL"),
                            }).ToList();

                        }
                    }

                    return user_list;
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

        

        //class end
    }
    public class sec_User_Change_Password
    {
        [Required(ErrorMessage = "Username required.", AllowEmptyStrings = false)]
        public string User_Id { get; set; }
        [Required(ErrorMessage = "Old Password required.", AllowEmptyStrings = false)]
        public string Old_Password { get; set; }
        [Required(ErrorMessage = "New Password required.", AllowEmptyStrings = false)]
        public string New_Password { get; set; }
        [Required(ErrorMessage = "Confirm Password required.", AllowEmptyStrings = false)]
        [CompareAttribute("New_Password", ErrorMessage = "Confirm Password does not match.")]
        public string Confirm_Password { get; set; }
    }


}
