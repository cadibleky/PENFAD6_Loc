
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.pfmSetup;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.pfmSetup
{
    public class pfm_FeesController : Controller
    {
        readonly pfm_FeesRepo FeesRepo = new pfm_FeesRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("fees");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddFeesTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "FeesPartial",
                Model = FeesRepo.GetFeesList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }
        public ActionResult SaveRecord(pfm_FeesRepo FeesRepo)

        {
            try
            {
               
                //if (FeesRepo.Fee > 100)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "ERROR",
                //        Message = "Sorry! Fee can't be more than 100%.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                if (FeesRepo.Fee < 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "ERROR",
                        Message = "Sorry! Fee can't be less than 0%.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }


                if (ModelState.IsValid)
                {
                    this.FeesRepo.SaveRecord(FeesRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Fee Successfully Saved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    ClearControls();
                    Store store = X.GetCmp<Store>("feesStore");
                    store.Reload();

                    return this.Direct();

                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "ERROR",
                        Message = "Incomplete Entry.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    Store store = X.GetCmp<Store>("feesStore");
                    store.Reload();

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
            finally
            {

            }     
    }
        public ActionResult DeleteRecord(string FEE_ID)
        {
            try
            { 
            if (FEE_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Fee' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            FeesRepo.DeleteRecord(FEE_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Fee Successfully Deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("feesStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult Read()
        {
            try
            { 
            return this.Store(FeesRepo.GetFeesList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }  
}
}

