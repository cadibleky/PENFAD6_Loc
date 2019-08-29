using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Investment.Bond;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Investment.Controllers.PortFixedIncome
{
    public class ApproveBondController : Controller
    {
        readonly BondRepo TransFixRepo = new BondRepo();
        // GET: Investment/ApproveFixed_InCome_Transaction Richard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddApproveBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ApproveBondPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddApproveSecBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ApproveSecBondPartial",
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
                var x = X.GetCmp<FormPanel>("frm_ApproveBondPartial_main");
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

        public ActionResult ClearControls_Sec()
        {
            try
            {
                var y = X.GetCmp<FormPanel>("frm_ApproveSecBondPartial_main");
                y.Reset();
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


        //disapprove bond
        public ActionResult DisapproveRecord(string Invest_No)
        {
            try
            {
                if (string.IsNullOrEmpty(Invest_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                TransFixRepo.DisapproveOrderRecord(Invest_No);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls();
				//Store store = X.GetCmp<Store>("Approvebondstore");
				//store.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ApproveBondPartial",
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

        // approve bond
        public ActionResult ApproveRecord(BondRepo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
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

                //get GL balance from GL_Account Table 
                TransFixRepo.Get_GL_Balance(TransFixRepo);

                if ((TransFixRepo.GL_Balance * -1) < TransFixRepo.Cost)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Insufficient Cash in Scheme-Fund Bank Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                TransFixRepo.Approve_MM_TBill(TransFixRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls();
				//Store store = X.GetCmp<Store>("Approvebondstore");
				//store.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ApproveBondPartial",
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

        // approve sec bond
        public ActionResult ApproveRecordSec(BondRepo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
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

                //get GL balance from GL_Account Table 
                TransFixRepo.Get_GL_Balance(TransFixRepo);
                if ((TransFixRepo.GL_Balance * -1) < TransFixRepo.Cost + TransFixRepo.Brokerage_Fee)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Insufficient Cash in Scheme-Fund Bank Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                TransFixRepo.Approve_MM_TBill(TransFixRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Sec();
				//Store storesec = X.GetCmp<Store>("Approvesecbondstore");
				//storesec.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ApproveSecBondPartial",
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

        public ActionResult DisapproveSecRecord(string Invest_No)
        {
            try
            {
                if (string.IsNullOrEmpty(Invest_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                TransFixRepo.DisapproveOrderRecord(Invest_No);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                //ClearControls();
                //Store store = X.GetCmp<Store>("Approvebondstore");
                //store.Reload();
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ApproveSecBondPartial",
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


        //Pending PRIMARY BONDS
        public ActionResult ReadPending_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_FixedIncome_List());
        }

        //Pending SECONDARY BONDS
        public ActionResult Sec_ReadPending_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_TBill_List());
        }


    }
}