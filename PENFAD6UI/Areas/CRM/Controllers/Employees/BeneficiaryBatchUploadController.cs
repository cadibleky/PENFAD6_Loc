using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;

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

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    // =====================kris==============
    public class BeneficiaryBatchUploadController : Controller
    {
        // readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_BeneNextRepo BeneNextRepo = new crm_BeneNextRepo();
        List<crm_BeneNextRepo> BeneList = new List<crm_BeneNextRepo>();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        readonly GlobalValue global_val = new GlobalValue();
        readonly crm_EmployeeBatchLogRepo emp_log_repo = new crm_EmployeeBatchLogRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddBeneBatchTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "BeneBatchPartial",
                   // Model = BeneNextRepo.GetBeneNextList(),
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

        public ActionResult BeneBatchUpload(crm_BeneNextRepo BeneNextRepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

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

                    if (BatchBeneUpload(ImageWork.Current_Path_For_Other_Files, BeneNextRepo))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Beneficiaries uploaded successfully.",
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
        
        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("BeneficiaryBatch_frm");
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


        public bool BatchBeneUpload(string filePath, crm_BeneNextRepo BeneNextRepo)
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

                string query1 = "Select COUNT(*) AS NOS From [EmployeeBeneficiary$]";
                string query2 = "Select * From [EmployeeBeneficiary$]";
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
                               // int error_nos = 0;
                                string erroor_msg = "Error:" + Environment.NewLine;
                                var param = new DynamicParameters();

                                while (srda.Read())
                                {
                                    a_value += 1;

                                   // crm_BeneNextRepo BeneNextRepo1 = new crm_BeneNextRepo();

                                 BeneNextRepo.Beneficiary_NextOfKin = "BENEFICIARY";

                                    //Employee Id
                        if (srda["CompanyEmployeeID"] != DBNull.Value)
                        {
                            BeneNextRepo.Employee_Id = srda["CompanyEmployeeID"].ToString();
                        }
                        else
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Employee Id can not be null. Please check your excel sheet.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return false;
                        }

                        //surname
                        if (srda["BeneficiaryLastName"] != DBNull.Value)
                        {
                            BeneNextRepo.Surname = srda["BeneficiaryLastName"].ToString();
                        }
                        else
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Beneficiary Surname can not be null. Please check your excel sheet.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                                        return false;
                                    }
                        //FirstName   
                        if (srda["BeneficiaryFirstName"] != DBNull.Value)
                        {
                            BeneNextRepo.First_Name = srda["BeneficiaryFirstName"].ToString();
                        }
                        else
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Beneficiary First Name can not be null. Please check your excel sheet.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                                        return false;
                                    }

                                    //Other Name   
                                    if (srda["BeneficiaryMiddleName"] != DBNull.Value)
                                    {
                                        BeneNextRepo.Other_Name = srda["BeneficiaryMiddleName"].ToString();
                                    }
                                    else
                                    {
                                        BeneNextRepo.Other_Name = "";
                                    }


                                    //Relationship  
                                    if (srda["BeneficiaryRelationship"] != DBNull.Value)
                        {
                            BeneNextRepo.Relationship_Name = srda["BeneficiaryRelationship"].ToString();
                        }
                        else
                        {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Relationship can not be null. Please check your excel sheet.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                                        return false;
                        }

                                    ////Beneficiary Address   
                                    //if (srda["BeneficiaryAddress"] != DBNull.Value)
                                    //{
                                    //    BeneNextRepo.Residential_Address = srda["BeneficiaryAddress"].ToString();
                                    //}

                                    ////Email Address   
                                    //if ((srda["BeneficiaryEmailAddress"] != DBNull.Value))

                                    //{
                                    //    BeneNextRepo.Email_Address = srda["BeneficiaryEmailAddress"].ToString();
                                    //}


                                    ////Beneficiary Phone Number  
                                    //if (srda["BeneficiaryPhoneNumber"] != DBNull.Value)/* && (Microsoft.VisualBasic.Information.IsNumeric (srda["BeneficiaryPhoneNumber"]) == true))*/

                                    //{
                                    //    BeneNextRepo.Phone_Number1 = srda["BeneficiaryPhoneNumber"].ToString();
                                    //}


                                    //Beneficiary Percentages 
                                    if (srda["BeneficiaryPercentages"] != DBNull.Value)
                                    {
                                        BeneNextRepo.Beneficiary_Rate = Convert.ToDecimal(srda["BeneficiaryPercentages"]);
                                    }
                                    else
                                    {

                                        BeneNextRepo.Beneficiary_Rate = 0;
                                    }


                                    //-----------------&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
                                    //get CUS_NO for employee 
                                    if (srda["BeneficiaryPercentages"] != DBNull.Value)
                                        {
                                            BeneNextRepo.Employee_Id = (srda["CompanyEmployeeID"].ToString());
                                        }

                                    //var connn = new AppSettings();
                                    var paramA = new DynamicParameters();
                                    paramA.Add("P_EMPLOYEE_ID", BeneNextRepo.Employee_Id, DbType.String, ParameterDirection.Input);
                                    paramA.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                                    conn.Execute("SEL_BENE_EMPLOYEE_ID", paramA, commandType: CommandType.StoredProcedure);
                                    BeneNextRepo.Cust_No = paramA.Get<String>("VDATA");

                                    if (string.IsNullOrEmpty(BeneNextRepo.Cust_No))
                                    {
                                        X.Mask.Hide();
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Employee ID does not exist (" + srda["CompanyEmployeeID"].ToString() + ")",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });
                                        //con_ex.Close();
                                         return false;
                                    }

                                    param.Add(name: "P_BENEFICIARY_NEXTOFKIN_ID", value: BeneNextRepo.Beneficiary_NextOfKin_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_BENEFICIARY_NEXTOFKIN", value: BeneNextRepo.Beneficiary_NextOfKin, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_SURNAME", value: BeneNextRepo.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_FIRST_NAME", value: BeneNextRepo.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_OTHER_NAME", value: BeneNextRepo.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_MAIDEN_NAME", value: BeneNextRepo.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_PHONE_NUMBER1", value: BeneNextRepo.Phone_Number1, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_PHONE_NUMBER2", value: BeneNextRepo.Phone_Number2, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_RESIDENTIAL_ADDRESS", value: BeneNextRepo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_EMAIL_ADDRESS", value: BeneNextRepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_RELATIONSHIP_NAME", value: BeneNextRepo.Relationship_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_DATE_OF_BIRTH", value: BeneNextRepo.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                                    param.Add(name: "P_BENEFICIARY_RATE", value: BeneNextRepo.Beneficiary_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                    param.Add(name: "P_CUST_NO", value: BeneNextRepo.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                    conn.Execute(sql: "MIX_CRM_BENE_NEXT", param: param, commandType: CommandType.StoredProcedure);                                   
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


    } //end class
}