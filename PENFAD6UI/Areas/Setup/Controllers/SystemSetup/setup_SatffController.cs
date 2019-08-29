
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using System;
using System.Linq;
using System.Web.Mvc;


namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_StaffController : Controller
    {
        readonly setup_StaffRepo staffRepo = new setup_StaffRepo();
        cLogger logger = new cLogger();
        public ActionResult Index()
        {
            return View();
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmAddSatff");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddStaffTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "setup_StaffPartial",
                Model = staffRepo.GetStaffList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult DropStaffTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "setup_DropStaffPartial",
                Model = staffRepo.GetStaffList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult SaveRecord(setup_StaffRepo staffRepo)
        {
            try
            {
                if (ModelState.IsValid)
                    staffRepo.Staff_Status = "HIRED";

                {
                    this.staffRepo.SaveRecord(staffRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Saved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    Store store = X.GetCmp<Store>("staffStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("frmAddSatff");
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

        public ActionResult DeleteRecord(string Staff_Id)
        {
            if (Staff_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Staff has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            staffRepo.DeleteRecord(Staff_Id);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Staff Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("staffStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("frmAddSatff");
            reset.Reset();
            return this.Direct();
        }

        public ActionResult DropRecord(string Staff_Id)
        {
            if (Staff_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Staff has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            staffRepo.DropRecord(Staff_Id);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Staff Successfully Dropped.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("staffdropStore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("frmDropSatff");
            reset.Reset();
            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(staffRepo.GetStaffList());
        }

        public ActionResult UpdateRecord(setup_StaffRepo staffRepo)
        {
            var mTitle = staffRepo.Staff_Id;
            if (mTitle == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a staff to update.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            staffRepo.SaveRecord(staffRepo);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Staff Successfully updated.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });

            Store store = X.GetCmp<Store>("staffStore");
            store.Reload();

            return this.Direct();
        }
    }
}

