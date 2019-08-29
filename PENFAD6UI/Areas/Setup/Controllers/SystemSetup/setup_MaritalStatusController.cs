
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.SystemSetup;

using System.Data;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_MaritalStatusController : Controller
    {
        //Richard..................................................
        // GET: setupMaritalStatus
        readonly setup_MaritalStatusRepo MaritalStatuss = new setup_MaritalStatusRepo();


        cLogger logger = new cLogger();
        string error = "";

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddMaritalStatusTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "MaritalStatuspartial",
                
                Model = MaritalStatuss.GetMaritalstatusList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
      

        public ActionResult SaveRecord(setup_MaritalStatusRepo MaritalStatuss)
        {
            try
            {
                if (ModelState.IsValid)

                {

                    if (this.MaritalStatuss.MaritalStatusExist(MaritalStatuss.Marital_Status, out error))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "MaritalStatus name - " + MaritalStatuss.Marital_Status.ToUpper() + " already exist.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();
                    }




                    this.MaritalStatuss.SaveRecord(MaritalStatuss);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("maritalStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("MaritalStatus");
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
        public ActionResult DeleteRecord(string marital_status_Id)
        {
            if (marital_status_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a marital Status to delete.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            MaritalStatuss.DeleteRecord(marital_status_Id);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            Store store = X.GetCmp<Store>("maritalStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("MaritalStatus");
            reset.Reset();

            return this.Direct();
        }

        public ActionResult Read()
        {
            return this.Store(MaritalStatuss.GetMaritalstatusList());
        }

    }
}