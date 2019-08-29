
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_DistrictController : Controller
    {
        //Richard..................................................
        
        readonly setup_DistrictRepo DistrictRepo = new setup_DistrictRepo();



        string error = "";
        // GET: setupDistrict
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddDistrictTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Districtpartial",

                Model = DistrictRepo.GetDistrictList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        public ActionResult SaveRecord(setup_DistrictRepo DistrictRepo)
        {
            try
            { 
            if (ModelState.IsValid)

            {

               
                this.DistrictRepo.SaveRecord(DistrictRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Saved Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });


                Store store = X.GetCmp<Store>("districtStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("frmDistrict");
                reset.Reset();
               

                return this.Direct();


            }

                string messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
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

                return this.Direct();
            }


        }



        public ActionResult DeleteRecord(string District_Id)
        {
            if (District_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a District Name to delete.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

           DistrictRepo.DeleteRecord(District_Id);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            Store store = X.GetCmp<Store>("districtStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("District");
            reset.Reset();
            return this.Direct();
           
        }

        public ActionResult Read()
        {
            return this.Store(DistrictRepo.GetDistrictList());
        }
    }
}