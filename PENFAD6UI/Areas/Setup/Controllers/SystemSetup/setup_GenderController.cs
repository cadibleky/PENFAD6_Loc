using Ext.Net;
using Ext.Net.MVC;
using System.Web.Mvc;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_GenderController : Controller
    {
        readonly setup_GenderRepo GenderRepo = new setup_GenderRepo();
        cLogger logger = new cLogger();
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("gender");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult AddGenderTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GenderPartial",
                Model = GenderRepo.GetGenderList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult SaveRecord(setup_GenderRepo GenderRepo)
        {
            try
            {
                if (ModelState.IsValid)

                { 
                    this.GenderRepo.SaveRecord(GenderRepo);
 
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("genderStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("gender");
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
       
        public ActionResult DeleteRecord(string GENDER_ID)
        {
            if (GENDER_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Gender' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            GenderRepo.DeleteRecord(GENDER_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Gender Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("genderStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("gender");
            reset.Reset();

            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(GenderRepo.GetGenderList());
        }
 
}
}

