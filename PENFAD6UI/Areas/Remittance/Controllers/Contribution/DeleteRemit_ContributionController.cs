using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Remittance.Contribution;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Contribution
{
    public class DeleteRemit_ContributionController : Controller
    {
        readonly Remit_Contribution_Upload_LogRepo RemitConRepo = new Remit_Contribution_Upload_LogRepo();
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        readonly Remit_BatchLogRepo Batch_Log = new Remit_BatchLogRepo();
        IDbConnection con;
        cLogger logger = new cLogger();
        // GET: Contribution/ApprovedRemit_Contribution
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddDeleteContributionTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "DeleteContributionPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddDeleteContributionBatchTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "DeleteContributionBatchPartial",
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
                var x = X.GetCmp<FormPanel>("frm_remittancebatchdelete");
                x.Reset();

                this.GetCmp<Store>("deleteemppppbatchstore").RemoveAll();
                this.GetCmp<Store>("deleteempconremitance_list_store").RemoveAll();
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
        public ActionResult DeleteRetmit_Record(Remit_Contribution_Upload_LogRepo conLogRepo)
        {
            try
            {
                if (!string.IsNullOrEmpty(conLogRepo.Con_Log_Id))
                {

                    //check if pending list exist for employer 
                    var app = new AppSettings();
                    con = app.GetConnection();

                    if (conLogRepo.isValidDelete(conLogRepo.Con_Log_Id) == true)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! There is a pending purchase for this schedule. Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    GlobalValue.Get_Scheme_Today_Date((conLogRepo.Scheme_Id));

                    if (this.RemitConRepo.DeleteRecord(conLogRepo))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Remittance batch upload deleted succesfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        ClearControls();

                    }
                    return this.Direct();
                }
                else
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select a batch to delete.", // " Insufficient data. Operation Aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
                //return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
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
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
               // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }

        public ActionResult ReadEmployer()
        {
            try
            {
                return this.Store(ESRepo.GetEmployerList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult DeleteMigrated(Remit_Contribution_Upload_LogRepo conLogRepo)
        {
            try
            {
                if (!string.IsNullOrEmpty(conLogRepo.Employer_Id) && !string.IsNullOrEmpty(conLogRepo.Scheme_Id))
                {

                    //check if pending list exist for employer 
                    var app = new AppSettings();
                    con = app.GetConnection();


                    if (this.RemitConRepo.DeleteBatchRecord(conLogRepo))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Remittance batch upload deleted succesfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                    }
                    return this.Direct();
                }
                else
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select employer to delete uploaded history.", // " Insufficient data. Operation Aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
                //return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
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
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }


        public ActionResult Get_BatchLogdelete()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(Batch_Log.GetBatch_RemitList_ForDelete("ACTIVE"));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }
        public ActionResult Get_RemittnaceInBatchLogdelete(Remit_Contribution_Upload_LogRepo remLog)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(remLog.GetEmployeeCon_BatchList_delete(remLog.Con_Log_Id));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

       
    }
}

