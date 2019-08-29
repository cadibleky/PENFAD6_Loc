using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using IBankWebService.Utils;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class ApproveNewEmployeeBatchUploadController : Controller
    {
        readonly crm_EmployeeBatchLogRepo batch_log = new crm_EmployeeBatchLogRepo();
        readonly crm_EmployeeRepo employee_repo = new crm_EmployeeRepo();
        IDbConnection con;

        // GET: CRM/EmployeeBatchUploadApproval
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployeeBatchUploadApprovalTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeBatchUploadApprovalPartial",
                    //Model = empList,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,

                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

                return pvr;
            }
            catch (Exception)
            {
                X.Mask.Hide();
                throw;
            }

        }

        public ActionResult Get_BatchLogPending()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(batch_log.GetBatch_EmployeeList_ByStatus("PENDING"));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult Get_EmployeesInBatchLogPending(crm_EmployeeRepo emp)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(employee_repo.GetEmployee_BatchList_ByStatus( emp.Batch_No));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }
        

        public ActionResult Approve_Pending_Batch(crm_EmployeeBatchLogRepo repo_emplog)
        {
            try
            {

                if (!string.IsNullOrEmpty(repo_emplog.Batch_No))
                {

                    //check if pending list exist for employer 
                    var app = new AppSettings();
                    con = app.GetConnection();
                    var param_k = new DynamicParameters();
                    decimal emp_num = 0;
                    param_k.Add(name: "p_employer_id", value: repo_emplog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_k.Add(name: "p_status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_k.Add(name: "p_result", value: emp_num, dbType: DbType.Decimal, direction: ParameterDirection.Output);
                    con.Execute(sql: "sel_crm_employerbatchpendexist", param: param_k, commandType: CommandType.StoredProcedure);
                    decimal tot_emp = param_k.Get<decimal>("p_result");
                    if (tot_emp <= 0)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Pending upload does not exists for this employer." + Environment.NewLine + ".Proces aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();
                    }

                    if (employee_repo.Approve_BatchEmployee_Pending(repo_emplog))
                    {
                        //SEND SMS
                        string queryString = "select * from vw_crm_employee where batch_no = '" + repo_emplog.Batch_No + "' ";

                        using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                        {
                            OracleCommand command = new OracleCommand(queryString, connection);
                            connection.Open();
                            OracleDataReader reader;
                            reader = command.ExecuteReader();
                            // Always call Read before accessing data.


                            while (reader.Read())
                            {
                                if (string.IsNullOrEmpty((string)reader["Mobile_Number"]))
                                {
                                    repo_emplog.Mobile_Number = "000000000";
                                }
                                else
                                {
                                    repo_emplog.Mobile_Number = (string)reader["Mobile_Number"];
                                }
                                repo_emplog.First_Name = (string)reader["First_Name"];
                               // repo_emplog.Mobile_Number = (string)reader["Mobile_Number"];
                                repo_emplog.Scheme_Name = (string)reader["Scheme_Name"];
                                repo_emplog.Employer_Name = (string)reader["Employer_Name"];
                                repo_emplog.SEND_SMS = (string)reader["SEND_SMS"];
                               String mcode = (string)reader["CUST_NO"];

                                if (repo_emplog.SEND_SMS == "YES")
                                {
									repo_emplog.Mobile_Number = repo_emplog.Mobile_Number.Replace(" ", string.Empty);
									if (repo_emplog.Mobile_Number.Length < 9)
                                    {
                                        repo_emplog.Mobile_Number = "000000000";
                                    }

                                    //SEND SMS
                                    string smsmsg = "Dear " + repo_emplog.First_Name + ", you have successfully been registered on the " + repo_emplog.Scheme_Name + " with memeber code " + mcode +". Thank you";
                                    string fonnum = "233" + repo_emplog.Mobile_Number.Substring(repo_emplog.Mobile_Number.Length - 9, 9);

                                    Dictionary<string, string> paramSMS = new Dictionary<string, string>();
                                    paramSMS.Add("to", fonnum);
                                    paramSMS.Add("text", smsmsg);
                                    Request request = new Request
                                    {
                                        Parameters = paramSMS
                                    };

                                    var content = Volley.PostRequest(request);
                                    //END SEND SMS
                                }

                            }

                            reader.Close();


                        }

                        //SEND SMS



                        X.Mask.Hide();
                        ClearControls();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Employee batch upload approved successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }                 
                    return this.Direct();
                }
                else
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select a batch to approve.", // " Insufficient data. Operation Aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //return this.Direct();


            }
            catch (Exception ex)
            {
                X.Mask.Hide();
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

        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_empbatchapproval");
                x.Reset();
                
                this.GetCmp<Store>("batchstore").RemoveAll();
                this.GetCmp<Store>("emp_list_store").RemoveAll();
                return this.Direct();
            }
            catch (System.Exception ex)
            {
                X.Mask.Hide();
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

        public ActionResult Delete_BatchRecord(crm_EmployeeBatchLogRepo repo_emplog)
        {
            try
            {
                if (string.IsNullOrEmpty(repo_emplog.Batch_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select Batch to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }              
                //delete Record
                if (batch_log.Delete_BatchRecord(repo_emplog.Batch_No))
                {
                    X.Mask.Hide();
                    ClearControls();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Delete Batch",
                        Message = "Batch Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }
             
                return this.Direct();

            }
            catch (System.Exception ex)
            {
                X.Mask.Hide();
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

// send batch id SMS
        public ActionResult Sms_Batch(crm_EmployeeBatchLogRepo repo_emplog)
        {
            try
            {

                //SEND SMS
                string queryString = "select * from vw_crm_employee where employer_id = '000438'";

                    using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                    {
                        OracleCommand command = new OracleCommand(queryString, connection);
                        connection.Open();
                        OracleDataReader reader;
                        reader = command.ExecuteReader();
                        // Always call Read before accessing data.


                        while (reader.Read())
                        {
                        if (string.IsNullOrEmpty((string)reader["Mobile_Number"]))
                        {
                            repo_emplog.Mobile_Number = "000000000";
                        }
                        else
                        {
                            repo_emplog.Mobile_Number = (string)reader["Mobile_Number"];
                        }
                            repo_emplog.First_Name = (string)reader["First_Name"];
                           // repo_emplog.Mobile_Number = (string)reader["Mobile_Number"];
                            repo_emplog.Scheme_Name = (string)reader["Scheme_Name"];
                            repo_emplog.Employer_Name = (string)reader["Employer_Name"];
                            String mcode = (string)reader["CUST_NO"];

                               if (repo_emplog.Mobile_Number.Length < 9)
                                {
                                    repo_emplog.Mobile_Number = "000000000";
                                }

                                //SEND SMS
                                string smsmsg = "Dear " + repo_emplog.First_Name + ", due to a system upgrade to improve service, you have been assigned a new Member ID "  + mcode +   ". Kindly quote this Member ID in addition to your full name anytime you make payment. EveryDay Pensions: A cedi today, a million tomorrow. ";
                                string fonnum = "233" + repo_emplog.Mobile_Number.Substring(repo_emplog.Mobile_Number.Length - 9, 9);

                                Dictionary<string, string> paramSMS = new Dictionary<string, string>();
                                paramSMS.Add("to", fonnum);
                                paramSMS.Add("text", smsmsg);
                                Request request = new Request
                                {
                                    Parameters = paramSMS
                                };

                                var content = Volley.PostRequest(request);
                                //END SEND SMS
                            }

                        reader.Close();

                        //X.Mask.Hide();
                        //ClearControls();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }
            

            }
            catch (Exception ex)
            {
                X.Mask.Hide();
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
//end

    }//end s class
}