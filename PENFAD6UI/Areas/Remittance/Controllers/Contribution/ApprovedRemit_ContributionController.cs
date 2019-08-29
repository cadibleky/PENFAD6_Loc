using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
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
    public class ApprovedRemit_ContributionController : Controller
    {
        readonly Remit_Contribution_Upload_LogRepo RemitConRepo = new Remit_Contribution_Upload_LogRepo();

        readonly Remit_BatchLogRepo Batch_Log = new Remit_BatchLogRepo();
        IDbConnection con;
        cLogger logger = new cLogger();
        // GET: Contribution/ApprovedRemit_Contribution
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddApproveContributionTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ApproveContributionPartial",
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
                var x = X.GetCmp<FormPanel>("frm_remittancebatchapproval");
                x.Reset();

                this.GetCmp<Store>("approveemppppbatchstore").RemoveAll();
                this.GetCmp<Store>("approveempconremitance_list_store").RemoveAll();
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
        public ActionResult ApprovedRetmit_Record(Remit_Contribution_Upload_LogRepo conLogRepo)
        {
            try
            {
                if (!string.IsNullOrEmpty(conLogRepo.Con_Log_Id))
                {

                    //check if pending list exist for employer 
                    var app = new AppSettings();
                    con = app.GetConnection();

                    GlobalValue.Get_Scheme_Today_Date(conLogRepo.Scheme_Id);

                    if (this.RemitConRepo.ApprovedRecord(conLogRepo))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Remittance batch upload approved succesfully.",
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
                        Message = "Select a batch to approve.", // " Insufficient data. Operation Aborted",
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
                throw ex;
            }
        }

        public ActionResult Get_BatchLogPending()
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(Batch_Log.GetBatch_RemitList_ByStatus("PENDING"));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }
        public ActionResult Get_RemittnaceInBatchLogPending(Remit_Contribution_Upload_LogRepo remLog)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(remLog.GetEmployeeCon_BatchList_ByStatus(remLog.Con_Log_Id));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult DisapprovedRecord(string Con_Log_Id)
        {
            if (Con_Log_Id == string.Empty)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Employer Name to disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350

                });
                return this.Direct();
            }

            RemitConRepo.DisapprovedRecord(Con_Log_Id);
            X.Mask.Hide();
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            ClearControls();

            return this.Direct();
        }    
        
    }
}

