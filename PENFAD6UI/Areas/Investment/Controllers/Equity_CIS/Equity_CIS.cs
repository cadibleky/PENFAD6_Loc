
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Linq;
using System.Web.Mvc;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using System.Collections.Generic;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.Investment.Controllers.Equity_CIS   
{
    public class Equity_CISController : Controller  
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
                ViewName = "Equity_CISPartial",
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
                ViewName = "Equity_CISApprovePartial",
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
                ViewName = "Equity_CISReversePartial",
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
                var x = X.GetCmp<FormPanel>("frmEquityCISApprove");
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
                var x = X.GetCmp<FormPanel>("frmEquityCISReverse");
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
                var x = X.GetCmp<FormPanel>("frmAddEquity_CIS");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        

       

        public ActionResult AddReverseEquity_CISTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ReverseEquity_CISPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
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

                //check if issuer exist
                Equity_CISRepo.ck_issuer(Equity_CISRepo);

                if ((Equity_CISRepo.CH_NUMBER <= 0))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Unregistered Issuer",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (string.IsNullOrEmpty (Equity_CISRepo.Issuer_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Select Issuer.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }
                //get GL balance from GL_Account Table 
                Equity_CISRepo.Get_GL_Balance(Equity_CISRepo);

                if ((Equity_CISRepo.GL_Balance * -1) < Equity_CISRepo.Total_Cost)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Cash balance is not enough for this transaction",
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
                            Message = "Sorry! Order date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                this.Equity_CISRepo.SaveRecord(Equity_CISRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Order Successfully Processed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "Equity_CISPartial",
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
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Record already exist. Process aborted..";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Not all records are supplied. Process aborted..";
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
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
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

                //get GL balance from GL_Account Table 
                Equity_CISRepo.Get_GL_Balance(Equity_CISRepo);

                if ((Equity_CISRepo.GL_Balance * -1) < Equity_CISRepo.Total_Cost)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Insufficient  Cash in Scheme-Fund Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

            // update Invest_Equity table
            //Update Invest_Equity_Balance table
            Equity_CISRepo.Approve_Equity_CIS(Equity_CISRepo); 

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Order Successfully Approved.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Equity_CISApprovePartial",
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
                        Message = "Sorry! No Order has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get GL balance from GL_Account Table 
                Equity_CISRepo.Get_GL_Balance(Equity_CISRepo);

                //if ((Equity_CISRepo.GL_Balance * -1) < Equity_CISRepo.Total_Cost)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! Insufficient  Cash in Scheme Account.",
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
                Equity_CISRepo.Reverse_Equity_CIS(Equity_CISRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Order Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Equity_CISReversePartial",
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
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Equity_CISApprovePartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;

                return this.Direct();
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

                data.Add(new { Id = id, Name = name });
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

        

        public ActionResult ReadSchemeFund()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundList());
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
                return this.Store(Equity_CISRepo.GetECISPendingList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        public ActionResult ReadECISForRev()
        {
            try
            {
                return this.Store(Equity_CISRepo.GetECISReverseList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

    }
}



