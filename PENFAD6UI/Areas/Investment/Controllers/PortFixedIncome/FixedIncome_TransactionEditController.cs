//using Ext.Net;
//using Ext.Net.MVC;
//using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace PENFAD6UI.Areas.Investment.Controllers.PortFixedIncome
//{
//    public class FixedIncome_TransactionEditController : Controller
//    {
//        readonly Invest_Trans_Fix_Repo TransFixRepo = new Invest_Trans_Fix_Repo();
//        // GET: Investment/FixedIncome_TransactionEdit
//        public ActionResult Index()
//        {
//            return View();
//        }
//        public ActionResult UpdFixedIncomeTransTab(string containerId = "MainArea")
//        {
//            var pvr = new Ext.Net.MVC.PartialViewResult
//            {
//                ViewName = "UpdFixedIncomeTransPartial",


//                //Model = RemitInitialRepo.GetEmployerList(),
//                ContainerId = containerId,
//                RenderMode = RenderMode.AddTo,

//            };
//            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

//            return pvr;
//        }
//        public ActionResult ClearControls()
//        {
//            try
//            {
//                var x = X.GetCmp<FormPanel>("frm_UpdateFixedDepositTransPartial");
//                x.Reset();
//                return this.Direct();
//            }
//            catch (System.Exception ex)
//            {
//                X.Msg.Show(new MessageBoxConfig
//                {
//                    Title = "Error",
//                    Message = ex.ToString(),
//                    Buttons = MessageBox.Button.OK,
//                    Icon = MessageBox.Icon.ERROR,
//                    Width = 350

//                });
//                return this.Direct();
//            }
//        }
//        public ActionResult Editing_Submit_FixedIncTransaction(Invest_Trans_Fix_Repo FixedRepo)
//        {
//            try
//            {


//                if (this.ModelState.IsValid)
//                {
//                    if (string.IsNullOrEmpty(FixedRepo.Account_No.ToString()))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = " Select Account No. Operation Aborted",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Account_Name))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Account  Name.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Scheme_Id))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Scheme.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }

//                    if (string.IsNullOrEmpty(FixedRepo.Product_Id))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Products Name.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Security_Type))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Interest Type.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    //if (string.IsNullOrEmpty(FixedRepo.Fund_Manager))
//                    //{
//                    //    X.Msg.Show(new MessageBoxConfig
//                    //    {
//                    //        Title = "Error",
//                    //        Message = "Select Fund Manager .Process aborted.",
//                    //        Buttons = MessageBox.Button.OK,
//                    //        Icon = MessageBox.Icon.INFO,
//                    //        Width = 350
//                    //    });
//                    //    return this.Direct();
//                    //}

//                    if (string.IsNullOrEmpty(FixedRepo.Auth_Id))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Authorizer.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
                  


//                    if (FixedRepo.Duration_In_Days <= 0)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Tenor in Days.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (FixedRepo.Interest_Day_Basic <= 0)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Interest Day Base.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }


//                    if (Microsoft.VisualBasic.Information.IsNumeric(FixedRepo.Amount_Invested))
//                    {
//                        FixedRepo.Amount_Invested = Convert.ToDecimal(FixedRepo.Amount_Invested);
//                        if (FixedRepo.Amount_Invested < 0)
//                        {
//                            X.Msg.Show(new MessageBoxConfig
//                            {
//                                Title = "Error",
//                                Message = "Invalid   Amount  Invested for " + FixedRepo.Account_Name,
//                                Buttons = MessageBox.Button.OK,
//                                Icon = MessageBox.Icon.ERROR,
//                                Width = 350

//                            });
//                            return this.Direct();
//                        }
//                    }
//                    else
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Invalid Amount Invested for " + FixedRepo.Account_Name,
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350

//                        });
//                        return this.Direct();

//                    }
//                    ////Validate Cash Balance 
//                    if (Microsoft.VisualBasic.Information.IsNumeric(FixedRepo.Amount_Invested > FixedRepo.Avaliable_Balance))
//                    {
//                        //.Unit_Price = Convert.ToDecimal(PurchaseLog_Repo.Unit_Price > PurchaseLog_Repo.Cash_Balance);
//                        if (FixedRepo.Amount_Invested > FixedRepo.Avaliable_Balance)
//                        {
//                            X.Msg.Show(new MessageBoxConfig
//                            {
//                                Title = "Error",
//                                Message = "Invalid Amount Invested  can not be more than Cash Balance.Process Aborted for " + FixedRepo.Account_Name,
//                                Buttons = MessageBox.Button.OK,
//                                Icon = MessageBox.Icon.ERROR,
//                                Width = 350

//                            });
//                            return this.Direct();
//                        }
//                    }
//                    else
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Invalid  Amount Invested  for " + FixedRepo.Account_Name,
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350

//                        });
//                        return this.Direct();

//                    }
//                    ////if (string.IsNullOrEmpty(FixedRepo.product_Name))
//                    ////{
//                    ////    X.Msg.Show(new MessageBoxConfig
//                    ////    {
//                    ////        Title = "Error",
//                    ////        Message = "Select Product Name.Process aborted.",
//                    ////        Buttons = MessageBox.Button.OK,
//                    ////        Icon = MessageBox.Icon.INFO,
//                    ////        Width = 350
//                    ////    });
//                    ////    return this.Direct();
//                    ////}
//                    //if (string.IsNullOrEmpty(FixedRepo.Bank_Account_No))
//                    //{
//                    //    X.Msg.Show(new MessageBoxConfig
//                    //    {
//                    //        Title = "Error",
//                    //        Message = "Select Bank Account.Process aborted.",
//                    //        Buttons = MessageBox.Button.OK,
//                    //        Icon = MessageBox.Icon.INFO,
//                    //        Width = 350
//                    //    });
//                    //    return this.Direct();
//                    //}

//                    ////add Fixed Income Investment
//                    if (this.TransFixRepo.UpDate_Submit_FixedIncTransaction(FixedRepo))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Success",
//                            Message = "Transaction submitted for approval successfully.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });

//                        Store store = X.GetCmp<Store>("UpdateFixedDepositTransPartialaccounttore");
//                        store.RemoveAll ();

//                        ClearControls();
//                    }
//                    return this.Direct();
//                }
//                else
//                {
//                    //var errors1 = ModelState .Where(x => x.Value.Errors.Count > 0) .Select(x => new { x.Key, x.Value.Errors }).ToArray();
//                    //var errors = ModelState.Values.SelectMany(v => v.Errors);

//                    string messages = string.Join(Environment.NewLine, ModelState.Values
//                                        .SelectMany(x => x.Errors)
//                                        .Select(x => x.ErrorMessage));

//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = messages, // " Insufficient data. Operation Aborted",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.ERROR,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }

//                //return this.Direct();


//            }
//            catch (Exception ex)
//            {
//                string sss = ex.ToString();
//                X.Msg.Show(new MessageBoxConfig
//                {
//                    Title = "Error",
//                    Message = "This Transaction submitted not Approved. Process Aborted",
//                    Buttons = MessageBox.Button.OK,
//                    Icon = MessageBox.Icon.ERROR,
//                    Width = 350
//                });
//                return this.Direct();

//            }
//        }
//        public ActionResult Compute_InterestAndMaturitydate(Invest_Trans_Fix_Repo TransFixRepo)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(TransFixRepo.Product_Id))
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Select Products Name.Process aborted.",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.INFO,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }
//                if (string.IsNullOrEmpty(TransFixRepo.Security_Type))
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Select Interest Type.Process aborted.",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.INFO,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }
//                if (string.IsNullOrEmpty(TransFixRepo.Fund_Manager_Id))
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Select Fund Manager .Process aborted.",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.INFO,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }

//                if (string.IsNullOrEmpty(TransFixRepo.Auth_Id))
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Select Authorizer.Process aborted.",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.INFO,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }

//                //if (TransFixRepo.Start_Date = DateTime.Today)
//                //{
//                //    X.Msg.Show(new MessageBoxConfig
//                //    {
//                //        Title = "Error",
//                //        Message = "Select Transaction Investment Start Date.Process aborted.",
//                //        Buttons = MessageBox.Button.OK,
//                //        Icon = MessageBox.Icon.INFO,
//                //        Width = 350

//                //    });
//                //    return this.Direct();

//                //}

//                if (TransFixRepo.Duration_In_Days <= 0)
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Select Tenor in Days.Process aborted.",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.INFO,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }
//                if (TransFixRepo.Interest_Day_Basic <= 0)
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Select Interest Day Base.Process aborted.",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.INFO,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }
//                if (Microsoft.VisualBasic.Information.IsNumeric(TransFixRepo.Amount_Invested))
//                {
//                    TransFixRepo.Amount_Invested = Convert.ToDecimal(TransFixRepo.Amount_Invested);
//                    if (TransFixRepo.Amount_Invested <= 0)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Invalid   Amount  Invested for " + TransFixRepo.Account_Name,
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350

//                        });
//                        return this.Direct();
//                    }
//                }
//                else
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Invalid Amount Invested for " + TransFixRepo.Account_Name,
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.ERROR,
//                        Width = 350

//                    });
//                    return this.Direct();

//                }
//                if (Microsoft.VisualBasic.Information.IsNumeric(TransFixRepo.Annual_Int_Rate))
//                {
//                    TransFixRepo.Annual_Int_Rate = Convert.ToDecimal(TransFixRepo.Annual_Int_Rate);
//                    if (TransFixRepo.Annual_Int_Rate <= 0)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Invalid   Annual Interest Rate for " + TransFixRepo.Account_Name,
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350

//                        });
//                        return this.Direct();
//                    }
//                }
//                else
//                {
//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = "Invalid Annual Interest  Rate for " + TransFixRepo.Account_Name,
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.ERROR,
//                        Width = 350

//                    });
//                    return this.Direct();

//                }
//                ////Interest on Maturity
//                decimal Interest_Amt = 0;
//                decimal An_Int_Rate = TransFixRepo.Annual_Int_Rate / 100;
//                decimal daily_rate = An_Int_Rate / TransFixRepo.Interest_Day_Basic;
//                decimal Periodrate_Customer = TransFixRepo.Duration_In_Days * daily_rate;
//                decimal Principal = TransFixRepo.Amount_Invested;
//                string Simple_Compunding = TransFixRepo.Interest_Type;
//                string Security_Type = TransFixRepo.Security_Type;
//                decimal Interest_DayBasis = TransFixRepo.Interest_Day_Basic;
//                decimal NoOf_Days_RemainingToComplete = TransFixRepo.Duration_In_Days;
//                Simple_Compunding = Simple_Compunding.ToUpper();
//                if (Simple_Compunding == "SIMPLE INTEREST")
//                {
//                    Interest_Amt = (Periodrate_Customer * Convert.ToDecimal(Principal));
//                    if (Security_Type.ToUpper() == "TREASURY BILL")
//                    {
//                        Interest_Amt = (Periodrate_Customer * Convert.ToDecimal(Principal));

//                    }
//                }
//                else if (Simple_Compunding == "ANNUAL COMPOUNDING")
//                {
//                    decimal Day_ss = NoOf_Days_RemainingToComplete / Convert.ToInt32(Interest_DayBasis);
//                    // decimal Maturity_Value = Principal * (Math.Round((1 + An_Int_Rate), Day_ss));
//                    decimal Maturity_Value = Principal * (1 + An_Int_Rate) + Day_ss;
//                    Interest_Amt = Maturity_Value - Principal;

//                }
//                else if (Simple_Compunding == "FLAT RATE")
//                {
//                    Interest_Amt = Periodrate_Customer * Convert.ToDecimal(TransFixRepo.Amount_Invested);
//                    // ''ElseIf Simple_Compunding = "Amortization Daily" Then
//                    // ''    Interest_Amt = FormatNumber(Periodrate_Customer * CDbl(Principal), 2)
//                }


//                decimal ddIn = Math.Round(Interest_Amt, 2);
//                var tot_tal_Int_On_Mat = X.GetCmp<NumberField>("frmeditInterest_On_Maturity");
//                tot_tal_Int_On_Mat.SetValue(ddIn);

//                decimal dd = Math.Round(Interest_Amt + TransFixRepo.Amount_Invested, 2);
//                var tot_tal = X.GetCmp<NumberField>("frmeditAmount_on_txtMaturity");
//                tot_tal.SetValue(dd);

//                decimal TenorIndays = TransFixRepo.Duration_In_Days;
//                DateTime Invest_StartDate = TransFixRepo.Start_Date.Date.AddDays(Convert.ToInt32(TenorIndays));
//                var end_date = X.GetCmp<DateRangeField>("dateeditMaturity_Date");
//                end_date.SetValue(Invest_StartDate);
//                //if (Information.IsNumeric(this.TransFixRepo.Duration_In_Days) == true)
//                //  {
//                //TransFixRepo.Maturity_Date = TransFixRepo.Start_Date.Date.AddDays(Convert.ToInt32(TransFixRepo.Duration_In_Days));

//                // }


//                return this.Direct();

              
//            }
//            catch (System.Exception)
//            {
//                return this.Direct();
//                // throw;
//            }
//        }
//        public ActionResult ReadPending_Transaction()
//        {
//            return this.Store(TransFixRepo.GetInvest_EditingFixedIncome_List());
//        }
//        //return this.Direct();
//        public ActionResult Update_Submit_FixedIncTransaction(Invest_Trans_Fix_Repo FixedRepo)
//        {
//            try
//            {


//                if (this.ModelState.IsValid)
//                {
//                    if (string.IsNullOrEmpty(FixedRepo.Account_No.ToString()))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = " Select Account No. Operation Aborted",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Account_Name))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Account  Name.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Scheme_Name))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Scheme.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }

//                    if (string.IsNullOrEmpty(FixedRepo.product_Name))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Products Name.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Security_Type))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Interest Type.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.Fund_Manager))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Fund Manager .Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }

//                    if (string.IsNullOrEmpty(FixedRepo.Full_Name))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Authorizer.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (FixedRepo.Start_Date == DateTime.Today)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Transaction Instment Start  Date can not be  more than  Today.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }


//                    if (FixedRepo.Duration_In_Days <= 0)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Tenor in Days.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    if (FixedRepo.Interest_Day_Basic <= 0)
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Interest Day Base.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }


//                    if (Microsoft.VisualBasic.Information.IsNumeric(FixedRepo.Amount_Invested))
//                    {
//                        FixedRepo.Amount_Invested = Convert.ToDecimal(FixedRepo.Amount_Invested);
//                        if (FixedRepo.Amount_Invested < 0)
//                        {
//                            X.Msg.Show(new MessageBoxConfig
//                            {
//                                Title = "Error",
//                                Message = "Invalid   Amount  Invested for " + FixedRepo.Account_Name,
//                                Buttons = MessageBox.Button.OK,
//                                Icon = MessageBox.Icon.ERROR,
//                                Width = 350

//                            });
//                            return this.Direct();
//                        }
//                    }
//                    else
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Invalid Amount Invested for " + FixedRepo.Account_Name,
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350

//                        });
//                        return this.Direct();

//                    }
//                    ////Validate Cash Balance 
//                    if (Microsoft.VisualBasic.Information.IsNumeric(FixedRepo.Amount_Invested > FixedRepo.Avaliable_Balance))
//                    {
//                        //.Unit_Price = Convert.ToDecimal(PurchaseLog_Repo.Unit_Price > PurchaseLog_Repo.Cash_Balance);
//                        if (FixedRepo.Amount_Invested > FixedRepo.Avaliable_Balance)
//                        {
//                            X.Msg.Show(new MessageBoxConfig
//                            {
//                                Title = "Error",
//                                Message = "Invalid Amount Invested  can not be more than Cash Balance.Process Aborted for " + FixedRepo.Account_Name,
//                                Buttons = MessageBox.Button.OK,
//                                Icon = MessageBox.Icon.ERROR,
//                                Width = 350

//                            });
//                            return this.Direct();
//                        }
//                    }
//                    else
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Invalid  Amount Invested  for " + FixedRepo.Account_Name,
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.ERROR,
//                            Width = 350

//                        });
//                        return this.Direct();

//                    }
//                    if (string.IsNullOrEmpty(FixedRepo.product_Name))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Error",
//                            Message = "Select Product Name.Process aborted.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });
//                        return this.Direct();
//                    }
//                    //if (string.IsNullOrEmpty(FixedRepo.Bank_Account_No))
//                    //{
//                    //    X.Msg.Show(new MessageBoxConfig
//                    //    {
//                    //        Title = "Error",
//                    //        Message = "Select Bank Account.Process aborted.",
//                    //        Buttons = MessageBox.Button.OK,
//                    //        Icon = MessageBox.Icon.INFO,
//                    //        Width = 350
//                    //    });
//                    //    return this.Direct();
//                    //}

//                    ////add Fixed Income Investment
//                    if (this.TransFixRepo.Approve_Submit_Trans(FixedRepo))
//                    {
//                        X.Msg.Show(new MessageBoxConfig
//                        {
//                            Title = "Success",
//                            Message = "Transaction saved for future editing successfully.",
//                            Buttons = MessageBox.Button.OK,
//                            Icon = MessageBox.Icon.INFO,
//                            Width = 350
//                        });

//                        Store store = X.GetCmp<Store>("frm_UpdateFixedDepositTransPartial");
//                        store.Reload();

//                        ClearControls();
//                    }
//                    return this.Direct();
//                }
//                else
//                {
//                    //var errors1 = ModelState .Where(x => x.Value.Errors.Count > 0) .Select(x => new { x.Key, x.Value.Errors }).ToArray();
//                    //var errors = ModelState.Values.SelectMany(v => v.Errors);

//                    string messages = string.Join(Environment.NewLine, ModelState.Values
//                                        .SelectMany(x => x.Errors)
//                                        .Select(x => x.ErrorMessage));

//                    X.Msg.Show(new MessageBoxConfig
//                    {
//                        Title = "Error",
//                        Message = messages, // " Insufficient data. Operation Aborted",
//                        Buttons = MessageBox.Button.OK,
//                        Icon = MessageBox.Icon.ERROR,
//                        Width = 350
//                    });
//                    return this.Direct();
//                }

//                //return this.Direct();


//            }
//            catch (Exception ex)
//            {
//                string sss = ex.ToString();
//                X.Msg.Show(new MessageBoxConfig
//                {
//                    Title = "Error",
//                    Message = "This Transaction submitted not Approved. Process Aborted",
//                    Buttons = MessageBox.Button.OK,
//                    Icon = MessageBox.Icon.ERROR,
//                    Width = 350
//                });
//                return this.Direct();

//            }
//        }
//        public ActionResult DeleteRecord(string invest_no)
//        {
//            if (invest_no == string.Empty)
//            {
//                X.Msg.Show(new MessageBoxConfig
//                {
//                    Title = "Error",
//                    Message = "Please select a Investment No. to delete.",
//                    Buttons = MessageBox.Button.OK,
//                    Icon = MessageBox.Icon.ERROR,
//                    Width = 350


//                });
//                return this.Direct();
//            }
//            if (string.IsNullOrEmpty(TransFixRepo.product_Name))
//            {
//                X.Msg.Show(new MessageBoxConfig
//                {
//                    Title = "Error",
//                    Message = "Select Products Name.Process aborted.",
//                    Buttons = MessageBox.Button.OK,
//                    Icon = MessageBox.Icon.INFO,
//                    Width = 350
//                });
//                return this.Direct();
//            }

//            TransFixRepo.DeleteRecord(invest_no);


//            X.Msg.Show(new MessageBoxConfig
//            {
//                Title = "Success",
//                Message = "Transaction deleted successfully.",
//                Buttons = MessageBox.Button.OK,
//                Icon = MessageBox.Icon.INFO,
//                Width = 350


//            });

//            Store store = X.GetCmp<Store>("authorizerstore");
//            store.Reload();
//            var reset = X.GetCmp<FormPanel>("authorizerstore");
//            reset.Reset();

//            return this.Direct();
//        }
//        // filter GL Account for scheme
//        public ActionResult GetpID(string Class_Id)
//        {
//            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
//            var mydata = misdepartmentrepo.GetFixedIncPList(Class_Id);

//            List<object> data = new List<object>();
//            foreach (var ddd in mydata)
//            {
//                string id = ddd.Product_Id;
//                string name = ddd.product_Name;

//                data.Add(new { pId = id, pName = name });
//            }

//            return this.Store(data);
//        }
//        // Fund Manager 
//        public ActionResult GetFM(string Scheme_Id)
//        {
//            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
//            var mydata = misdepartmentrepo.GetFMList(Scheme_Id);

//            List<object> data = new List<object>();
//            foreach (var ddd in mydata)
//            {
//                string id = ddd.Fund_Manager_Id;
//                string name = ddd.Fund_Manager;

//                data.Add(new { Id = id, Name = name });
//            }

//            return this.Store(data);

//        }
//        //public ActionResult GetGLAccount_List(Invest_Trans_Fix_Repo InvestTrans_Repo)
//        //{
//        //    try
//        //    {
//        //        if (string.IsNullOrEmpty(InvestTrans_Repo.Scheme_Fund_Id))
//        //        {
//        //            X.Msg.Show(new MessageBoxConfig
//        //            {
//        //                Title = "Error",
//        //                Message = "Select Scheme and Account Name. Process Aborted",
//        //                Buttons = MessageBox.Button.OK,
//        //                Icon = MessageBox.Icon.ERROR,
//        //                Width = 350
//        //            });
//        //            return this.Direct();
//        //        }
//        //        return this.Store(TransFixRepo.Get_Scheme_GLAccountList(InvestTrans_Repo.Scheme_Fund_Id));
//        //    }
//        //    catch (System.Exception)
//        //    {
//        //        return this.Direct();
//        //        // throw;
//        //    }
//        //}
//        //Pending Fixed Income
//        public ActionResult ReadEditing_Transaction()
//        {
//            return this.Store(TransFixRepo.GetInvestEdit_FixedIncome_List());
//        }
//    }
//}