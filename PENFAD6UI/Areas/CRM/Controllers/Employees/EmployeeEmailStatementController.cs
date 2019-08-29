using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using Serilog;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
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
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    // =====================kris==============
    public class EmployeeEmailStatementController : Controller
    {
        string NEW_DATE;
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        readonly GlobalValue global_val = new GlobalValue();
        readonly setup_InternetRepo internetRepo = new setup_InternetRepo();
        IDbConnection con;
        string path;

        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployeeEmailStatementTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeEmailStatementPartial",
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

        public ActionResult AddEmployeeEmailStatementIndiTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeEmailStatementIndiPartial",
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

                    return this.Direct();
                }

                String date = DateTime.Now.ToString("dd-MMM-yyyy");
                String Month = DateTime.Now.Month.ToString();
                String Year = DateTime.Now.Year.ToString();
                
                ////correct nulls in tables
                //var con = new AppSettings();
                //var paramc = new DynamicParameters();
                //con.GetConnection().Execute("upd_employee_email", paramc, commandType: CommandType.StoredProcedure);


                string queryString = "select * from vw_es_esf where employer_id = '" + employeeRepo.Employer_Id + "' and scheme_id = '" + employeeRepo.Scheme_Id + "' and ESF_STATUS = 'ACTIVE' and  EMAIL_SENT = 'NO' AND EMPLOYER_YES_NO='NO' ";
                string queryinternet = "select * from setup_company";

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.

                    var con = new AppSettings();
                    var paramc = new DynamicParameters();   
                    con.GetConnection().Execute("CHECK_EMAIL_NA", paramc, commandType: CommandType.StoredProcedure);



                    while (reader.Read())
                    {
                    
                        employeeRepo.Surname = (string)reader["surname"];
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.ESF_Id = (string)reader["esf_id"];
                        employeeRepo.Email_Address = (string)reader["email_address"];
                        employeeRepo.Scheme_Name = (string)reader["SCHEME_NAME"];

                        if (Regex.IsMatch(((string)reader["email_address"]), MatchEmailPattern) == false)
                        {
                            //log this
                        }
                        else
                        {
							//load report table
							//var con = new AppSettings();
							var param = new DynamicParameters();
							param.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
							param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
							con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure);

							employeeRepo.Email_Address = (string)reader["email_address"];
                            string DocumentName = "NA";
                            string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionMonth_On_Month_1.dll");

                            System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                            StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                            //////asign constring
                            My_Report.Dictionary.DataStore.Clear();
                           
                            My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                            My_Report[":P_ESF_ID"] = employeeRepo.ESF_Id;
							My_Report[":P_USER_ID"] = GlobalValue.User_ID;
                            NEW_DATE = System.DateTime.Now.ToString().Replace("/", ".");
                            NEW_DATE = NEW_DATE.Replace(":", ".");

                            path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
							if (!(System.IO.Directory.Exists(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year)))
							{
								System.IO.Directory.CreateDirectory(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year);
							}

							if (!(System.IO.File.Exists(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name+ NEW_DATE + "_Statement.pdf")))
							{
								
								System.IO.File.Delete(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.Employer_Name + @"\" +System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name+ NEW_DATE + "_Statement.pdf");

								DocumentName = (path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name+ NEW_DATE + "_Statement.pdf");

								My_Report.Render();
								My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);

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
								}
                                var msg = $@"Dear {employeeRepo.First_Name}," + "<br/>" + $@" Kindly find attached, your {employeeRepo.Scheme_Name} statement." + "<br/>" + $@"Thank you." + "<br/>" + $@" { internetRepo.company_name}";
                                //var msg = $@"Dear {employeeRepo.First_Name}," + "\n" +  "Kindly find attached, your {employeeRepo.Scheme_Name} statement." + "\n" + " Thank you.  {internetRepo.company_name}";
								string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Statement of Account", to = employeeRepo.Email_Address;
								string smtp = internetRepo.smtp;
								int port = internetRepo.port;
								string attach = DocumentName;
								internetRepo.SendIt2(from, pass, subj, msg, to, smtp, port, employeeRepo, attach);

								#endregion

								//update crm_employee_scheme_fund for email sent
								var parame = new DynamicParameters();
								parame.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
								//parame.Add("P_DATE_ID", System.DateTime.Now.ToString("dd-MMM-yyyy"), DbType.DateTime, ParameterDirection.Input);
								con.GetConnection().Execute("REPORT_UP_CRM_ACCOUNT", parame, commandType: CommandType.StoredProcedure);


							}

						}

                    }
                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Sent",
                        Message = "Emails Successfully Sent",
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
                    Message = "Email traffic issue. Please send again later to continue",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
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
                string queryString = "select * from vw_es_esf where esf_id = '" + employeeRepo.ESF_Id + "'";
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
                        employeeRepo.Surname = (string)reader["surname"];
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.ESF_Id = (string)reader["esf_id"];
                        employeeRepo.Email_Address = (string)reader["email_address"];
                        employeeRepo.Scheme_Name = (string)reader["SCHEME_NAME"];

                        if (Regex.IsMatch(((string)reader["email_address"]), MatchEmailPattern) == false)
                        {
                            //log this
                        }
                        else
                        {
							//load report table
							//var con = new AppSettings();
							var param = new DynamicParameters();
							param.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
							param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
							con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure);

							employeeRepo.Email_Address = (string)reader["email_address"];

                            string DocumentName = "NA";
                            string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionMonth_On_Month_1.dll");

                            System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                            StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                            //////asign constring
                            My_Report.Dictionary.DataStore.Clear();
                            My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                            My_Report[":P_ESF_ID"] = employeeRepo.ESF_Id;
							My_Report[":P_USER_ID"] = GlobalValue.User_ID;
                            NEW_DATE = System.DateTime.Now.ToString().Replace("/", ".");
                            NEW_DATE = NEW_DATE.Replace(":", ".");

                            if (!(System.IO.Directory.Exists(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year)))
							{
								System.IO.Directory.CreateDirectory(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year);
							}

							if ((System.IO.File.Exists(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.Scheme_Name +"_" + NEW_DATE + "_Statement.pdf")))
							{
								System.IO.File.Delete(path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.Scheme_Name + "_" + NEW_DATE + "_Statement.pdf");
							}

							DocumentName = (path + @"\" + "STATEMENTS_MAIL" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.Scheme_Name + "_" + NEW_DATE + "_Statement.pdf");

							
							My_Report.Render();
                            My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);

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
                            }

                            var msg = $@"Dear {employeeRepo.First_Name},"  + "<br/>" + $@" Kindly find attached, your {employeeRepo.Scheme_Name} statement." + "<br/>" + $@"Thank you." + "<br/>" + $@" { internetRepo.company_name}";
                            string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Statement of Account", to = employeeRepo.Email_Address;
                            string smtp = internetRepo.smtp;
                            int port = internetRepo.port;
                            string attach = DocumentName;
                            internetRepo.SendIt2(from, pass, subj, msg, to, smtp, port, employeeRepo, attach);

                            #endregion

                            //update crm_employee_scheme_fund for email sent
                            var parame = new DynamicParameters();
                            parame.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
                            //parame.Add("P_DATE_ID", System.DateTime.Now.ToString("dd-MMM-yyyy"), DbType.DateTime, ParameterDirection.Input);
                            con.GetConnection().Execute("REPORT_UP_CRM_ACCOUNT", parame, commandType: CommandType.StoredProcedure);
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

		public ActionResult Read5(string Employer_Id)
		{
			try
			{
				if (string.IsNullOrEmpty(Employer_Id))
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
				X.GetCmp<Store>("change_ESF_employeeStore").Reload();

				Store store = X.GetCmp<Store>("change_ESF_employeeStore");
				store.Reload();
				store.DataBind();
				List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList5(Employer_Id);
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


        public ActionResult Read5e(string Employer_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id))
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
                X.GetCmp<Store>("change_ESF_employeeStore").Reload();

                Store store = X.GetCmp<Store>("change_ESF_employeeStore");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList5e(Employer_Id);
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


        public ActionResult Read6(string Employer_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id))
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
                X.GetCmp<Store>("change_ESF_employeeStore_send").Reload();

                Store store = X.GetCmp<Store>("change_ESF_employeeStore_send");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeSchemeFundRepo> obj = employeeRepo.GetEmployeeList6(Employer_Id);
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