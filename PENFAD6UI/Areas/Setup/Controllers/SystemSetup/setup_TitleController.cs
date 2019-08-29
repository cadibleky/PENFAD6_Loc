
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using System.Web.Mvc;


namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_TitleController : Controller
    {
        readonly setup_TitleRepo titleRepo = new setup_TitleRepo();
        cLogger logger = new cLogger();
        public ActionResult Index()
        {
            return View();
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("title");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddTitleTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "setup_TitlePartial",
                Model = titleRepo.GetTitleList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult SaveRecord(setup_TitleRepo titleRepo)
        {
            try
            {
                if (ModelState.IsValid)

                {
                    this.titleRepo.SaveRecord(titleRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("titleStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("title");
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
      
        public ActionResult DeleteRecord(string TITLE_ID)
        {
            if (TITLE_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Title' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            titleRepo.DeleteRecord(TITLE_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Title Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("titleStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("title");
            reset.Reset();
            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(titleRepo.GetTitleList());
        }

        public ActionResult UpdateRecord(setup_TitleRepo titleRepo)
        {
            var mTitle = titleRepo.Title_ID;
            if (mTitle == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a title to update.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

           titleRepo.updateRecord(titleRepo);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Title Successfully updated.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });

            Store store = X.GetCmp<Store>("titleStore");
            store.Reload();

            return this.Direct();
        }
    }
}

