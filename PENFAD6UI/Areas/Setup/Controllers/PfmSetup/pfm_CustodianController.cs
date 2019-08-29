using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.PfmSetup
{
    public class pfm_CustodianController : Controller
    {
        readonly pfm_CustodianRepo CustodianRepo = new pfm_CustodianRepo();
        cLogger logger = new cLogger();
        // GET: Areas/PfmCustodia.........................Richard
        string error = ""; 
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddCustodianTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "CustodianPartial",

                Model = CustodianRepo.GetCustodianList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }


        public ActionResult SaveRecord(pfm_CustodianRepo CustodianRepo)
        {
            try
            
            ////if (ModelState.IsValid)

            {

              


                this.CustodianRepo.SaveRecord(CustodianRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Saved Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                    Store store = X.GetCmp<Store>("custodianStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("custodian");
                    reset.Reset();

                    return this.Direct();


            //}
            //return this.Direct();
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
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                logger.WriteLog(ex.Message);
                return this.Direct();
            }
            finally
            {

            }
        }

      



        public ActionResult DeleteRecord(string Custodian_Id)
        {
            if (Custodian_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Custodian Name to delete.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            CustodianRepo.DeleteRecord(Custodian_Id);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            Store store = X.GetCmp<Store>("custodianStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("custodian");
            reset.Reset();

            return this.Direct();
        }

        public ActionResult Read()
        {
            return this.Store(CustodianRepo.GetCustodianList());
        }
    }
}