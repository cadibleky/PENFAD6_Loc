using Ext.Net;
using Ext.Net.MVC;

using System;
using System.Web.Mvc;
using System.Collections.Generic;
using PENFAD6DAL.Repository.Setup.SystemSetup;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_EmployeeTypeController : Controller
    {
        readonly setup_EmployeeTypeRepo empType = new setup_EmployeeTypeRepo();
        IEnumerable<setup_EmployeeTypeRepo> empTypeList = new List<setup_EmployeeTypeRepo>();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddEmployeeTypeTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeTypePartial",
                    Model = empTypeList,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo

                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

                return pvr;
            }
            catch (Exception)
            {
                X.Mask.Hide();
                throw;
            }

        }
        public ActionResult SaveRecord(setup_EmployeeTypeRepo employeeTypes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(employeeTypes.Employee_Type_Id))
                    {
                        if (this.empType.EmployeeTypeExist(employeeTypes.Employee_Type) == true)
                        {

                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Employee Type - " + employeeTypes.Employee_Type.ToUpper() + " already exist.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (this.empType.SaveRecord(employeeTypes))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Saved Successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });

                        //Clear Controls
                        ClearControls();
                    }
                    Store store = X.GetCmp<Store>("empTypeStore");
                    store.Reload();
                    var reset = X.GetCmp<FormPanel>("employeeType");
                    reset.Reset();

                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Process failed. Check and correct all errors",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                return this.Direct();

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

        }
        public ActionResult DeleteRecord(string employee_Type_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(employee_Type_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select an employee Type to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }

                //Delete Record
                if (empType.DeleteRecord(employee_Type_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    //Clear Controls
                    ClearControls();
                }
                Store store = X.GetCmp<Store>("empTypeStore");
                store.Reload();
                var reset = X.GetCmp<FormPanel>("employeeType");
                reset.Reset();

                return this.Direct();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public ActionResult Read()
        {
            try
            {
                return this.Store(empType.GetEmployeeTypeList());
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("employeeType");
                x.Reset();
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}