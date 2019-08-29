using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
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
    public class Remit_Unit_MigrateController : Controller
    {
        string plog;
        readonly Remit_Unit_MigrateRepo remitConLogdetailsrepo = new Remit_Unit_MigrateRepo();
        static List<Remit_Unit_MigrateRepo> remitConLogDetailsStaticlist = new List<Remit_Unit_MigrateRepo>();
        readonly Remit_Contribution_Upload_LogRepo rcul = new Remit_Contribution_Upload_LogRepo();
        readonly Remit_Contribution_Upload_LogRepo RemitInitialRepo = new Remit_Contribution_Upload_LogRepo();

        public ActionResult AddUnitMigrationTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_Unit_MigrationPartial",
                //Model = remitMig2,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult Index()
        {
            return View();
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
        public ActionResult RemitConMigrationUpload(Remit_Unit_MigrateRepo remitConLogdetailsrepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("Remit_Unit_Migration_remitfile_upload1").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("Remit_Unit_Migration_remitfile_upload1").PostedFile;

                    string extension = Path.GetExtension(file_posted.FileName);

                    if (extension != ".xlsx" && extension != ".xls")
                    {
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

                    if (BatchUnitUpload(ImageWork.Current_Path_For_Other_Files, remitConLogdetailsrepo))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Employee Cut-Off Units uploaded successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();
                    }


                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a file to upload.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });

                    var x = X.GetCmp<FormPanel>("Remit_Unit_MigrationFormPanel");
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
        
        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("Remit_Unit_MigrationFormPanel");
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


        public bool BatchUnitUpload(string filePath, Remit_Unit_MigrateRepo remitConLogdetailsrepo)
        {
            //try
            //{

            //    if (System.IO.File.Exists(filePath) == false)
            //    {
            //        X.Msg.Show(new MessageBoxConfig
            //        {
            //            Title = "Error",
            //            Message = "File does not exist.",
            //            Buttons = MessageBox.Button.OK,
            //            Icon = MessageBox.Icon.ERROR,
            //            Width = 350

            //        });
            //        return false;
            //    }
            //    //'get file extension
            //    string file_ext = Path.GetExtension(filePath);

            //    string consString_excel = "";

            //    switch (file_ext)
            //    {
            //        case ".xls":
            //            consString_excel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;HDR=Yes;IMEX=2";

            //            break;
            //        case ".xlsx":
            //            consString_excel = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";

            //            break;
            //    }

            //    OleDbConnection con_ex = new OleDbConnection();
            //    OleDbCommand cmd = new OleDbCommand();

            //    string query1 = "Select COUNT(*) AS NOS From [SalaryUnit$]";
            //    string query2 = "Select * From [SalaryUnit$]";
            //    int totalsum = 1;

            //    con_ex.ConnectionString = consString_excel;
            //    con_ex.Open();

            //    cmd.Connection = con_ex;
            //    cmd.CommandText = query1;

            //    totalsum = Convert.ToInt32(cmd.ExecuteScalar()); //();
            //    con_ex.Close();

            //    if (con_ex.State == ConnectionState.Closed)
            //    {
            //        con_ex.Open();
            //    }



            //    if (con_ex.State == ConnectionState.Open)
            //    {
            //        cmd.Connection = con_ex;
            //        cmd.CommandText = query2;
            //        OleDbDataReader srda = cmd.ExecuteReader();


            //        if (srda.HasRows)
            //        {
            //            string errormsg = "";

            //            //Stopwatch sw = new Stopwatch();
            //            // Start The StopWatch ...From 000
            //            //  sw.Start();

            //            TransactionOptions tsOp = new TransactionOptions();
            //            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            //            tsOp.Timeout = TimeSpan.FromMinutes(100);
            //            var app = new AppSettings();

            //            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            //            {
            //                conn.Open();
            //                // int error_nos = 0;
            //                string erroor_msg = "Error:" + Environment.NewLine;
            //                var param = new DynamicParameters();

            //                while (srda.Read())
            //                {

            //                    if (srda["EmployeeId"] != DBNull.Value)
            //                    {
            //                        remitConLogdetailsrepo.Employee_Id = srda["EmployeeId"].ToString();
            //                    }

            //                    //Get connection
            //                    //var paramw = new DynamicParameters();
            //                    //paramw.Add("P_EMP_ID", srda["EmployeeId"].ToString(), DbType.String, ParameterDirection.Input);
            //                    //paramw.Add("P_EMPLOYER_ID", remitConLogdetailsrepo.Employer_Id, DbType.String, ParameterDirection.Input);
            //                    //paramw.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
            //                    //conn.Execute("SEL_EMPLOYER_EMP_EXIST", paramw, commandType: CommandType.StoredProcedure);
            //                    //int paramoptionw = paramw.Get<int>("VDATA");

            //                    //if (paramoptionw <= 0)
            //                    //{
            //                    //    X.Msg.Show(new MessageBoxConfig
            //                    //    {
            //                    //        Title = "Error",
            //                    //        Message = srda["EmployeeId"].ToString() + " - not registered under the select employer. Process aborted",
            //                    //        Buttons = MessageBox.Button.OK,
            //                    //        Icon = MessageBox.Icon.INFO,
            //                    //        Width = 350
            //                    //    });
            //                    //    return false;
            //                    //}

            //                    //else

            //                    ////////////////////////////////////////////////
            //                    if (remitConLogdetailsrepo.For_Month.ToString().Length == 1)
            //                    {
            //                        plog = "0" + remitConLogdetailsrepo.For_Month;
            //                    }
            //                    else
            //                    {
            //                        plog = remitConLogdetailsrepo.For_Month.ToString();
            //                    }
            //                    ///////////////////////////////////////////////////

            //                    //foreach (var conlogcols in remitConLogDetailsStaticlist)
            //                    {
            //                        remitConLogdetailsrepo.Purchase_Log_Id = remitConLogdetailsrepo.ES_Id + remitConLogdetailsrepo.For_Year + plog + "01";

            //                        // check if purchase log exist in Purchase log table
            //                        var paramP = new DynamicParameters();
            //                        paramP.Add("P_PURCHASE_LOG_ID", remitConLogdetailsrepo.Purchase_Log_Id, DbType.String, ParameterDirection.Input);
            //                        paramP.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
            //                        conn.Execute("SEL__MIG_PURCHASE_ID", paramP, commandType: CommandType.StoredProcedure);
            //                        int paramoption = paramP.Get<int>("VDATA");

            //                        if (paramoption <= 0)
            //                        {
            //                            X.Msg.Show(new MessageBoxConfig
            //                            {
            //                                Title = "Error",
            //                                Message = "Sorry! Invalid selected Month/Year.",
            //                                Buttons = MessageBox.Button.OK,
            //                                Icon = MessageBox.Icon.INFO,
            //                                Width = 350
            //                            });
            //                            return false;
            //                        }


            //                        remitConLogdetailsrepo.Employee_Units = Convert.ToDecimal(srda["EmployeeUnit"]);
            //                        remitConLogdetailsrepo.Employer_Units = Convert.ToDecimal(srda["EmployerUnit"]);

            //                        remitConLogdetailsrepo.Esf_Id = "";
            //                        //get esf id for employee 
            //                        var paramE = new DynamicParameters();
            //                        paramE.Add("p_Employee_Id", remitConLogdetailsrepo.Employee_Id, DbType.String, ParameterDirection.Input);
            //                        paramE.Add("p_Scheme_Fund_Id", remitConLogdetailsrepo.Scheme_Id + srda["FUND"].ToString(), DbType.String, ParameterDirection.Input);
            //                        paramE.Add("p_result", "", DbType.String, ParameterDirection.Output);
            //                        conn.Execute("SEL_MIGRATE_ESF", paramE, commandType: CommandType.StoredProcedure);
            //                        remitConLogdetailsrepo.Esf_Id = paramE.Get<string>("p_result");

            //                        var paramA = new DynamicParameters();
            //                        paramA.Add("P_SFID", remitConLogdetailsrepo.Scheme_Id + srda["FUND"].ToString(), DbType.String, ParameterDirection.Input);
            //                        paramA.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
            //                        conn.Execute("SEL_UNIT_PRICE_MIG", paramA, commandType: CommandType.StoredProcedure);
            //                        decimal u_price = paramA.Get<decimal>("VDATA");

            //                        if (string.IsNullOrEmpty(remitConLogdetailsrepo.Esf_Id))
            //                        {
            //                            X.Msg.Show(new MessageBoxConfig
            //                            {
            //                                Title = "Error",
            //                                Message = "Problem with excel (" + srda["SURNAME"].ToString() + " " + srda["MIDDLENAME"].ToString() + " " + srda["FirstName"].ToString() + ")",
            //                                Buttons = MessageBox.Button.OK,
            //                                Icon = MessageBox.Icon.INFO,
            //                                Width = 350
            //                            });
            //                            con_ex.Close();
            //                            return false;
            //                        }

            //                        // check if purchase already exist for employee
            //                        var paramPe = new DynamicParameters();
            //                        paramPe.Add("P_PURCHASE_LOG_ID", remitConLogdetailsrepo.Purchase_Log_Id, DbType.String, ParameterDirection.Input);
            //                        paramPe.Add("P_ESF_ID", remitConLogdetailsrepo.Esf_Id, DbType.String, ParameterDirection.Input);
            //                        paramPe.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
            //                        conn.Execute("SEL__MIG_PURCHASE_ID_EMPCHE", paramPe, commandType: CommandType.StoredProcedure);
            //                        int paramoptione = paramPe.Get<int>("VDATA");

            //                        if (paramoptione > 0)
            //                        {
            //                            X.Msg.Show(new MessageBoxConfig
            //                            {
            //                                Title = "Error",
            //                                Message = "Sorry unit already migrated for employee with the Staff ID - " + remitConLogdetailsrepo.Employee_Id,
            //                                Buttons = MessageBox.Button.OK,
            //                                Icon = MessageBox.Icon.INFO,
            //                                Width = 350
            //                            });
            //                            return false;
            //                        }


            //                        if (remitConLogdetailsrepo.Employee_Units > 0 || remitConLogdetailsrepo.Employer_Units > 0)
            //                        {
            //                            param.Add(name: "p_PURCHASE_LOG_ID", value: remitConLogdetailsrepo.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
            //                            param.Add(name: "p_ESF_ID", value: remitConLogdetailsrepo.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
            //                            param.Add(name: "p_Employer_Units", value: remitConLogdetailsrepo.Employer_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
            //                            param.Add(name: "p_Employee_Units", value: remitConLogdetailsrepo.Employee_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
            //                            param.Add(name: "p_Purchase_Type", value: "UNIT BF", dbType: DbType.String, direction: ParameterDirection.Input);
            //                            param.Add(name: "p_Unit_Price", value: u_price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
            //                            param.Add(name: "p_ES_ID", value: remitConLogdetailsrepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);

            //                            conn.Execute("MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);
            //                        }

            //                    }
            //                    ts.Complete();
            //                }

            //            }

            //        }  //end for transscope



            //    }
            //    return true;
            //}
            //catch (Exception ex_mainbody)
            //{
            //    string rroorr = ex_mainbody.ToString();
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Error",
            //        Message = ex_mainbody.ToString(),
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.ERROR,
            //        Width = 350

            //    });
            //    return false;
            //}
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

                string query1 = "Select COUNT(*) AS NOS From [SalaryUnit$]";
                string query2 = "Select * From [SalaryUnit$]";
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
                        tsOp.Timeout = TimeSpan.FromMinutes(20);

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
                                    if (srda["EmployeeId"] != DBNull.Value)
                                    {
                                        remitConLogdetailsrepo.Employee_Id = srda["EmployeeId"].ToString();
                                    }



                                    ////////////////////////////////////////////////
                                    if (remitConLogdetailsrepo.For_Month.ToString().Length == 1)
                                    {
                                        plog = "0" + remitConLogdetailsrepo.For_Month;
                                    }
                                    else
                                    {
                                        plog = remitConLogdetailsrepo.For_Month.ToString();
                                    }
                                    ///////////////////////////////////////////////////


                                    remitConLogdetailsrepo.Purchase_Log_Id = remitConLogdetailsrepo.ES_Id + remitConLogdetailsrepo.For_Year + plog + "01";

                                    // check if purchase log exist in Purchase log table
                                    var paramP = new DynamicParameters();
                                    paramP.Add("P_PURCHASE_LOG_ID", remitConLogdetailsrepo.Purchase_Log_Id, DbType.String, ParameterDirection.Input);
                                    paramP.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                                    conn.Execute("SEL__MIG_PURCHASE_ID", paramP, commandType: CommandType.StoredProcedure);
                                    int paramoption = paramP.Get<int>("VDATA");

                                    if (paramoption <= 0)
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Sorry! Invalid selected Month/Year.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });
                                        return false;
                                    }


                                    remitConLogdetailsrepo.Employee_Units = Convert.ToDecimal(srda["EmployeeUnit"]);
                                    remitConLogdetailsrepo.Employer_Units = Convert.ToDecimal(srda["EmployerUnit"]);

                                    remitConLogdetailsrepo.Esf_Id = "";
                                    //get esf id for employee 
                                    var paramE = new DynamicParameters();
                                    paramE.Add("p_Employee_Id", remitConLogdetailsrepo.Employee_Id, DbType.String, ParameterDirection.Input);
                                    paramE.Add("p_Scheme_Fund_Id", remitConLogdetailsrepo.Scheme_Id + srda["FUND"].ToString(), DbType.String, ParameterDirection.Input);
                                    paramE.Add("p_result", "", DbType.String, ParameterDirection.Output);
                                    conn.Execute("SEL_MIGRATE_ESF", paramE, commandType: CommandType.StoredProcedure);
                                    remitConLogdetailsrepo.Esf_Id = paramE.Get<string>("p_result");

                                    var paramA = new DynamicParameters();
                                    paramA.Add("P_SFID", remitConLogdetailsrepo.Scheme_Id + srda["FUND"].ToString(), DbType.String, ParameterDirection.Input);
                                    paramA.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                                    conn.Execute("SEL_UNIT_PRICE_MIG", paramA, commandType: CommandType.StoredProcedure);
                                    decimal u_price = paramA.Get<decimal>("VDATA");

                                    if (string.IsNullOrEmpty(remitConLogdetailsrepo.Esf_Id))
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Problem with excel (" + srda["SURNAME"].ToString() + " " + srda["MIDDLENAME"].ToString() + " " + srda["FirstName"].ToString() + ")",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });
                                        con_ex.Close();
                                        return false;
                                    }

                                    //// check if purchase already exist for employee
                                    //var paramPe = new DynamicParameters();
                                    //paramPe.Add("P_PURCHASE_LOG_ID", remitConLogdetailsrepo.Purchase_Log_Id, DbType.String, ParameterDirection.Input);
                                    //paramPe.Add("P_ESF_ID", remitConLogdetailsrepo.Esf_Id, DbType.String, ParameterDirection.Input);
                                    //paramPe.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                                    //conn.Execute("SEL__MIG_PURCHASE_ID_EMPCHE", paramPe, commandType: CommandType.StoredProcedure);
                                    //int paramoptione = paramPe.Get<int>("VDATA");

                                    //if (paramoptione > 0)
                                    //{
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Sorry unit already migrated for employee with the Staff ID - " + remitConLogdetailsrepo.Employee_Id,
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.INFO,
                                    //        Width = 350
                                    //    });
                                    //    return false;
                                    //}


                                    if (remitConLogdetailsrepo.Employee_Units > 0 || remitConLogdetailsrepo.Employer_Units > 0)
                                    {
                                        param.Add(name: "p_PURCHASE_LOG_ID", value: remitConLogdetailsrepo.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_ESF_ID", value: remitConLogdetailsrepo.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employer_Units", value: remitConLogdetailsrepo.Employer_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Employee_Units", value: remitConLogdetailsrepo.Employee_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Purchase_Type", value: "UNIT BF", dbType: DbType.String, direction: ParameterDirection.Input);
                                        param.Add(name: "p_Unit_Price", value: u_price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                        param.Add(name: "p_ES_ID", value: remitConLogdetailsrepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                                        conn.Execute("MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);
                                    }



                                    
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
                throw ex_mainbody;
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


    } //end class
}