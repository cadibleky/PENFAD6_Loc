using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    // =====================kris==============
    public class EmployeePasswordController : Controller
    {      
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        readonly GlobalValue global_val = new GlobalValue();
        readonly setup_InternetRepo internetRepo = new setup_InternetRepo();
        IDbConnection con;
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployeePasswordTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeePasswordPartial",
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return pvr;
            }
            catch (System.Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }

        public ActionResult AddEmployeePasswordIndiTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeePasswordIndiPartial",
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return pvr;
            }
            catch (System.Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }
        public ActionResult GetESFEmployerList()
        {
            try
            {
                return this.Store(employeeRepo.GetESFEmployerList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult GetESFEmployeeList()
        {
            try
            {
                return this.Store(employeeRepo.GetESFEmployeeList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public ActionResult SendEmail(crm_EmployeeRepo employeeRepo)
        {
            try
            {
                if (String.IsNullOrEmpty(employeeRepo.Employer_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Employer",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    X.Mask.Hide();
                    return this.Direct();
                }

                string queryString = "select * from VW_REP_ESF_EMP where employer_id = '" + employeeRepo.Employer_Id + "' and CUST_STATUS = 'HIRED' AND LOGIN_SENT = 'NO' AND EMPLOYER_YES_NO = 'NO'";
                string queryinternet = "select * from setup_company";

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    var con = new AppSettings();
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();


                    //var con = new AppSettings();
                    var paramc = new DynamicParameters();
                    con.GetConnection().Execute("CHECK_EMAIL_NA", paramc, commandType: CommandType.StoredProcedure);


                    // Always call Read before accessing data.
                    while (reader.Read())
                    {
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.Cust_No = (string)reader["cust_no"];
                        employeeRepo.Scheme_Name = (string)reader["scheme_name"];
                        string ppaswword = (string)reader["user_password"];

                        if (Regex.IsMatch(((string)reader["email_address"]), MatchEmailPattern) == false)
                        {
                            //employeeRepo.Email_Address = "clientservices@securepensionstrust.com";
                        }
                        else
                        {
                            employeeRepo.Email_Address = (string)reader["email_address"];
                            #region send email
                            OracleCommand commandinternet = new OracleCommand(queryinternet, connection);
                            //connection.Open();
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
                                internetRepo.website_address = (string)readerinternet["website_address"];
                                internetRepo.postal_address = (string)readerinternet["postal_address"];
                                internetRepo.telephone_number = (string)readerinternet["telephone_number"];

                            }

                            string security_Code = "teksolencrypt$@teksol.com987908123";
                            employeeRepo.user_Password = GlobalValue.AES_Decrypt(ppaswword, security_Code);

                            var msg = $@"Dear {employeeRepo.First_Name},"
                              + " < br /> " + $@"Your Scheme Administrator has granted you online access to enable you view transactions on your {employeeRepo.Scheme_Name} Membership Accounts."
                              + "<br/>" + $@"Your User Id is {employeeRepo.Cust_No} and your Password is {employeeRepo.user_Password}."
                              + "<br/>" + $@"Please use your username and password to log in to your online account at  {internetRepo.website_address}. "
                             + "<br/>" + $@" Should you have any query or require further assistance, please contact us on telephone number {internetRepo.telephone_number } or email address {internetRepo.email_from} "
                              + "<br/>" + $@"Thank you.   {internetRepo.company_name}";

                            string from = internetRepo.email_from, pass = internetRepo.email_password, subj = internetRepo.company_name + " Client Portal Authentication for " + employeeRepo.First_Name, to = employeeRepo.Email_Address;
                            string smtp = internetRepo.smtp;
                            int port = internetRepo.port;
                            internetRepo.SendIt(from, pass, subj, msg, to, smtp, port, internetRepo.company_name);

                            //update crm_employee_scheme_fund for email sent
                            var parame = new DynamicParameters();
                            parame.Add("P_CUST_NO", employeeRepo.Cust_No, DbType.String, ParameterDirection.Input);
                            //parame.Add("P_DATE_ID", System.DateTime.Now.ToString("dd-MMM-yyyy"), DbType.DateTime, ParameterDirection.Input);
                            con.GetConnection().Execute("REPORT_UP_CRM_LOGIN", parame, commandType: CommandType.StoredProcedure);


                            #endregion
                        }
                    }

                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Emails Successfully Sent",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                }
                X.Mask.Hide();
                return this.Direct();
            }

            catch (Exception ex)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Sent",
                    Message = "Email traffic issue. Please send again later to continue",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                X.Mask.Hide();
                return this.Direct();
            }
        }

        public ActionResult SendIndiMail(crm_EmployeeRepo employeeRepo)
        {

            try
            {
                if (String.IsNullOrEmpty(employeeRepo.Employer_Id))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Employer",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

                string queryString = "select * from VW_REP_ESF_EMP where cust_no = '" + employeeRepo.Cust_No + "'";
                string queryinternet = "select * from setup_company";


                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();

                    var con = new AppSettings();
                    var paramc = new DynamicParameters();
                    con.GetConnection().Execute("CHECK_EMAIL_NA", paramc, commandType: CommandType.StoredProcedure);

                    // Always call Read before accessing data.
                    while (reader.Read())
                    {
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.Cust_No = (string)reader["cust_no"];
                        employeeRepo.Scheme_Name = (string)reader["scheme_name"];
                        string ppaswword = (string)reader["user_password"];

                        if (Regex.IsMatch(((string)reader["email_address"]), MatchEmailPattern) == false)
                        {
                           // employeeRepo.Email_Address = "na@yahoo.com";
                        }
                        else
                        {
                            employeeRepo.Email_Address = (string)reader["email_address"];

                            #region send email

                            OracleCommand commandinternet = new OracleCommand(queryinternet, connection);
                            //connection.Open();
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
                                internetRepo.website_address = (string)readerinternet["website_address"];
                                internetRepo.postal_address = (string)readerinternet["postal_address"];
                                internetRepo.telephone_number = (string)readerinternet["telephone_number"];
                            }

                            string security_Code = "teksolencrypt$@teksol.com987908123";
                            employeeRepo.user_Password = GlobalValue.AES_Decrypt(ppaswword, security_Code);

                            var msg = $@"Dear {employeeRepo.First_Name},"
                             + " < br /> " + $@"Your Scheme Administrator has granted you online access to enable you view transactions on your {employeeRepo.Scheme_Name} Membership Accounts."
                             + "<br/>" + $@"Your User Id is {employeeRepo.Cust_No} and your Password is {employeeRepo.user_Password}."
                             + "<br/>" + $@"Please use your username and password to log in to your online account at  {internetRepo.website_address}. "
                            + "<br/>" + $@" Should you have any query or require further assistance, please contact us on telephone number {internetRepo.telephone_number } or email address {internetRepo.email_from} "
                             + "<br/>" + $@"Thank you.   {internetRepo.company_name}";

                            //var msg = $@"Dear {employeeRepo.First_Name},
                            //  Your Scheme Administrator has granted you online access to enable you view transactions on your {employeeRepo.Scheme_Name} Membership Accounts. 
                            //  Your User Id is {employeeRepo.Cust_No} and your Password is {employeeRepo.user_Password}.   
                            //  Please use your username and password to log in to your online account at  {internetRepo.website_address}.   
                            //  Should you have any query or require further assistance, please contact us on telephone number {internetRepo.telephone_number } or email address {internetRepo.email_from} 
                            //  Thank you.   {internetRepo.company_name}";


                            string from = internetRepo.email_from, pass = internetRepo.email_password, subj = internetRepo.company_name + " Client Portal Authentication for " + employeeRepo.First_Name, to = employeeRepo.Email_Address;
                            string smtp = internetRepo.smtp;
                            int port = internetRepo.port;
                            internetRepo.SendIt(from, pass, subj, msg, to, smtp, port, internetRepo.company_name);
                            #endregion
                        }
                    }

                    // Always call Close when done reading.
                    reader.Close();
                    
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Sent",
                            Message = "Email Successfully Sent",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
    
                }
                return this.Direct();
            }

            catch (Exception ex)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not send email, check internet connection",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

		public ActionResult Read5(string Employer_Id, string Employer_Name)
		{
			try
			{
				if (string.IsNullOrEmpty(Employer_Id) ||  Employer_Id == "null")
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Employer name is required",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
					return this.Direct();
				}



				// X.GetCmp<Store>("nokStore").Reload();
				//X.GetCmp<Store>("BeneStore").Reload();
				X.GetCmp<Store>("change_ESF_employeeStore_send_pass").Reload();

				Store store = X.GetCmp<Store>("change_ESF_employeeStore_send_pass");
				store.Reload();
				store.DataBind();
				List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList5(Employer_Id, Employer_Name);
				if (obj.Count == 0)
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "There are no employees for this employer.",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
					return this.Direct();
				}

				return this.Store(obj);

			}
			catch (Exception ex)
			{
				//logger.WriteLog(ex.Message);
				X.Msg.Show(new MessageBoxConfig
				{
					Title = "Error",
					Message = "Process failed -" + ex.Message,
					Buttons = MessageBox.Button.OK,
					Icon = MessageBox.Icon.INFO,
					Width = 350


				});
				return this.Direct();
			}

		}

	} //end class
}