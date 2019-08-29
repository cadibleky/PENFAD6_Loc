using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Investment.Bond;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Investment.Controllers.PortFixedIncome
{
    public class ReceiptController : Controller
    {
        readonly Invest_Trans_Fix_Repo TransFixRepo = new Invest_Trans_Fix_Repo();
        readonly BondRepo BondRepo = new BondRepo();
        // GET: Investment/ApproveFixed_InCome_Transaction Richard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReceiptTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult ReceiptIntTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptInterestPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult ReceiptAccruedTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptAccruedPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        
     public ActionResult AdjustAccruedBondsTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptAccruedPartial_Bonds",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        public ActionResult ReceiptBTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptPartial_Bonds",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }


        public ActionResult ReceiptDisinvestBondsTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptDisinvestPartial_Bonds",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult ReceiptMBTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptMaturityPartial_Bonds",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult ReceiptMTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptMaturityPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult ReceiptReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptReversePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult ReceiptReverseBondsTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReceiptReversePartial_Bonds",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptFixedDepositTransPartial_main");
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

        public ActionResult ClearControls_Int()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_IntReceiptFixedDepositTransPartial_main");
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

        public ActionResult ClearControls_ReverseR()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptReverseFixedDepositTransPartial_main");
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

        public ActionResult ClearControls_ReverseR_Bonds()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptReverseBondsPartial_main");
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


        public ActionResult ClearControls_Maturity()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptMFixedDepositTransPartial_main");
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


        public ActionResult ClearControlsB()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptBFixedDepositTransPartial_main"); 
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

        public ActionResult ClearControlsB_Dis()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptDisBFixedDepositTransPartial_main");
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

        public ActionResult ClearControlsMB()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_ReceiptMBFixedDepositTransPartial_main");
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
        // SAVE ACTIVE TBILL/MM
        public ActionResult ReceiptRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for payment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (string.IsNullOrEmpty (TransFixRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Receipt Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Payment Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (TransFixRepo.Principal_Amount > 0 && TransFixRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (TransFixRepo.Principal_Amount <=0 && TransFixRepo.Interest_Amount <=0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter 'Principal Amount' and 'Interest Amount' to disinvest",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                //if (TransFixRepo.Principal_Amount <= 0 )
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal Amount can not be less or equal to 0. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                if (TransFixRepo.Principal_Amount != TransFixRepo.Principal_Bal)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal Amount must be same as Principal Balance. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (TransFixRepo.Interest_Amount > TransFixRepo.Interest_Accrued)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Accrued Interest. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(TransFixRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued - TransFixRepo.Interest_Amount;
                TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid + TransFixRepo.Interest_Amount;
                TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal - TransFixRepo.Interest_Amount;
                TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid + TransFixRepo.Principal_Amount;
                TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal - TransFixRepo.Principal_Amount;
       

                TransFixRepo.Receipt_MM_TBill_Dis(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Payment Successfully Confirmed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptPartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,

				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();

				return pvr;
				//ClearControls();
    //            Store store = X.GetCmp<Store>("Receiptfixedincomestore");
    //            store.RemoveAll();

    //            return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
               throw ex;
            }
        }

        //INTEREST RECEIPT TBILL/MM
        public ActionResult IntReceiptRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for payment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (string.IsNullOrEmpty(TransFixRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Receipt Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Receipt Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (TransFixRepo.Principal_Amount > 0 && TransFixRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if ( TransFixRepo.Interest_Amount <= 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter 'Interest Amount' to confirm interest receipt",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                //if (TransFixRepo.Principal_Amount <= 0 )
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal Amount can not be less or equal to 0. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                //if (TransFixRepo.Principal_Amount != TransFixRepo.Principal_Bal)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal Amount must be same as Principal Balance. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (TransFixRepo.Interest_Amount > TransFixRepo.Interest_Accrued)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Accrued Interest. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(TransFixRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued - TransFixRepo.Interest_Amount;
                TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid + TransFixRepo.Interest_Amount;
                TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal - TransFixRepo.Interest_Amount;
                TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid + TransFixRepo.Principal_Amount;
                TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal - TransFixRepo.Principal_Amount;


                TransFixRepo.IntReceipt_MM_TBill_Dis(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Interest Payment Successfully Confirmed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptInterestPartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,

				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();

				return pvr;

				//ClearControls_Int();
				//Store store = X.GetCmp<Store>("IntReceiptfixedincomestore");
				//store.RemoveAll();

				//return this.Direct();
			}
			catch (Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }



        // SAVE MATURED TBILL/MM
        public ActionResult MReceiptRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for payment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (string.IsNullOrEmpty(TransFixRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Receipt Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Payment Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                if (TransFixRepo.Principal_Amount > 0 && TransFixRepo.Invest_Status == "ACTIVE")
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal can not be received. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (TransFixRepo.Principal_Amount <= 0 && TransFixRepo.Interest_Amount <= 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter 'Principal Amount' or 'Interest Amount' to confirm investment payment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (TransFixRepo.Principal_Amount > TransFixRepo.Principal_Bal)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal Amount can not be more than Principal Balance. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (TransFixRepo.Interest_Amount > TransFixRepo.Interest_Accrued)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Accrued Interest. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(TransFixRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued - TransFixRepo.Interest_Amount;
                TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid + TransFixRepo.Interest_Amount;
                TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal - TransFixRepo.Interest_Amount;
                TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid + TransFixRepo.Principal_Amount;
                TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal - TransFixRepo.Principal_Amount;

                TransFixRepo.Receipt_MM_TBill(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Payment Successfully Confirmed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                //ClearControls_Maturity();
                //Store store = X.GetCmp<Store>("MReceiptfixedincomestore");
                //store.RemoveAll();

                //return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptMaturityPartial",
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


        // REVERSE TBILL/MM RECEIPT
        public ActionResult ReverseReceiptRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.TID))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment Receipt has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);

                //if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                //    if (TransFixRepo.Payment_Date > GlobalValue.Scheme_Today_Date)
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
                //        Message = "Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}

                //TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid - TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal + TransFixRepo.Interest_Amount;
                //TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid - TransFixRepo.Principal_Amount;
                //TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal + TransFixRepo.Principal_Amount;

                TransFixRepo.Receipt_MM_TBill_Reverse(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Receipt Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                //ClearControls_ReverseR();
                //Store store = X.GetCmp<Store>("ReverseReceiptfixedincomestore");
                //store.Reload();

                //return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptReversePartial",
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

        // REVERSE BONDS
        public ActionResult ReverseReceiptRecord_Bonds(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.TID))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment Receipt has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! This transaction can not be reversed. Process aborted",
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid - TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal + TransFixRepo.Interest_Amount;
                //TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid - TransFixRepo.Principal_Amount;
                //TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal + TransFixRepo.Principal_Amount;

                TransFixRepo.Receipt_MM_TBill_Reverse(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Receipt Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
				//ClearControls_ReverseR_Bonds();
				//Store store = X.GetCmp<Store>("ReverseReceiptBondsstore");
				//store.RemoveAll();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptReversePartial_Bonds",
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



        // RECEIPT BONDS
        public ActionResult ReceiptBondRecord(BondRepo BondRepo)
        {
            try  
            {
                if (string.IsNullOrEmpty(BondRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                BondRepo.GetFMRecord(BondRepo);

                if (string.IsNullOrEmpty(BondRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Receipt Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(BondRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(BondRepo.Scheme_Id);
                    if (BondRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Receipt Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (BondRepo.Principal_Amount > 0 && BondRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (BondRepo.Interest_Amount <= 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter 'Interest Amount' to confirm Interest payment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                //if (BondRepo.Principal_Amount > BondRepo.Principal_Bal)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal Amount can not be more than Principal Balance. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (BondRepo.Interest_Amount > BondRepo.Interest_Payable_Bal)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Interest Payable. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(BondRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                BondRepo.Interest_Accrued = BondRepo.Interest_Accrued - BondRepo.Interest_Amount;
                BondRepo.Interest_Paid = BondRepo.Interest_Paid + BondRepo.Interest_Amount;
                BondRepo.Interest_Bal = BondRepo.Interest_Bal - BondRepo.Interest_Amount;
                BondRepo.Principal_Paid = BondRepo.Principal_Paid + BondRepo.Principal_Amount;
                BondRepo.Principal_Bal = BondRepo.Principal_Bal - BondRepo.Principal_Amount;

                BondRepo.Receipt_Bond(BondRepo);

                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Payment Successfully Confirmed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                //ClearControlsB();
                //Store store = X.GetCmp<Store>("ReceiptBfixedincomestore");
                //store.RemoveAll();

                //return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptPartial_Bonds",
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


        // DISINVEST BONDS
        public ActionResult DisReceiptBondRecord(BondRepo BondRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(BondRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                BondRepo.GetFMRecord(BondRepo);

                if (string.IsNullOrEmpty(BondRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Receipt Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(BondRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(BondRepo.Scheme_Id);
                    if (BondRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Disinvestment Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (BondRepo.Principal_Amount > 0 && BondRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (BondRepo.Principal_Amount != BondRepo.Principal_Bal)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal Amount must be equal to Principal Balance",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                //if (BondRepo.Principal_Amount > BondRepo.Principal_Bal)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal Amount can not be more than Principal Balance. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (BondRepo.Interest_Amount > BondRepo.Interest_Accrued)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Interest Accrued. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(BondRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                BondRepo.Interest_Accrued = BondRepo.Interest_Accrued - BondRepo.Interest_Amount;
                BondRepo.Interest_Paid = BondRepo.Interest_Paid + BondRepo.Interest_Amount;
                BondRepo.Interest_Bal = BondRepo.Interest_Bal - BondRepo.Interest_Amount;
                BondRepo.Principal_Paid = BondRepo.Principal_Paid + BondRepo.Principal_Amount;
                BondRepo.Principal_Bal = BondRepo.Principal_Bal - BondRepo.Principal_Amount;

                BondRepo.DisReceipt_Bond(BondRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Disinvested.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
				//ClearControlsB_Dis();
				//Store store = X.GetCmp<Store>("ReceiptDisBfixedincomestore");
				//store.RemoveAll();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptDisinvestPartial_Bonds",
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


        // RECEIPT MATURED BONDS
        public ActionResult ReceiptMBondRecord(BondRepo BondRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(BondRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                BondRepo.GetFMRecord(BondRepo);

                if (string.IsNullOrEmpty(BondRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Receipt Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(BondRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(BondRepo.Scheme_Id);
                    if (BondRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Payment Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                if (BondRepo.Principal_Amount > 0 && BondRepo.Invest_Status == "ACTIVE")
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal can not be received. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (BondRepo.Principal_Amount <= 0 && BondRepo.Interest_Amount <= 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter 'Principal Amount' or 'Interest Amount' to confirm investment payment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (BondRepo.Principal_Amount > BondRepo.Principal_Bal)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal Amount can not be more than Principal Balance. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (BondRepo.Interest_Amount > BondRepo.Interest_Accrued)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Accrued Interest. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(BondRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                BondRepo.Interest_Accrued = BondRepo.Interest_Accrued - BondRepo.Interest_Amount;
                BondRepo.Interest_Paid = BondRepo.Interest_Paid + BondRepo.Interest_Amount;
                BondRepo.Interest_Bal = BondRepo.Interest_Bal - BondRepo.Interest_Amount;
                BondRepo.Principal_Paid = BondRepo.Principal_Paid + BondRepo.Principal_Amount;
                BondRepo.Principal_Bal = BondRepo.Principal_Bal - BondRepo.Principal_Amount;

                BondRepo.Receipt_Bond(BondRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Payment Successfully Confirmed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                //ClearControlsMB();
                //Store store = X.GetCmp<Store>("ReceiptMBfixedincomestore");
                //store.RemoveAll();

                //return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReceiptMaturityPartial_Bonds",
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


        //Active Fixed Income
        public ActionResult ReadPending_Transaction()
        {

            return this.Store(TransFixRepo.GetInvest_FixedIncome_List_PAYA1());
        }


        public ActionResult ReadPendingTBill_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_TBill_List_PAYA1());
        }


      
        public ActionResult ReadPending_TransactionA1()
        {

            return this.Store(TransFixRepo.GetInvest_FixedIncome_List_PAYA1());
        }


        public ActionResult ReadPendingTBill_TransactionA1()
        {
            return this.Store(TransFixRepo.GetInvest_TBill_List_PAYA1());
        }

        //Matured Fixed Income
        public ActionResult MReadPending_Transaction()
        {
            return this.Store(TransFixRepo.MGetInvest_FixedIncome_List_PAY());
        }
        public ActionResult MReadPendingTBill_Transaction()
        {
            return this.Store(TransFixRepo.MGetInvest_TBill_List_PAY());
        }


        //Reverse  Payement
        public ActionResult ReverseReadPending_Transaction(Invest_Trans_Fix_Repo MM_TBILL)
        {
            Store store = X.GetCmp<Store>("ReverseReceiptfixedincomestore");
            store.Reload();
            store.DataBind();
            return this.Store(TransFixRepo.MGetInvest_FixedIncome_List_reverse(MM_TBILL));
        }

        public ActionResult ReverseReadPendingTBill_Transaction(Invest_Trans_Fix_Repo MM_TBILL)
        {
            Store store = X.GetCmp<Store>("ReverseReceiptfixedincomestore");
            store.Reload();
            store.DataBind();
            return this.Store(TransFixRepo.MGetInvest_TBill_List_reverse(MM_TBILL));
        }

        public ActionResult ReverseReadPendingBonds_Transaction(Invest_Trans_Fix_Repo MM_TBILL)
        {
            Store store = X.GetCmp<Store>("ReverseReceiptfixedincomestore");
            store.Reload();
            store.DataBind();
            return this.Store(TransFixRepo.ReverseReadPendingBonds_Transaction(MM_TBILL));
        }

        //Active Bonds
        public ActionResult ReadB_Transaction()
        {

            return this.Store(BondRepo.GetInvest_FixedIncome_List_Pay());
        }
        //All Active Bonds
        public ActionResult ReadB_Transaction2()
        {

            return this.Store(BondRepo.GetInvest_FixedIncome_List_Pay21());
        }

        //All Active Bonds
        public ActionResult ReadB_Transaction21()
        {

            return this.Store(BondRepo.GetInvest_FixedIncome_List_Pay21());
        }

        //Matured Bonds
        public ActionResult ReadMB_Transaction()
        {
            return this.Store(BondRepo.GetInvest_FixedIncome_List_MPay());
        }

        // filter GL Account for scheme
        public ActionResult GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { Mrp_GLB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        public ActionResult rp_GLBMB_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { rp_GLBMB_gId = id, gName = name });
            }

            return this.Store(data);
        }
        public ActionResult rp_GLB_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { rp_GLB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        public ActionResult Intrp_GLB_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { Intrp_GLB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        public ActionResult rp_GLBB_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { rp_GLBB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        public ActionResult Disrp_GLBB_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { Disrp_GLBB_gId = id, gName = name });
            }

            return this.Store(data);
        }



        //INTEREST AACRUED ADJUST -  TBILL/MM
        public ActionResult AccruedReceiptRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for adjustment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (string.IsNullOrEmpty(TransFixRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Adjustment Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Adjustment Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (TransFixRepo.Principal_Amount > 0 && TransFixRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                //if (TransFixRepo.Interest_Amount !> 0)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please enter 'Adjusted Amount' ",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

         

                TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal - TransFixRepo.Interest_Amount;
                //TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid + TransFixRepo.Principal_Amount;
                //TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal - TransFixRepo.Principal_Amount;

                if (TransFixRepo.Interest_Accrued <0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Current Interest Accrued can not be less than 0. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                TransFixRepo.AccruedReceipt_MM_TBill_Dis(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Interest Accrued Successfully Adjusted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ReceiptAccruedPartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,

                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();

                return pvr;

                //ClearControls_Int();
                //Store store = X.GetCmp<Store>("IntReceiptfixedincomestore");
                //store.RemoveAll();

                //return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }


        //INTEREST AACRUED ADJUST -  BOND
        public ActionResult AccruedReceiptRecord2(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for adjustment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (string.IsNullOrEmpty(TransFixRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Adjustment Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Adjustment Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (TransFixRepo.Principal_Amount > 0 && TransFixRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                //if (TransFixRepo.Interest_Amount !> 0)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please enter 'Adjusted Amount' ",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}



                TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal - TransFixRepo.Interest_Amount;
                //TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid + TransFixRepo.Principal_Amount;
                //TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal - TransFixRepo.Principal_Amount;

                if (TransFixRepo.Interest_Accrued < 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Current Interest Accrued can not be less than 0. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                TransFixRepo.AccruedReceipt_MM_TBill_Dis(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Interest Accrued Successfully Adjusted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ReceiptAccruedPartial_Bonds",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,

                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();

                return pvr;

                //ClearControls_Int();
                //Store store = X.GetCmp<Store>("IntReceiptfixedincomestore");
                //store.RemoveAll();

                //return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }


    }
}