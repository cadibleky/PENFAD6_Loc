
using Ext.Net;
using Ext.Net.MVC;
using IBankWebService.Utils;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Remittance.Contribution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Purchase
{
    public class Remit_PurchaseController : Controller
    {
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        Remit_PurchaseRepo PurchaseRepo = new Remit_PurchaseRepo();
        readonly crm_EmployeeRepo employee_repo = new crm_EmployeeRepo();

        public ActionResult AddPurchaseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PurchasePartial",
                // Model = ReceiptRepo.GetReceiptList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddPurchaseApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PurchaseApprovePartial",
                // Model = PurchaseRepo.GetPurchasePendingList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmPurchaseApprove");
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
                var x = X.GetCmp<FormPanel>("frmPurchaseReverse");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("Purchase_Contribution");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult AddPurchaseReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReversePurchasePartial",
                // Model = ReceiptRepo.GetReceiptList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }


        public ActionResult SaveRecord(Remit_PurchaseRepo PurchaseRepo)

        {
            try
            {
                if (PurchaseRepo.isPurchaseUnique(PurchaseRepo.Con_Log_Id) == true)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Duplicate",
                        Message = "Pending Purchase for same Contribution already exist.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }


                // validate scheme date  
                if (!string.IsNullOrEmpty(PurchaseRepo.ES_Id))
                {
                    PurchaseRepo.Scheme_Id = PurchaseRepo.ES_Id.Substring(PurchaseRepo.ES_Id.Length - 2);
                }
                if (!string.IsNullOrEmpty(PurchaseRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(PurchaseRepo.Scheme_Id);
                    if (PurchaseRepo.Trans_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! trans or purchase date can not be more than Scheme date",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });

                        return this.Direct();
                    }

                    //check if purchase date exist in remit_unit_price table
                    if (PurchaseRepo.isDate(PurchaseRepo) == false)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Invalid Purchase Date",
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
                }


                // pull cash balance from Employer-Scheme Table 
                PurchaseRepo.Cash_Balance = 0;
                PurchaseRepo.Get_Cash_Balance(PurchaseRepo);


                if (PurchaseRepo.Cash_Balance <= 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! insufficient Cash in Employer Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (PurchaseRepo.isYearMonthValid(PurchaseRepo) == true)
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Ealier Contributions must be purchased first.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

                // check if contribution is valid for purchasing.

                //check if purchase date exist in remit_unit_price table
                if (PurchaseRepo.isValidP(PurchaseRepo) == false)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! this schedule has already been purchased for.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "PurchasePartial",
                        // Model = ReceiptRepo.GetReceiptList(),
                        ContainerId = "MainArea",
                        RenderMode = RenderMode.AddTo,
                    };
                    this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                    return pvr;
                }



                if (ModelState.IsValid)
                {
                    this.PurchaseRepo.SaveRecord(PurchaseRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Purchase Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                     ClearControls();
                    Store store = X.GetCmp<Store>("purconStore");
                    store.RemoveAll();
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
            catch (Exception ex)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Unit Purchasing Failed. Process aborted",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }
        }


        public ActionResult ApproveRecord(Remit_PurchaseRepo PurchaseRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(PurchaseRepo.Purchase_Id))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Purchase' has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                // get the pending purchase record
                PurchaseRepo.GetPurchasePendingList();


                //pull cash balance from Employer-Scheme Table 
                PurchaseRepo.Cash_Balance = 0;
                PurchaseRepo.Get_Cash_Balance(PurchaseRepo);

                if (PurchaseRepo.Cash_Balance <= 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! insufficient Cash in Employer Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

                if (PurchaseRepo.isYearMonthValid(PurchaseRepo) == true)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Ealier Contributions must be purchased before. Purchase aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }
                GlobalValue.Get_Scheme_Today_Date(PurchaseRepo.Scheme_Id);
                ///approve pending purchases
                PurchaseRepo.Approve_Unit_Purchases(PurchaseRepo);
				string p_purid = PurchaseRepo.Purchase_Id;
				//ClearControls_Approve();
				//SEND SMS
				string queryString = "select * from VW_SMS_EMP_PURCHASE where PURCHASE_LOG_ID = '" + p_purid + "' ";
                using (OracleConnection connection = new OracleConnection(GlobalValue.ConString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    // Always call Read before accessing data.

                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty((string)reader["Mobile_Number"]))
                        {
                            employee_repo.Mobile_Number = "000000000";
                        }
                        else
                        {
                           employee_repo.Mobile_Number = (string)reader["Mobile_Number"];
                        }

                        employee_repo.First_Name = (string)reader["First_Name"];                      
                        employee_repo.Scheme_Name = (string)reader["Scheme_Name"];                  
                        employee_repo.SEND_SMS = (string)reader["SEND_SMS"];
                        decimal EMPLOYEE_AMT = (decimal)reader["EMPLOYEE_AMT"];
                        decimal EMPLOYER_AMT = (decimal)reader["EMPLOYER_AMT"];
                        decimal TOTAL_AMT = (decimal)reader["EMPLOYER_AMT"] + (decimal)reader["EMPLOYEE_AMT"];
                        decimal EMPLOYEE_UNIT = (decimal)reader["EMPLOYEE_UNITS"];
                        decimal EMPLOYER_UNIT = (decimal)reader["EMPLOYER_UNITS"];
                        decimal FMONTH = (decimal)reader["FOR_MONTH"];
                        decimal FYEAR = (decimal)reader["FOR_YEAR"]; 
                        decimal UNIT_PRICE = (decimal)reader["UNIT_PRICE"];
                        int cFMONTH = Convert.ToInt32(FMONTH);
                        string SMONTH = new DateTime(1900, cFMONTH, 01).ToString("MMMM");
                        string mcode = (string)reader["CUST_NO"];
                        DateTime tDate = ((DateTime)reader["TRANS_DATE"]);
                        string tDate2 = tDate.ToString("MMMM,dd,yyyy");

                        if (employee_repo.SEND_SMS == "YES")
                        {
							employee_repo.Mobile_Number = employee_repo.Mobile_Number.Replace(" ", string.Empty);
							if (employee_repo.Mobile_Number.Length < 9 || string.IsNullOrEmpty(employee_repo.Mobile_Number))
                            {
                                employee_repo.Mobile_Number = "000000000";
                            }

                            //SEND SMS
                            string smsmsg = "Dear " + employee_repo.First_Name + ", your " + employee_repo.Scheme_Name + " account with member code "+ mcode + " was credited with GHc " + TOTAL_AMT + " on " + tDate2 + " for "+ SMONTH + ","+ FYEAR+". Thank you.";
                            string fonnum = "233" + employee_repo.Mobile_Number.Substring(employee_repo.Mobile_Number.Length - 9, 9);

                            Dictionary<string, string> paramSMS = new Dictionary<string, string>();
                            paramSMS.Add("to", fonnum);
                            paramSMS.Add("text", smsmsg);
                            Request request = new Request
                            {
                                Parameters = paramSMS
                            };

                            var content = Volley.PostRequest(request);
                            //END SEND SMS
                        }

                    }

                    reader.Close();


                }

                //SEND SMS
                

                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Purchase Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "PurchaseApprovePartial",
                    // Model = PurchaseRepo.GetPurchasePendingList(),
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
               // return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Could not complete approval process. Process aborted.";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Could not complete approval process. Process aborted.";
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
             return this.Direct();
            }
        }

        public ActionResult ReverseRecord(Remit_PurchaseRepo PurchaseRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(PurchaseRepo.Purchase_Id))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Purchase' has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
                /////////////
                //if (!string.IsNullOrEmpty(PurchaseRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(PurchaseRepo.Scheme_Id);
                //    if (PurchaseRepo.Trans_Date > GlobalValue.Scheme_Today_Date)
                //    {
                //        X.Mask.Hide();
                //        X.Msg.Show(new MessageBoxConfig
                //        {
                //            Title = "Error",
                //            Message = "Sorry! This transaction can not be reversed. Process aborted",
                //            Buttons = MessageBox.Button.OK,
                //            Icon = MessageBox.Icon.INFO,
                //            Width = 350

                //        });

                //        return this.Direct();
                //    }

                //}
                //else
                //{
                //    X.Mask.Hide();
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Employer Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}

                GlobalValue.Get_Scheme_Today_Date(PurchaseRepo.Scheme_Id);         

                // get the purchase record
                PurchaseRepo.GetPurchaseReverseList();


                //pull cash balance from Employer-Scheme Table 
                PurchaseRepo.Get_Cash_Balance(PurchaseRepo);

               
                ///reverse  purchase
                PurchaseRepo.Reverse_Unit_Purchases(PurchaseRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Purchase Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ReversePurchasePartial",
                    // Model = ReceiptRepo.GetReceiptList(),
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Unit Purchase Reversal failed. Process aborted.";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Unit Purchase Reversal failed. Process aborted.";
                }
                else
                {
                    ora_code = ex.ToString();
                }
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }
        }

        public ActionResult DisapproveRecord(string Purchase_Id)
        {
            try
            {
                if (Purchase_Id == string.Empty)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Purchase' has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                PurchaseRepo.DisapprovePurchaseRecord(Purchase_Id);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Purchase Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "PurchaseApprovePartial",
                    // Model = PurchaseRepo.GetPurchasePendingList(),
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                return this.Direct();
            }
        }

        public ActionResult ReadEmployerScheme()
        {
            try
            {
                return this.Store(PurchaseRepo.GetPurchaseESList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReadPurchasePending()
        {
            try
            {
                return this.Store(PurchaseRepo.GetPurchasePendingList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReadPurchaseForReverse()
        {
            try
            {
                return this.Store(PurchaseRepo.GetPurchaseReverseList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }
        // filter employer scheme for employee
        public ActionResult FilterESGrid(string ES_Id)
        {
            try
            {

                Store store = X.GetCmp<Store>("purconStore");
                store.Reload();
                store.DataBind();

                return this.Store(PurchaseRepo.GetESCHEMEList(ES_Id));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

    }
}



