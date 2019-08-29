using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Remittance.Contribution;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Serilog;


namespace PENFAD6UI.Areas.Remittance.Controllers.Contribution
{
    public class Remit_ContributionSupUpLoad_LogController : Controller
    {
        string plog;
        readonly Remit_Contribution_Supplimentary_LogRepo RemitInitialRepo = new Remit_Contribution_Supplimentary_LogRepo();
        readonly crm_EmployerRepo EmployerRepo = new crm_EmployerRepo();
        readonly Remit_BatchLogRepo repo_Remit_Batch = new Remit_BatchLogRepo();

        string error = "";
        // GET: Contribution/Remit_ContributionLoad_Log------by Richard--------
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddContributionRemitTab_Sup(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ContributionRemit_SupPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        //Get Employer  Scheme Account
        public ActionResult Get_EmployersScheme(string status, string param_employer_id, string param_scheme_id, string param_employer_scheme_id)
        {
            status = "ACTIVE";
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(RemitInitialRepo.Get_Crm_Employer_SchemeByStatus(status));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }

        //public ActionResult Get_Month_Year_For_Employer_Upload(string param_employer_scheme_id, string param_scheme_id)
        //{
        //    var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
        //    try
        //    {
        //        decimal mon_th = 0;
        //        decimal year_r = 0;
        //        DateTime d_date = DateTime.Now;
        //        repo_Remit_Batch.Get_Employer_Scheme_Month_Year_For_Remitance_Upload(param_employer_scheme_id, out mon_th, out year_r, out d_date);
        //        if (mon_th == 0 && year_r == 0)
        //        {
        //            var cmd_month = X.GetCmp<ComboBox>("ContributionRemitPartial_txtremittancemonth_Sup");
        //            cmd_month.Enable();
        //            var cmd_year = X.GetCmp<ComboBox>("ContributionRemitPartial_txtfrmRemittance_year_Sup");
        //            cmd_year.Enable();
        //        }
        //        else
        //        {
        //            if (mon_th == 12)
        //            {
        //                mon_th = 1;
        //                year_r += 1;
        //            }
        //            else
        //            {
        //                mon_th += 1;
        //            }

        //            var cmd_month = X.GetCmp<ComboBox>("ContributionRemitPartial_txtremittancemonth_Sup");
        //            cmd_month.Value = mon_th;
        //            cmd_month.Disable();// = false;
        //            var cmd_year = X.GetCmp<ComboBox>("ContributionRemitPartial_txtfrmRemittance_year_Sup");
        //            cmd_year.Value = year_r;
        //            cmd_year.Disable(); // = false;
        //        }


        //        /// get next deadline date
        //        //decimal dead_line_day = 0;
        //        DateTime scheme_today_date = DateTime.Now.Date;
        //        DateTime? scheme_deadLine_date = DateTime.Now.Date;
        //        IEnumerable<pfm_SchemeRepo> repo_schemeee = repo_Remit_Batch.Get_Scheme_Current_Dates(param_scheme_id);
        //        foreach (var item in repo_schemeee)
        //        {
        //            //dead_line_day = item.DeadLine_Day;
        //            scheme_today_date = item.Today_Date;
        //           // scheme_deadLine_date = item.Next_Deadline_Date;
        //        }
        //        ///getenratenew deadline date
        //        if (mon_th > 0 || year_r > 0)
        //        {
        //            if (mon_th == 12)
        //            {
        //                mon_th = 1;
        //                year_r += 1;
        //            }
        //            else
        //            {
        //                mon_th += 1;
        //            }
        //            DateTime? next_deadline_date = Convert.ToDateTime(d_date).AddMonths(1);

        //            var cmd_deadlineDate = X.GetCmp<TextField>("txtDeadline_Date_Sup");
        //            cmd_deadlineDate.SetValue(next_deadline_date);

        //        }
        //        return this.Direct();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
        //        return this.Direct();
        //    }

        //}

        public ActionResult GetMDate(Remit_Contribution_Supplimentary_LogRepo RemitInitialRepo)
        {
            try
            {
                DateTime dd = Convert.ToDateTime(RemitInitialRepo.For_Month + "/" + "14" + "/" + RemitInitialRepo.For_Year);
                dd = dd.AddMonths(1);
                this.GetCmp<DateField>("txtDeadline_Date_Sup").SetValue(dd);              
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }

        public ActionResult GetEmployer_SchemesList(string employer_id)
        {
            try
            {
                return this.Store(RemitInitialRepo.GetEmployerSchemeByEmployerId(employer_id));
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult GetEmployer()
        {
            try
            {
                return this.Store(RemitInitialRepo.GetEmployerList());
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_remittanceContribution_Sup");
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

        public ActionResult UploadRemittanceClick(Remit_Contribution_Upload_LogRepo remit_cont_repo, string param_employer_scheme_id, string Month_str, string year_str)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                //Check if file Exist
                if (this.GetCmp<FileUploadField>("remitfile_upload1_Sup").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("remitfile_upload1_Sup").PostedFile;

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

                    if (string.IsNullOrEmpty(remit_cont_repo.Employer_Id))
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
                    if (string.IsNullOrEmpty(remit_cont_repo.Scheme_Id))
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


                    //decimal mon_th = 0;
                    //decimal year_r = 0;
                    //DateTime d_date = DateTime.Now;
                    //repo_Remit_Batch.Get_Employer_Scheme_Month_Year_For_Remitance_Upload(param_employer_scheme_id, out mon_th, out year_r, out d_date);
                    //if (mon_th == 0 && year_r == 0)
                    //{
                    //    if (Microsoft.VisualBasic.Information.IsNumeric(Month_str))
                    //    {
                    //        remit_cont_repo.For_Month = Convert.ToInt32(Month_str);
                    //    }
                    //    if (Microsoft.VisualBasic.Information.IsNumeric(year_str))
                    //    {
                    //        remit_cont_repo.For_Year = Convert.ToInt32(year_str);
                    //    }

                    //}
                    //else
                    //{
                    //    if (mon_th == 12)
                    //    {
                    //        remit_cont_repo.For_Month = 1;
                    //        remit_cont_repo.For_Year = Convert.ToInt32(year_r) + 1;
                    //    }
                    //    else
                    //    {
                    //        remit_cont_repo.For_Month = Convert.ToInt32(mon_th + 1);
                    //        remit_cont_repo.For_Year = Convert.ToInt32(year_r);
                    //    }
                    //}

                    ////check if screen values are still valid
                    if (Month_str != remit_cont_repo.For_Month.ToString() && year_str != remit_cont_repo.For_Year.ToString())
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Error with Selected month and year.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }


                    if (Microsoft.VisualBasic.Information.IsNumeric(remit_cont_repo.For_Month) == false)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Month.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    if (remit_cont_repo.For_Year <= 0)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Year.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }


                    ImageWork.Upload_Any_File_Not_Image(file_posted);

                    ///check last upload date for employer



                    if (BatchRemitInitialUpload(ImageWork.Current_Path_For_Other_Files, remit_cont_repo, remit_cont_repo.For_Month, remit_cont_repo.For_Year))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Sucess",
                            Message = "Remittance uploaded successfully.",
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

                    var x = X.GetCmp<FormPanel>("frm_remittanceContribution_Sup");
                    x.Reset();

                    Store store = X.GetCmp<Store>("employerscheme_store_Sup");
                    store.Reload();

                    return this.Direct();
                }

                return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }


        }
        public ActionResult Get_Employers()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(EmployerRepo.GetEmployerData());
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }


        public bool BatchRemitInitialUpload(string filePath, Remit_Contribution_Upload_LogRepo remit_cont_repo, int current_month, int current_year)

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

                string query1 = "Select COUNT(*) AS NOS From [SalaryData$]";
                string query2 = "Select * From [SalaryData$]";
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

                        // Stopwatch sw = new Stopwatch();
                        // Start The StopWatch ...From 000
                        //  sw.Start();
                        var app = new AppSettings();
                        //IDbConnection con;


                        string p_SEmployer_Id = remit_cont_repo.GetRemit_EmployerSchemes_Datasets(remit_cont_repo.Employer_Id, remit_cont_repo.Scheme_Id);

                        decimal p_Salary_Rate = remit_cont_repo.Get_EmployeeSalSchemes_Rate(remit_cont_repo.Scheme_Id);

                        decimal p_Grace_Period = remit_cont_repo.Get_EmployeeScheme_GracePeriod(remit_cont_repo.Scheme_Id);



                        //get scheme_today_date 
                        GlobalValue.Get_Scheme_Today_Date(remit_cont_repo.Scheme_Id);
                        
                        TransactionOptions tsOp = new TransactionOptions();
                        tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                        TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
                        tsOp.Timeout = TimeSpan.FromMinutes(20);

                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();

                            try
                            {
                               
                               
                                ////////////////////////////////////////////////
                                if (remit_cont_repo.For_Month.ToString().Length == 1)
                                {
                                    plog = "0" + remit_cont_repo.For_Month;
                                }
                                else
                                {
                                    plog = remit_cont_repo.For_Month.ToString();
                                }
                                ///////////////////////////////////////////////////

                                ///create log for the upload Remit Log
                                var paramb = new DynamicParameters();
                               // string batchno = "";
                                
                                paramb.Add(name: "p_Employer_Id", value: remit_cont_repo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_ES_Id", value: p_SEmployer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_For_Month", value: plog, dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_For_Year", value: remit_cont_repo.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_DeadLine_Date", value: remit_cont_repo.DeadLine_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Auth_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Log_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_GracePeriod", value: remit_cont_repo.Grace_Period, dbType: DbType.Int32, direction: ParameterDirection.Input);
                                paramb.Add(name: "p_Con_Log_Id", value: string.Empty, dbType: DbType.String, direction: ParameterDirection.Output);
                                conn.Execute(sql: "ADD_REMIT_CON_LOG", param: paramb, commandType: CommandType.StoredProcedure);

                                string batchno = paramb.Get<string>("p_Con_Log_Id");

                                if (batchno == "na")
                                {
                                    X.Mask.Hide();
                                    X.Msg.Show(new MessageBoxConfig
                                    {
                                        Title = "Error",
                                        Message = "Remittance Log details could not be logged.",
                                        Buttons = MessageBox.Button.OK,
                                        Icon = MessageBox.Icon.ERROR,
                                        Width = 350

                                    });
                                    return false;
                                }
                                //-----end batch number

                                var param = new DynamicParameters();
                                while (srda.Read())
                                {
                                    int error_nos = 0;
                                    string erroor_msg = "Error:" + Environment.NewLine;

                                    a_value += 1;
                                    Remit_Contribution_Supplimentary_LogRepo Remit_ConRepo = new Remit_Contribution_Supplimentary_LogRepo();

                                    //string employee_fund_id = "";

                                    int create_new_or_justacct = 1; // 1 for create new employee , 2 for creat just account
                                    string cust_noo = "";
                                    if (srda["EmployeeId"] != DBNull.Value)
                                    {
                                        Remit_ConRepo.Employee_Id = srda["EmployeeId"].ToString();
                                        ///// Remit_ConRepo.Employer_Id = srda["p_EmployerId"].ToString();

                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "sel_crm_EmployeeIdExist";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection) conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_EmployerId", OracleDbType.Varchar2, ParameterDirection.Input).Value = remit_cont_repo.Employer_Id;
                                        cmd_emp_id.Parameters.Add("p_EmployeeId", OracleDbType.Varchar2, ParameterDirection.Input).Value = Remit_ConRepo.Employee_Id;
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
                                            create_new_or_justacct = 1;
                                            Remit_ConRepo.Cust_No = cust_noo;

                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employee ID does not exist for this employer . - " + Remit_ConRepo.Employee_Id + Environment.NewLine,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350

                                            });
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Employee ID does not exist for this employer . - " + Remit_ConRepo.Employee_Id + Environment.NewLine,
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }


                                    //check if employee remittance already exist for the month

                                    //var param_k = new DynamicParameters();

                                    //decimal emp_num = 0;
                                    //param_k.Add(name: "p_ESF_ID", value: Remit_ConRepo.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param_k.Add(name: "p_CON_LOG_ID", value: "CON" + remit_cont_repo.ES_Id  + remit_cont_repo.For_Year + plog, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param_k.Add(name: "p_result", value: emp_num, dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                    //conn.Execute(sql: "SEL_REMIT_CONEMPEXIST", param: param_k, commandType: CommandType.StoredProcedure);
                                    //decimal tot_emp = param_k.Get<decimal>("p_result");
                                    //if (tot_emp > 0)
                                    //{
                                    //    X.Mask.Hide();
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Contribution already exists for " + srda["Employeename"].ToString() + Environment.NewLine + ". Proces aborted",
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.ERROR,
                                    //        Width = 350

                                    //    });
                                    //    return false;
                                    //}



                                    if (create_new_or_justacct == 1)
                                    {

                                       
                                        if (srda["Employeename"] != DBNull.Value)
                                        {
                                            Remit_ConRepo.Employee_Name = srda["Employeename"].ToString();
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Supply Employee Name For " + Remit_ConRepo.Employee_Name + Environment.NewLine;
                                        }

                                        //Employer Id  
                                        if (srda["EmployerId"] != DBNull.Value)
                                        {
                                            string employer_Iddd = srda["EmployerId"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(employer_Iddd))
                                            {
                                                if (remit_cont_repo.Employer_Id != employer_Iddd)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid Employer ID for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }

                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Employer ID for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employer ID cannot be null. for " + Remit_ConRepo.Employee_Name,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;

                                        }

                                        //Scheme ID  
                                        if (srda["SchemeId"] != DBNull.Value)
                                        {
                                            string scheme_Iddd = srda["SchemeId"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(scheme_Iddd))
                                            {
                                                if (remit_cont_repo.Scheme_Id != scheme_Iddd)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid scheme ID for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid scheme ID for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "scheme cannot be null for " + Remit_ConRepo.Employee_Name,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;

                                        }

                                        //Employee Contribution  
                                        if (srda["EmployeeCon"] != DBNull.Value)
                                        {
                                            string employee_con_amt = srda["EmployeeCon"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(employee_con_amt))
                                            {
                                                Remit_ConRepo.Employee_Con = Convert.ToDecimal(employee_con_amt);
                                                if (Remit_ConRepo.Employee_Con < 0)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid Employee Contribution for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Employee Contribution for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employee Contribution cannot be null.",
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;

                                        }


                                        //Employer Contribution  
                                        if (srda["EmployerCon"] != DBNull.Value)
                                        {
                                            string employer_con_amt = srda["EmployerCon"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(employer_con_amt))
                                            {
                                                Remit_ConRepo.Employer_Con = Convert.ToDecimal(employer_con_amt);
                                                if (Remit_ConRepo.Employer_Con < 0)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid Employer Contribution for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Employer Contribution for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employer Contribution cannot be null.",
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }
                                        /////Employee Salary
                                        if (srda["EmployeeSalary"] != DBNull.Value)
                                        {
                                            string employer_Sal_Amt = srda["EmployeeSalary"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(employer_Sal_Amt))
                                            {
                                                Remit_ConRepo.Employee_Salary = Convert.ToDecimal(employer_Sal_Amt);
                                                if (Remit_ConRepo.Employee_Salary < 0)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid Employee Salary for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Employee Salary for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employer Contribution cannot be null.",
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;

                                        }


                                        //verify Month
                                        if (srda["Month"] != DBNull.Value)
                                        {
                                            string con_month = srda["Month"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(con_month))
                                            {
                                                remit_cont_repo.For_Month = Convert.ToInt32(con_month);
                                                if (remit_cont_repo.For_Month <= 0)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid Month for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Month for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Month cannot be null.",
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;

                                        }

                                        //verify Year
                                        if (srda["Year"] != DBNull.Value)
                                        {
                                            string con_year = srda["Year"].ToString();
                                            if (Microsoft.VisualBasic.Information.IsNumeric(con_year))
                                            {
                                                remit_cont_repo.For_Year = Convert.ToInt32(con_year);
                                                if (remit_cont_repo.For_Year <= 0)
                                                {
                                                    X.Mask.Hide();
                                                    X.Msg.Show(new MessageBoxConfig
                                                    {
                                                        Title = "Error",
                                                        Message = "Invalid Year for " + Remit_ConRepo.Employee_Name,
                                                        Buttons = MessageBox.Button.OK,
                                                        Icon = MessageBox.Icon.ERROR,
                                                        Width = 350
                                                    });
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Year for " + Remit_ConRepo.Employee_Name,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350
                                                });
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Year cannot be null.",
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }


                                        if (remit_cont_repo.For_Month != current_month || remit_cont_repo.For_Year != current_year)
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "The month and year provided in excel must be the same as the selected period.",
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }



                                        ////Sceme_Fund_ID
                                        //string Scheme_fund_id = remit_cont_repo.Scheme_Id + employee_fund_id;
                                        //if (string.IsNullOrEmpty(Scheme_fund_id) == false)
                                        //{

                                        //    //Validate Employee SchemeFundID  to check if its already exist
                                        //    OracleCommand cmd_emp_id = new OracleCommand();
                                        //    cmd_emp_id.CommandText = "sel_pfm_SchemeFundIdExist";
                                        //    cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        //    cmd_emp_id.Connection = (OracleConnection) conn;
                                        //    //Input param
                                        //    cmd_emp_id.Parameters.Add("p_scheme_fund_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_fund_id;
                                        //    //Output param
                                        //    OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                        //    cmd_emp_id.Parameters.Add(count);
                                        //    cmd_emp_id.ExecuteNonQuery();
                                        //    string mtotal = (count.Value).ToString();
                                        //    int total = Convert.ToInt32(mtotal);
                                        //    if (total <= 0)
                                        //    {
                                        //        error_nos += 1;
                                        //        erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + Remit_ConRepo.Employee_Name + Environment.NewLine;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    error_nos += 1;
                                        //    erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + Remit_ConRepo.Employee_Name + Environment.NewLine;
                                        //}

                                        //     //check Employee Scheme Account exist for  contribution or remittance 
                                        /////     var param_ka = new DynamicParameters();
                                        //  ///   decimal emp_numa = 0;
                                        //   ///  param_ka.Add(name: "p_Cust_No", value: cust_noo, dbType: DbType.String, direction: ParameterDirection.Input);
                                        //    /// param_ka.Add(name: "p_Fund_Id", value: employee_fund_id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                                        //     ///param_ka.Add(name: "p_Scheme_Id", value: remit_cont_repo.Scheme_Id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                                        //     param_ka.Add(name: "p_count", value: emp_numa, dbType: DbType.Decimal, direction: ParameterDirection.Output);

                                        //     conn.Execute(sql: "SEL_Crm_Employee_Account_Exit", param: param_ka, commandType: CommandType.StoredProcedure);

                                        //     decimal tot_emppa = param_ka.Get<decimal>("p_count");
                                        //     if (tot_emppa <= 0)
                                        //     {
                                        //         X.Msg.Show(new MessageBoxConfig
                                        //         {
                                        //             Title = "Error",
                                        //             Message = "Employee Scheme Account does not    exist for . - " + remit_cont_repo.Employee_Name  + " " + remit_cont_repo.Fund_Name  + " " + remit_cont_repo.Scheme_Name  + Environment.NewLine + ".Proces aborted",
                                        //             Buttons = MessageBox.Button.OK,
                                        //             Icon = MessageBox.Icon.ERROR,
                                        //             Width = 350

                                        //         });
                                        //         return false;
                                        //     }


                                        //get  scheme rate

                                        // esf_noo = Remit_ConRepo.Employee_Id + Remit_ConRepo.Scheme_Fund_Id;


                                        decimal req_Con = 0;
                                        if (p_Salary_Rate <= 0)
                                        {
                                            req_Con = Remit_ConRepo.Employee_Con + Remit_ConRepo.Employer_Con;
                                        }
                                        else
                                        {
                                            req_Con = decimal.Round(p_Salary_Rate / 100 * Remit_ConRepo.Employee_Salary, 2);
                                        }
                                        
                                        decimal Rate = p_Salary_Rate;
                                        decimal diff_amount = decimal.Round(req_Con - Remit_ConRepo.Employee_Con, 2);
                                        ////string Employee_Scheme_Fund_ID = cust_noo + Scheme_fund_id;

                                        ///check if this esf_id exists------------------------------------------
                                        //Validate Employee SchemeFundID  to check if its already exist
                                        //Get connection

                                        OracleCommand cmd_emp_id_ESFID = new OracleCommand();
                                        cmd_emp_id_ESFID.CommandText = "SEL_CRM_EMPLOYEE_GET_ESFID";
                                        cmd_emp_id_ESFID.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id_ESFID.Connection = (OracleConnection) conn;
                                        //Input param
                                        cmd_emp_id_ESFID.Parameters.Add("p_custNo", OracleDbType.Varchar2, ParameterDirection.Input).Value = cust_noo;
                                        cmd_emp_id_ESFID.Parameters.Add("p_scheme_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = remit_cont_repo.Scheme_Id;
                                        //Output param
                                        OracleParameter count_ESFIDEXISTS = new OracleParameter("p_result", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                                        cmd_emp_id_ESFID.Parameters.Add(count_ESFIDEXISTS);
                                        cmd_emp_id_ESFID.ExecuteNonQuery();
                                        string Employee_Scheme_Fund_ID = (count_ESFIDEXISTS.Value).ToString();
                                        if (Employee_Scheme_Fund_ID == "NA")
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + Remit_ConRepo.Employee_Name + Environment.NewLine;
                                        }
                                        //////////--------------end check---------------------

                                        //if (employee_fund_id == "01")
                                        //{
                                        //    error_nos += 1;
                                        //    erroor_msg = erroor_msg + "Employee cannot subscribe to system fund (01)- " + Remit_ConRepo.Employee_Name + Environment.NewLine;
                                        //}


                                        if (error_nos > 0)
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = erroor_msg,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }

										////check for duplicates

										//var param_ka = new DynamicParameters();
										//decimal emp_numa = 0;
										//param_ka.Add(name: "p_EmployeeId", value: Employee_Scheme_Fund_ID, dbType: DbType.String, direction: ParameterDirection.Input);
										//param_ka.Add(name: "p_month", value: remit_cont_repo.For_Month, dbType: DbType.Int32, direction: ParameterDirection.Input);
										//param_ka.Add(name: "p_year", value: remit_cont_repo.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
										//param_ka.Add(name: "p_contype", value: "SUPPLIMENTARY-CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
										//param_ka.Add(name: "p_result", value: emp_numa, dbType: DbType.Decimal, direction: ParameterDirection.Output);
										//conn.Execute(sql: "SEL_CRM_EMPLOYEEIDEXISTCON", param: param_ka, commandType: CommandType.StoredProcedure);

										//decimal tot_emppa = param_ka.Get<decimal>("p_result");
										//if (tot_emppa > 0)
										//{
										//    X.Mask.Hide();
										//    X.Msg.Show(new MessageBoxConfig
										//    {
										//        Title = "Error",
										//        Message = "Duplicate contribution for   " + Remit_ConRepo.Employee_Id + "-" + srda["EmployeeName"] + Environment.NewLine,
										//        Buttons = MessageBox.Button.OK,
										//        Icon = MessageBox.Icon.ERROR,
										//        Width = 350

										//    });
										//    return false;
										//}

										////

										////check if employee is active
										//var param_kaa = new DynamicParameters();
										//decimal emp_numaa = 0;
										//param_kaa.Add(name: "p_EmployeeId", value: Employee_Scheme_Fund_ID, dbType: DbType.String, direction: ParameterDirection.Input);
										//param_kaa.Add(name: "p_result", value: emp_numaa, dbType: DbType.Decimal, direction: ParameterDirection.Output);
										//conn.Execute(sql: "SEL_CRM_EMPLOYEEIDEXIS_ACT", param: param_kaa, commandType: CommandType.StoredProcedure);

										//decimal tot_emppaa = param_kaa.Get<decimal>("p_result");
										//if (tot_emppaa <= 0)
										//{
										//	X.Mask.Hide();
										//	X.Msg.Show(new MessageBoxConfig
										//	{
										//		Title = "Error",
										//		Message = "Member not Active   " + Remit_ConRepo.Employee_Id + "-" + srda["EmployeeName"] + Environment.NewLine,
										//		Buttons = MessageBox.Button.OK,
										//		Icon = MessageBox.Icon.ERROR,
										//		Width = 350

										//	});
										//	return false;
										//}


										Remit_ConRepo.Employer_Con = Math.Round(Remit_ConRepo.Employer_Con, 2);
                                        Remit_ConRepo.Employee_Con = Math.Round(Remit_ConRepo.Employee_Con, 2);
                                        Remit_ConRepo.Employee_Salary = Math.Round(Remit_ConRepo.Employee_Salary, 2);

                                        param.Add(name: "p_Employee_Id", value: Remit_ConRepo.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_ESF_ID", value: Employee_Scheme_Fund_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Con_Log_Id", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employer_Con", value: Remit_ConRepo.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employee_Con", value: Remit_ConRepo.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employer_Bal", value: Remit_ConRepo.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employee_Bal", value: Remit_ConRepo.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employer_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employee_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Auth_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employee_Salary", value: Remit_ConRepo.Employee_Salary, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employee_Sal_Rate", value: Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Req_Con", value: req_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Difference", value: diff_amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Req_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Con_Type", value: "SUPPLIMENTARY-CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_BatchNo", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                                        int result = conn.Execute(sql: "ADD_REMIT_CON_DETAILS", param: param, commandType: CommandType.StoredProcedure);


                                    }//end if for create_new_or_justacct =1

                                } ///while


                                ts.Complete();

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
                                errormsg = ex.ToString();
                                return false;
                                //throw;
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


                                // '''  ts.Dispose()
                            }


                        }  //end for transscope



                    }

                }
                return true;

            }
            catch (Exception ex_mainbody)
            {
                X.Mask.Hide();
                string rroorr = ex_mainbody.ToString();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ex_mainbody.ToString(),
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350

                });
                return false;
            }
        }

    }
}



