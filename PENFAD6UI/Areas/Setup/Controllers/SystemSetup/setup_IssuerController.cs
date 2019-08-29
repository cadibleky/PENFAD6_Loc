using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_IssuerController : Controller
    {
        readonly setup_IssuerRepo issuerrepo = new setup_IssuerRepo();
        // GET: setup_CustomerClass

  //      public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
  //+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
  //+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
  //+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public ActionResult Index()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return RedirectToAction("AddIssuerTab");
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

        public ActionResult AddIssuerTab(string containerId = "MainArea")
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341/").CreateLogger();

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "setup_IssuerPartial",
                    Model = issuerrepo.GetIssuerList(),
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


        public ActionResult AddIssuerUnApproveTab(string containerId = "MainArea")
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "setup_ApproveIssuerPartial",
                    Model = issuerrepo.GetIssuerUnApproveList(),
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

        [HttpPost]
        public ActionResult SaveRecord(setup_IssuerRepo setup_sectorrepo)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                setup_sectorrepo.Maker_Id = GlobalValue.User_ID;
                setup_sectorrepo.Make_Date = GlobalValue.Scheme_Today_Date;
                setup_sectorrepo.Maker_Id = GlobalValue.User_ID;
                setup_sectorrepo.Update_Date = GlobalValue.Scheme_Today_Date;
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(setup_sectorrepo.Issuer_Name))
                        setup_sectorrepo.Issuer_Name = setup_sectorrepo.Issuer_Name.ToUpper();
                    if (!string.IsNullOrEmpty(setup_sectorrepo.Postal_Address))
                        setup_sectorrepo.Postal_Address = setup_sectorrepo.Postal_Address.ToUpper();
                    //if (!string.IsNullOrEmpty(setup_sectorrepo.Email_Address))
                    //{
                    //    setup_sectorrepo.Email_Address = setup_sectorrepo.Email_Address.ToLower();
                    //    if (Regex.IsMatch(setup_sectorrepo.Email_Address, MatchEmailPattern) == false)
                    //    {
                    //        X.Msg.Show(new MessageBoxConfig
                    //        {
                    //            Title = "Error",
                    //            Message = "Email address is not valid.",
                    //            Buttons = MessageBox.Button.OK,
                    //            Icon = MessageBox.Icon.INFO,
                    //            Width = 350

                    //        });
                    //        return this.Direct();
                    //    }
                    //}

                    this.issuerrepo.SaveRecord(setup_sectorrepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    Store store = X.GetCmp<Store>("IssuerStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("IssuerPan");
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
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
            finally
            {

            }


        }


        public ActionResult ApproveRecord(setup_IssuerRepo setup_issuerrepo)
        {
            setup_issuerrepo.Auth_Id = "teksol.admin";
            setup_issuerrepo.Auth_Date = DateTime.Now;

            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                if (setup_issuerrepo.Issuer_Id == string.Empty)
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

                issuerrepo.ApproveRecord(setup_issuerrepo);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Approve Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });

                Store store = X.GetCmp<Store>("ApproveIssuerStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("ApproveIssuerPan");
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

        public ActionResult DeleteRecord(string issuer_id)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                if (issuer_id == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select an issuer to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }

                issuerrepo.DeleteRecord(issuer_id);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Deleted Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });

                Store store = X.GetCmp<Store>("IssuerStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("IssuerPan");
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
                return this.Store(issuerrepo.GetIssuerList());
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

        public ActionResult ReadUnApporved()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return this.Store(issuerrepo.GetIssuerUnApproveList());
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