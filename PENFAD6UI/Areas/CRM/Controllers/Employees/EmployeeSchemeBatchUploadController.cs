using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Security;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    // =====================Louis==============
    public class EmployeeSchemeBatchUploadController : Controller
    {
       // readonly crm_EmployeeBatchLogRepo crmEmployeelog_repo = new crm_EmployeeBatchLogRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_BeneNextRepo nxt = new crm_BeneNextRepo();
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        readonly GlobalValue global_val = new GlobalValue();
        readonly crm_EmployeeBatchLogRepo emp_log_repo = new crm_EmployeeBatchLogRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployeeSchemeBatchTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeSchemeBatchPartial", //"EmployeeBatchPatial",
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

        public ActionResult GetEmployer_SchemesList(string employer_id)
        {
            try
            {
                return this.Store(emp_log_repo.GetEmployerSchemeByEmployerId(employer_id));
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        public ActionResult UploadNewBatchEmployee(crm_EmployeeBatchLogRepo crmEmployeelog_repo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("file_upload1_Scheme").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("file_upload1_Scheme").PostedFile;

                    string extension = Path.GetExtension(file_posted.FileName);

                    if (extension != ".xlsx" && extension != ".xls")
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "selected file must be an excel file.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        
                        return this.Direct();
                    }

                    if (string.IsNullOrEmpty(crmEmployeelog_repo.Employer_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select employer.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                       
                        return this.Direct();
                    }
                    if (string.IsNullOrEmpty(crmEmployeelog_repo.Scheme_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select scheme for employer.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();
                    }


                    ImageWork.Upload_Any_File_Not_Image(file_posted);


                    if (BatchEmployeeUpload(ImageWork.Current_Path_For_Other_Files, crmEmployeelog_repo.Employer_Id, crmEmployeelog_repo.Scheme_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Sucess",
                            Message = "Employees uploaded successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }


                }
                else
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a file to upload.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    var x = X.GetCmp<FormPanel>("pn_employeebatchupload");
                    x.Reset();

                    return this.Direct();
                }

                return this.Direct();
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }


        }
        public ActionResult Get_EmployersScheme(string status)
        {
            status = "ACTIVE";
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(emp_log_repo.Get_Crm_Employer_SchemeByStatus(status));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult Get_Employers()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(employer.GetEmployerData());
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("pn_employeeSchemebatchupload");
                x.Reset();
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


        public bool BatchEmployeeUpload(string filePath, string Employer_number, string scheme_id)
        {
            try
            {

                if (System.IO.File.Exists(filePath) == false)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "File does not exist.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return false;
                }
                //'get file extension
                string file_ext = Path.GetExtension(filePath);

                string consString_excel = "";

                switch (file_ext)
                {
                    case ".xls":
                        consString_excel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;HDR=Yes;IMEX=2";

                        break;
                    case ".xlsx":
                        consString_excel = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";

                        break;

                }

                OleDbConnection con_ex = new OleDbConnection();
                OleDbCommand cmd = new OleDbCommand();

                string query1 = "Select COUNT(*) AS NOS From [EmployeeScheme$]";
                string query2 = "Select * From [EmployeeScheme$]";
                int totalsum = 1;

                con_ex.ConnectionString = consString_excel;
                con_ex.Open();

                cmd.Connection = con_ex;
                cmd.CommandText = query1;

                totalsum = Convert.ToInt32(cmd.ExecuteScalar()); //();
                con_ex.Close();

                if (con_ex.State == ConnectionState.Closed)
                {
                    con_ex.Open();
                }



                if (con_ex.State == ConnectionState.Open)
                {
                    cmd.Connection = con_ex;
                    cmd.CommandText = query2;
                    OleDbDataReader srda = cmd.ExecuteReader();

                    int a_value = 0;

                    if (srda.HasRows)
                    {
                        string errormsg = "";

                        Stopwatch sw = new Stopwatch();
                        // Start The StopWatch ...From 000
                        sw.Start();
                        var app = new AppSettings();
                        //IDbConnection con;

                       DateTime date_default = Convert.ToDateTime("01/01/1901");
                         
                        TransactionOptions tsOp = new TransactionOptions();
                        tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                        TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
                        tsOp.Timeout = TimeSpan.FromMinutes(20);

                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();

                            try
                            {
                                

                                //check if pending list exist for employer already
                                var param_k = new DynamicParameters(); 
                                decimal emp_num = 0;
                                param_k.Add(name: "p_employer_id", value: Employer_number, dbType: DbType.String, direction: ParameterDirection.Input);
                                param_k.Add(name: "p_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                                param_k.Add(name: "p_result", value: emp_num, dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                conn.Execute(sql: "sel_crm_employerbatchpendexist", param: param_k, commandType: CommandType.StoredProcedure);
                                decimal tot_emp = param_k.Get<decimal>("p_result");
                                if (tot_emp > 0)
                                {
                                    X.Mask.Hide();
                                    X.Msg.Show(new MessageBoxConfig
                                    {
                                        Title = "Error",
                                        Message = "Pending upload exists for this employer already." + Environment.NewLine + "Approve the pending employees.Proces aborted",
                                        Buttons = MessageBox.Button.OK,
                                        Icon = MessageBox.Icon.ERROR,
                                        Width = 350

                                    });
                                    return false;
                                }

                                ///create log for the upload P_scheme_id
                                var paramb = new DynamicParameters();
                                string batchno = "na";
                                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Make_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_employer_id", value: Employer_number, dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_BatchNo", value: batchno, dbType: DbType.String, direction: ParameterDirection.Output);
                                paramb.Add(name: "p_scheme_id", value: scheme_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                conn.Execute(sql: "ADD_CRM_EMPLOYEE_BATCH_TWO", param: paramb, commandType: CommandType.StoredProcedure);
                                batchno = paramb.Get<string>("p_BatchNo");
                                if (batchno == "na")
                                {
                                    X.Mask.Hide();
                                    X.Msg.Show(new MessageBoxConfig
                                    {
                                        Title = "Error",
                                        Message = "Employer details could not be logged.",
                                        Buttons = MessageBox.Button.OK,
                                        Icon = MessageBox.Icon.ERROR,
                                        Width = 350

                                    });
                                    return false;
                                }
                                //-----end batch number
                                int error_nos = 0;
                                string erroor_msg = "Error:" + Environment.NewLine;
                                var param = new DynamicParameters();

                                //this.GetCmp<Label>("lblload").Text = "Loading";

                                while (srda.Read())
                                {
                                    a_value += 1;

                                    crm_EmployeeRepo employee_Repo = new crm_EmployeeRepo();

                                    string employee_fund_id = "";

                                    //surname
                                    if (srda["Surname"] != DBNull.Value)
                                    {
                                        employee_Repo.Surname = srda["Surname"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        error_nos += 1;
                                        erroor_msg = erroor_msg + "Surname cannot be null." + Environment.NewLine;
                                    }
                                    //FirstName   
                                    if (srda["FirstName"] != DBNull.Value)
                                    {
                                        employee_Repo.First_Name = srda["FirstName"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        error_nos += 1;
                                        erroor_msg = erroor_msg + "Supply First Name For " + employee_Repo.Surname + Environment.NewLine;
                                    }
                                    //verify Company's EmployeID
                                    //get CustNo for employee
                                    int create_new_or_justacct = 1; // 1 for create new employee , 2 for creat just account
                                    string cust_noo = "";
                                    if (srda["CompanyEmployeeID"] != DBNull.Value)
                                    {
                                        employee_Repo.Employee_Id = srda["CompanyEmployeeID"].ToString();
                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "sel_crm_EmployeeIdExist";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_EmployerId", OracleDbType.Varchar2, ParameterDirection.Input).Value = Employer_number;
                                        cmd_emp_id.Parameters.Add("p_EmployeeId", OracleDbType.Varchar2, ParameterDirection.Input).Value = employee_Repo.Employee_Id;
                                        //Output param
                                        OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                        OracleParameter cust_num = new OracleParameter("p_custNo", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                                        cmd_emp_id.Parameters.Add(count);
                                        cmd_emp_id.Parameters.Add(cust_num);

                                        cmd_emp_id.ExecuteNonQuery();
                                        cust_noo = (cust_num.Value).ToString();
                                        string mtotal = (count.Value).ToString();
                                        int total = Convert.ToInt32(mtotal);
                                        if (total > 0)
                                        {
                                            create_new_or_justacct = 2;
                                            employee_Repo.Cust_No = cust_noo;
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Employee ID already exist For this employer. - " + employee_Repo.Surname + Environment.NewLine;
                                            //get custNo

                                        }
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        error_nos += 1;
                                        erroor_msg = erroor_msg + "Supply Employee ID For " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                    }


                                    if (create_new_or_justacct == 1)
                                    {

                                        //Fundid
                                        if (srda["FundID"] != DBNull.Value)
                                        {
                                            employee_fund_id = srda["FundID"].ToString();
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Scheme ID not provided for ." + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        if (employee_fund_id == "01")
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Employee cannot subscribe to system fund (01)- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }
                                        //Sceme_Fund_ID
                                        string Scheme_fund_id = scheme_id + employee_fund_id;
                                        if (string.IsNullOrEmpty(Scheme_fund_id) == false)
                                        {

                                            //Validate Employee SchemeFundID  to check if its already exist
                                            OracleCommand cmd_emp_id = new OracleCommand();
                                            cmd_emp_id.CommandText = "sel_pfm_SchemeFundIdExist";
                                            cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                            cmd_emp_id.Connection = (OracleConnection)conn;
                                            //Input param
                                            cmd_emp_id.Parameters.Add("p_scheme_fund_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_fund_id;
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_emp_id.Parameters.Add(count);
                                            cmd_emp_id.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);
                                            if (total <= 0)
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }
                                    }

                             

                                    else if (create_new_or_justacct == 2)  /////Just create schme fund account for employee
                                    {

                                        //Fundid
                                        if (srda["FundID"] != DBNull.Value)
                                        {
                                            employee_fund_id = srda["FundID"].ToString();
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Fund ID not provided for ." + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }


                                        //Sceme_Fund_ID
                                        string Scheme_fund_id = scheme_id + employee_fund_id;
                                        if (string.IsNullOrEmpty(Scheme_fund_id) == false)
                                        {

                                            //Validate Employee SchemeFundID  to check if its already exist
                                            OracleCommand cmd_emp_id = new OracleCommand();
                                            cmd_emp_id.CommandText = "sel_pfm_SchemeFundIdExist";
                                            cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                            cmd_emp_id.Connection = (OracleConnection)conn;
                                            //Input param
                                            cmd_emp_id.Parameters.Add("p_scheme_fund_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_fund_id;
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_emp_id.Parameters.Add(count);
                                            cmd_emp_id.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);
                                            if (total <= 0)
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }



                                        //create account now
                                        ////create scheme fund account for employee
                                        decimal successs_1_2 = 0;
                                        string Employee_Scheme_fund_id = cust_noo + Scheme_fund_id;

                                        //check if ESF ID exist
                                        var param_t = new DynamicParameters();
                                        param_t.Add(name: "p_esf_id", value: Employee_Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param_t.Add(name: "p_result", value: emp_num, dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                        conn.Execute(sql: "SEL_CRM_ESFIDEXIST", param: param_t, commandType: CommandType.StoredProcedure);
                                        decimal t_res = param_t.Get<decimal>("p_result");
                                        if (t_res > 0)
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employer Scheme already exist for " + employee_Repo.First_Name + " " + employee_Repo.Surname,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }

                                        param.Add(name: "p_emp_schme_fund_id", value: Employee_Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_scheme_fund_id", value: Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_custNo", value: cust_noo, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_BatchNo", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                        param.Add(name: "p_result", value: successs_1_2, dbType: DbType.Decimal, direction: ParameterDirection.Output);

                                        conn.Execute(sql: "ADD_CRM_REMITSCHEMEFUNDBNEW", param: param, commandType: CommandType.StoredProcedure);


                                    }//end if for create_new_or_justacct =2

                                } ///while


                                ts.Complete();
                               // this.GetCmp<Window>("loading_EmpBatch").Hide();
                                return true;

                            }
                            catch (TransactionException transexeption)
                            {
                                X.Mask.Hide();
                                X.Msg.Show(new MessageBoxConfig
                                {
                                    Title = "Error",
                                    Message = transexeption.ToString(),
                                    Buttons = MessageBox.Button.OK,
                                    Icon = MessageBox.Icon.ERROR,
                                    Width = 350

                                });

                                errormsg = transexeption.ToString();
                                return false;
                                //throw;
                            }
                            catch (System.Exception ex)
                            {
                                string ora_code = ex.Message.Substring(0, 9);
                                if (ora_code == "ORA-20000")
                                {
                                    ora_code = "Record already exist. Process aborted..";
                                }
                                else if (ora_code == "ORA-20100")
                                {
                                    ora_code = "Record is uniquely defined in the system. Process aborted..";
                                }
                                else
                                {
                                    ora_code = ex.ToString();
                                }
                                X.Mask.Hide();
                                X.Msg.Show(new MessageBoxConfig
                                {
                                    Title = "Error",
                                    Message = ora_code,
                                    Buttons = MessageBox.Button.OK,
                                    Icon = MessageBox.Icon.INFO,
                                    Width = 350


                                });
                                // logger.WriteLog(ex.Message);

                                return false;
                            }
                            finally
                            {
                                ts.Dispose();
                                //a_value = a_value;
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.Close();
                                }

                                if (con_ex.State == ConnectionState.Open)
                                {
                                    con_ex.Close();
                                }

                            }


                        }  //end for transscope



                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Record already exist. Process aborted..";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Record is uniquely defined in the system. Process aborted..";
                }
                else
                {
                    ora_code = ex.ToString();
                }
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                // logger.WriteLog(ex.Message);

                return false;
            }
        }


        public ActionResult DeleteBatchRecord(string batch_no)
        {
            try
            {
                if (string.IsNullOrEmpty(batch_no))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a Employee Batch List to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
              
                //delete Record
                if (emp_log_repo.Delete_BatchRecord(batch_no))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    //clear controls
                    ClearControls();
                }

                Store store = X.GetCmp<Store>("emp_list_store");
                store.Reload();

                return this.Direct();
            }
            catch (System.Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }

        }


        private string GetRandomvalue()
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(10000, 90000);

                return randomNumber.ToString();
            }
            catch (Exception)
            {
                return "19074";
                // throw;
            }
        }


        //public ActionResult Button6_Click(crm_EmployeeBatchLogRepo crmEmployeelog_repo)
        //{
        //    X.Msg.Show(new MessageBoxConfig
        //    {
        //        Message = "Saving your data, please wait...",
        //        ProgressText = "Saving...",
        //        Width = 300,
        //        Wait = true,
        //        WaitConfig = new WaitConfig { Interval = 200 },
        //        IconCls = "ext-mb-download",
        //        //AnimEl = UploadNewBatchEmployee(crmEmployeelog_repo).ToString()
        //    });
        //    X.AddScript("setTimeout(function () { Ext.MessageBox.hide(); Ext.Msg.notify('Done', 'Your data was saved!'); }, 10000);");
        //    return this.Direct();
            
        //}


    } //end class
}