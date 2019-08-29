
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using System.Data;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_IdentityTypeController : Controller
    {

        cLogger logger = new cLogger();
        // Richard .....................................
        // GET: setupIdentityType
        readonly setup_IdentityTypesRepo IdentityTypeRepo = new setup_IdentityTypesRepo();

        string error = "";


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddIdentityTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "IdentityPartial",
                Model = IdentityTypeRepo.GetIdentityTypeList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        public ActionResult SaveRecord(setup_IdentityTypesRepo IdentityTypeRepo)
        {
            try
            {
                if (ModelState.IsValid)

                {

                    if (this.IdentityTypeRepo.IdentityExist(IdentityTypeRepo.Identity_Type, out error))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Identity Type name - " + IdentityTypeRepo.Identity_Type.ToUpper() + " already exist.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();
                    }


                    this.IdentityTypeRepo.SaveRecord(IdentityTypeRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("identityStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("IdentityType");
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
                logger.WriteLog(ex.Message );
               
                return this.Direct();
            }
            

        }
      
        public ActionResult DeleteRecord(string Identity_Type_Id)
        {
            if (Identity_Type_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Identity Type to delete.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            IdentityTypeRepo.DeleteRecord(Identity_Type_Id);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            Store store = X.GetCmp<Store>("identityStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("IdentityType");
            reset.Reset();


            return this.Direct();
        }

        public ActionResult Read()
        {
            return this.Store(IdentityTypeRepo.GetIdentityTypeList());
        }

    }
}