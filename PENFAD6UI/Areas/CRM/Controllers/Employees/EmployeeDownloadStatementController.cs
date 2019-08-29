using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;

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
using System.Net.Mail;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    // =====================kris==============
    public class EmployeeDownloadStatementController : Controller
    {   
           
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        readonly GlobalValue global_val = new GlobalValue();
        IDbConnection con;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployeeDownloadStatementTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeDownloadStatementPartial",
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

        public ActionResult AddEmployeeDownloadStatementActiveTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeDownloadStatementActivePartial",
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

        public ActionResult AddEmployeeDownloadStatementIndiTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeDownloadStatementIndiPartial",
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
        public ActionResult Download(crm_EmployeeRepo employeeRepo)
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
				string queryString = "select * from vw_es_esf where employer_id = '" + employeeRepo.Employer_Id + "' and scheme_id = '" + employeeRepo.Scheme_Id + "' and ESF_STATUS = 'ACTIVE'";
				

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.

                  
                    while (reader.Read())
                    {
						

						employeeRepo.Surname = (string)reader["surname"];
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.ESF_Id = (string)reader["esf_id"];
                        //employeeRepo.Email_Address = (string)reader["email_address"];
                        employeeRepo.Scheme_Name = (string)reader["scheme_name"];

                        employeeRepo.Scheme_Name = employeeRepo.Scheme_Name.Replace("/", "&");

                        ////load report table
                        //var con = new AppSettings();
                        //var param = new DynamicParameters();
                        //param.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
                        //param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        //con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure);


                        string DocumentName = "NA";
                        string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionMonth_On_Month_1.dll");

                        System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                        StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                        //////asign constring
                        My_Report.Dictionary.DataStore.Clear();
                        My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                        My_Report[":P_ESF_ID"] = employeeRepo.ESF_Id;

                        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                        employeeRepo.Employer_Name = employeeRepo.Employer_Name.Replace("/", "&");

                        if (!(System.IO.Directory.Exists(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day +"_"+ System.DateTime.Now.Month + "_" + System.DateTime.Now.Year  )))
                        {
                            System.IO.Directory.CreateDirectory(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year );
                        }

                        if (!(System.IO.File.Exists(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\"+ employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name + "_Statement.pdf")))
                        {
                            System.IO.File.Delete(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" +  employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name + "_Statement.pdf");
                        }
                        DocumentName = (path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" +  employeeRepo.First_Name + "_" + employeeRepo.Surname +"_" + employeeRepo.ESF_Id +"_" + employeeRepo.Scheme_Name + "_Statement.pdf");

                        My_Report.Render();
                        My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);
                        ////return StiMvcViewer.GetReportSnapshotResult(My_Report);

                    }
                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Successful",
                        Message = "Statements Successfully Downloaded",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                }
                return this.Direct();
            }

            catch (Exception EX)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not download statements",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

        public ActionResult DownloadIndi(crm_EmployeeRepo employeeRepo)
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


                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.
                    while (reader.Read())
                    {
						
						employeeRepo.Surname = (string)reader["surname"];
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.ESF_Id = (string)reader["esf_id"];
                        employeeRepo.Email_Address = (string)reader["email_address"];
                        employeeRepo.Scheme_Name = (string)reader["SCHEME_NAME"];

						////load report table
						//var con = new AppSettings();
						//var param = new DynamicParameters();
						//param.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
						//param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
						//con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure);


						string DocumentName = "NA";
                        string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionMonth_On_Month_1.dll");

                        System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                        StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                        //////asign constring
                        My_Report.Dictionary.DataStore.Clear();
                        //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                        My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                        My_Report[":P_ESF_ID"] = employeeRepo.ESF_Id;

						string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

						if (!(System.IO.Directory.Exists(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year)))
						{
							System.IO.Directory.CreateDirectory(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year);
						}

						if ((System.IO.File.Exists(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname  + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.Scheme_Name + "_Statement.pdf")))
						{
							System.IO.File.Delete(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.Scheme_Name + "_Statement.pdf");
						}

						DocumentName = (path + @"\" + "STATEMENTS" + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname  + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.Scheme_Name + "_Statement.pdf");
						
						My_Report.Render();
                        My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);
                        //return StiMvcViewer.GetReportSnapshotResult(My_Report);
                    }

                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Statements Successfully Downloaded",
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
                    Message = "Could not download statements.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }


        public ActionResult DownloadActive(crm_EmployeeRepo employeeRepo)
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
                string queryString = "select * from vw_es_esf where employer_id = '" + employeeRepo.Employer_Id + "' and scheme_id = '" + employeeRepo.Scheme_Id + "' and ESF_STATUS = 'ACTIVE' and deferred < 3";


                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.


                    while (reader.Read())
                    {


                        employeeRepo.Surname = (string)reader["surname"];
                        employeeRepo.First_Name = (string)reader["first_name"];
                        employeeRepo.ESF_Id = (string)reader["esf_id"];
                        //employeeRepo.Email_Address = (string)reader["email_address"];
                        employeeRepo.Scheme_Name = (string)reader["SCHEME_NAME"];

                        ////load report table
                        //var con = new AppSettings();
                        //var param = new DynamicParameters();
                        //param.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
                        //param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        //con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure);


                        string DocumentName = "NA";
                        string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionMonth_On_Month_1.dll");

                        System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                        StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                        //////asign constring
                        My_Report.Dictionary.DataStore.Clear();
                        My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                        My_Report[":P_ESF_ID"] = employeeRepo.ESF_Id;

                        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                        if (!(System.IO.Directory.Exists(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year)))
                        {
                            System.IO.Directory.CreateDirectory(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year);
                        }

                        if (!(System.IO.File.Exists(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name + "_Statement.pdf")))
                        {
                            System.IO.File.Delete(path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name + "_Statement.pdf");
                        }
                        DocumentName = (path + @"\" + "STATEMENTS" + @"\" + employeeRepo.Employer_Name + @"\" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + @"\" + employeeRepo.First_Name + "_" + employeeRepo.Surname + "_" + employeeRepo.ESF_Id + "_" + employeeRepo.Scheme_Name + "_Statement.pdf");

                        My_Report.Render();
                        My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);
                        ////return StiMvcViewer.GetReportSnapshotResult(My_Report);

                    }
                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Successful",
                        Message = "Statements Successfully Downloaded",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                }
                return this.Direct();
            }

            catch (Exception EX)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not download statements",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }


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

		public ActionResult Read5(string Employer_Id,string Employer_Name)
		{
			try
			{
				if (string.IsNullOrEmpty(Employer_Id) || Employer_Id == "null")
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
				X.GetCmp<Store>("change_ESF_employeeStore_send_down").Reload();

				Store store = X.GetCmp<Store>("change_ESF_employeeStore_send_down");
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