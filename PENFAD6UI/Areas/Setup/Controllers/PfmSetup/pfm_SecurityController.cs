
using Ext.Net;
using Ext.Net.MVC;

using PENFAD6DAL.Repository.Setup.pfmSetup;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.pfmSetup
{
    public class pfm_SecurityController : Controller
    {
        readonly pfm_SecurityRepo SecurityRepo = new pfm_SecurityRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("security");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddSecurityTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SecurityPartial",
                Model = SecurityRepo.GetSecurityList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult SaveRecord(pfm_SecurityRepo SecurityRepo)
 
        {
            if (SecurityRepo.isSecurityUnique(SecurityRepo.Security_Name) == true)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Duplicate",
                    Message = "Security '"+ SecurityRepo.Security_Name+"' already exist.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }
            if (ModelState.IsValid)
            {         
               this.SecurityRepo.SaveRecord(SecurityRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Security Successfully added.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("securityStore");
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
                Store store = X.GetCmp<Store>("securityStore");
                store.Reload();

                return this.Direct();
            }
            // return this.Direct();        

        }
        public ActionResult DeleteRecord(string SECURITY_ID)
        {
            if (SECURITY_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Security' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            SecurityRepo.DeleteRecord(SECURITY_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Security Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("securityStore");
            store.Reload();

            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(SecurityRepo.GetSecurityList());
        }  
}
}

