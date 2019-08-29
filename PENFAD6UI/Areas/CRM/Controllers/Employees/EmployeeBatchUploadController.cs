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
    // ===================================
    public class EmployeeBatchUploadController : Controller
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

        public ActionResult AddEmployeeBatchTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeBatchPartial", //"EmployeeBatchPatial",
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
            //var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("file_upload1").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("file_upload1").PostedFile;

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


                    if (BatchEmployeeUpload(ImageWork.Current_Path_For_Other_Files, crmEmployeelog_repo.Employer_Id, crmEmployeelog_repo.Scheme_Id, crmEmployeelog_repo.Employer_Name))
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
                X.Mask.Hide();
                // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }


        }
        public ActionResult Get_EmployersScheme(string status)
        {
            status = "ACTIVE";
            //var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(emp_log_repo.Get_Crm_Employer_SchemeByStatus(status));
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult Get_Employers()
        {
           // var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(employer.GetEmployerData());
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("pn_employeebatchupload");
                x.Reset();
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


        public bool BatchEmployeeUpload(string filePath, string Employer_number, string scheme_id, string Employer_Name)
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

                string query1 = "Select COUNT(*) AS NOS From [Employee$]";
                string query2 = "Select * From [Employee$]";
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
                        tsOp.Timeout = TimeSpan.FromMinutes(60);

                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();

                            try
                            {


                                //check if pending list exist for employer already
                                var param_k = new DynamicParameters();
                                decimal emp_num = 0;
                                param_k.Add(name: "p_employer_id", value: Employer_number, dbType: DbType.String, direction: ParameterDirection.Input);
                                param_k.Add(name: "p_status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
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
                                conn.Execute(sql: "ADD_CRM_EMPLOYEE_BATCH_LOG", param: paramb, commandType: CommandType.StoredProcedure);
                                batchno = paramb.Get<string>("p_BatchNo");
                                if (batchno == "na")
                                {
                                    X.Mask.Hide();
                                    X.Msg.Show(new MessageBoxConfig
                                    {
                                        Title = "Error",
                                        Message = "Employer details cound not be logged.",
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
                                var param_new = new DynamicParameters();
                                //this.GetCmp<Label>("lblload").Text = "Loading";

                                while (srda.Read())
                                {
                                    a_value += 1;

                                    crm_EmployeeRepo employee_Repo = new crm_EmployeeRepo();

                                    string employee_fund_id = "";

                                   
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
                                            employee_Repo.Cust_No = cust_noo;
                                            create_new_or_justacct = 2;                                         
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Employee ID already exist For this employer. - " + employee_Repo.Surname + Environment.NewLine;
                                            //get custNo

                                        }
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Supply Employee ID For " + employee_Repo.First_Name + " " + employee_Repo.Surname,
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                        //    X.Mask.Hide();
                                        //    error_nos += 1;
                                        //    erroor_msg = erroor_msg + "Supply Employee ID For " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
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


                                        /// CHECK IF EMPLOYEE EXIST IN THE SCHEME
                                        OracleCommand cmd_emp_id_EMP = new OracleCommand();
                                        cmd_emp_id_EMP.CommandText = "SEL_PFM_SCHEMEEMPLOYEEEXIST";
                                        cmd_emp_id_EMP.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id_EMP.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id_EMP.Parameters.Add("p_scheme_fund_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_fund_id;
                                        cmd_emp_id_EMP.Parameters.Add("p_cust_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = employeeRepo.Cust_No;

                                        //Output param
                                        OracleParameter count_EMP = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                        cmd_emp_id_EMP.Parameters.Add(count_EMP);
                                        cmd_emp_id_EMP.ExecuteNonQuery();
                                        string mtotal_EMP = (count_EMP.Value).ToString();
                                        int total_EMP = Convert.ToInt32(mtotal_EMP);
                                        if (total_EMP > 0)
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employee with same ID already exist on Scheme" + employee_Repo.Employee_Id + Environment.NewLine,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.INFO,
                                                Width = 350
                                            });

                                            return false;
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Employee with same ID already exist on Scheme. - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }




                                        //DOB   
                                        //if (srda["DateOfBirth"] != DBNull.Value)
                                        //{
                                        //    employee_Repo.Date_Of_Birth = Convert.ToDateTime(srda["DateOfBirth"].ToString());
                                        //}
                                        //else
                                        //{
                                        //    //error_nos += 1;
                                        //    //erroor_msg = erroor_msg + "Date Of Birth is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        //    employee_Repo.Date_Of_Birth = date_default;
                                        //}
                                        //surname
                                        if (srda["Surname"] != DBNull.Value)
                                        {
                                            employee_Repo.Surname = srda["Surname"].ToString();
                                        }
                                        else
                                        {
                                            //X.Mask.Hide();
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Surname cannot be null." + Environment.NewLine;
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Supply Surname For " + employee_Repo.Employee_Id + Environment.NewLine,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.INFO,
                                                Width = 350
                                            });

                                            return false;
                                        }
                                        //FirstName   
                                        if (srda["FirstName"] != DBNull.Value)
                                        {
                                            employee_Repo.First_Name = srda["FirstName"].ToString();
                                        }
                                        else
                                        {
                                            //X.Mask.Hide();
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Supply First Name For " + employee_Repo.Surname + Environment.NewLine;
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Supply First Name For " + employee_Repo.Surname + Environment.NewLine,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.INFO,
                                                Width = 350
                                            });

                                            return false;
                                        }
                                        //Othername   
                                        if (srda["MiddleName"] != DBNull.Value)
                                        {
                                            employee_Repo.Other_Name = srda["MiddleName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Other_Name = "";
                                        }

                                        if (srda["Nationality"] != DBNull.Value)
                                        {
                                            employee_Repo.Nationtionality = srda["Nationality"].ToString();
                                            //validate region
                                            //con = app.GetConnection();
                                            OracleCommand cmd_re = new OracleCommand();
                                            cmd_re.CommandText = "sel_crm_Check_RegionCountry";
                                            cmd_re.CommandType = CommandType.StoredProcedure;
                                            cmd_re.Connection = (OracleConnection)conn;
                                            //Input param
                                            string regionorcountry = ("nationality").ToLower();
                                            string regionorcountryname = employee_Repo.Nationtionality.ToUpper();
                                            cmd_re.Parameters.Add("p_regionorcountry", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountry;
                                            cmd_re.Parameters.Add("p_regionorcountryname", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountryname;
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_re.Parameters.Add(count);
                                            cmd_re.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);

                                            if (total <= 0)
                                            {
                                                //error_nos += 1;
                                                //  erroor_msg = erroor_msg + "Nationality/country not setup for .- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                                employee_Repo.Nationtionality = "NA";
                                            }
                                        }
                                        else
                                        {
                                            ///error_nos += 1;
                                            ///erroor_msg = erroor_msg + "Nationality is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            employee_Repo.Nationtionality = "NA";
                                        }
                                        //FatherFirstName
                                        if (srda["FatherFirstName"] != DBNull.Value)
                                        {
                                            employee_Repo.Father_First_Name = srda["FatherFirstName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Father_First_Name = "NA";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Father's first name is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }
                                        //FatherLastName
                                        if (srda["FatherLastName"] != DBNull.Value)
                                        {
                                            employee_Repo.Father_Last_Name = srda["FatherLastName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Father_First_Name = "NA";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Father's last name is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }
                                        //FatherBirthDate
                                        if (srda["FatherBirthDate"] != DBNull.Value)
                                        {
                                            //DateTime dateofbirth;
                                            if (Microsoft.VisualBasic.Information.IsDate(srda["FatherBirthDate"].ToString()))
                                            {
                                                employee_Repo.Father_Birth_Date = Convert.ToDateTime(srda["FatherBirthDate"].ToString());
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Father's date of birth is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            employee_Repo.Father_Birth_Date = date_default; ;
                                        }
                                        // MotherLastName
                                        if (srda["MotherLastName"] != DBNull.Value)
                                        {
                                            employee_Repo.Mother_Last_Name = srda["MotherLastName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Mother_Last_Name = "NA";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Mother's last name is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        //
                                        // MotherFirstName
                                        if (srda["MotherFirstName"] != DBNull.Value)
                                        {
                                            employee_Repo.Mother_First_Name = srda["MotherFirstName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Mother_Last_Name = "NA";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Mother's first name is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }
                                        if (srda["MotherBirthDate"] != DBNull.Value)
                                        {
                                            //DateTime dateofbirth;

                                            if (Microsoft.VisualBasic.Information.IsDate(srda["MotherBirthDate"].ToString()))
                                            {
                                                employee_Repo.Mother_Birth_Date = Convert.ToDateTime(srda["MotherBirthDate"].ToString());
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Mother's date of birth is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            employee_Repo.Mother_Birth_Date = date_default;
                                        }

                                        if (srda["DateOfBirth"] != DBNull.Value)
                                        {
                                            // DateTime dateofbirth;
                                            if (Microsoft.VisualBasic.Information.IsDate(srda["DateOfBirth"].ToString()))
                                            {
                                                employee_Repo.Date_Of_Birth = Convert.ToDateTime(srda["DateOfBirth"].ToString());
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Date of birth is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            employee_Repo.Date_Of_Birth = date_default;
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Date of birth is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        //SocialSecurityNumber
                                        if (srda["SSNITNumber"] != DBNull.Value)
                                        {
                                            employee_Repo.SSNIT_NO = srda["SSNITNumber"].ToString();
                                            //validate ssnitno
                                            //con = app.GetConnection();
                                            OracleCommand cmd_ssnit = new OracleCommand();
                                            cmd_ssnit.CommandText = "sel_crm_EmployeeSSNITNo";
                                            cmd_ssnit.CommandType = CommandType.StoredProcedure;
                                            cmd_ssnit.Connection = (OracleConnection)conn;

                                            //Input param
                                            OracleParameter empSsnittnum = new OracleParameter("p_SSNITNo", OracleDbType.Varchar2, employee_Repo.SSNIT_NO, ParameterDirection.Input);
                                            cmd_ssnit.Parameters.Add(empSsnittnum);
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_count", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_ssnit.Parameters.Add(count);
                                            cmd_ssnit.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);
                                            if (total > 0)
                                            {
                                                //employee_Repo.SSNIT_NO = "NA";
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "SSNIT No already assigned to someone else.- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                            else
                                            {
                                                employee_Repo.SSNIT_NO = srda["SSNITNumber"].ToString();
                                            }

                                        }
                                        else
                                        {
                                            employee_Repo.SSNIT_NO = "NA";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "SSNIT No is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        if (srda["TINNumber"] != DBNull.Value)
                                        {
                                            employee_Repo.TIN = srda["TINNumber"].ToString();
                                            //validate TIN
                                            OracleCommand cmd_tin = new OracleCommand();
                                            cmd_tin.CommandText = "sel_CRM_ValEMPLOYEETIN";
                                            cmd_tin.CommandType = CommandType.StoredProcedure;
                                            cmd_tin.Connection = (OracleConnection)conn;
                                            //Input param
                                            OracleParameter empTin = new OracleParameter("p_TIN", OracleDbType.Varchar2, employee_Repo.TIN, ParameterDirection.Input);
                                            cmd_tin.Parameters.Add(empTin);
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_count", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_tin.Parameters.Add(count);
                                            cmd_tin.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);

                                            if (total > 0)
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "TIN has already been assigned.- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }

                                        }
                                        else
                                        {
                                            employee_Repo.TIN = "NA";
                                        }
                                        //Region of birth ID
                                        if (srda["BirthRegion"] != DBNull.Value)
                                        {
                                            employee_Repo.Town_Of_Birth_Region = srda["BirthRegion"].ToString();

                                            //validate region
                                            //con = app.GetConnection();
                                            OracleCommand cmd_re = new OracleCommand();
                                            cmd_re.CommandText = "sel_crm_Check_RegionCountry";
                                            cmd_re.CommandType = CommandType.StoredProcedure;
                                            cmd_re.Connection = (OracleConnection)conn;
                                            //Input param
                                            string regionorcountry = ("region").ToLower();
                                            string regionorcountryname = employee_Repo.Town_Of_Birth_Region.ToUpper();
                                            cmd_re.Parameters.Add("p_regionorcountry", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountry;
                                            cmd_re.Parameters.Add("p_regionorcountryname", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountryname;
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_re.Parameters.Add(count);
                                            cmd_re.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);

                                            if (total <= 0)
                                            {
                                                /// error_nos += 1;
                                                /// erroor_msg = erroor_msg + "Birth Region name not setup for .- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                                employee_Repo.Town_Of_Birth_Region = "NA";
                                            }

                                        }
                                        else
                                        {
                                            employee_Repo.Town_Of_Birth_Region = "NA";
                                        }
                                        //resident country
                                        if (srda["ResidentCountry"] != DBNull.Value)
                                        {
                                            employee_Repo.Resident_Country = srda["ResidentCountry"].ToString();
                                            //validate region
                                            //con = app.GetConnection();
                                            OracleCommand cmd_recx = new OracleCommand();
                                            cmd_recx.CommandText = "sel_crm_Check_RegionCountry";
                                            cmd_recx.CommandType = CommandType.StoredProcedure;
                                            cmd_recx.Connection = (OracleConnection)conn;
                                            //Input param
                                            string regionorcountry = ("country").ToLower();
                                            string regionorcountryname = employee_Repo.Resident_Country.ToUpper();
                                            cmd_recx.Parameters.Add("p_regionorcountry", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountry;
                                            cmd_recx.Parameters.Add("p_regionorcountryname", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountryname;
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_recx.Parameters.Add(count);
                                            cmd_recx.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);

                                            if (total <= 0)
                                            {
                                                //error_nos += 1;
                                                //erroor_msg = erroor_msg + "Birth Region name not setup for .- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                                employee_Repo.Resident_Country = "NA";
                                            }
                                        }
                                        else
                                        {
                                            employee_Repo.Resident_Country = "NA";
                                        }
                                        //region
                                        if (srda["ResidentRegion"] != DBNull.Value)
                                        {
                                            employee_Repo.Resident_Region = srda["ResidentRegion"].ToString();
                                            //validate region
                                            //con = app.GetConnection();
                                            OracleCommand cmd_regr = new OracleCommand();
                                            cmd_regr.CommandText = "sel_crm_Check_RegionCountry";
                                            cmd_regr.CommandType = CommandType.StoredProcedure;
                                            cmd_regr.Connection = (OracleConnection)conn;
                                            //Input param
                                            string regionorcountry = ("region").ToLower();
                                            string regionorcountryname = employee_Repo.Resident_Region.ToUpper();
                                            cmd_regr.Parameters.Add("p_regionorcountry", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountry;
                                            cmd_regr.Parameters.Add("p_regionorcountryname", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountryname;
                                            //Output param
                                            OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                            cmd_regr.Parameters.Add(count);
                                            cmd_regr.ExecuteNonQuery();
                                            string mtotal = (count.Value).ToString();
                                            int total = Convert.ToInt32(mtotal);

                                            if (total <= 0)
                                            {
                                                employee_Repo.Resident_Region = "NA";
                                                ///error_nos += 1;
                                               ///// erroor_msg = erroor_msg + "Resident Region name not setup for .- " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            employee_Repo.Resident_Region = "NA";
                                        }
                                        //NextOfKinFirstName
                                        if (srda["NextOfKinFirstName"] != DBNull.Value)
                                        {
                                            nxt.First_Name = srda["NextOfKinFirstName"].ToString();
                                        }
                                        else
                                        {
                                            nxt.First_Name = "";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Next of kin first name is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }
                                        //NextOfKinFirstName
                                        if (srda["NextOfKinMiddleName"] != DBNull.Value)
                                        {
                                            nxt.Other_Name = srda["NextOfKinMiddleName"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Other_Name = "";
                                        }
                                        if (srda["NextOfKinSurname"] != DBNull.Value)
                                        {
                                            nxt.ESurname = srda["NextOfKinSurname"].ToString();
                                        }
                                        else
                                        {
                                            nxt.ESurname = "";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Next of kin surname is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        if (srda["NextOfKinPhoneNumber"] != DBNull.Value)
                                        {
                                            nxt.Phone_Number1 = srda["NextOfKinPhoneNumber"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Phone_Number1 = "";
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Next of kin phone number is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        if (srda["RegistrationDate"] != DBNull.Value)
                                        {
                                            if (Microsoft.VisualBasic.Information.IsDate(srda["RegistrationDate"].ToString()))
                                            {
                                                employee_Repo.Employee_Registration_Date = Convert.ToDateTime(srda["RegistrationDate"].ToString());
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Registration date is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            error_nos += 1;
                                            erroor_msg = erroor_msg + "Registration date is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }

                                        if (error_nos > 0)
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = erroor_msg,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.INFO,
                                                Width = 350

                                            });
                                            return false;
                                        }

                                        //// ########### optional fields #############################################



                                        //Place of birth
                                        if (srda["PlaceOfBirth"] != DBNull.Value)
                                        {
                                            employee_Repo.Town_Of_Birth = srda["PlaceOfBirth"].ToString(); //.ToString("yyyy/MM/dd");
                                        }
                                        else
                                        {
                                            employee_Repo.Town_Of_Birth = "NA";
                                        }
                                        //PlaceofBirthDistrict
                                        if (srda["BirthDistrict"] != DBNull.Value)
                                        {
                                            employee_Repo.Town_Of_Birth_District = srda["BirthDistrict"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Town_Of_Birth_District = "NA";
                                        }

                                        if (srda["BirthCity"] != DBNull.Value)
                                        {
                                            employee_Repo.Town_Of_Birth_City = srda["BirthCity"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Town_Of_Birth_City = "NA";
                                        }
                                        //Hometown
                                        if (srda["HomeTown"] != DBNull.Value)
                                        {
                                            employee_Repo.HomeTown = srda["HomeTown"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.HomeTown = "NA";
                                        }

                                        if (srda["PostalAddress"] != DBNull.Value)
                                        {
                                            employee_Repo.Postal_Address = srda["PostalAddress"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Postal_Address = "NA";
                                        }
                                        if (srda["ResidentialAddress"] != DBNull.Value)
                                        {
                                            employee_Repo.Residential_Address = srda["ResidentialAddress"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Residential_Address = "NA";
                                        }

                                        //district
                                        if (srda["ResidentDistrict"] != DBNull.Value)
                                        {
                                            employee_Repo.Resident_District = srda["ResidentDistrict"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Resident_District = "NA";
                                        }
                                        //city
                                        if (srda["ResidentCity"] != DBNull.Value)
                                        {
                                            employee_Repo.Resident_City = srda["ResidentCity"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Resident_City = "NA";
                                        }
                                        //marital
                                        if (srda["MaritalStatus"] != DBNull.Value)
                                        {
                                            employee_Repo.Marital_Status = srda["MaritalStatus"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Marital_Status = "NA";
                                        }
                                        //  gender
                                        if (srda["Gender"] != DBNull.Value)
                                        {
                                            employee_Repo.Gender = srda["Gender"].ToString().ToUpper();
                                            if (employee_Repo.Gender.StartsWith("F"))
                                            {
                                                employee_Repo.Gender = "FEMALE";
                                            }
                                            else
                                            {
                                                employee_Repo.Gender.StartsWith("M");
                                            }

                                        }
                                        else
                                        {
                                            employee_Repo.Gender = "MALE";
                                        }
                                        //  Employee_Title
                                        if (srda["EmployeeTitle"] != DBNull.Value)
                                        {
                                            employee_Repo.Employee_Title = srda["EmployeeTitle"].ToString().ToUpper();
                                        }
                                        else
                                        {
                                            employee_Repo.Employee_Title = "";
                                        }
                                        //employement date
                                        if (srda["EmploymentDate"] != DBNull.Value)
                                        {
                                            if (Microsoft.VisualBasic.Information.IsDate(srda["EmploymentDate"].ToString()))
                                            {
                                                employee_Repo.Date_Of_Employment = Convert.ToDateTime(srda["EmploymentDate"].ToString());
                                            }
                                            else
                                            {
                                                X.Mask.Hide();
                                                error_nos += 1;
                                                erroor_msg = erroor_msg + "Employment date is required for " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            employee_Repo.Date_Of_Employment = date_default;
                                        }

                                        //father's
                                        if (srda["FatherMiddleName"] != DBNull.Value)
                                        {
                                            employee_Repo.Father_Middle_Name = srda["FatherMiddleName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Father_Middle_Name = "NA";
                                        }

                                        if (srda["MotherMiddleName"] != DBNull.Value)
                                        {
                                            employee_Repo.Father_Middle_Name = srda["MotherMiddleName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Mother_Middle_Name = "NA";
                                        }


                                        if (srda["MotherMaidenLastName"] != DBNull.Value)
                                        {
                                            employee_Repo.Mother_Maiden_Name = srda["MotherMaidenLastName"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Mother_Maiden_Name = "NA";
                                        }


                                        if (srda["PostalAddress"] != DBNull.Value)
                                        {
                                            employee_Repo.Postal_Address = srda["PostalAddress"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Postal_Address = "";
                                        }

                                        if (srda["MobileNo"] != DBNull.Value)
                                        {
                                            employee_Repo.Mobile_Number = srda["MobileNo"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Mobile_Number = "0";
                                        }

                                        //OtherNumber
                                        if (srda["OtherPhoneNos"] != DBNull.Value)
                                        {
                                            employee_Repo.Other_Number = srda["OtherPhoneNos"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Other_Number = "NA";
                                        }
                                        //EmailAddress
                                        if (srda["EmailAddress"] != DBNull.Value)
                                        {
                                            employee_Repo.Email_Address = srda["EmailAddress"].ToString();
                                        }
                                        else
                                        {
                                            employee_Repo.Email_Address = "na";
                                        }

                                        /// Next Of KIN Data

                                        if (srda["NextOfKinFirstName"] != DBNull.Value)
                                        {
                                            nxt.Other_Name = srda["NextOfKinFirstName"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Other_Name = "NA";
                                        }

                                        if (srda["NextOfKinMiddleName"] != DBNull.Value)
                                        {
                                            nxt.Other_Name = srda["NextOfKinMiddleName"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Other_Name = "NA";
                                        }

                                        if (srda["NextOfKinSurname"] != DBNull.Value)
                                        {
                                            nxt.Other_Name = srda["NextOfKinSurname"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Other_Name = "NA";
                                        }
                                        //EmailAddress
                                        if (srda["NextOfKinEmailAddress"] != DBNull.Value)
                                        {
                                            nxt.Email_Address = srda["NextOfKinEmailAddress"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Email_Address = "na";
                                        }

                                        if (srda["NextOfKinAddress"] != DBNull.Value)
                                        {
                                            nxt.Residential_Address = srda["NextOfKinAddress"].ToString();
                                        }
                                        else
                                        {
                                            nxt.Residential_Address = "NA";
                                        }

                                        
                                        employee_Repo.user_Password = employee_Repo.Cust_No + "@" + GetRandomvalue().ToString();
                                        employee_Repo.user_Password = cSecurityRepo.AES_Encrypt(employee_Repo.user_Password);

                                        ////default values
                                        employee_Repo.Identity_Type = "NA";
                                        employee_Repo.Identity_Number = "NA";
                                        employee_Repo.Identity_Issue_Date = GlobalValue.Default_Date_Value;
                                        employee_Repo.Identity_Expiry_Date = GlobalValue.Default_Date_Value;
                                        employee_Repo.Employment_Status = "HIRED";

                                        nxt.Relationship_Name = "NA";
                                        nxt.Date_Of_Birth = GlobalValue.Default_Date_Value;
                                        /////add employee

                                        if (Employer_Name == "PERSONAL PENSIONS")
                                        {

                                            param.Add(name: "p_EmployeeId", value: employee_Repo.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_EmployerId", value: Employer_number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_Title", value: employee_Repo.Employee_Title, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_Surname", value: employee_Repo.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FirstName", value: employee_Repo.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_OtherName", value: employee_Repo.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MaidenName", value: employee_Repo.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_Gender", value: employee_Repo.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_DateofBirth", value: employee_Repo.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_HomeTown", value: employee_Repo.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownOfBirth", value: employee_Repo.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownofBirthDistrict", value: employee_Repo.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownofBirthCity", value: employee_Repo.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownofBirthRegion", value: employee_Repo.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_Nationality", value: employee_Repo.Nationtionality, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_PostalAddress", value: employee_Repo.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_ResAddress", value: employee_Repo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_ResidentCountry", value: employee_Repo.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MobileNumber", value: employee_Repo.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_OtherNumber", value: employee_Repo.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_EmailAddress", value: employee_Repo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_OtherEmail", value: employee_Repo.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_IdentityType", value: employee_Repo.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_IdentityNumber", value: employee_Repo.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_IdentityIssueDate", value: employee_Repo.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_IdentityExpiryDate", value: employee_Repo.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MaritalStatus", value: employee_Repo.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                                            // not included in batch -- param.Add(name: "p_Position", value: employee_Repo.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_EmployeeType", value: employee_Repo.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_DateOfEmployment", value: employee_Repo.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_Cust_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                                            param.Add(name: "p_IndividualBatch", value: "BATCH", dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_BatchNo", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                                            // not included in batch -- param.Add(name: "p_Profession", value: employee_Repo.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_SSNIT", value: employee_Repo.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TIN", value: employee_Repo.TIN, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_FatherLastName", value: employee_Repo.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherFirstName", value: employee_Repo.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherMiddleName", value: employee_Repo.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherBirthDate", value: employee_Repo.Father_Birth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherFirstName", value: employee_Repo.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherLastName", value: employee_Repo.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherMiddleName", value: employee_Repo.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherMaidenName", value: employee_Repo.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherBirthDate", value: employee_Repo.Mother_Birth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherPhoneNumber", value: employee_Repo.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherPhoneNo", value: employee_Repo.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_RegistrationDate", value: employee_Repo.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_Resident_City", value: employee_Repo.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_Resident_District", value: employee_Repo.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_Resident_Region", value: employee_Repo.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "P_User_Password", value: employee_Repo.user_Password, dbType: DbType.String, direction: ParameterDirection.Input);

                                            ////Next Of Kin
                                            param.Add(name: "b_BENEFICIARY_NEXTOFKIN", value: "NEXT OF KIN", dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_SURNAME", value: nxt.ESurname, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_FIRST_NAME", value: nxt.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_OTHER_NAME", value: nxt.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_MAIDEN_NAME", value: nxt.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_PHONE_NUMBER1", value: nxt.Phone_Number1, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_PHONE_NUMBER2", value: nxt.Phone_Number2, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_RESIDENTIAL_ADDRESS", value: nxt.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_EMAIL_ADDRESS", value: nxt.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_RELATIONSHIP", value: nxt.Relationship_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_DATE_OF_BIRTH", value: nxt.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            //param.Add(name: "b_RegistrationDate", value: nxt.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);

                                            ////create scheme fund account for employee
                                            //param.Add(name: "P_ESF_ID", value: ESFRepo.Cust_No + ESFRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_SCHEME_FUND_ID", value: Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_PERSONAL_PENSIONS", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);

                                            conn.Execute(sql: "ADD_CRM_EMPLOYEE_BATCH_P", param: param, commandType: CommandType.StoredProcedure);
                                        }
                                        else
                                        {
                                            param.Add(name: "p_EmployeeId", value: employee_Repo.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_EmployerId", value: Employer_number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_Title", value: employee_Repo.Employee_Title, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_Surname", value: employee_Repo.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FirstName", value: employee_Repo.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_OtherName", value: employee_Repo.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MaidenName", value: employee_Repo.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_Gender", value: employee_Repo.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_DateofBirth", value: employee_Repo.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_HomeTown", value: employee_Repo.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownOfBirth", value: employee_Repo.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownofBirthDistrict", value: employee_Repo.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownofBirthCity", value: employee_Repo.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TownofBirthRegion", value: employee_Repo.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_Nationality", value: employee_Repo.Nationtionality, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_PostalAddress", value: employee_Repo.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_ResAddress", value: employee_Repo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_ResidentCountry", value: employee_Repo.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MobileNumber", value: employee_Repo.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_OtherNumber", value: employee_Repo.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_EmailAddress", value: employee_Repo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_OtherEmail", value: employee_Repo.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_IdentityType", value: employee_Repo.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_IdentityNumber", value: employee_Repo.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_IdentityIssueDate", value: employee_Repo.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_IdentityExpiryDate", value: employee_Repo.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MaritalStatus", value: employee_Repo.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                                            // not included in batch -- param.Add(name: "p_Position", value: employee_Repo.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_EmployeeType", value: employee_Repo.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_DateOfEmployment", value: employee_Repo.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_Cust_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                                            param.Add(name: "p_IndividualBatch", value: "BATCH", dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_BatchNo", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                                            // not included in batch -- param.Add(name: "p_Profession", value: employee_Repo.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_SSNIT", value: employee_Repo.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_TIN", value: employee_Repo.TIN, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_FatherLastName", value: employee_Repo.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherFirstName", value: employee_Repo.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherMiddleName", value: employee_Repo.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherBirthDate", value: employee_Repo.Father_Birth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherFirstName", value: employee_Repo.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherLastName", value: employee_Repo.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherMiddleName", value: employee_Repo.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherMaidenName", value: employee_Repo.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherBirthDate", value: employee_Repo.Mother_Birth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_FatherPhoneNumber", value: employee_Repo.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "p_MotherPhoneNo", value: employee_Repo.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "p_RegistrationDate", value: employee_Repo.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            param.Add(name: "p_Resident_City", value: employee_Repo.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_Resident_District", value: employee_Repo.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_Resident_Region", value: employee_Repo.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                                            param.Add(name: "P_User_Password", value: employee_Repo.user_Password, dbType: DbType.String, direction: ParameterDirection.Input);

                                            ////Next Of Kin
                                            param.Add(name: "b_BENEFICIARY_NEXTOFKIN", value: "NEXT OF KIN", dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_SURNAME", value: nxt.ESurname, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_FIRST_NAME", value: nxt.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_OTHER_NAME", value: nxt.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_MAIDEN_NAME", value: nxt.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_PHONE_NUMBER1", value: nxt.Phone_Number1, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_PHONE_NUMBER2", value: nxt.Phone_Number2, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_RESIDENTIAL_ADDRESS", value: nxt.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_EMAIL_ADDRESS", value: nxt.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_RELATIONSHIP", value: nxt.Relationship_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "b_DATE_OF_BIRTH", value: nxt.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                                            //param.Add(name: "b_RegistrationDate", value: nxt.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);

                                            ////create scheme fund account for employee
                                            //param.Add(name: "P_ESF_ID", value: ESFRepo.Cust_No + ESFRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                            param.Add(name: "P_SCHEME_FUND_ID", value: Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);

                                            conn.Execute(sql: "ADD_CRM_EMPLOYEE_BATCH", param: param, commandType: CommandType.StoredProcedure);
                                        }

                                    } // end for create_new_or_justacct =1


                                    else if (create_new_or_justacct == 2)  /////Just create scheme fund account for employee
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
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Scheme fund does not exist for - " + employee_Repo.Employee_Id + Environment.NewLine,
                                                    Buttons = MessageBox.Button.OK,
                                                    Icon = MessageBox.Icon.ERROR,
                                                    Width = 350


                                                });
                                                return false;
                                                //error_nos += 1;
                                                //erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Scheme fund does not exist for - " + employee_Repo.Employee_Id + Environment.NewLine,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350


                                            });
                                            return false;
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Scheme fund does not exist for . - " + employee_Repo.Employee_Id + Environment.NewLine;
                                        }


                                        //verify Company's EmployeID
                                        //get CustNo for employee
                                        cust_noo = "";
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
                                                employee_Repo.Cust_No = cust_noo;              
                                            }
                                        }
                                        else
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Supply Employee ID For " + employee_Repo.First_Name + " " + employee_Repo.Surname,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                            //    X.Mask.Hide();
                                            //    error_nos += 1;
                                            //    erroor_msg = erroor_msg + "Supply Employee ID For " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }



                                        /// CHECK IF EMPLOYEE EXIST IN THE SCHEME
                                        OracleCommand cmd_emp_id_EMP = new OracleCommand();
                                        cmd_emp_id_EMP.CommandText = "SEL_PFM_SCHEMEEMPLOYEEEXIST";
                                        cmd_emp_id_EMP.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id_EMP.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id_EMP.Parameters.Add("p_scheme_fund_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_fund_id;
                                        cmd_emp_id_EMP.Parameters.Add("p_cust_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = employee_Repo.Cust_No;

                                        //Output param
                                        OracleParameter count_EMP = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                                        cmd_emp_id_EMP.Parameters.Add(count_EMP);
                                        cmd_emp_id_EMP.ExecuteNonQuery();
                                        string mtotal_EMP = (count_EMP.Value).ToString();
                                        int total_EMP = Convert.ToInt32(mtotal_EMP);
                                        if (total_EMP > 0)
                                        {
                                            X.Mask.Hide();
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Employee with same ID already exist on Scheme" + employee_Repo.Employee_Id + Environment.NewLine,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.INFO,
                                                Width = 350
                                            });

                                            return false;
                                            //error_nos += 1;
                                            //erroor_msg = erroor_msg + "Employee with same ID already exist on Scheme. - " + employee_Repo.First_Name + " " + employee_Repo.Surname + Environment.NewLine;
                                        }



                                        //create account now
                                        ////create scheme fund account for employee
                                        decimal successs_1_2 = 0;
                                        string Employee_Scheme_fund_id = cust_noo + Scheme_fund_id;
                                        param_new.Add(name: "p_emp_scheme_fund_id", value: Employee_Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param_new.Add(name: "p_scheme_fund_id", value: Scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param_new.Add(name: "p_custNo", value: cust_noo, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param_new.Add(name: "p_BatchNo", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param_new.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param_new.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                        param_new.Add(name: "p_result", value: successs_1_2, dbType: DbType.Decimal, direction: ParameterDirection.Output);

                                        conn.Execute(sql: "add_crm_remitSchemeFundAcctNew", param: param_new, commandType: CommandType.StoredProcedure);


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