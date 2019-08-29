using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Remittance.Contribution;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using Serilog;
using Serilog.Core;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Contribution
{
    public class PayAdviceRemit_ContributionController : Controller
    {
        readonly Remit_Contribution_Upload_LogRepo RemitConRepo = new Remit_Contribution_Upload_LogRepo();
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        readonly Remit_BatchLogRepo Batch_Log = new Remit_BatchLogRepo();
        readonly setup_InternetRepo internetRepo = new setup_InternetRepo();
        IDbConnection con;
        cLogger logger = new cLogger();
        // GET: Contribution/ApprovedRemit_Contribution
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddPayAdviceTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PayAdvicePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }


        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_remittancebatchdelete");
                x.Reset();

                this.GetCmp<Store>("deleteemppppbatchstore").Reload();
                this.GetCmp<Store>("deleteempconremitance_list_store").Reload();
                return this.Direct();
            }
            catch (System.Exception ex)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ex.ToString(),
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350

                });
                return this.Direct();
            }
        }


   

        public ActionResult PayAdvice_Record(Remit_Contribution_Upload_LogRepo employeeRepo)
        {
            try
            {
                if (String.IsNullOrEmpty(employeeRepo.Employer_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Contribution",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

                
                string DD = System.DateTime.Now.ToString("dd-MMM-yyyy");

                string queryString = "select * from VW_REMIT_CON_PURSHACE where con_log_id = '" + employeeRepo.Con_Log_Id + "' ";
                string queryinternet = "select * from setup_company";

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.


                    while (reader.Read())
                    {
                        employeeRepo.Scheme_Name = (string)reader["Scheme_Name"];
                        employeeRepo.Employer_Name = (string)reader["Employer_Name"];
                        employeeRepo.Con_Log_Id = (string)reader["Con_Log_Id"];
                        employeeRepo.For_MonthN = ((decimal)reader["For_Month"]).ToString();
                        employeeRepo.For_YearN = ((decimal)reader["For_Year"]).ToString();
                        employeeRepo.Contact_Person = (string)reader["Contact_Person"];
                        employeeRepo.Contact_Email = (string)reader["Contact_Email"];

                        if (String.IsNullOrEmpty(employeeRepo.Contact_Email))
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Sorry! No Email Address. Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });

                            return this.Direct();
                        }

                        if (Regex.IsMatch(((string)reader["Contact_Email"]), MatchEmailPattern) == false)
                        {
                            //log this
                        }
                        else
                        {
                            ////load report table
                            //var con = new AppSettings();
                            //var param = new DynamicParameters();
                            //param.Add("P_ESF_ID", employeeRepo.ESF_Id, DbType.String, ParameterDirection.Input);
                            //param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                            //con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure);

                            employeeRepo.Contact_Email = (string)reader["Contact_Email"];
                            string DocumentName = "NA";
                            string pa = Server.MapPath("~/Penfad_Reports/employer_Payment_Advice.dll");

                            System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                            StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                            //////asign constring
                            My_Report.Dictionary.DataStore.Clear();

                            My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                            My_Report[":P_CON_LOG"] = employeeRepo.Con_Log_Id;

                            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                            if (!(System.IO.Directory.Exists(path + @"\" + "PAYMENT_ADVICE" + @"\" + employeeRepo.Scheme_Name + @"\" + employeeRepo.Employer_Name)))
                            {
                                System.IO.Directory.CreateDirectory(path + @"\" + "PAYMENT_ADVICE" + @"\" + employeeRepo.Scheme_Name + @"\" + employeeRepo.Employer_Name);
                            }

                            if (!(System.IO.File.Exists(path + @"\" + "PAYMENT_ADVICE" + @"\" + employeeRepo.Scheme_Name + @"\" + employeeRepo.Employer_Name + @"\" + employeeRepo.For_MonthN + "_" + employeeRepo.For_YearN + ".pdf")))
                            {
                                System.IO.File.Delete(path + @"\" + "PAYMENT_ADVICE" + @"\" + employeeRepo.Scheme_Name + @"\" + employeeRepo.Employer_Name + @"\" + employeeRepo.For_MonthN + "_" + employeeRepo.For_YearN + ".pdf");
                            }
                            DocumentName = (path + @"\" + "PAYMENT_ADVICE" + @"\" + employeeRepo.Scheme_Name + @"\" + employeeRepo.Employer_Name + @"\" + employeeRepo.For_MonthN + "_" + employeeRepo.For_YearN + ".pdf");

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

                            var msg = $@"Dear {employeeRepo.Contact_Person},   Kindly find attached, Payment Advice for  {employeeRepo.Scheme_Name}.    Thank you.  {internetRepo.company_name}";
                            string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Payment Advice", to = employeeRepo.Contact_Email;
                            string smtp = internetRepo.smtp;
                            int port = internetRepo.port;
                            string attach = DocumentName;
                            internetRepo.SendIt3(from, pass, subj, msg, to, smtp, port, employeeRepo, attach);

                            #endregion


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
                    Message = "Email traffic issue. Process aborted",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
}


public ActionResult ReadEmployer()
        {
            try
            {
                return this.Store(ESRepo.GetEmployerList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
      
        public ActionResult Get_BatchLogdelete()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(Batch_Log.GetBatch_RemitList_ForDelete("ACTIVE"));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }
        public ActionResult Get_RemittnaceInBatchLogdelete(Remit_Contribution_Upload_LogRepo remLog)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(remLog.GetEmployeeCon_BatchList_delete(remLog.Con_Log_Id));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult download_PayAdvice_Record(Remit_Contribution_Upload_LogRepo conLogRepo)
        {

            try
            {
                if (String.IsNullOrEmpty(conLogRepo.Con_Log_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Contribution Schedule",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }
                string queryString = "select * from VW_REMIT_CON_PURSHACE where con_log_id = '" + conLogRepo.Con_Log_Id + "' ";
                
                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.


                    while (reader.Read())
                    {


                        conLogRepo.Scheme_Name = (string)reader["Scheme_Name"];
                        conLogRepo.Employer_Name = (string)reader["Employer_Name"];
                        conLogRepo.Con_Log_Id = (string)reader["Con_Log_Id"];
                        conLogRepo.For_MonthN = ((decimal)reader["For_Month"]).ToString();
                        conLogRepo.For_YearN = ((decimal)reader["For_Year"]).ToString();


                        string DocumentName = "NA";
                        string pa = Server.MapPath("~/Penfad_Reports/employer_Payment_Advice.dll");

                        System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                        StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                        //////asign constring
                        My_Report.Dictionary.DataStore.Clear();
                        My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                        My_Report[":P_CON_LOG"] = conLogRepo.Con_Log_Id;

                        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                        if (!(System.IO.Directory.Exists(path + @"\" + "PAYMENT_ADVICE" + @"\" + conLogRepo.Scheme_Name + @"\" + conLogRepo.Employer_Name  )))
                        {
                            System.IO.Directory.CreateDirectory(path + @"\" + "PAYMENT_ADVICE" + @"\" + conLogRepo.Scheme_Name + @"\" + conLogRepo.Employer_Name );
                        }

                        if (!(System.IO.File.Exists(path + @"\" + "PAYMENT_ADVICE" + @"\" + conLogRepo.Scheme_Name + @"\" + conLogRepo.Employer_Name + @"\" + conLogRepo.For_MonthN + "_" + conLogRepo.For_YearN + ".pdf")))
                        {
                            System.IO.File.Delete(path + @"\" + "PAYMENT_ADVICE" + @"\" + conLogRepo.Scheme_Name + @"\" + conLogRepo.Employer_Name + @"\" + conLogRepo.For_MonthN + "_" + conLogRepo.For_YearN+".pdf");
                        }
                        DocumentName = (path + @"\" + "PAYMENT_ADVICE" + @"\" + conLogRepo.Scheme_Name + @"\" + conLogRepo.Employer_Name + @"\" + conLogRepo.For_MonthN + "_" + conLogRepo.For_YearN + ".pdf");

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
                        Message = "Payment Advice Successfully Downloaded",
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
                    Message = "Could not download Payment Advice",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }


    }
}

