using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using IBankWebService.Utils;
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
    public class EmployeeSMSStatementController : Controller
    {      
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

        public ActionResult AddEmployeeSMSStatementTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeSMSStatementPartial",
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

        public ActionResult AddEmployeeSMSStatementIndiTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeSMSStatementIndiPartial",
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

        public ActionResult GetESFEmployerList2(string Scheme_Id)
        {
            try
            {
                return this.Store(employeeRepo.GetEmployeeSFList2(Scheme_Id));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        
             public ActionResult GetESFEmployeeListscheme()
        {
            try
            {
                return this.Store(employeeRepo.GetESFEmployeeListscheme());
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
                if (String.IsNullOrEmpty(employeeRepo.Scheme_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Scheme",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }


				string queryString = "select * from vw_employer_es where scheme_id = '" + employeeRepo.Scheme_Id + "' and ES_STATUS = 'ACTIVE'  ";
                //string queryinternet = "select * from setup_company";

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.

                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty((string)reader["CONTACT_NUMBER"]))
                        {
                            employeeRepo.Mobile_Number = "000000000";
                        }
                        else
                        {
                            employeeRepo.Mobile_Number = (string)reader["CONTACT_NUMBER"];
                        }
                        employeeRepo.Employer_Name = (string)reader["Employer_Name"];
                        // repo_emplog.Mobile_Number = (string)reader["Mobile_Number"];
                        employeeRepo.Scheme_Name = (string)reader["Scheme_Name"];
                        employeeRepo.Scheme_Id = (string)reader["Scheme_Id"];
                        //employeeRepo.SEND_SMS = (string)reader["SEND_SMS"];
                        String mcode = (string)reader["Employer_Id"];

                        //if (employeeRepo.SEND_SMS == "YES")
                        //{
                            employeeRepo.Mobile_Number = employeeRepo.Mobile_Number.Replace(" ", string.Empty);
                            if (employeeRepo.Mobile_Number.Length < 9)
                            {
                                employeeRepo.Mobile_Number = "000000000";
                            }

                            //SEND SMS
                            string smsmsg = "Dear Sir/Madam (" + employeeRepo.Employer_Name + ") , " + employeeRepo.Message + " . Thank you";
                            string fonnum = "233" + employeeRepo.Mobile_Number.Substring(employeeRepo.Mobile_Number.Length - 9, 9);

                            Dictionary<string, string> paramSMS = new Dictionary<string, string>();
                            paramSMS.Add("to", fonnum);
                            paramSMS.Add("text", smsmsg);
                            Request request = new Request
                            {
                                Parameters = paramSMS
                            };

                            var content = Volley.PostRequest(request);
                            //END SEND SMS
                       // }

                    }

                    reader.Close();
                    X.Mask.Hide();

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Sent",
                        Message = "SMS Successfully Sent to Employers",
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
                    Message = "SMS traffic issue. Please send again to continue",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

        public ActionResult SendEmailIndi(crm_EmployeeRepo employeeRepo)
        {

            try
            {
                if (String.IsNullOrEmpty(employeeRepo.Scheme_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Scheme",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }


                string queryString = "select * from VW_EMPLOYEE_ESF_SCHEME_REPORT where scheme_id = '" + employeeRepo.Scheme_Id + "' and ESF_STATUS = 'ACTIVE'  ";
                //string queryinternet = "select * from setup_company";

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.

                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty((string)reader["MOBILE_NUMBER"]))
                        {
                            employeeRepo.Mobile_Number = "000000000";
                        }
                        else
                        {
                            employeeRepo.Mobile_Number = (string)reader["MOBILE_NUMBER"];
                        }
                        employeeRepo.First_Name = (string)reader["First_Name"];
                        // repo_emplog.Mobile_Number = (string)reader["Mobile_Number"];
                        employeeRepo.Scheme_Name = (string)reader["Scheme_Name"];
                        employeeRepo.Scheme_Id = (string)reader["Scheme_Id"];
                        //employeeRepo.SEND_SMS = (string)reader["SEND_SMS"];
                        String mcode = (string)reader["CUST_NO"];

                        //if (employeeRepo.SEND_SMS == "YES")
                        //{
                        employeeRepo.Mobile_Number = employeeRepo.Mobile_Number.Replace(" ", string.Empty);
                        if (employeeRepo.Mobile_Number.Length < 9)
                        {
                            employeeRepo.Mobile_Number = "000000000";
                        }

                        //SEND SMS
                        string smsmsg = "Dear " + employeeRepo.First_Name + " , " + employeeRepo.Message + " . Thank you";
                        string fonnum = "233" + employeeRepo.Mobile_Number.Substring(employeeRepo.Mobile_Number.Length - 9, 9);

                        Dictionary<string, string> paramSMS = new Dictionary<string, string>();
                        paramSMS.Add("to", fonnum);
                        paramSMS.Add("text", smsmsg);
                        Request request = new Request
                        {
                            Parameters = paramSMS
                        };

                        var content = Volley.PostRequest(request);
                        //END SEND SMS
                        // }

                    }

                    reader.Close();
                    X.Mask.Hide();

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Sent",
                        Message = "SMS Successfully Sent to Employees",
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
                    Message = "SMS traffic issue. Please send again to continue",
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


	} //end class
}