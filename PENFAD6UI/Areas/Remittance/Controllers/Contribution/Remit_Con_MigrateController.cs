
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
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Contribution
{

    public class Remit_Con_MigrateController : Controller
    {
        string plog;
        string clog;
        readonly Remit_Con_Log remitConLogrepo = new Remit_Con_Log();
        readonly Remit_Con_Log_Details remitConLogdetailsrepo = new Remit_Con_Log_Details();
        readonly Remit_Contribution_Upload_LogRepo RemitInitialRepo = new Remit_Contribution_Upload_LogRepo();
        static List<Remit_Con_Log> remitConLogstaticlist = new List<Remit_Con_Log>();
        static List<Remit_Con_Log_Details> remitConLogDetailsStaticlist = new List<Remit_Con_Log_Details>();
        readonly Remit_Contribution_Upload_LogRepo rcul = new Remit_Contribution_Upload_LogRepo();

        public ActionResult AddConMigrationTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_Con_MigrationPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult RemitConMigrationUpload(Remit_Con_Log remit_con_logrepo)
        {
            try
            {

                string extension = string.Empty;
                string file_loc = string.Empty;
                remitConLogstaticlist.Clear();
                remitConLogDetailsStaticlist.Clear();
                if (this.GetCmp<FileUploadField>("Remit_Con_Migration_remitfile_upload1").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("Remit_Con_Migration_remitfile_upload1").PostedFile;

                    extension = Path.GetExtension(file_posted.FileName);

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
                    file_loc = ImageWork.Upload_Any_File_Not_Image(file_posted);
                }

                string consString_excel = "";

                switch (extension)
                {
                    case ".xls":
                        consString_excel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + file_loc + ";Extended Properties=Excel 8.0;HDR=Yes;IMEX=2";

                        break;
                    case ".xlsx":
                        consString_excel = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file_loc + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";

                        break;
                }

                OleDbConnection con_ex = new OleDbConnection();
                OleDbCommand cmd = new OleDbCommand();

                //string query1 = "Select COUNT(*) AS NOS From [SalaryData$]";
                string query2 = "Select * From [SalaryData$]";

                con_ex.ConnectionString = consString_excel;
                con_ex.Open();

                cmd.Connection = con_ex;
                cmd.CommandText = query2;
                OleDbDataReader srda = cmd.ExecuteReader();
                srda.Read();

                remitConLogstaticlist.Clear();
                if (srda.HasRows)
                {

                    int iii = srda.GetSchemaTable()
                            .Rows
                            .OfType<DataRow>()
                            .Count();

                 // DateTime con_date = Convert.ToDateTime("8/1/2018");
                    DateTime con_date = Convert.ToDateTime("10/1/2012");
                    for (int i = 0; i < iii + iii;
                        i++)

                    {
                        con_date = con_date.AddMonths(1);
                        string con_date_string = Convert.ToString(con_date.ToString("MMM-yy"));

                        if (srda.GetSchemaTable()
                            .Rows
                            .OfType<DataRow>()
                            .Any(row => row["ColumnName"].ToString() == con_date_string))
                        {

                            ////////////////////////////////////////////////
                            if ((con_date.ToString("MM")).ToString().Length == 1)
                            {
                                clog = "0" + (con_date.ToString("MM"));
                            }
                            else
                            {
                                clog = (con_date.ToString("MM")).ToString();
                            }
                            ///////////////////////////////////////////////////

                            //Generate con log
                            Remit_Con_Log new_conlog = new Remit_Con_Log();
                            string conlog = "CON" + remit_con_logrepo.ES_Id + Convert.ToInt32(con_date.ToString("yyyy")) + clog + "01";
                            new_conlog.Con_Log_Id = conlog;
                            new_conlog.Employer_Id = remit_con_logrepo.Employer_Id;
                            new_conlog.ES_Id = remit_con_logrepo.ES_Id;
                            new_conlog.For_Month = con_date.Month;
                            new_conlog.For_Year = con_date.Year;

                            remitConLogstaticlist.Add(new_conlog);
                        }

                    }

                    while (srda.Read())
                    {

                        Remit_Con_Log_Details new_conlogdetails = new Remit_Con_Log_Details();
                       
                        if (srda["EmployeeId"] != DBNull.Value)
                        {
                            new_conlogdetails.Employee_Id = srda["EmployeeId"].ToString();
                        }

                        //get esf id for employee 
                        string new_EsfID = rcul.GetRemit_EmployeeSchemeFunds_DataReaderFund(srda["EmployeeId"].ToString(), remit_con_logrepo.Scheme_Id + srda["FUND"].ToString());
                        if (string.IsNullOrEmpty(new_EsfID))
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
                            return this.Direct();
                        }


                        foreach (var conlogcols in remitConLogstaticlist)
                        {
                            DateTime ddd = Convert.ToDateTime(conlogcols.For_Month.ToString() + "/" + conlogcols.For_Year.ToString());
                            string con_date_str = ddd.ToString("MMM-yy");




                            ////////////////////////////////////////////////
                            if (conlogcols.For_Month.ToString().Length == 1)
                            {
                                plog = "0" + conlogcols.For_Month;
                            }
                            else
                            {
                                plog = conlogcols.For_Month.ToString();
                            }
                            ///////////////////////////////////////////////////

                            if (srda[con_date_str] != DBNull.Value && srda[con_date_str].ToString() != "-" && Convert.ToDecimal(srda[con_date_str]) != 0)
                            {
                                Remit_Con_Log_Details new_conlogdetails_Newest = new Remit_Con_Log_Details();
                                new_conlogdetails_Newest.Employee_Id = srda["EmployeeId"].ToString();
                                new_conlogdetails_Newest.Employee_Con = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Con_Log_Id = conlogcols.Con_Log_Id;
                                new_conlogdetails_Newest.Esf_Id = new_EsfID;
                                new_conlogdetails_Newest.Employee_Amt = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Purchase_Log_Id = remit_con_logrepo.ES_Id + conlogcols.For_Year + plog + "01";
                                new_conlogdetails_Newest.ES_Id = remit_con_logrepo.ES_Id;
                                remitConLogDetailsStaticlist.Add(new_conlogdetails_Newest);
                            }
                        }

                    }
                    con_ex.Close();

                }

                TransactionOptions tsOp = new TransactionOptions();
                tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
                tsOp.Timeout = TimeSpan.FromMinutes(12000);
                var app = new AppSettings();

                using (OracleConnection conn = new OracleConnection(app.conString()))  //
                {
                    conn.Open();

                    try
                    {
                        //Push Con Log details
                        foreach (var conlogcols in remitConLogstaticlist)
                        {
                            //string cnl = remitConLogrepo.Create_Con_Log(conlogcols);
                            ////////////////////////////////////////////////
                            if (conlogcols.For_Month.ToString().Length == 1)
                            {
                                clog = "0" + conlogcols.For_Month;
                            }
                            else
                            {
                                clog = conlogcols.For_Month.ToString();
                            }
                            ///////////////////////////////////////////////////

                            ///create log for the upload Remit Log
                            var paramb = new DynamicParameters();
                            string batchno = "";
                            paramb.Add(name: "p_Con_Log_Id ", value: conlogcols.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Employer_Id", value: conlogcols.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_ES_Id", value: conlogcols.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_For_Month", value: clog, dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_For_Year", value: conlogcols.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_DeadLine_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                            paramb.Add(name: "p_BatchNo", value: conlogcols.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                            conn.Execute(sql: "ADD_MIGRATE_ALL_CON_LOG", param: paramb, commandType: CommandType.StoredProcedure);

                            batchno = paramb.Get<string>("p_Con_Log_Id ");
                            //return batchno;
                        }

                        //create purchase log
                        foreach (var conlogcols in remitConLogstaticlist)
                        {
                            // string cnl = remitConLogrepo.PurchaseSaveRecord(conlogcols);
                            ////////////////////////////////////////////////
                            if (conlogcols.For_Month.ToString().Length == 1)
                            {
                                plog = "0" + conlogcols.For_Month;
                            }
                            else
                            {
                                plog = conlogcols.For_Month.ToString();
                            }
                            ///////////////////////////////////////////////////


                            //Get Connection
                            DynamicParameters param = new DynamicParameters();

                            param.Add(name: "P_PURCHASE_LOG_ID", value: conlogcols.ES_Id + conlogcols.For_Year + plog, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "P_CON_LOG_ID", value: conlogcols.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "P_TRANS_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                            param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                            param.Add(name: "p_Purchase_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_BatchNo", value: conlogcols.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                            conn.Execute("ADD_MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);

                        }

                        //create payment logs 
                        //{
                        //    remit_con_logrepo.PaymentSaveRecord(remit_con_logrepo);
                        //}

                        //Push Con Log details
                        foreach (var conlogcolsdetails in remitConLogDetailsStaticlist)
                        {
                            //remitConLogdetailsrepo.Create_Con_Log_Details(conlogcolsdetails);
                            var param = new DynamicParameters();
                            ///create log for the upload Remit Log
                            param.Add(name: "p_Employee_Id", value: conlogcolsdetails.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_ESF_ID", value: conlogcolsdetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Con_Log_Id", value: conlogcolsdetails.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employer_Con", value: conlogcolsdetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employee_Con", value: conlogcolsdetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employer_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employee_Amt_Used", value: conlogcolsdetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                            param.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employee_Salary", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_BatchNo", value: conlogcolsdetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);
                            conn.Execute(sql: "ADD_REMIT_CON_DETAILS_MIG", param: param, commandType: CommandType.StoredProcedure);
                        }

                        //Push Contribution into purchase trans table
                        foreach (var conlogcolsdetails in remitConLogDetailsStaticlist)
                        {
                            //remitConLogdetailsrepo.Create_Unit_Log_Details(conlogcolsdetails);
                            var param = new DynamicParameters();
                            ///create log for the upload Remit Log
                            param.Add(name: "p_ESF_ID", value: conlogcolsdetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employer_Amt", value: conlogcolsdetails.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Employee_Amt", value: conlogcolsdetails.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param.Add(name: "p_Purchase_Log_Id", value: conlogcolsdetails.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_Purchase_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                            param.Add(name: "p_BatchNo", value: conlogcolsdetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                            conn.Execute(sql: "ADD_MIGRATE_CON", param: param, commandType: CommandType.StoredProcedure);
                        }


                        ts.Complete();



                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Employees history remittances uploaded successfully",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        return this.Direct();

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
                return this.Direct();
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
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                //logger.WriteLog(ex.Message);

                return this.Direct();
            }
            finally
            {

            }
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
    }
}