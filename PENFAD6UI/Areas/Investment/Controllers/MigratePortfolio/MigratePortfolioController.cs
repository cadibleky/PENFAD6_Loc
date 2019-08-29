
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using System.Text;
using PENFAD6DAL.Repository.GL;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System.IO;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Investment.Bond;
using System.Data.OleDb;
using System.Data;
using System.Transactions;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.Diagnostics;
using PENFAD6DAL.DbContext;
using Serilog;
using PENFAD6DAL.Repository.Investment.Equity_CIS;

namespace PENFAD6UI.Areas.Investment.Controllers.MigratePortfolio
{
    public class MigratePortfolioController : Controller
    {
       // int counter = 0;
        readonly GLInitialRepo GLRepo = new GLInitialRepo();
        readonly GLAccountRepo GLARepo = new GLAccountRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddMigrateMTBTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "MigrateMM_TBill_BondPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddMigrateECTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "MigrateECPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }


        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmMigrateMTB");
                x.Reset();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void ClearControls_EC()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmMigrateEC");
                x.Reset();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult UploadMTB(BondRepo bondRepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("file_upload_MigrateMTB").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("file_upload_MigrateMTB").PostedFile;

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

                    if (string.IsNullOrEmpty(bondRepo.Scheme_Fund_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Scheme Account.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        return this.Direct();
                    }
                   
                    ImageWork.Upload_Any_File_Not_Image(file_posted);
                     

                    if (MTBUpload(ImageWork.Current_Path_For_Other_Files, bondRepo.Scheme_Fund_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Portfolio migrated successfully.",
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
               // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }
        }

        public bool MTBUpload(string filePath, string Scheme_Fund_Id)
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

                string query1 = "Select COUNT(*) AS NOS From [MTB$]";
                string query2 = "Select * From [MTB$]";
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
                                while (srda.Read())
                                {
                                    a_value += 1;

                                    BondRepo bondRepo = new BondRepo();

                                    bondRepo.Contract_No = srda["ContractNumber"].ToString();

                                    bondRepo.Class_Id = srda["AssetClassID"].ToString();

                                    if (srda["ProductID"] != DBNull.Value)
                                    {
                                        bondRepo.Product_Id = srda["ProductID"].ToString();
                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_PRODUCTIDEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_ProductId", OracleDbType.Varchar2, ParameterDirection.Input).Value = bondRepo.Product_Id;
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
                                                Message = "Wrong Product Id.Process aborted-" + srda["ProductID"],
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
                                            Message = "Product Id is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }

                                    if (srda["IssuerID"] != DBNull.Value)
                                    {
                                        bondRepo.Issuer_Id = srda["IssuerID"].ToString();
                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_ISSUERIDEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_IssuerId", OracleDbType.Varchar2, ParameterDirection.Input).Value = bondRepo.Issuer_Id;
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
                                                Message = "Wrong Issuer Id.Process aborted",
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
                                            Message = "Issuer Id is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }

                                    if (srda["FundManagerID"] != DBNull.Value)
                                    {
                                        bondRepo.Fund_Manager_Id = srda["FundManagerID"].ToString();
                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_FMDEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_IssuerId", OracleDbType.Varchar2, ParameterDirection.Input).Value = bondRepo.Fund_Manager_Id;
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
                                                Message = "Wrong Fund Manager Id.Process aborted",
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
                                            Message = "Fund Manager Id is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }

                                    if (srda["AmountInvested"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["AmountInvested"]) == false)                                   
                                    {

                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Amount Invested.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }
                                    else
                                    //if (srda["AmountInvested"] != DBNull.Value)
                                    {
                                        bondRepo.Amount_Invested = Convert.ToDecimal(srda["AmountInvested"]);
                                    }

                                    if (srda["DuraitonInDays"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["DuraitonInDays"]) == false)
                                    {

                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Duration In Days.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }
                                    else
                                    //if (srda["DuraitonInDays"] != DBNull.Value)
                                    {
                                        bondRepo.Duration_In_Days = Convert.ToDecimal(srda["DuraitonInDays"]);
                                    }

                                    if (srda["AnnualInterestRate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["AnnualInterestRate"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Annual Interest Rate.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    //if (srda["AnnualInterestRate"] != DBNull.Value)
                                    {
                                        bondRepo.Annual_Int_Rate = Convert.ToDecimal(srda["AnnualInterestRate"]);
                                    }

                                    if (Microsoft.VisualBasic.Information.IsDate(srda["StartDate"].ToString()))
                                    {
                                        bondRepo.Start_Date = Convert.ToDateTime(srda["StartDate"].ToString());
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Start Date.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (Microsoft.VisualBasic.Information.IsDate(srda["EndDate"].ToString()))
                                    {
                                        bondRepo.Maturity_Date = Convert.ToDateTime(srda["EndDate"].ToString());
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid End Date.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (srda["DailyInterestRate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["DailyInterestRate"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Daily Interest Rate.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Daily_Int_Rate = Convert.ToDecimal(srda["DailyInterestRate"]);
                                    }
                                    // bondRepo.Daily_Int_Rate = ((bondRepo.Annual_Int_Rate/100)/bondRepo.Duration_In_Days);


                                    if (srda["InterestOnMaturity"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["InterestOnMaturity"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Interest on maturity.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Interest_On_Maturity = Convert.ToDecimal(srda["InterestOnMaturity"]);
                                    }

                                    if (srda["InterestDayBasis"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["InterestDayBasis"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Inavalid Interest Day Basis.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Interest_Day_Basic = Convert.ToDecimal(srda["InterestDayBasis"]);
                                    }


                                    if (srda["PrincipalBalanceAsAtCutOffDate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["PrincipalBalanceAsAtCutOffDate"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Principal Balance.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Principal_Bal = Convert.ToDecimal(srda["PrincipalBalanceAsAtCutOffDate"]);
                                    }

                                    //if (srda["InterestBalanceAsAtCutOffDate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["InterestBalanceAsAtCutOffDate"]) == false)
                                    //{
                                    //    X.Mask.Hide();
                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Invalid Interest Balance.Process aborted",
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.ERROR,
                                    //        Width = 350
                                    //    });
                                    //    return false;
                                    //}
                                    //else
                                    //{
                                    //    bondRepo.Interest_Bal = Convert.ToDecimal(srda["InterestBalanceAsAtCutOffDate"]);
                                    //}

                                    if (srda["PrincipalPaidAsAtCutOffDate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["PrincipalPaidAsAtCutOffDate"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Principal Paid.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Principal_Paid = Convert.ToDecimal(srda["PrincipalPaidAsAtCutOffDate"]);
                                    }

                                    if (srda["InterestPaidAsAtCutOffDate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["InterestPaidAsAtCutOffDate"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Interest Paid.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Interest_Paid = Convert.ToDecimal(srda["InterestPaidAsAtCutOffDate"]);
                                    }


                                    if (srda["InterestAccruedAsAtCutOffDate"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["InterestAccruedAsAtCutOffDate"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Interest Accrued.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.BF_ACCRUED_INTEREST = Convert.ToDecimal(srda["InterestAccruedAsAtCutOffDate"]);
                                    }

                                    if (srda["CostOfBond"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["InterestAccruedAsAtCutOffDate"]) == false)
                                    {
                                        bondRepo.Cost = Convert.ToDecimal(srda["AmountInvested"]);
                                    }
                                    else
                                    {
                                        bondRepo.Cost = Convert.ToDecimal(srda["CostOfBond"]);
                                    }

                                    if (Microsoft.VisualBasic.Information.IsDate(srda["LastPaymentDate"].ToString()))
                                    {
                                        bondRepo.Last_Coupon_Payment_Date = Convert.ToDateTime(srda["LastPaymentDate"].ToString());
                                    }
                                     else  if (Convert.ToDecimal(srda["InterestPaidAsAtCutOffDate"]) > 0 && !Microsoft.VisualBasic.Information.IsDate(srda["LastPaymentDate"].ToString()))
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Last Coupon Payment Date.Process aborted-" + bondRepo.Cost,
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        bondRepo.Last_Coupon_Payment_Date = bondRepo.Start_Date;

                                    }

                                    if (Microsoft.VisualBasic.Information.IsDate(srda["SettlementDate"].ToString()))
                                    {
                                        bondRepo.Settlement_Date = Convert.ToDateTime(srda["SettlementDate"].ToString());
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Settlement Date.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    var param = new DynamicParameters();

                                    string NKRollOver = "DO NOT ROLLOVER";
                                    string ff = srda["ProductID"].ToString();
                                    //string gl_status_new = "PENDING".ToString().ToUpper();
                                    param.Add(name: "p_Invest_No", value: bondRepo.Product_Id + Scheme_Fund_Id + bondRepo.Start_Date.ToString("ddMMyy"), dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Contract_No", value: bondRepo.Contract_No, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Class_Id", value: bondRepo.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Product_Id", value: bondRepo.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Amount_Invested", value: bondRepo.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Duration_In_Days", value: bondRepo.Duration_In_Days, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Annual_Int_Rate", value: bondRepo.Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Daily_Int_Rate", value: bondRepo.Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Start_Date", value: bondRepo.Start_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Maturity_Date", value: bondRepo.Maturity_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Expected_Int", value: bondRepo.Interest_On_Maturity, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Issur_Id", value: bondRepo.Issuer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Interest_Day_Basic", value: bondRepo.Interest_Day_Basic, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_ROLLOVER_INST_YES_NO ", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Rollover_Instrution", value: NKRollOver, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Principal_Bal", value: bondRepo.Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Interest_Bal", value: bondRepo.Interest_On_Maturity - (bondRepo.Interest_Paid), dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Principal_Paid", value: bondRepo.Principal_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Interest_Paid", value: bondRepo.Interest_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Last_Date_Int_Accrued", value: "02/28/2019", dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Next_Coupon_Payment_Date", value: bondRepo.Last_Coupon_Payment_Date.AddDays(182), dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                    param.Add(name: "p_NeworBF_Status", value: "BF", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Payment_Confirmed", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Amount_on_Maturity", value: bondRepo.Amount_Invested + bondRepo.Interest_On_Maturity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Interest_Type", value: bondRepo.Interest_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Trans_CuCode", value: 1, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Base_CusCode", value: 1, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Invest_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Make_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Auth_Id", value: bondRepo.Auth_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Fund_Manager_Id", value: bondRepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_GL_Account_No", value: "1010101001", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Cost", value: bondRepo.Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_InterestAccrued", value: bondRepo.BF_ACCRUED_INTEREST, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Bond_Type", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Bond_Class", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "p_Settlement_Date", value: bondRepo.Settlement_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                    int result = conn.Execute(sql: "MIGRATE_INVEST_TRANS_FIX", param: param, commandType: CommandType.StoredProcedure);
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

        public ActionResult UploadEC(Invest_Equity_CISRepo CISRepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("file_upload_MigrateEC").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("file_upload_MigrateEC").PostedFile;

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

                    if (string.IsNullOrEmpty(CISRepo.Scheme_Fund_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Scheme Account.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        return this.Direct();
                    }

                    ImageWork.Upload_Any_File_Not_Image(file_posted);


                    if (ECUpload(ImageWork.Current_Path_For_Other_Files, CISRepo.Scheme_Fund_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Portfolio migrated successfully.",
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
                // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }
        }

        public bool ECUpload(string filePath, string Scheme_Fund_Id)
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

                string query1 = "Select COUNT(*) AS NOS From [EC$]";
                string query2 = "Select * From [EC$]";
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
                        tsOp.Timeout = TimeSpan.FromMinutes(20);

                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();

                            try
                            {
                                while (srda.Read())
                                {
                                    a_value += 1;

                                    Invest_Equity_CISRepo CISRepo = new Invest_Equity_CISRepo();

                                    CISRepo.Invest_Description = srda["Description"].ToString();

                                    CISRepo.Class_Id = srda["AssetClassID"].ToString();

                                    if (srda["ProductID"] != DBNull.Value)
                                    {
                                        CISRepo.Product_Id = srda["ProductID"].ToString();
                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_PRODUCTIDEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_ProductId", OracleDbType.Varchar2, ParameterDirection.Input).Value = CISRepo.Product_Id;
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
                                                Message = "Wrong Product Id.Process aborted -"+  srda["ProductID"],
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
                                            Message = "Product Id is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }

                                    if (srda["FundManagerID"] != DBNull.Value)
                                    {
                                        CISRepo.Fund_Manager_Id = srda["FundManagerID"].ToString();
                                        //Validate EmployeeID against Employer to check if its already exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_FMDEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_FM_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = CISRepo.Fund_Manager_Id;
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
                                                Message = "Wrong Fund Manager Id.Process aborted",
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
                                            Message = "Fund Manager Id is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }

                                    if (srda["IssuerID"] != DBNull.Value)
                                    {
                                        CISRepo.Issuer_Id = srda["IssuerID"].ToString();
                                        //Validate issuerid against issuer to check if it exist
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_ISSEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_ISSUER_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = CISRepo.Issuer_Id;
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
                                                Message = "Wrong Issuer Id Id.Process aborted",
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
                                            Message = "Issuer Id is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }


                                    if (srda["OrderQuantity"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["OrderQuantity"]) == false)
                                    {

                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Order Quantity.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        CISRepo.Order_Quantity = Convert.ToDecimal(srda["OrderQuantity"]);
                                    }

                                    if (srda["UnitPrice"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["UnitPrice"]) == false)
                                    {

                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Unit Price.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        CISRepo.Order_Unit_Price = Convert.ToDecimal(srda["UnitPrice"]);
                                    }

                                    if (srda["TotalCost"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["TotalCost"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Total Cost.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                    else
                                    //if (srda["AnnualInterestRate"] != DBNull.Value)
                                    {
                                        CISRepo.Total_Cost = Convert.ToDecimal(srda["TotalCost"]);
                                    }

                                    if (Microsoft.VisualBasic.Information.IsDate(srda["PurchaseDate"].ToString()))
                                    {
                                        CISRepo.Order_Date = Convert.ToDateTime(srda["PurchaseDate"].ToString());
                                    }
                                    else
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid Purchase Date.Process aborted",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }
                                   
                                    //DynamicParameters param = new DynamicParameters();
                                    //param.Add(name: "P_INVEST_NO", value: CISRepo.Product_Id + Scheme_Fund_Id + "000001", dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_INVEST_DESCRIPTION", value: CISRepo.Invest_Description, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_PRODUCT_ID", value: CISRepo.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_FUND_MANAGER_ID", value: CISRepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_ISSUER_ID", value: CISRepo.Issuer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_ORDER_QUANTITY", value: CISRepo.Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_ORDER_UNIT_PRICE", value: CISRepo.Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_CONSIDERATION", value: CISRepo.Consideration, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_ORDER_DATE", value: "12/31/2016", dbType: DbType.Date, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_INVEST_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_TRANS_TYPE", value: "BUY", dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_TOTAL_LEVIES", value: CISRepo.Total_Levies, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_TOTAL_COST", value: CISRepo.Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_AUTHORIZER", value: CISRepo.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_GL_ACCOUNT_NO", value: CISRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_EQUITY_CIS", value: CISRepo.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_CAUC", value: CISRepo.CURRENTAVERAGEUNITCOST, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_CTC", value: CISRepo.Current_Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //param.Add(name: "P_GL", value: CISRepo.Gain_Loss, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    //conn.Execute("ADD_INVEST_EQUITY_CIS", param, commandType: CommandType.StoredProcedure);

                                    //Update Invest_Equity_Balance table
                                    DynamicParameters param_B = new DynamicParameters();
                                    param_B.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_PRODUCT_ID", value:CISRepo.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_CURRENT_QUANTITY", value: CISRepo.Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_TOTAL_COST", value: CISRepo.Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_UNIT_PRICE", value: CISRepo.Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_FUND_MANAGER_ID", value: CISRepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                                    conn.Execute("MIX_INVEST_EQUITY_AFTER", param_B, commandType: CommandType.StoredProcedure);
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


        public ActionResult ReadSF()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

    }//end class


}