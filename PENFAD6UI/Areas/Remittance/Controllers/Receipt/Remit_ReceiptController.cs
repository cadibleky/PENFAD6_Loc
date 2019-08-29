
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Remittance.Contribution;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Data;
using System.Text.RegularExpressions;

namespace PENFAD6UI.Areas.Remittance.Controllers.Receipt
{
    public class Remit_ReceiptController : Controller
    {
        String NEW_DATE;
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        readonly Remit_ReceiptRepo ReceiptRepo = new Remit_ReceiptRepo();
        readonly setup_InternetRepo internetRepo = new setup_InternetRepo();
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("receipt");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("Aprovereceipt");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Reverse()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("Reversereceipt");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult AddReceiptTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptPartial",
                Model = ReceiptRepo.GetReceiptList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddReceiptApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptApprovePartial",
                //Model = ReceiptRepo.GetReceiptPendingList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddReverseReceiptTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReverseReceiptPartial",
                // Model = ReceiptRepo.GetReceiptList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddSendReceiptTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SendReceiptPartial",
                // Model = ReceiptRepo.GetReceiptList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }


        public ActionResult SaveRecord(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {

                if (!string.IsNullOrEmpty(ReceiptRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);
                    if (ReceiptRepo.Actual_Receipt_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Actual payment date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Employer Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }



                if (ReceiptRepo.Trans_Amount <= 0)
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Amount must be more than '0'.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

               
                if (ModelState.IsValid)
                {
                    this.ReceiptRepo.SaveRecord(ReceiptRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Receipt Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    ClearControls();
                    Store store = X.GetCmp<Store>("receiptStore");
                    store.Reload();
                    return this.Direct();
                }
                else
                {

                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = messages, // " Insufficient data. Operation Aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
            }

            catch (Exception)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Operation  failed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }


        public ActionResult ApproveRecord(string RECEIPT_ID, Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                if (RECEIPT_ID == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Receipt' has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
                GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);
               
                if ( ReceiptRepo.ApproveReceiptRecord(RECEIPT_ID, ReceiptRepo))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Receipt Successfully Approved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    ClearControls_Approve();
                    Store store = X.GetCmp<Store>("receiptStore");
                    store.Reload();

                    return this.Direct();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Operation  failed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }




            }
            catch (Exception ex)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Error in approving payment. process aborted. Contact system admin.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

        public ActionResult DisapproveRecord(string RECEIPT_ID)
        {
            try
            {
                if (RECEIPT_ID == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Receipt' has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //if (!string.IsNullOrEmpty(ReceiptRepo.ES_Id))
                //{
                //    ReceiptRepo.Scheme_Id = ReceiptRepo.ES_Id.Substring(ReceiptRepo.ES_Id.Length - 2);
                //}
                //if (!string.IsNullOrEmpty(ReceiptRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);
                //}

                //// get record
                //if (!string.IsNullOrEmpty(ReceiptRepo.Receipt_Id))
                //{
                //    ReceiptRepo.GetERPENDList(ReceiptRepo);
                //}

                if (  ReceiptRepo.DisapproveReceiptRecord(RECEIPT_ID))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Receipt Successfully Disapproved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    ClearControls_Approve();
                    Store store = X.GetCmp<Store>("receiptStore");
                    store.Reload();

                    return this.Direct();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Operation  failed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReverseRecord(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {

                if (ReceiptRepo.Receipt_Id == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Receipt' has been selected for Reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);

                //if (!string.IsNullOrEmpty(ReceiptRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);
                //    if (ReceiptRepo.Actual_Receipt_Date != GlobalValue.Scheme_Today_Date)
                //    {
                //        X.Msg.Show(new MessageBoxConfig
                //        {
                //            Title = "Error",
                //            Message = "Sorry! This transaction can not be reversed. Process aborted" + GlobalValue.Scheme_Today_Date.Date.ToString(),
                //            Buttons = MessageBox.Button.OK,
                //            Icon = MessageBox.Icon.INFO,
                //            Width = 350

                //        });

                //        return this.Direct();
                //    }
                //}
                //else
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Employer Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //    return this.Direct();

                //}


                if (string.IsNullOrEmpty(ReceiptRepo.Reverse_Reason))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Reason is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //if (!string.IsNullOrEmpty(ReceiptRepo.ES_Id))
                //{
                //    ReceiptRepo.Scheme_Id = ReceiptRepo.ES_Id.Substring(ReceiptRepo.ES_Id.Length - 2);
                //}
                //if (!string.IsNullOrEmpty(ReceiptRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);
                //}

                if(ReceiptRepo.ReverseReceiptRecord(ReceiptRepo))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Receipt Successfully Reversed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    ClearControls_Reverse();
                    Store store = X.GetCmp<Store>("RreceiptStore");
                    store.Reload();

                    return this.Direct();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Operation  failed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
               
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        //public ActionResult ApproveReverseRecord(string RECEIPT_ID)
        //{
        //    if (RECEIPT_ID == string.Empty)
        //    {
        //        X.Msg.Show(new MessageBoxConfig
        //        {
        //            Title = "Error",
        //            Message = "Sorry! No 'Reversed Receipt' has been selected for Approval.",
        //            Buttons = MessageBox.Button.OK,
        //            Icon = MessageBox.Icon.ERROR,
        //            Width = 350
        //        });
        //        return this.Direct();
        //    }

        //    ReceiptRepo.ApproveReverseReceiptRecord(RECEIPT_ID);
        //    X.Msg.Show(new MessageBoxConfig
        //    {
        //        Title = "Success",
        //        Message = "Reversed Receipt Successfully Approved.",
        //        Buttons = MessageBox.Button.OK,
        //        Icon = MessageBox.Icon.INFO,
        //        Width = 350

        //    });
        //    ClearControls();
        //    Store store = X.GetCmp<Store>("receiptStore");
        //    store.Reload();

        //    return this.Direct();
        //}

        public ActionResult Read()
        {
            try
            {
                return this.Store(ReceiptRepo.GetReceiptList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }
        public ActionResult ReadEmployerScheme()
        {
            try
            {
                
                {
                    return this.Store(ReceiptRepo.GetReceiptESList());
                }
         
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }
        public ActionResult ReadReceiptPending()
        {
            return this.Store(ReceiptRepo.GetReceiptPendingList());
        }



        // filter employer payments
        public ActionResult FilterERDGrid(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {

                Store store = X.GetCmp<Store>("purconStore");
                store.Reload();
                store.DataBind();

                return this.Store(ReceiptRepo.GetERList(ReceiptRepo));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }
        public ActionResult ReadReceiptActive()
        {
            try
            {
                return this.Store(ReceiptRepo.GetReceiptActiveList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult Get_Con_Balance(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                X.GetCmp<TextField>("ReceiptPartial_Con_Total").Value = ReceiptRepo.getConBalance(ReceiptRepo);
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        
        public ActionResult ReadReverseReceipt()
        {
            try
            {
                return this.Store(ReceiptRepo.GetReverseReceiptList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult DownloadRecord(Remit_ReceiptRepo ReceiptRepo)
        {

            try
            {
                if (String.IsNullOrEmpty(ReceiptRepo.Receipt_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Payment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }
                string queryString = "select * from VW_RECEIPT_EMPLOYER where receipt_id = '" + ReceiptRepo.Receipt_Id + "' ";

                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.


                    while (reader.Read())
                    {


                        ReceiptRepo.Scheme_Name = (string)reader["Scheme_Name"];
                        ReceiptRepo.Employer_Name = (string)reader["Employer_Name"];
                        ReceiptRepo.Actual_Receipt_Date = (DateTime)reader["Actual_Receipt_Date"];
                        NEW_DATE   = ReceiptRepo.Actual_Receipt_Date.ToString().Replace("/", ".");
                        NEW_DATE = NEW_DATE.Replace(":", ".");

                        string DocumentName = "NA";
                        string pa = Server.MapPath("~/Penfad_Reports/employer_Payment_Confirmation.dll");

                        System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                        StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                        //////asign constring
                        My_Report.Dictionary.DataStore.Clear();
                        My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                        My_Report[":P_RECEIPT_ID"] = ReceiptRepo.Receipt_Id;

                        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                        if (!(System.IO.Directory.Exists(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name)))
                        {
                            System.IO.Directory.CreateDirectory(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name);
                        }

                        if (!(System.IO.File.Exists(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name + @"\" + NEW_DATE + ".pdf")))
                        {
                            System.IO.File.Delete(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name + @"\" + NEW_DATE + ".pdf");
                        }
                        DocumentName = (path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name + @"\" + NEW_DATE + ".pdf");

                        My_Report.Render();
                        My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);
                        ////return StiMvcViewer.GetReportSnapshotResult(My_Report);

                    }
                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Successful",
                        Message = "Payment Confirmation Successfully Downloaded",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                }
                return this.Direct();
            }

            catch (Exception EX)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not download Payment Advice",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }


        public ActionResult SendRecord(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                if (String.IsNullOrEmpty(ReceiptRepo.Receipt_Id))

                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Payment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

               

                string queryString = "select * from VW_RECEIPT_EMPLOYER where receipt_id = '" + ReceiptRepo.Receipt_Id + "' ";
                string queryinternet = "select * from setup_company";


                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.


                    while (reader.Read())
                    {

                        ReceiptRepo.Scheme_Name = (string)reader["Scheme_Name"];
                        ReceiptRepo.Employer_Name = (string)reader["Employer_Name"];
                        ReceiptRepo.Contact_Email = (string)reader["Contact_Email"];
                        ReceiptRepo.Actual_Receipt_Date = (DateTime)reader["Actual_Receipt_Date"];
                        NEW_DATE = ReceiptRepo.Actual_Receipt_Date.ToString().Replace("/", ".");
                        NEW_DATE = NEW_DATE.Replace(":", ".");


                        if (String.IsNullOrEmpty(ReceiptRepo.Contact_Email))

                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Sorry! No Email Address. Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });

                            return this.Direct();
                        }

                        if (Regex.IsMatch(((string)reader["Contact_Email"]), MatchEmailPattern) == false)
                        {
                            //log this
                        }

                        else
                        {

                            ReceiptRepo.Contact_Email = (string)reader["Contact_Email"];
                            string DocumentName = "NA";
                            string pa = Server.MapPath("~/Penfad_Reports/employer_Payment_Confirmation.dll");

                            System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                            StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                            //////asign constring
                            My_Report.Dictionary.DataStore.Clear();

                            My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                            My_Report[":P_RECEIPT_ID"] = ReceiptRepo.Receipt_Id;

                            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                            if (!(System.IO.Directory.Exists(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name)))
                            {
                                System.IO.Directory.CreateDirectory(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name);
                            }

                            if (!(System.IO.File.Exists(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name + @"\" + NEW_DATE + ".pdf")))
                            {
                                System.IO.File.Delete(path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name + @"\" + NEW_DATE + ".pdf");
                            }
                            DocumentName = (path + @"\" + "PAYMENT_CONFIRMATION" + @"\" + ReceiptRepo.Scheme_Name + @"\" + ReceiptRepo.Employer_Name + @"\" + NEW_DATE + ".pdf");

                            My_Report.Render();
                            My_Report.ExportDocument(StiExportFormat.Pdf, DocumentName);

                            #region send email

                            OracleCommand commandinternet = new OracleCommand(queryinternet, connection);
                            //connection.Open();
                            OracleDataReader readerinternet;
                            readerinternet = commandinternet.ExecuteReader();
                            // Always call Read before accessing data.
                            while (readerinternet.Read())
                            {
                                internetRepo.smtp = (string)readerinternet["smtp"];
                                internetRepo.email_from = (string)readerinternet["email_from"];
                                internetRepo.email_password = (string)readerinternet["email_password"];
                                internetRepo.port = Convert.ToInt16(readerinternet["port"]);
                                internetRepo.company_name = (string)readerinternet["company_name"];
                            }

                            var msg = $@"Dear {ReceiptRepo.Contact_Person},   Kindly find attached, Payment Confirmation for  {ReceiptRepo.Scheme_Name}.    Thank you.  {internetRepo.company_name}";
                            string from = internetRepo.email_from, pass = internetRepo.email_password, subj = "Payment Confirmation", to = ReceiptRepo.Contact_Email;
                            string smtp = internetRepo.smtp;
                            int port = internetRepo.port;
                            string attach = DocumentName;
                            internetRepo.SendIt4(from, pass, subj, msg, to, smtp, port, ReceiptRepo, attach);

                            #endregion

                        }

                    }
                    // Always call Close when done reading.
                    reader.Close();
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Sent",
                        Message = "Emails Successfully Sent",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                }
                return this.Direct();
            }

            catch (Exception ex)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Email traffic issue. Process aborted",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }


    }
}



