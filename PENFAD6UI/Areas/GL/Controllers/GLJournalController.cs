
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using System.Text;
using PENFAD6DAL.Repository.GL;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class GLJournalController : Controller
    {
        readonly GLJournalRepo GLRepo = new GLJournalRepo();
        readonly GLAccountRepo GLARepo = new GLAccountRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddGLJournalTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLJournalCreatePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLJournalApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLJournalApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLJournalReversalTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLJournalReversalPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmGLJournalCreate");
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
                var x = X.GetCmp<FormPanel>("frmGLJournalApprove");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Reversal()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmJournaldetails_Reversal");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    
        public ActionResult ReadCreditGL(string scheme_fund_id)
        {
            try
            {
                return this.Store(GLRepo.CreditList(scheme_fund_id));
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult ReadDebitGL(string scheme_fund_id)
        {
            try
            {
                return this.Store(GLRepo.debitList(scheme_fund_id));
              
            }

            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult ReadGLApprove()
        {
            try
            {
                return this.Store(GLRepo.GLApproveList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult ReadGLReversal(DateTime? From_Date, DateTime? To_Date)
        {
            try
            {
   
                return this.Store(GLRepo.GLReversalList(From_Date,  To_Date));
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult SaveRecord(GLJournalRepo GLRepo)
        {
            try
            {
               
                if (string.IsNullOrEmpty(GLRepo.Scheme_Fund_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Scheme Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }


                if (GLRepo.Debit_Gl_No == GLRepo.Credit_Gl_No)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry Debit Account can not be same as Credit Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                // GET GL BALANCES
                GLRepo.Get_BalanCredit(GLRepo);

                //if (((GLRepo.Credit_Gl_Balance * -1) < GLRepo.Amount) && (GLRepo.Memo_Code == "101"))
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry Debit Account Balance not sufficient for transaction",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                if (!string.IsNullOrEmpty(GLRepo.Scheme_Fund_Id.Substring(0, 2)))
                {
                    GlobalValue.Get_Scheme_Today_Date((GLRepo.Scheme_Fund_Id.Substring(0, 2)));
                    if (GLRepo.Trans_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Transaction date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
                    GLRepo.SaveRecord(GLRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Journal Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "GLJournalCreatePartial",
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

        public ActionResult ApproveRecord(GLJournalRepo GLRepo)
        {
            try
            {
                if (GLRepo.TID < 1)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transaction has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                // GET GL BALANCES
                GLRepo.Get_BalanCredit(GLRepo);


                //if (((GLRepo.Credit_Gl_Balance * -1) < GLRepo.Amount) && (GLRepo.Memo_Code == "101"))
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry Debit Account Balance not sufficient for transaction",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}
                GlobalValue.Get_Scheme_Today_Date((GLRepo.Scheme_Fund_Id.Substring(0, 2)));

                ///approve pending purchases
                GLRepo.Approve_Journal(GLRepo);
             
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Journal Transaction Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLJournalApprovePartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult DisapproveRecord(GLJournalRepo GLRepo)
        {
            try
            {
                if (GLRepo.TID < 1)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transaction has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //GET GL BALANCES
                GLRepo.Get_Balance(GLRepo);

                //if ((GLRepo.Debit_Gl_Balance * -1) < GLRepo.Amount)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry Debit Account Balance not sufficient for transaction",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //    return this.Direct();
                //}


                ///approve pending purchases
                GLRepo.Disapprove_Journal(GLRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Journal Transaction Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLJournalApprovePartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReverseRecord(GLJournalRepo GLRepo)
        {
            try
            {
               
                if (GLRepo.TID < 1)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transaction has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(GLRepo.Reversal_Reason))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter reason for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                GlobalValue.Get_Scheme_Today_Date((GLRepo.Scheme_Fund_Id.Substring(0, 2)));

                //if (!string.IsNullOrEmpty(GLRepo.Scheme_Fund_Id.Substring(0, 2)))
                //{
                //    GlobalValue.Get_Scheme_Today_Date((GLRepo.Scheme_Fund_Id.Substring(0, 2)));
                //    if (GLRepo.Trans_Date != GlobalValue.Scheme_Today_Date)
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

                // GET GL BALANCES
                GLRepo.Get_BalanCredit_Rev(GLRepo);


                //if (((GLRepo.Debit_Gl_Balance * -1) < GLRepo.Amount) && (GLRepo.Memo_Code == "101"))
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! This transaction can not be reversed. Contact System Administrator.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}


                ///reverse
                GLRepo.Reverse_Journal(GLRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Journal Transaction Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLJournalReversalPartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadSF()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

    }//end class


}