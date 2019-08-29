using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.Setup.RemitSetup;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class GLConsolidatedController : Controller
    {
        readonly pfm_Scheme_FundRepo schemefundrepo = new pfm_Scheme_FundRepo();
        // GET: pfm_CustomerClass
        public ActionResult Index()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return RedirectToAction("AddSchemeFundTab");
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

        public ActionResult AddSchemeab(string containerId = "MainArea")
        {
            //var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLConsolidated",
                    Model = schemefundrepo.GetSchemeFundList(),
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,

                };
                X.Mask.Hide();

                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }

        }

       
        public ActionResult SaveRecord(pfm_Scheme_FundRepo pfm_scheme_fundrepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                pfm_scheme_fundrepo.Maker_Id = GlobalValue.User_ID;
                pfm_scheme_fundrepo.Make_Date = System.DateTime.Now;
                pfm_scheme_fundrepo.Update_Id = GlobalValue.User_ID;
                pfm_scheme_fundrepo.Update_Date = System.DateTime.Now;
             
                {
                    if (String.IsNullOrEmpty(pfm_scheme_fundrepo.Account_Name))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Account Name is required",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });
                        return this.Direct();
                    }

                    if (String.IsNullOrEmpty(pfm_scheme_fundrepo.Scheme_Id))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Scheme is required",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });
                        return this.Direct();
                    }

                    if (String.IsNullOrEmpty(pfm_scheme_fundrepo.GL_Account_No))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "GL Account is required",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });
                        return this.Direct();
                    }

                    this.schemefundrepo.SaveRecordGL(pfm_scheme_fundrepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Added Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "GLConsolidated",
                        ContainerId = "MainArea",
                        RenderMode = RenderMode.AddTo,
                    };
                    this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                    return pvr;
                    
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
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
                
            }
            finally
            {

            }


        }


        public ActionResult DeleteRecord(string TID)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                if (string.IsNullOrEmpty(TID))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a record to REMOVE.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }

                schemefundrepo.DeleteRecordGL(TID);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Removed Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLConsolidated",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                X.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            
            }
            catch (Exception ex)
            {
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20100")
                {
                    ora_code = "Record is a dependant in other sections in the system. Process aborted..";
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
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();

            }
            finally
            {

            }

        }

        public ActionResult Read()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return this.Store(schemefundrepo.GetSchemeFundGLList());
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


        // filter GL account for scheme-fund
        public ActionResult GetGLAB(string Scheme_Id)
        {
            var misdepartmentrepo = new remit_BankSchemeFundRepo();
            var mydata = misdepartmentrepo.GetGLASFListGL(Scheme_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { Id = id, Name = name });
            }

            return this.Store(data);

        }
    }
}