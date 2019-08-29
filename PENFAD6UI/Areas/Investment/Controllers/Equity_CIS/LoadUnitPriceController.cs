
using System;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.GL;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System.IO;
using PENFAD6DAL.GlobalObject;
using System.Data.OleDb;
using System.Data;
using System.Transactions;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.Diagnostics;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.Repository.Setup.InvestSetup;

namespace PENFAD6UI.Areas.Investment.Controllers.Equity_CIS
{
    public class LoadUnitPriceController : Controller
    {
        // int counter = 0;
        readonly GLInitialRepo GLRepo = new GLInitialRepo();
        readonly GLAccountRepo GLARepo = new GLAccountRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();

        public ActionResult AddLoadUnitPriceTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "LoadUnitPricePartial",
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
                var x = X.GetCmp<FormPanel>("frmLUP");
                x.Reset();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult UploadUP(Invest_ProductRepo PRepo, pfm_Scheme_FundRepo SFRepo)
        {
            try
            {
                if (!PRepo.E_Unit_Date.HasValue)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Date is required.Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

             

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("file_upload_UP_EC").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("file_upload_UP_EC").PostedFile;

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


                    if (ECUpload(ImageWork.Current_Path_For_Other_Files, PRepo))
                    {
						
						X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Unit Prices successfully uploaded.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        this.ClearControls();
                        //var pvr = new Ext.Net.MVC.PartialViewResult
                        //{
                        //    ViewName = "LoadUnitPricePartial",
                        //    ContainerId = "MainArea",
                        //    RenderMode = RenderMode.AddTo,
                        //};
                        //this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                        //return pvr;

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
                   

                    return this.Direct();
                }

                return this.Direct();
            }
            catch (Exception ex)
            {

                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! File upload failed. Contact System Administrator",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350

                });
                return this.Direct();
            }
            
        }

        public bool ECUpload(string filePath, Invest_ProductRepo PRepo)
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

                string query1 = "Select COUNT(*) AS NOS From [sheet1$]";
                string query2 = "Select * From [sheet1$]";
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

                                 //   Invest_ProductRepo PRepo = new Invest_ProductRepo();


                                    if (srda["Share Code"] != DBNull.Value)
                                    {
                                        PRepo.Product_Name = srda["Share Code"].ToString();
                                        OracleCommand cmd_emp_id = new OracleCommand();
                                        cmd_emp_id.CommandText = "SEL_INVEST_PRODUCTNAMEEXIST";
                                        cmd_emp_id.CommandType = CommandType.StoredProcedure;
                                        cmd_emp_id.Connection = (OracleConnection)conn;
                                        //Input param
                                        cmd_emp_id.Parameters.Add("p_ProductId", OracleDbType.Varchar2, ParameterDirection.Input).Value = PRepo.Product_Name;
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
                                                Message = "Wrong Share Code .Process aborted " + PRepo.Product_Name,
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
                                            Message = "Share Code is required",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }

                                  
                                    if (srda["Closing Price VWAP (GHS)"] == DBNull.Value || Microsoft.VisualBasic.Information.IsNumeric(srda["Closing Price VWAP (GHS)"]) == false)
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Invalid 'Closing Price VWAP (GHS)'.Process aborted " + PRepo.Product_Name,
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return false;
                                    }
                                    else
                                    {
                                        PRepo.Equity_CIS_Unit_Price = Convert.ToDecimal(srda["Closing Price VWAP (GHS)"]);
                                    }
                                  
                                    //Update Invest_Product table
                                    DynamicParameters param_B = new DynamicParameters();
                                    param_B.Add(name: "P_ProductName", value: PRepo.Product_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_UNIT_PRICE", value: PRepo.Equity_CIS_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_Unit_Date", value: PRepo.E_Unit_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                    param_B.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                    conn.Execute("UPD_EC_UNITPRICE", param_B, commandType: CommandType.StoredProcedure);
                                } ///while
                                ts.Complete();
                                // this.GetCmp<Window>("loading_EmpBatch").Hide();
                                //var pvr = new Ext.Net.MVC.PartialViewResult
                                //{
                                //    ViewName = "LoadUnitPricePartial",
                                //    ContainerId = "MainArea",
                                //    RenderMode = RenderMode.AddTo,
                                //};
                                //this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                                //return true;
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
                               // errormsg = transexeption.ToString();
                                return false;
                                //throw;
                                //var pvr = new Ext.Net.MVC.PartialViewResult
                                //{
                                //    ViewName = "LoadUnitPricePartial",
                                //    ContainerId = "MainArea",
                                //    RenderMode = RenderMode.AddTo,
                                //};
                                //this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                                //return true;
                            }
                            catch (Exception ex)
                            {
                                X.Mask.Hide();
                                string ora_code = ex.Message.Substring(0, 9);
                                if (ora_code == "ORA-20000")
                                {
                                    ora_code = "Record already exist. Process aborted..";
                                }
                                else if (ora_code == "ORA-20100")
                                {
                                    ora_code = "Not all records are supplied. Process aborted..";
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
                                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                                return false;
                                //var pvr = new Ext.Net.MVC.PartialViewResult
                                //{
                                //    ViewName = "LoadUnitPricePartial",
                                //    ContainerId = "MainArea",
                                //    RenderMode = RenderMode.AddTo,
                                //};
                                //this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                                //return true;
                            }
                            finally
                            {
                                ts.Dispose();
                                ////a_value = a_value;
                                //if (conn.State == ConnectionState.Open)
                                //{
                                //    conn.Close();
                                //}

                                //if (con_ex.State == ConnectionState.Open)
                                //{
                                //    con_ex.Close();
                                //}
                            }
                        } //end for transscope

                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Record already exist. Process aborted..";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Not all records are supplied. Process aborted..";
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
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
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