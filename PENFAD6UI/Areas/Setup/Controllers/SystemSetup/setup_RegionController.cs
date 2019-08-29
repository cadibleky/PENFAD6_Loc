using Ext.Net;
using Ext.Net.MVC;
using System.Web.Mvc;

using PENFAD6DAL.Repository.Setup.SystemSetup;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_RegionController : Controller
    {
        readonly setup_RegionRepo RegionRepo = new setup_RegionRepo();
        cLogger logger = new cLogger();
        public ActionResult Index()
        {
            return View();
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("region");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddRegionTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "RegionPartial",
                Model = RegionRepo.GetRegionList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult SaveRecord(setup_RegionRepo RegionRepo)
        {
            try
            {
                if (ModelState.IsValid)

                {

                   



                    this.RegionRepo.SaveRecord(RegionRepo);


                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("regionStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("region");
                    reset.Reset();

                    return this.Direct();


                }
                return this.Direct();
            }
            catch (System.Exception ex)
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
                logger.WriteLog(ex.Message);

                return this.Direct();
            }


        }
      
        public ActionResult DeleteRecord(string REGION_ID)
        {
            if (REGION_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Region' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            RegionRepo.DeleteRecord(REGION_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Region Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("regionStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("region");
            reset.Reset();
            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(RegionRepo.GetRegionList());
        }

    
}
}

