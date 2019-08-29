using Ext.Net;
using Ext.Net.MVC;
using System.Web.Mvc;
using System;

using PENFAD6DAL.Repository.Setup.SystemSetup;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_CountryController : Controller
    {
        readonly setup_CountryRepo countryRepo = new setup_CountryRepo();
        cLogger logger = new cLogger();
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("country");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddCountryTab(string containerId= "MainArea")
        {
            try {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "CountryPartial",
                    Model = countryRepo.GetCountryList(),
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return pvr;
            }
            catch(Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }

        public ActionResult SaveRecord(setup_CountryRepo countryRepo)
        {
            try
            {
                if (ModelState.IsValid)

                {

                    countryRepo.SaveRecord();


                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("countryStore");
                    store.Reload();
                  
                     X.GetCmp<FormPanel>("country").Reset();
                    //reset.Reset();

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
       
        public ActionResult DeleteRecord(string COUNTRY_ID)
        {
            try {
                if (COUNTRY_ID == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No country has been selected.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                countryRepo.DeleteRecord(COUNTRY_ID);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Country Successfully deleted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("countryStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("country");
                reset.Reset();
                return this.Direct();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult Read()
        {
            try {
                return this.Store(countryRepo.GetCountryList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     
    }
}

