using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Investment.Controllers.PortFixedIncome
{
    public class DisinvestFixed_InCome_TransactionController : Controller
    {
        readonly Invest_Trans_Fix_Repo TransFixRepo = new Invest_Trans_Fix_Repo();
        // GET: Investment/ApproveFixed_InCome_Transaction Richard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DisinvestTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "DisinvestFixedIncomeTransPartial",
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
                var x = X.GetCmp<FormPanel>("frm_DisinvestFixedDepositTransPartial_main");
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

       
        public ActionResult ReverseRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.Invest_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for Reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get GL balance from GL_Account Table 
                TransFixRepo.Get_GL_Balance(TransFixRepo);

                GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);

                //if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                //    if (TransFixRepo.Settlement_Date > GlobalValue.Scheme_Today_Date)
                //    {
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
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}


                // update Invest_Equity table
                //Update Invest_Equity_Balance table
                TransFixRepo.Reverse_MM_TBill(TransFixRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                //ClearControls();
                //Store store = X.GetCmp<Store>("Disinvestfixedincomestore");
                //store.RemoveAll();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "DisinvestFixedIncomeTransPartial",
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

      
        //Active Fixed Income
        public ActionResult ReadPending_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_FixedIncome_List_A());
        }

        public ActionResult ReadPendingTBill_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_TBill_List_A());
        }

        // for reversal
        public ActionResult ReadReversePending_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_FixedIncome_List_Re());
        }

        public ActionResult ReadReverseTBill_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_TBill_List_Re());
        }


    }
}