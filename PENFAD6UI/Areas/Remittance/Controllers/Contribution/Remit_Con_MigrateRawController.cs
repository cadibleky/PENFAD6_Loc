
using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Remittance.Contribution;
using Serilog;
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

namespace PENFAD6UI.Areas.Remittance.Controllers.Contribution
{

    public class Remit_Con_MigrateRawController : Controller
    {
     
        readonly Remit_Con_Log remitConLogrepo = new Remit_Con_Log();
        readonly Remit_Con_Log_Details remitConLogdetailsrepo = new Remit_Con_Log_Details();
        readonly Remit_Contribution_Upload_LogRepo RemitInitialRepo = new Remit_Contribution_Upload_LogRepo();
        static List<Remit_Con_Log> remitConLogstaticlist = new List<Remit_Con_Log>();
        static List<Remit_Con_Log_Details> remitConLogDetailsStaticlist = new List<Remit_Con_Log_Details>();
        readonly Remit_Contribution_Upload_LogRepo rcul = new Remit_Contribution_Upload_LogRepo();
        public ActionResult AddEmpMigrationRawTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_Emp_MigrationRawPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddConMigrationRawTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_Con_MigrationRawPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

     

        ////Get Employer  Scheme Account
        //public ActionResult Get_EmployersScheme(string status, string param_employer_id, string param_scheme_id, string param_employer_scheme_id)
        //{
        //    status = "ACTIVE";
        //    var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
        //    try
        //    {
        //        return this.Store(RemitInitialRepo.Get_Crm_Employer_SchemeByStatus(status));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
        //        return this.Direct();
        //    }

        //}

        public ActionResult RemitConMigrationUpload()
        {
           
            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("BeneNext_FileUp").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("BeneNext_FileUp").PostedFile;

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


                    ImageWork.Upload_Any_File_Not_Image(file_posted);

                    if (BatchBeneUpload(ImageWork.Current_Path_For_Other_Files))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Raw data uploaded successfully.",
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

                    var x = X.GetCmp<FormPanel>("frm_raw");
                    x.Reset();

                    return this.Direct();
                }

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




        public bool BatchBeneUpload(string filePath)
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
                         default:
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid file format. Process aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
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

                        Stopwatch sw = new Stopwatch();
                        // Start The StopWatch ...From 000
                        sw.Start();
                        var app = new AppSettings();
                        //IDbConnection con;

                        TransactionOptions tsOp = new TransactionOptions();
                        tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                        TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
                        tsOp.Timeout = TimeSpan.FromMinutes(60);

                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();

                            try
                            {
                                // int error_nos = 0;
                                string erroor_msg = "Error:" + Environment.NewLine;
                                var param = new DynamicParameters();

                                while (srda.Read())
                                {
                                    a_value += 1;

                                    string emp_id;
                                    string surname;
                                    string firstname;
                                    string othername;
                                    string for_month;
                                    string for_year;
                                    decimal employee_con;
                                    decimal employer_con;
                                    string scheme;
                                    string employer;
                                    string SSNIT;



                                    if (srda["ID"] != DBNull.Value)
                                    {
                                        emp_id = srda["ID"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid ID" + srda["SURNAME"].ToString() + "," + srda["FIRSTNAME"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["SURNAME"] != DBNull.Value)
                                    {
                                        surname = srda["SURNAME"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Surname" + srda["ID"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["FIRSTNAME"] != DBNull.Value)
                                    {
                                        firstname = srda["FIRSTNAME"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid firstname" + srda["ID"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["OTHERNAME"] != DBNull.Value)
                                    {
                                        othername = srda["OTHERNAME"].ToString();
                                    }
                                    else
                                    {
                                        othername = "NA";
                                    }

                                    if (srda["MONTH"] != DBNull.Value)
                                    {
                                        for_month = srda["MONTH"].ToString();
                                    }
                                    else
                                    {
                                        for_month = "0";
                                    }

                                    if (srda["YEAR"] != DBNull.Value)
                                    {
                                        for_year = srda["YEAR"].ToString();
                                    }
                                    else
                                    {
                                        for_year = "0";
                                    }


                                    if (srda["SCHEME"] != DBNull.Value)
                                    {
                                        scheme = srda["SCHEME"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid scheme" + srda["SCHEME"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["SSNIT"] != DBNull.Value)
                                    {
                                        SSNIT = srda["SSNIT"].ToString();
                                    }
                                    else
                                    {
                                        SSNIT = "NA";
                                    }

                                    if (srda["EMPLOYER"] != DBNull.Value)
                                    {
                                        employer = srda["EMPLOYER"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Employer" + emp_id,
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }


                                    if (srda["EmployeeCon"] != DBNull.Value)
                                    {
                                        string st_employee_con = srda["EmployeeCon"].ToString();
                                        if (Microsoft.VisualBasic.Information.IsNumeric(st_employee_con))
                                        {
                                            employee_con = Convert.ToDecimal(st_employee_con);
                                            employee_con = Math.Round(employee_con, 2);
                                            if (employee_con < 0)
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Employee Contribution for- " + emp_id,
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
                                                Message = "Invalid Employee Contribution for- " + emp_id,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        employee_con = 0;
                                    }

                                    if (srda["EmployerCon"] != DBNull.Value)
                                    {
                                        string st_employer_con = srda["EmployerCon"].ToString();
                                        if (Microsoft.VisualBasic.Information.IsNumeric(st_employer_con))
                                        {
                                            employer_con = Convert.ToDecimal(st_employer_con);
                                            employer_con = Math.Round(employer_con, 2);
                                            if (employer_con < 0)
                                            {
                                                X.Mask.Hide();
                                                X.Msg.Show(new MessageBoxConfig
                                                {
                                                    Title = "Error",
                                                    Message = "Invalid Employer Contribution for- " + emp_id,
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
                                                Message = "Invalid Employer Contribution for- " + emp_id,
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        employer_con = 0;

                                    }

                                   
                                    var parama = new DynamicParameters();
                                    parama.Add(name: "p_emp_id", value: emp_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_surname", value: surname, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_firstname", value: firstname, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_othername", value: othername, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_for_month", value: for_month, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_for_year", value: for_year, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_employee_con", value: employee_con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_employer_con", value: employer_con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_scheme", value: scheme, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_employer", value: employer, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_SSNIT", value: SSNIT, dbType: DbType.String, direction: ParameterDirection.Input);
                                    conn.Execute(sql: "MIGRATE_RAW_DATA", param: parama, commandType: CommandType.StoredProcedure);


                              
                                }
                                ts.Complete();
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

        ////////////////////

        public ActionResult RemitConMigrationUpload1()
        {

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("BeneNext_FileUp").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("BeneNext_FileUp").PostedFile;

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


                    ImageWork.Upload_Any_File_Not_Image(file_posted);

                    if (BatchBeneUpload1(ImageWork.Current_Path_For_Other_Files))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Raw data uploaded successfully.",
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

                    var x = X.GetCmp<FormPanel>("frm_raw");
                    x.Reset();

                    return this.Direct();
                }

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




        public bool BatchBeneUpload1(string filePath)
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
                    default:
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid file format. Process aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
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

                        Stopwatch sw = new Stopwatch();
                        // Start The StopWatch ...From 000
                        sw.Start();
                        var app = new AppSettings();
                        //IDbConnection con;

                        TransactionOptions tsOp = new TransactionOptions();
                        tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                        TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
                        tsOp.Timeout = TimeSpan.FromMinutes(60);

                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();

                            try
                            {
                                // int error_nos = 0;
                                string erroor_msg = "Error:" + Environment.NewLine;
                                var param = new DynamicParameters();

                                while (srda.Read())
                                {
                                    a_value += 1;

                                    string emp_id;
                                    string surname;
                                    string firstname;
                                    string othername;
                                    //string for_month;
                                    //string for_year;
                                    //decimal employee_con;
                                    //decimal employer_con;
                                    string scheme;
                                    string employer;
                                    string SSNIT;



                                    if (srda["ID"] != DBNull.Value)
                                    {
                                        emp_id = srda["ID"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid ID" + srda["SURNAME"].ToString() + "," + srda["FIRSTNAME"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["SURNAME"] != DBNull.Value)
                                    {
                                        surname = srda["SURNAME"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Surname" + srda["ID"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["FIRSTNAME"] != DBNull.Value)
                                    {
                                        firstname = srda["FIRSTNAME"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid firstname" + srda["ID"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["OTHERNAME"] != DBNull.Value)
                                    {
                                        othername = srda["OTHERNAME"].ToString();
                                    }
                                    else
                                    {
                                        othername = "NA";
                                    }

                                    //if (srda["MONTH"] != DBNull.Value)
                                    //{
                                    //    for_month = srda["MONTH"].ToString();
                                    //}
                                    //else
                                    //{
                                    //    X.Mask.Hide();
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Invalid month" + srda["MONTH"].ToString(),
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.INFO,
                                    //        Width = 350
                                    //    });

                                    //    return false;
                                    //}

                                    //if (srda["YEAR"] != DBNull.Value)
                                    //{
                                    //    for_year = srda["YEAR"].ToString();
                                    //}
                                    //else
                                    //{
                                    //    X.Mask.Hide();
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Invalid year" + srda["YEAR"].ToString(),
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.INFO,
                                    //        Width = 350
                                    //    });

                                    //    return false;
                                    //}


                                    if (srda["SCHEME"] != DBNull.Value)
                                    {
                                        scheme = srda["SCHEME"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid scheme" + srda["SCHEME"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }

                                    if (srda["SSNIT"] != DBNull.Value)
                                    {
                                        SSNIT = srda["SSNIT"].ToString();
                                    }
                                    else
                                    {
                                        SSNIT = "NA";
                                    }

                                    if (srda["EMPLOYER"] != DBNull.Value)
                                    {
                                        employer = srda["EMPLOYER"].ToString();
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Employer" + emp_id,
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });

                                        return false;
                                    }


                                    //if (srda["EmployeeCon"] != DBNull.Value)
                                    //{
                                    //    string st_employee_con = srda["EmployeeCon"].ToString();
                                    //    if (Microsoft.VisualBasic.Information.IsNumeric(st_employee_con))
                                    //    {
                                    //        employee_con = Convert.ToDecimal(st_employee_con);
                                    //        employee_con = Math.Round(employee_con, 2);
                                    //        if (employee_con < 0)
                                    //        {
                                    //            X.Mask.Hide();
                                    //            X.Msg.Show(new MessageBoxConfig
                                    //            {
                                    //                Title = "Error",
                                    //                Message = "Invalid Employee Contribution for- " + emp_id,
                                    //                Buttons = MessageBox.Button.OK,
                                    //                Icon = MessageBox.Icon.ERROR,
                                    //                Width = 350
                                    //            });
                                    //            return false;
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        X.Mask.Hide();
                                    //        X.Msg.Show(new MessageBoxConfig
                                    //        {
                                    //            Title = "Error",
                                    //            Message = "Invalid Employee Contribution for- " + emp_id,
                                    //            Buttons = MessageBox.Button.OK,
                                    //            Icon = MessageBox.Icon.ERROR,
                                    //            Width = 350
                                    //        });
                                    //        return false;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    X.Mask.Hide();
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Employee Contribution not in correct format- " + emp_id,
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.ERROR,
                                    //        Width = 350
                                    //    });
                                    //    return false;
                                    //}

                                    //if (srda["EmployerCon"] != DBNull.Value)
                                    //{
                                    //    string st_employer_con = srda["EmployerCon"].ToString();
                                    //    if (Microsoft.VisualBasic.Information.IsNumeric(st_employer_con))
                                    //    {
                                    //        employer_con = Convert.ToDecimal(st_employer_con);
                                    //        employer_con = Math.Round(employer_con, 2);
                                    //        if (employer_con < 0)
                                    //        {
                                    //            X.Mask.Hide();
                                    //            X.Msg.Show(new MessageBoxConfig
                                    //            {
                                    //                Title = "Error",
                                    //                Message = "Invalid Employer Contribution for- " + emp_id,
                                    //                Buttons = MessageBox.Button.OK,
                                    //                Icon = MessageBox.Icon.ERROR,
                                    //                Width = 350
                                    //            });
                                    //            return false;
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        X.Mask.Hide();
                                    //        X.Msg.Show(new MessageBoxConfig
                                    //        {
                                    //            Title = "Error",
                                    //            Message = "Invalid Employer Contribution for- " + emp_id,
                                    //            Buttons = MessageBox.Button.OK,
                                    //            Icon = MessageBox.Icon.ERROR,
                                    //            Width = 350
                                    //        });
                                    //        return false;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    X.Mask.Hide();
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Employer Contribution not in correct format- " + emp_id,
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.ERROR,
                                    //        Width = 350
                                    //    });
                                    //    return false;

                                    //}


                                    var parama = new DynamicParameters();
                                    parama.Add(name: "p_emp_id", value: emp_id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_surname", value: surname, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_firstname", value: firstname, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_othername", value: othername, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //parama.Add(name: "p_for_month", value: for_month, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //parama.Add(name: "p_for_year", value: for_year, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //parama.Add(name: "p_employee_con", value: employee_con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //parama.Add(name: "p_employer_con", value: employer_con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_scheme", value: scheme, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_employer", value: employer, dbType: DbType.String, direction: ParameterDirection.Input);
                                    parama.Add(name: "p_SSNIT", value: SSNIT, dbType: DbType.String, direction: ParameterDirection.Input);
                                    conn.Execute(sql: "MIGRATE_RAW_EMP", param: parama, commandType: CommandType.StoredProcedure);



                                }
                                ts.Complete();
                            
                             
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

    }
}