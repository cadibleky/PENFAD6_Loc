
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using System.Collections.Generic;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.Investment.Controllers.Equity_CIS
{
    public class Equity_CIS_SellController : Controller  
    {
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly pfm_FundManagerRepo FundManagerRepo = new pfm_FundManagerRepo();
        readonly Invest_Equity_CISRepo Equity_CISRepo = new Invest_Equity_CISRepo();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEquity_CISTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Equity_CIS_SellPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddEquity_CISApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Equity_CIS_SellApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddEquity_CISReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Equity_CIS_SellReversePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls_Reverse()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmEquityCIS_SellReverse");
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
                var x = X.GetCmp<FormPanel>("frmEquityCIS_SellApprove");
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
                var x = X.GetCmp<FormPanel>("frmAddEquity_CIS_Sell");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        

 

        public ActionResult SaveRecord(Invest_Equity_CISRepo Equity_CISRepo)

        {
            try
            {
                if (Equity_CISRepo.isOrderUnique(Equity_CISRepo.Invest_No) == true)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Duplicate",
                        Message = "Order already exist.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get current product quantity from Equity Balance table Table 
                Equity_CISRepo.Get_Product_Current_Quantity(Equity_CISRepo);

                if ((Equity_CISRepo.Order_Quantity) > Equity_CISRepo.Current_Quantity)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Current Quantity is not enough for this transaction.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (!string.IsNullOrEmpty(Equity_CISRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(Equity_CISRepo.Scheme_Id);
                    if (Equity_CISRepo.Order_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry!  Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }


                if (ModelState.IsValid)
            {
                this.Equity_CISRepo.sellSaveRecord(Equity_CISRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Order Successfully Processed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
					//ClearControls();
					//    Store store = X.GetCmp<Store>("ecis_SellSFStore");
					//    store.Reload();
					//    return this.Direct();
					var pvr = new Ext.Net.MVC.PartialViewResult
					{
						ViewName = "Equity_CIS_SellPartial",
						ContainerId = "MainArea",
						RenderMode = RenderMode.AddTo,
					};
					this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
					return pvr;
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
                throw ex;
            }
        }
     
        public ActionResult ApproveRecord(Invest_Equity_CISRepo Equity_CISRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(Equity_CISRepo.Invest_No))
                {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Order has been selected for approval.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

                //get current product quantity from Equity Balance table Table 
                Equity_CISRepo.Get_Product_Current_Quantity(Equity_CISRepo);

                if ((Equity_CISRepo.Order_Quantity) > Equity_CISRepo.Current_Quantity)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Current Quantity is not enough for this transaction.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


           Equity_CISRepo.Approve_Equity_CIS_Sell(Equity_CISRepo); 


            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Order Successfully Approved.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
				//ClearControls_Approve();
				//Store store = X.GetCmp<Store>("ApproveECIS_SellStore");
				//store.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "Equity_CIS_SellApprovePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;

            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReverseRecord(Invest_Equity_CISRepo Equity_CISRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(Equity_CISRepo.Invest_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No transaction has been selected.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get current product quantity from Equity Balance table Table 
                Equity_CISRepo.Get_Product_Current_Quantity(Equity_CISRepo);

                //if ((Equity_CISRepo.Order_Quantity) > Equity_CISRepo.Current_Quantity)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! Current Quantity is not enough for this transaction.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                GlobalValue.Get_Scheme_Today_Date(Equity_CISRepo.Scheme_Id);

                //if (!string.IsNullOrEmpty(Equity_CISRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(Equity_CISRepo.Scheme_Id);
                //    if (Equity_CISRepo.Order_Date != GlobalValue.Scheme_Today_Date)
                //    {
                //        X.Msg.Show(new MessageBoxConfig
                //        {
                //            Title = "Error",
                //            Message = "Sorry!  This transaction can not be reversed. Process aborted",
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
                //        Message = "Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}


                Equity_CISRepo.Reverse_Equity_CIS_Sell(Equity_CISRepo);


                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Transaction Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Reverse();
				//Store store = X.GetCmp<Store>("ReverseECIS_SellStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "Equity_CIS_SellReversePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;
			}
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult DisapproveRecord(string Invest_No)
        {
            try
            {
                if (string.IsNullOrEmpty(Invest_No))
                {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Order has been selected for disapproval.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            Equity_CISRepo.DisapproveOrderRecord(Invest_No);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Order Successfully Disapproved.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
				//ClearControls_Approve();
				//Store store = X.GetCmp<Store>("ApproveECIS_SellStore");
				//store.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "Equity_CIS_SellApprovePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;

			}
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        // filter Fund Manager for scheme
        public ActionResult GetFM(string Scheme_Id)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetFMList(Scheme_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Fund_Manager_Id;
                string name = ddd.Fund_Manager;

                data.Add(new { ECSELLNEWFMId = id, Name = name });
            }

            return this.Store(data);

        }        

        // filter GL Account for scheme
        public ActionResult GetpID(string Class_Id)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetECISPList(Class_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Product_Id;
                string name = ddd.Product_Name;

                data.Add(new { pId = id, pName = name });
            }

            return this.Store(data);
        }


        // get total cost
        public ActionResult GetTCost(Invest_Equity_CISRepo Equity_CISRepo)
        {
            try
            {
                X.GetCmp<TextField>("Equity_CIS_Sell_Current_Total_Cost").SetValue(Equity_CISRepo.Order_Quantity * Equity_CISRepo.CURRENTAVERAGEUNITCOST);

                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

  
        // filter GL Account for scheme
        public ActionResult GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { ECNEWId = id, gName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme
        public ActionResult GetGLacc(string GL_Account_No)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetGLAccFList(GL_Account_No);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                decimal id = ddd.GL_Balance * -1;
                decimal name = ddd.GL_Balance * -1;

                data.Add(new { ECNEWId_B = id, mName = name });
            }

            return this.Store(data);
        }

        

        public ActionResult ReadSchemeFundProduct()
        {
            try
            {
                return this.Store(Equity_CISRepo.GetSchemeFundProductList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReadECISPending()
        {
            try
            {
                return this.Store(Equity_CISRepo.GetECIS_SellPendingList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReadECISReverse()
        {
            try
            {
                return this.Store(Equity_CISRepo.GetECIS_SellReverseList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }


    }
}



