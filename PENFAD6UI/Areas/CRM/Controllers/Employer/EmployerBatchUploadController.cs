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

namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    // =====================kris==============
    public class EmployerBatchUploadController : Controller
    {
        readonly crm_BeneNextRepo BeneNextRepo = new crm_BeneNextRepo();
        List<crm_EmployerRepo> EmployerList = new List<crm_EmployerRepo>();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        readonly GlobalValue global_val = new GlobalValue();
        DateTime date_default = Convert.ToDateTime("01/01/1901");
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployerBatchTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployerBatchPartial",
                    Model = BeneNextRepo.GetBeneNextList(),
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }

        public ActionResult EmployerBatchUpload(crm_EmployerRepo employer)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {

                //Check if file Exist   file_upload1
                if (this.GetCmp<FileUploadField>("EmployerP_FileUp").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("EmployerP_FileUp").PostedFile;

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

                    if (BatchEmployerUpload(ImageWork.Current_Path_For_Other_Files, employer))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Employers/Accounts uploaded successfully.",
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

                    var x = X.GetCmp<FormPanel>("EmployerBatch_frm");
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
                var x = X.GetCmp<FormPanel>("EmployerBatch_frm");
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


        public bool BatchEmployerUpload(string filePath, crm_EmployerRepo employer)
        {
            try
            {

                if (System.IO.File.Exists(filePath) == false)
                {
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

                string query1 = "Select COUNT(*) AS NOS From [EmployerData$]";
                string query2 = "Select * From [EmployerData$]";
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
                               // var param = new DynamicParameters();

                                while (srda.Read())
                                {
                                    a_value += 1;

                                    if (srda["ID NUMBER"] != DBNull.Value)
                                    {
                                        employer.Employer_Id = srda["ID NUMBER"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Employer ID can not be null. Please check your excel sheet.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (srda["NAME OF EMPLOYER"] != DBNull.Value)
                                    {
                                        employer.Employer_Name = srda["NAME OF EMPLOYER"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Employer Name can not be null. Please check your excel sheet.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }


                                    if (srda["ID NUMBER"] != DBNull.Value)
                                    {
                                        employer.Employer_Id = srda["ID NUMBER"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Employer ID can not be null. Please check your excel sheet." + srda["NAME OF EMPLOYER"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (srda["BUSINESS LOCATION ADDRESS"] != DBNull.Value)
                                    {
                                        employer.Office_Location = srda["BUSINESS LOCATION ADDRESS"].ToString();
                                    }


                                    if (srda["REGION ID"] != DBNull.Value)
                                    {
                                        employer.Region_Id = srda["REGION ID"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Region ID can not be null. Please check your excel sheet." + srda["NAME OF EMPLOYER"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (srda["DISTRICT ID"] != DBNull.Value)
                                    {
                                        employer.District_Id = srda["DISTRICT ID"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "District ID can not be null. Please check your excel sheet." + srda["NAME OF EMPLOYER"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }


                                    if (srda["SECTOR ID"] != DBNull.Value)
                                    {
                                        employer.Sector_Id = srda["SECTOR ID"].ToString();
                                    }
                                    else
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Sector ID can not be null. Please check your excel sheet." + srda["NAME OF EMPLOYER"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (srda["POSTAL ADDRESS"] != DBNull.Value)
                                    {
                                        employer.Employer_Postal_Address = srda["POSTAL ADDRESS"].ToString();
                                    }


                                    if (srda["TELEPHONE NUMBER"] != DBNull.Value)
                                    {
                                        employer.Phone_Number = srda["TELEPHONE NUMBER"].ToString();
                                    }


                                    //if ((srda["FAX NUMBER"] != DBNull.Value))

                                    //{
                                       
                                    //}


                                    //if (srda["BUSINESS ACTIVITY"] != DBNull.Value)

                                    //{
                                    //    employer.Sector_Id = srda["BUSINESS ACTIVITY"].ToString();
                                    //}

                                    if (srda["CONTACT PERSON'S NAME"] != DBNull.Value)
                                    {
                                        employer.Contact_Person = (srda["CONTACT PERSON'S NAME"].ToString());
                                    }
                                    //else
                                    //{

                                    //    X.Msg.Show(new MessageBoxConfig
                                    //    {
                                    //        Title = "Error",
                                    //        Message = "Contact Person can not be null. Please check your excel sheet.",
                                    //        Buttons = MessageBox.Button.OK,
                                    //        Icon = MessageBox.Icon.ERROR,
                                    //        Width = 350
                                    //    });
                                    //    return false;
                                    //}


                                    if (srda["CONTACT PERSON'S MOBILE"] != DBNull.Value)
                                    {
                                        employer.Contact_Number = (srda["CONTACT PERSON'S MOBILE"].ToString());
                                    }
                                    if (srda["CONTACT PERSON'S MOBILE 2"] != DBNull.Value)
                                    {
                                        employer.Employer_Other_Number = (srda["CONTACT PERSON'S MOBILE 2"].ToString());
                                    }

                                    if (srda["Email"] != DBNull.Value)
                                    {
                                        employer.Contact_Email = (srda["Email"].ToString());
                                    }

                                    if (srda["Business Registration Number"] != DBNull.Value)
                                    {
                                        employer.Registration_Number = (srda["Business Registration Number"].ToString());
                                    }
                                    if (srda["Tax Identification Number"] != DBNull.Value)
                                    {
                                        employer.Tin_No = (srda["Tax Identification Number"].ToString());
                                    }
                                    if (srda["Employer Social Security Number"] != DBNull.Value)
                                    {
                                        employer.Ssnit = (srda["Employer Social Security Number"].ToString());
                                    }
                                    if (srda["ENROLMENT DATE"] != DBNull.Value)
                                    {
                                        //DateTime dateofbirth;
                                        if (Microsoft.VisualBasic.Information.IsDate(srda["ENROLMENT DATE"].ToString()))
                                        {
                                            employer.Registration_Date = Convert.ToDateTime(srda["ENROLMENT DATE"].ToString());
                                        }
                                        else
                                        {

                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Registration date not in right format. Please check your excel sheet." + srda["NAME OF EMPLOYER"].ToString(),
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        employer.Registration_Date = date_default; ;
                                    }


                                    string newemployerid = "";


                                    if (!string.IsNullOrEmpty(srda["SCHEME 1"].ToString()))
                                    {

                                        // CHECK IF SCHEME EXIST
                                        var param = new DynamicParameters();
                                        param.Add("P_SCHEME_ID", srda["SCHEME 1"].ToString(), DbType.String, ParameterDirection.Input);
                                        param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                                        conn.Execute("SEL_SCHEME_EXIST", param, commandType: CommandType.StoredProcedure);
                                        int paramoption = param.Get<int>("VDATA");

                                        if (paramoption == 0)
                                        {
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Scheme does not exist - " + srda["NAME OF EMPLOYER"].ToString(),
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return true;
                                        }
                                        employer.Scheme_Id = (srda["SCHEME 1"].ToString());

                                         DynamicParameters paramS = new DynamicParameters();
                                        paramS.Add(name: "p_Employer_Id", value: employer.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Employer_Name", value: employer.Employer_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Tin", value: employer.Tin_No, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Ssnit", value: employer.Ssnit, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Npra_Number", value: employer.Npra_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Business_Address", value: employer.Business_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Postal_Address", value: employer.Employer_Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Phone_Number", value: employer.Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Email_Address", value: employer.Employer_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Web_Site", value: employer.Web_Site, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Registration_Number", value: employer.Registration_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Registration_Date", value: employer.Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Contact_Number", value: employer.Contact_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Contact_Email", value: employer.Contact_Email, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Contact_Person", value: employer.Contact_Person, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Other_Number", value: employer.Employer_Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Region_Id", value: employer.Region_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_District_Id", value: employer.District_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Office_Location", value: employer.Office_Location, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Sector_Id", value: employer.Sector_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Employer_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Auth_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramS.Add(name: "P_NEW_EMPLOYEE_ID", value: String.Empty, dbType: DbType.String, direction: ParameterDirection.Output);
                                        conn.Execute("ADD_CRM_EMPLOYER_BATCH", paramS, commandType: CommandType.StoredProcedure);

                                        newemployerid = paramS.Get<string>("P_NEW_EMPLOYEE_ID");

                                        DynamicParameters paramb = new DynamicParameters();
                                        paramb.Add(name: "P_ES_ID", value: newemployerid + employer.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramb.Add(name: "P_SCHEME_ID", value: employer.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramb.Add(name: "P_EMPLOYER_ID", value: newemployerid, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramb.Add(name: "p_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        paramb.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                                        conn.Execute("MIX_MIGRATE_EMPLOYER_SCHEME_MI", paramb, commandType: CommandType.StoredProcedure);
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "No scheme assigned to Employer - " + srda["NAME OF EMPLOYER"].ToString(),
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return false;
                                    }

                                    if (!string.IsNullOrEmpty(srda["SCHEME 2"].ToString()))
                                    {
                                        // CHECK IF SCHEME EXIST
                                        var paramSC2 = new DynamicParameters();
                                        paramSC2.Add("P_SCHEME_ID", srda["SCHEME 2"].ToString(), DbType.String, ParameterDirection.Input);
                                        paramSC2.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                                        conn.Execute("SEL_SCHEME_EXIST", paramSC2, commandType: CommandType.StoredProcedure);
                                        int paramoption2 = paramSC2.Get<int>("VDATA");

                                        if (paramoption2 == 0)
                                        {
                                            X.Msg.Show(new MessageBoxConfig
                                            {
                                                Title = "Error",
                                                Message = "Scheme does not exist - " + srda["NAME OF EMPLOYER"].ToString(),
                                                Buttons = MessageBox.Button.OK,
                                                Icon = MessageBox.Icon.ERROR,
                                                Width = 350
                                            });
                                            return false;
                                        }

                                         DynamicParameters parambs = new DynamicParameters();
                                        parambs.Add(name: "P_ES_ID", value: newemployerid + srda["SCHEME 2"], dbType: DbType.String, direction: ParameterDirection.Input);
                                        parambs.Add(name: "P_SCHEME_ID", value: srda["SCHEME 2"], dbType: DbType.String, direction: ParameterDirection.Input);
                                        parambs.Add(name: "P_EMPLOYER_ID", value: newemployerid, dbType: DbType.String, direction: ParameterDirection.Input);
                                        parambs.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                                        parambs.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                                        conn.Execute("MIX_MIGRATE_EMPLOYER_SCHEME", parambs, commandType: CommandType.StoredProcedure);
                                    }

                                }

                                ts.Complete();
                                return true;

                            }

                            catch (TransactionException ex)
                            {
                                throw ex;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
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