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

namespace PENFAD6UI.Areas.Remittance.Controllers.Contribution
{

    public class Remit_Unit_MigrateController : Controller
    {

        readonly Remit_Unit_Log remitConLogrepo = new Remit_Unit_Log();
        readonly Remit_Unit_Log_Details remitConLogdetailsrepo = new Remit_Unit_Log_Details();
        readonly Remit_Contribution_Upload_LogRepo RemitInitialRepo = new Remit_Contribution_Upload_LogRepo();
        static List<Remit_Unit_Log> remitConLogstaticlist = new List<Remit_Unit_Log>();
        static List<Remit_Unit_Log_Details> remitConLogDetailsStaticlist = new List<Remit_Unit_Log_Details>();
        readonly Remit_Contribution_Upload_LogRepo rcul = new Remit_Contribution_Upload_LogRepo();


        
        public ActionResult AddUnitMigrationTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Remit_Unit_MigrationPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult RemitConMigrationUpload(Remit_Unit_Log remit_con_logrepo)
        {
            try
            {
               
                string extension = string.Empty;
                string file_loc = string.Empty;
                remitConLogstaticlist.Clear();
                remitConLogDetailsStaticlist.Clear();

                if (this.GetCmp<FileUploadField>("Remit_Unit_Migration_remitfile_upload1").HasFile)
                {

                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("Remit_Unit_Migration_remitfile_upload1").PostedFile;

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
                string query2 = "Select * From [SalaryUnit$]";

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

                    DateTime con_date = Convert.ToDateTime("10/1/2012");
                    for (int i = 0; i < iii + iii ; i++)

                    {
                        con_date = con_date.AddMonths(1);
                        string con_date_string = Convert.ToString(con_date.ToString("MMM-yy"));

                        if (srda.GetSchemaTable()
                            .Rows
                            .OfType<DataRow>()
                            .Any(row => row["ColumnName"].ToString() == con_date_string))
                        {
                            //Generate con log
                            Remit_Unit_Log new_conlog = new Remit_Unit_Log();
                            string conlog = "CON" + remit_con_logrepo.ES_Id + Convert.ToInt32(con_date.ToString("yyyy")) + Convert.ToInt32(con_date.ToString("MM")) + "01";
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
                        Remit_Unit_Log_Details new_conlogdetails = new Remit_Unit_Log_Details();
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
                                    Message = "Problem with excel - employee (" + srda["EmployeeId"].ToString() + ")",
                                    Buttons = MessageBox.Button.OK,
                                    Icon = MessageBox.Icon.INFO,
                                    Width = 350
                                });
                                con_ex.Close();
                                return this.Direct();
                            }

                           
                            if (srda[con_date_str] != DBNull.Value && srda[con_date_str].ToString() != "-" && Convert.ToDecimal(srda[con_date_str]) != 0)
                            {
                                Remit_Unit_Log_Details new_conlogdetails_Newest = new Remit_Unit_Log_Details();
                                new_conlogdetails_Newest.Employee_Id = srda["EmployeeId"].ToString();
                                new_conlogdetails_Newest.Employee_Units = Convert.ToDecimal(srda[con_date_str]);
                                new_conlogdetails_Newest.Con_Log_Id = conlogcols.Con_Log_Id;
                                new_conlogdetails_Newest.Esf_Id = new_EsfID;
                                new_conlogdetails_Newest.Purchase_Log_Id = remit_con_logrepo.ES_Id + conlogcols.Con_Log_Id + "01";

                                remitConLogDetailsStaticlist.Add(new_conlogdetails_Newest);
                            }
                        }

                    }
                    con_ex.Close();

                }

                using (TransactionScope transcope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(1, 30, 0)))
                {
                   
                    //create purchase log
                    foreach (var remitConLogdetailsrepo in remitConLogDetailsStaticlist)
                    {
                        //get purchase log for employee
                        if (remitConLogdetailsrepo.GetPurchaseLog(remitConLogdetailsrepo.Esf_Id, remit_con_logrepo.ES_Id + remitConLogdetailsrepo.Con_Log_Id + "01".ToString()) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Problem with excel - employee (" + srda["EmployeeId"].ToString()+")",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350
                            });
                            con_ex.Close();
                            return this.Direct();
                        }

                        string cnl = remitConLogdetailsrepo.PurchaseSaveRecord(remitConLogdetailsrepo);
                    }

                    transcope.Complete();
                }


                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Success",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });

                return this.Direct();
            }
            catch (Exception ex)
            {
                throw ex;
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