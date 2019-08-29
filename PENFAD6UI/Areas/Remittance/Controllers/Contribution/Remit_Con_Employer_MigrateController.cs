
using Ext.Net;
using Ext.Net.MVC;
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

    public class Remit_Con_Employer_MigrateController : Controller
    {
        string plog;
        string clog;
        readonly Remit_Con_Log remitConLogrepo = new Remit_Con_Log();
        readonly Remit_Con_Log_Details remitConLogdetailsrepo = new Remit_Con_Log_Details();
        readonly Remit_Contribution_Upload_LogRepo RemitInitialRepo = new Remit_Contribution_Upload_LogRepo();
        static List<Remit_Con_Log> remitConLogstaticlist = new List<Remit_Con_Log>();
        static List<Remit_Con_Log_Details> remitConLogDetailsStaticlist = new List<Remit_Con_Log_Details>();
        readonly Remit_Contribution_Upload_LogRepo rcul = new Remit_Contribution_Upload_LogRepo();

        public ActionResult AddConEmployerMigrationTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_Con_Employer_MigrationPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddBPEmployerMigrationTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_BP_Employer_MigrationPartial",
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
                string query2 = "Select * From [SalaryData2$]";

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

                    //DateTime con_date = Convert.ToDateTime("10/1/2011");
                    DateTime con_date = Convert.ToDateTime("8/1/2018");
                    for (int i = 0; i < iii + iii; i++)

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
                        foreach (var conlogcols in remitConLogstaticlist)
                        {
                            DateTime ddd = Convert.ToDateTime(conlogcols.For_Month.ToString() + "/" + conlogcols.For_Year.ToString());
                            string con_date_str = ddd.ToString("MMM-yy");

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

                            if (srda[con_date_str] != DBNull.Value && srda[con_date_str].ToString() != "-" && Convert.ToDecimal(srda[con_date_str]) != 0 )
                            {
                                Remit_Con_Log_Details new_conlogdetails_Newest = new Remit_Con_Log_Details();
                                new_conlogdetails_Newest.Employee_Id = srda["EmployeeId"].ToString();
                                new_conlogdetails_Newest.Employer_Con = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Con_Log_Id = conlogcols.Con_Log_Id;
                                new_conlogdetails_Newest.Esf_Id = new_EsfID;
                                new_conlogdetails_Newest.Employer_Amt = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Purchase_Log_Id = remit_con_logrepo.ES_Id + conlogcols.For_Year + plog + "01";

                                remitConLogDetailsStaticlist.Add(new_conlogdetails_Newest);
                            }
                        }

                    }
                    con_ex.Close();

                }

                using (TransactionScope transcope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(1, 30, 0)))
                {
                    //Push Con Log 
                    foreach (var conlogcols in remitConLogstaticlist)
                    {
                        string cnl = remitConLogrepo.Create_Con_Log_ER(conlogcols);
                    }

                    //create purchase log
                    foreach (var conlogcols in remitConLogstaticlist)
                    {
                        string cnl = remitConLogrepo.PurchaseSaveRecord(conlogcols);
                    }

                    //create payment logs 
                    //{
                    //    remit_con_logrepo.PaymentSaveRecord(remit_con_logrepo);
                    //}

                    //Push Con Log details
                    foreach (var conlogcolsdetails in remitConLogDetailsStaticlist)
                    {
                        remitConLogdetailsrepo.Create_Con_Log_Details_employer(conlogcolsdetails);
                    }

                    //Push Contribution into purchase trans table
                    foreach (var conlogcolsdetails in remitConLogDetailsStaticlist)
                    {
                        remitConLogdetailsrepo.Create_Unit_Log_Details_Employer(conlogcolsdetails);
                    }
                  

                    transcope.Complete();
                }

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Employer Remittances Uploaded Successfully",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });

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
                    ora_code = "NO EMPLOYEE CONTRIBUTION";
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
                return this.Direct();
            }
            finally
            {

            }
        }


// for back pay
        public ActionResult RemitBPMigrationUpload(Remit_Con_Log remit_con_logrepo)
        {
            try
            {

                string extension = string.Empty;
                string file_loc = string.Empty;
                remitConLogstaticlist.Clear();
                remitConLogDetailsStaticlist.Clear();
                if (this.GetCmp<FileUploadField>("Remit_BP_Migration_remitfile_upload1").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("Remit_BP_Migration_remitfile_upload1").PostedFile;

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
                string query2 = "Select * From [SalaryData332$]";

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

                    DateTime con_date = Convert.ToDateTime("10/1/2011");
                    for (int i = 0; i < iii + iii; i++)

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
                            string conlog = "CON" + remit_con_logrepo.ES_Id + Convert.ToInt32(con_date.ToString("yyyy")) + clog + "05";
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

                        if (rcul.isEmployeeValid(srda["EmployeeId"].ToString(), remit_con_logrepo.Employer_Id) == true)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = srda["EmployeeId"].ToString() + " - not registered under the select employer. Process aborted",
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
                                new_conlogdetails_Newest.Employer_Con = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Con_Log_Id = conlogcols.Con_Log_Id;
                                new_conlogdetails_Newest.Esf_Id = new_EsfID;
                                new_conlogdetails_Newest.Employer_Amt = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Purchase_Log_Id = remit_con_logrepo.ES_Id + conlogcols.For_Year + plog + "05";

                                remitConLogDetailsStaticlist.Add(new_conlogdetails_Newest);
                            }
                        }

                    }
                    con_ex.Close();

                }

                using (TransactionScope transcope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(1, 30, 0)))
                {
                    //Push Con Log 
                    foreach (var conlogcols in remitConLogstaticlist)
                    {
                        string cnl = remitConLogrepo.Create_BP_Log_ER(conlogcols);
                    }

                    //create purchase log
                    foreach (var conlogcols in remitConLogstaticlist)
                    {
                        string cnl = remitConLogrepo.BPPurchaseSaveRecord(conlogcols);
                    }

                    //create payment logs 
                    //{
                    //    remit_con_logrepo.PaymentSaveRecord(remit_con_logrepo);
                    //}

                    //Push Con Log details
                    foreach (var conlogcolsdetails in remitConLogDetailsStaticlist)
                    {
                        remitConLogdetailsrepo.Create_BP_Log_Details_employer(conlogcolsdetails);
                    }

                    //Push Contribution into purchase trans table
                    foreach (var conlogcolsdetails in remitConLogDetailsStaticlist)
                    {
                        remitConLogdetailsrepo.Create_Unit_Log_Details_Employer(conlogcolsdetails);
                    }


                    transcope.Complete();
                }

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Employer BackPay Uploaded Successfully",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });

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
                    ora_code = "NO EMPLOYEE BACK PAY";
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