using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.PfmSetup
{
    public class pfm_FundController : Controller
    {
        readonly pfm_FundRepo fundrepo = new pfm_FundRepo();
        // GET: pfm_CustomerClass
        public ActionResult Index()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return RedirectToAction("AddFundTab");
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

        public ActionResult AddFundTab(string containerId = "MainArea")
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "pfm_FundPartial",
                    Model = fundrepo.GetFundList(),
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
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }

        }


        public ActionResult SaveRecord(pfm_FundRepo pfm_fundrepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                pfm_fundrepo.Maker_Id = GlobalValue.User_ID;
                pfm_fundrepo.Make_Date = GlobalValue.Scheme_Today_Date;
                pfm_fundrepo.Update_Id = GlobalValue.User_ID;
                pfm_fundrepo.Update_Date = GlobalValue.Scheme_Today_Date;
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(pfm_fundrepo.Fund_Name))
                        pfm_fundrepo.Fund_Name = pfm_fundrepo.Fund_Name.ToUpper();
                    if (!string.IsNullOrEmpty(pfm_fundrepo.Fund_Description))
                        pfm_fundrepo.Fund_Description = pfm_fundrepo.Fund_Description.ToUpper();
                    if (string.IsNullOrEmpty(pfm_fundrepo.Fund_Description))
                        pfm_fundrepo.Fund_Description = "N/A";

                    this.fundrepo.SaveRecord(pfm_fundrepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    Store store = X.GetCmp<Store>("FundStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("FundPan");
                    reset.Reset();

                    return this.Direct();
                }

                string messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
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
            finally
            {

            }
        }


        public ActionResult ApproveRecord(pfm_FundRepo pfm_fundrepo)
        {
            pfm_fundrepo.Auth_Id = GlobalValue.User_ID;
            pfm_fundrepo.Auth_Date = DateTime.Now;

            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                if (string.IsNullOrEmpty(pfm_fundrepo.Fund_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a record to approve.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }

                fundrepo.ApproveRecord(pfm_fundrepo);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Deleted Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });

                Store store = X.GetCmp<Store>("FundStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("FundPan");
                reset.Reset();

                return this.Direct();
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

        public ActionResult DeleteRecord(string fund_id)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                if (string.IsNullOrEmpty(fund_id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select a record to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }

                fundrepo.DeleteRecord(fund_id);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Deleted Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });

                Store store = X.GetCmp<Store>("FundStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("FundPan");
                reset.Reset();

                return this.Direct();
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
                return this.Store(fundrepo.GetFundList());
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
    }
}