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
    public class pfm_FundManagerController : Controller
    {
        readonly pfm_FundManagerRepo FundManagerRepo = new pfm_FundManagerRepo();
        cLogger logger = new cLogger();
        // GET: Areas/FundManager.........................Richard
        string error = "";
        // GET: Fund_Manager
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddFundManagerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "FundManagerPartial",

                Model = FundManagerRepo.GetFundmanagerList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        public ActionResult SaveRecord(pfm_FundManagerRepo FundManagerRepo)
        {
            try
            {
                if (ModelState.IsValid)

                {

                    this.FundManagerRepo.SaveRecord(FundManagerRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("fundmanagerStore");
                    store.Reload();

                    var reset = X.GetCmp<FormPanel>("fundmanager");
                    reset.Reset();

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
                return this.Direct();
            }
            finally
            {

            }
        }




        public ActionResult DeleteRecord(string Fund_Manager_Id)
        {
            if (Fund_Manager_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Fund Manager Name to delete.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            FundManagerRepo .DeleteRecord(Fund_Manager_Id);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            Store store = X.GetCmp<Store>("fundmanagerStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("fundmanager");
            reset.Reset();
            return this.Direct();
        }

        public ActionResult Read()
        {
            return this.Store(FundManagerRepo.GetFundmanagerList());
        }
    }
}
 