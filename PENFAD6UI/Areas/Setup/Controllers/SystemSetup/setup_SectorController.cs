using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_SectorController : Controller
    {
        readonly setup_SectorRepo sectorrepo = new setup_SectorRepo();
        // GET: setup_CustomerClass
        public ActionResult Index()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                return RedirectToAction("AddSectorTab");
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

        public ActionResult AddSectorTab(string containerId = "MainArea")
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "setup_SectorPartial",
                    Model = sectorrepo.GetSectorList(),
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


        public ActionResult SaveRecord(setup_SectorRepo setup_sectorrepo)
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
                    if (!string.IsNullOrEmpty(setup_sectorrepo.Sector_Name))
                        setup_sectorrepo.Sector_Name = setup_sectorrepo.Sector_Name.ToUpper();

                    this.sectorrepo.SaveRecord(setup_sectorrepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    Store store = X.GetCmp<Store>("SectorStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("SectorPan");
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

        public ActionResult DeleteRecord(string sector_id)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();

            try
            {
                if (string.IsNullOrEmpty(sector_id))
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

                sectorrepo.DeleteRecord(sector_id);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Deleted Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });

                Store store = X.GetCmp<Store>("SectorStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("SectorPan");
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
                return this.Store(sectorrepo.GetSectorList());
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