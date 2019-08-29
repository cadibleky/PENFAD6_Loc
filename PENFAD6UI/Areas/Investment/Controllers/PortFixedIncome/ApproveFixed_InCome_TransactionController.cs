using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Investment.Controllers.PortFixedIncome
{
    public class ApproveFixed_InCome_TransactionController : Controller
    {
        readonly Invest_Trans_Fix_Repo TransFixRepo = new Invest_Trans_Fix_Repo();
        // GET: Investment/ApproveFixed_InCome_Transaction Richard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ApproveFixedIncomeTransTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ApproveFixedIncomeTransPartial",
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
                var x = X.GetCmp<FormPanel>("frm_ApproveFixedDepositTransPartial_main");
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

        //disapprove investment
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
				//Store store = X.GetCmp<Store>("Approvefixedincomestore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ApproveFixedIncomeTransPartial",
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


        public ActionResult ApproveRecord(Invest_Trans_Fix_Repo TransFixRepo)
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

                if ((TransFixRepo.GL_Balance * -1) < TransFixRepo.Amount_Invested)
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

                // update Invest_Equity table
                //Update Invest_Equity_Balance table
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
				//Store store = X.GetCmp<Store>("Approvefixedincomestore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ApproveFixedIncomeTransPartial",
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

      
        //Pending Fixed Income
        public ActionResult ReadPending_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_FixedIncome_List());
        }

        public ActionResult ReadPendingTBill_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_TBill_List());
        }


    }
}