
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

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class GLInitialController : Controller
    {
        readonly GLInitialRepo GLRepo = new GLInitialRepo();
        readonly GLAccountRepo GLARepo = new GLAccountRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddGLInitialTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLInitialCreatePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLInitialApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLInitialApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLInitialReversalTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLInitialReversalPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmGLInitialCreate");
                x.Reset();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmGLInitialApprove");
                x.Reset();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void ClearControls_Reversal()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmInitialdetails_Reversal");
                x.Reset();
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
                return this.Store(GLRepo.GLList(scheme_fund_id));
              
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

        public ActionResult ReadGLReversal()
        {
            try
            {
   
                return this.Store(GLRepo.GLReversalList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult SaveRecord(GLInitialRepo GLRepo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    GLRepo.SaveRecord(GLRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Initial GL Amount Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    ClearControls();
                    Store store = X.GetCmp<Store>("GLInitialStore");
                    store.Reload();
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

        public ActionResult ApproveRecord(GLInitialRepo GLRepo)
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

               
                ///approve pending purchases
                GLRepo.Approve_Initial(GLRepo);
             
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "GL Initial Transaction Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                ClearControls_Approve();
                Store store = X.GetCmp<Store>("InitialApprove_Store");
                store.Reload();

                return this.Direct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReverseRecord(GLInitialRepo GLRepo)
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

                //if (string.IsNullOrEmpty(GLRepo.Reversal_Reason))
                //{
                //    X.Mask.Hide();
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please enter reason for reversal.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.ERROR,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

              
                ///approve pending purchases
                GLRepo.Reverse_Initial(GLRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Initial GL Transaction Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                ClearControls_Reversal();
                Store store = X.GetCmp<Store>("InitialReversal_Store");
                store.Reload();

                return this.Direct();
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