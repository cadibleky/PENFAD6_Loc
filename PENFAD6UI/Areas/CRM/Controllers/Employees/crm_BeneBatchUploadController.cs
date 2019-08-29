using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.Repository.CRM.Employee;
using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class crm_BeneBatchUploadController : Controller
    {
        readonly crm_BeneNextRepo BeneRepo = new crm_BeneNextRepo();
        readonly crm_BeneNextRepo NextRepo = new crm_BeneNextRepo();


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
                    Model = BeneRepo,
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

        public ActionResult UploadLogo()
        {
          //  var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
               
                string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                //Check if file Exist
                if (this.GetCmp<FileUploadField>("Img").HasFile)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Title = "Success",
                        Message = string.Format(tpl, this.GetCmp<FileUploadField>("Img").PostedFile.FileName, this.GetCmp<FileUploadField>("Img").PostedFile.ContentLength)
                    });
                    HttpPostedFile file = this.GetCmp<FileUploadField>("Img").PostedFile;

                    string extension = Path.GetExtension(file.FileName);
                    
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

                    string physicalPath = "";

                    string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/resources/" + "Excelfiles/"));
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    physicalPath = (filePath + "/" + "Bene" + extension);
                    //physicalPathLoc = Server.MapPath(physicalPath);

                    // save image in folder
                    file.SaveAs(physicalPath);

                    BatchBeneUpload(physicalPath);
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a Bene to upload.",
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
               // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }

        }

        public void BatchBeneUpload(string filePath)
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

            OleDbConnection con = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();
           
            string query1 = "Select COUNT(*) AS NOS From [EmployeeBeneficiary$]";
            string query2 = "Select * From [EmployeeBeneficiary$]";
            int totalsum = 1;
           
            con.ConnectionString = consString_excel;
            con.Open();
           
            cmd.Connection = con;
            cmd.CommandText = query1;

            totalsum = Convert.ToInt32(cmd.ExecuteScalar()); //();
            con.Close();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            if (con.State == ConnectionState.Open)
            {
                cmd.Connection = con;
                cmd.CommandText = query2;
                OleDbDataReader srda = cmd.ExecuteReader();

                // Me.ProgressBar1.Minimum = 0
                int a_value = 0;
                //Me.ProgressBar1.Maximum = totalsum
                if (srda.HasRows)
                {
                    //string errormsg = "";
                    
                    Stopwatch sw = new Stopwatch();
                    // Start The StopWatch ...From 000
                    sw.Start();
                    var app = new AppSettings();
                    //con = app.GetConnection();
                    using (TransactionScope transcope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(1, 30, 0)))
                    {
                        using (OracleConnection conn = new OracleConnection(app.conString()))  //
                        {
                            conn.Open();
                            try
                            {
                                var paramb = new DynamicParameters();

                                //string batchno = "na";
                                ////int staffId = "";

                                //paramb.Add(name: "@MakerId", value: "", dbType: DbType.String, direction: ParameterDirection.Input);
                                //paramb.Add(name: "@Description", value: "", dbType: DbType.String, direction: ParameterDirection.Input);

                                //paramb.Add(name: "@BatchNo", value: batchno, dbType: DbType.String, direction: ParameterDirection.Output);
                                //conn.Execute(sql: "add_setup_EmployeeBatchLog", param: paramb, commandType: CommandType.StoredProcedure);
                                //batchno = paramb.Get<string>("@BatchNo");
                                ////-----end batch number


                                var param = new DynamicParameters();

                                while (srda.Read())
                                {
                                    a_value += 1;


                                    BeneRepo.Beneficiary_NextOfKin = "BENEFICIARY";

                                    //Employee Id
                                    if (srda["EmployeeSrNo"] != DBNull.Value)
                                    {
                                        //BeneRepo.Employee_Id = srda["EmployeeSrNo"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Employee Id can not be null. Please check your excel sheet.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return;
                                    }

                                    //surname
                                    if (srda["BeneficiaryLastName"] != DBNull.Value)
                                    {
                                        BeneRepo.Surname = srda["BeneficiaryLastName"].ToString();
                                    }
                                    else
                                    {

                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Beneficiary Surname can not be null. Please check your excel sheet.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350
                                        });
                                        return;
                                    }
                                    //FirstName   
                                    if (srda["BeneficiaryFirstName"] != DBNull.Value)
                                    {
                                        BeneRepo.First_Name = srda["BeneficiaryFirstName"].ToString();
                                    }
                                    else
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Beneficiary First Name can not be null. Please check your excel sheet.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return;
                                    }

                                    //Other Name   
                                    if (srda["BeneficiaryMiddleName"] != DBNull.Value)
                                    {
                                        BeneRepo.Other_Name = srda["BeneficiaryMiddleName"].ToString();
                                    }
                               else
                                    {

                                    }

                                    //Relationship  
                                    if (srda["BeneficiaryRelationship"] != DBNull.Value)
                                    {
                                        BeneRepo.Relationship_Id = srda["BeneficiaryRelationship"].ToString();
                                    }
                                    else
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "Relationship Id can not be null. Please check your excel sheet.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.ERROR,
                                            Width = 350

                                        });
                                        return;
                                    }

                                    //Beneficiary Address   
                                    if (srda["BeneficiaryAddress"] != DBNull.Value)
                                    {
                                        BeneRepo.Residential_Address = srda["BeneficiaryAddress"].ToString();
                                    }
                                    else
                                    {

                                    }
                                    //Email Address   
                                    if ((srda["BeneficiaryEmailAddress"] != DBNull.Value))
                                   
                                        {
                                            BeneRepo.Email_Address = srda["BeneficiaryEmailAddress"].ToString();
                                        }
                                    
                                    else
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Error",
                                            Message = "'Email Address' is invalid.",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350

                                        });
                                        return;
                                    }
                                    //Beneficiary Phone Number  
                                    if (srda["BeneficiaryPhoneNumber"] != DBNull.Value)/* && (Microsoft.VisualBasic.Information.IsNumeric (srda["BeneficiaryPhoneNumber"]) == true))*/
                                    
                                        {
                                            BeneRepo.Phone_Number1 = srda["BeneficiaryPhoneNumber"].ToString();
                                        }
                                    //else
                                    //{
                                    //        X.Msg.Show(new MessageBoxConfig
                                    //        {
                                    //            Title = "Error",
                                    //            Message = "'Primary Phone Number' is invalid.",
                                    //            Buttons = MessageBox.Button.OK,
                                    //            Icon = MessageBox.Icon.INFO,
                                    //            Width = 350

                                    //        });
                  
                                    //    return;
                                    //}

                                    //Beneficiary Percentages 
                                    if (srda["BeneficiaryPercentages"] != DBNull.Value)
                                    {
                                        BeneRepo.Beneficiary_Rate = Convert.ToDecimal(srda["BeneficiaryPercentages"]);
                                    }
                                 
                                                                    
                                    param.Add(name: "P_BENEFICIARY_NEXTOFKIN_ID", value: BeneRepo.Beneficiary_NextOfKin_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_BENEFICIARY_NEXTOFKIN", value: BeneRepo.Beneficiary_NextOfKin, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_SURNAME", value: BeneRepo.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_FIRST_NAME", value: BeneRepo.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_OTHER_NAME", value: BeneRepo.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_MAIDEN_NAME", value: BeneRepo.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_PHONE_NUMBER1", value: BeneRepo.Phone_Number1, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_PHONE_NUMBER2", value: BeneRepo.Phone_Number2, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_RESIDENTIAL_ADDRESS", value: BeneRepo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_EMAIL_ADDRESS", value: BeneRepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_RELATIONSHIP_ID", value: BeneRepo.Relationship_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_DATE_OF_BIRTH", value: BeneRepo.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                                    param.Add(name: "P_BENEFICIARY_RATE", value: BeneRepo.Beneficiary_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                                   // param.Add(name: "P_EMPLOYEE_ID", value: BeneRepo.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_UPDATE_ID", value: "user", dbType: DbType.String, direction: ParameterDirection.Input);
                                    param.Add(name: "P_MAKER_ID", value: "user", dbType: DbType.String, direction: ParameterDirection.Input);

                                    int result = conn.Execute("MIX_CRM_BENE_NEXT", param, commandType: CommandType.StoredProcedure);
                                  


                                }

                             
                                transcope.Complete();

                                //return true;


                                //Stop the Timer
                                sw.Stop();
                                string ExecutionTimeTaken = string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);


                            }
                            catch (TransactionException transexeption)
                            {
                                //MessageBox.Show("ERROR: Data upload failed. " + transexeption.Message + Environment.NewLine, "TEB3", false);
                                //errormsg = "";
                                //return false;
                            }
                            catch (Exception ex)
                            {

                                //MessageBox.Show("ERROR: Data upload failed. " + ex.Message + Environment.NewLine, "TEB3", false);
                               // errormsg = "";
                                //return false;
                            }
                            finally
                            {
                                transcope.Dispose();
                               // a_value = a_value;
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.Close();
                                }

                                if (con.State == ConnectionState.Open)
                                {
                                    con.Close();
                                }


                                // '''  ts.Dispose()
                            }


                        }  //end for transscope

                    } //close for con
                    
                }
                
            }
           
        }
       
    }
}