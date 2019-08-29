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
    public class ReverseBondController : Controller
    {
        readonly BondRepo TransFixRepo = new BondRepo();
        // GET: Investment/ApproveFixed_InCome_Transaction Richard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddReverseBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReverseBondPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddReverseSecBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReverseSecBondPartial",
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
                var x = X.GetCmp<FormPanel>("frm_ReverseBondPartial_main");
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
                var y = X.GetCmp<FormPanel>("frm_ReverseSecBondPartial_main");
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
				//Store store = X.GetCmp<Store>("Reversebondstore");
				//store.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReverseBondPartial",
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
        public ActionResult ReversePRecord(BondRepo MM_TBill)
        {
            try
            {
                if (string.IsNullOrEmpty(MM_TBill.Invest_No))
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
                MM_TBill.Get_GL_Balance(MM_TBill);

                //if ((TransFixRepo.GL_Balance * -1) < TransFixRepo.Amount_Invested)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! Insufficient Cash in Scheme-Fund Bank Account.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                //if (!string.IsNullOrEmpty(MM_TBill.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(MM_TBill.Scheme_Id);
                //    if (MM_TBill.Settlement_Date > GlobalValue.Scheme_Today_Date)
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


                GlobalValue.Get_Scheme_Today_Date(MM_TBill.Scheme_Id);

                MM_TBill.Reverse_MM_TBill(MM_TBill);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Bond Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReverseBondPartial",
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
        public ActionResult ReverseRecordSec(BondRepo MM_TBill)
        {
            try
            {
                if (string.IsNullOrEmpty(MM_TBill.Invest_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get GL balance from GL_Account Table 
                MM_TBill.Get_GL_Balance(MM_TBill);

                //if ((TransFixRepo.GL_Balance * -1) < TransFixRepo.Amount_Invested)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! Insufficient Cash in Scheme-Fund Bank Account.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}
                GlobalValue.Get_Scheme_Today_Date(MM_TBill.Scheme_Id);

                //if (!string.IsNullOrEmpty(MM_TBill.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(MM_TBill.Scheme_Id);
                //    if (MM_TBill.Settlement_Date > GlobalValue.Scheme_Today_Date)
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

                MM_TBill.Reverse_MM_TBill(MM_TBill);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Sec();
				//Store storesec = X.GetCmp<Store>("Reversedsecbondstore");
				//storesec.Reload();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "ReverseSecBondPartial",
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



        //new PRIMARY BONDS
        public ActionResult ReadPending_Transaction_Rev()
        {
            return this.Store(TransFixRepo.GetInvest_FixedIncome_List_Rev());
        }

        //new SECONDARY BONDS
        public ActionResult Sec_ReadPending_Transaction()
        {
            return this.Store(TransFixRepo.GetInvest_FixedIncome_List_SecRev());
        }


    }
}