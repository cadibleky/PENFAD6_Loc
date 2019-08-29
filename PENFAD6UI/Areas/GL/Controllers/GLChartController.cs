
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
using Serilog;
using PENFAD6DAL.Repository.Investment.Bond;

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class GLChartController : Controller
    {
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly GLChartRepo GLRepo = new GLChartRepo();
        readonly GLAccountRepo GLARepo = new GLAccountRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddGLChartTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLChartCreatePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLAccountTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLAccountPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLAccountBankTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLBankAccountPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLAccountExpenseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLExpenseAccountPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLAccountBankDefaultTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLBankAccountDefaultPartial",
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
                var x = X.GetCmp<FormPanel>("frmGLChartCreate");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Right()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_gl_BankAccount");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Expense()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_gl_ExpenseAccount");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public ActionResult SaveRecord(GLChartRepo GLRepo)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    GLRepo.Add_GLChart(GLRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "GL Chart Successfully Created.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    ClearControls();
                    Store store = X.GetCmp<Store>("GLChartStore");
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
                return this.Direct();
            }
        }


        public ActionResult SaveGLAccountRecord(GLAccountRepo GLARepo)
        {
            try
            {

                if (string.IsNullOrEmpty(GLARepo.Bank_Account_Number))
                {
                   
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter Bank Account No",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(GLARepo.Bank))
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Bank",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(GLARepo.Bank_Branch))
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Please enter Bank Branch",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (ModelState.IsValid)
                {
                    GLARepo.Add_GL_Account_Bank(GLARepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "GL Bank Account Successfully Created.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                     Store store = X.GetCmp<Store>("BAccSFStore");
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
                    ora_code = "Record is uniquely defined in the system. Process aborted..";
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
                return this.Direct();
            }
        }

        public ActionResult SaveGLEAccountRecord(GLAccountRepo GLARepo)
        {
            try
            {

              
                if (ModelState.IsValid)
                {
                    GLARepo.Add_GL_Account_Expense(GLARepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "GL Expense Account Successfully Created.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    Store store = X.GetCmp<Store>("EAccSFStore");
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
                    ora_code = "Record is uniquely defined in the system. Process aborted..";
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
                return this.Direct();
            }
        }

        public ActionResult SaveDefaultAcc(GLAccountRepo GLARepo)
        {
            try
            {
                if (string.IsNullOrEmpty(GLARepo.GL_Account_No))
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Bank Account",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (ModelState.IsValid)
                {
                    GLARepo.Rename_GL_Account_Bank(GLARepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Default Bank Account Successfully Configured.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    Store store = X.GetCmp<Store>("coll_SchemeFundStore");
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
                    ora_code = "Record is uniquely defined in the system. Process aborted..";
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
                return this.Direct();
            }
        }


        public ActionResult Read()
        {
            try
            {
                GLRepo.Add_GLChart(GLRepo);
                return this.Store(GLRepo.GetGLList());
            }
            catch (Exception ex)
            {
                return this.Direct();
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

        public ActionResult DeleteGLAccountRecord(string GL_Account_No)
        {
            try
            {
                if (GL_Account_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No GL Account has been selected.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                GLARepo.DeleteGLRecord(GL_Account_No);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "GL Bank Account Successfully Deleted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls_Right();
                Store store = X.GetCmp<Store>("BAccSFStore");
                store.Reload();

                return this.Direct();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult DeleteGLEAccountRecord(string GL_Account_No)
        {
            try
            {
                if (GL_Account_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No GL Account has been selected.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                GLARepo.DeleteGLERecord(GL_Account_No);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "GL Expense Account Successfully Deleted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls_Expense();
                Store store = X.GetCmp<Store>("EAccSFStore");
                store.Reload();

                return this.Direct();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        // filter for memo by control
        public ActionResult GetMemo(string Con_Code)
        {
            var misdepartmentrepo = new GLAccountRepo();
            var mydata = misdepartmentrepo.GetMEMOList(Con_Code);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Memo_Code;
                string name = ddd.Memo_Name;

                data.Add(new { Id = id, Name = name });
            }

            return this.Store(data);

        }

        // filter GL Account for SF 
        public ActionResult FilterSFGrid(string Scheme_Fund_Id)
        {
            try
            {


                return this.Store(GLARepo.GetASFList(Scheme_Fund_Id));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }

        // filter Bank Account for SF 
        public ActionResult FilterSFGridBank(string Scheme_Fund_Id)
        {
            try
            {

                return this.Store(GLARepo.GetASFList2(Scheme_Fund_Id));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }

        // filter Expense Account for SF 
        public ActionResult FilterSFGridExpense(string Scheme_Fund_Id)
        {
            try
            {

                return this.Store(GLARepo.GetASFList3(Scheme_Fund_Id));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }

        // filter Default Bank Accounts 
        public ActionResult FilterSFGridBankDefault()
        {
            try
            {

                return this.Store(GLARepo.GetASFListDefault());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }

        // filter GL Account for scheme
        public ActionResult GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new BondRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { gId = id, gName = name });
            }

            return this.Store(data);
        }

        public ActionResult Readsfcol()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return this.Store(SFRepo.GetSchemeFundList_coll());
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }

        }

        // filter  scheme account
        public ActionResult FilterESGrid()
        {
            try
            {

                Store store = X.GetCmp<Store>("colschemeStore");
                store.Reload();
                store.DataBind();

                return this.Store(SFRepo.GetESCHEMEList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }
    }//end class


}