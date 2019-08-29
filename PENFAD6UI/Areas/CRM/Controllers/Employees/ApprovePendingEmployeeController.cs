using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Serilog;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.GlobalObject;
using IBankWebService.Utils;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class ApprovePendingEmployeeController : Controller
    {
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        IEnumerable<crm_EmployeeRepo> empRepoList = new List<crm_EmployeeRepo>();
        cLogger logger = new cLogger();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ApprovePendingEmployeeTab(string containerId = "MainArea")
        {
            
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ApprovePendingEmployeePartial",
                    Model = empRepoList,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,

                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                logger.WriteLog(ex.Message); //.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Process failed -" + ex.Message,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }

        }

        public ActionResult DisplayPhoto(string Cust_no)
        {
            try
            {
                if (string.IsNullOrEmpty(Cust_no))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Employee record is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });
                    return this.Direct();
                }

                empList = employeeRepo.GetEmpList(Cust_no);

                if (empList.Count > 0)
                {

                    byte[] pic = null;
                    byte[] signature = null;

                    // if (kycList[0].Pict_Image.Length > 0)
                    pic = empList[0].Employee_Photo;

                    //if (kycList[0].SIGNATURE.Length > 0)
                    signature = empList[0].Employee_Photo_Id;

                    if ((pic != null && pic.Length > 0))
                    {
                        this.GetCmp<Image>("empAppNew_pic1").ImageUrl = "../ImageProcessor/ViewImageProcessor.ashx?Cust_No=" + Cust_no; // + ", " + Pict_Image; 

                    }

                    if ( (signature != null && signature.Length > 0))
                    {
                             this.GetCmp<Image>("empAppNew_pic2").ImageUrl = "../ImageProcessor/ViewImageIDProcessor.ashx?Cust_No=" + Cust_no; // + ", " + Pict_Image; 
                    }

                }

                return this.Direct();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ApprovePendingEmployee(crm_EmployeeRepo setupEmployee)
        {
            
            try
            {
                //if (ModelState.IsValid)
                //{
                    if (string.IsNullOrEmpty(setupEmployee.Employee_Id))
                    {
                    X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select an employee to approve.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();
                    }

                    //Save new employee
                    if (this.employeeRepo.ApprovePendingEmployee(setupEmployee.Employee_Id, setupEmployee.Cust_No,setupEmployee.Scheme_Fund_Id))
                    {
                                            

                                            if (setupEmployee.SEND_SMS == "YES")
                                            {
                                                    if (string.IsNullOrEmpty(setupEmployee.Mobile_Number))
                                                    {
                                                        setupEmployee.Mobile_Number = "000000000";
                                                    }
                                                    else
                                                    {
                                                        setupEmployee.Mobile_Number = setupEmployee.Mobile_Number;
                                                    }
                                               if (setupEmployee.Mobile_Number.Length < 9)
                                                {
                                                    setupEmployee.Mobile_Number = "000000000";
                                                }
                                                //SEND SMS
                                                string smsmsg = "Dear " + setupEmployee.First_Name + ", you have successfully been registered on the " + setupEmployee.Scheme_Name + " with member code " + setupEmployee.Cust_No + ". Thank you";
                                                string fonnum = "233" + setupEmployee.Mobile_Number.Substring(setupEmployee.Mobile_Number.Length - 9, 9);

                                                Dictionary<string, string> param = new Dictionary<string, string>();
                                                param.Add("to", fonnum);
                                                param.Add("text", smsmsg);
                                                Request request = new Request
                                                {
                                                    Parameters = param
                                                };

                                                var content = Volley.PostRequest(request);
                                                //END SEND SMS
                                            }
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Employee: " + setupEmployee.Surname + " " + setupEmployee.First_Name + " approved Successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        //clear controls
                        ClearControls();
                    this.GetCmp<Image>("empAppNew_pic1").ImageUrl = "";
                    this.GetCmp<Image>("empAppNew_pic2").ImageUrl = "";
                    }
                    else
                    {
                    X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });
                        return this.Direct();
                    }
                    Store store = X.GetCmp<Store>("employeeStore");
                    store.Reload();

                //}
                //else
                //{
                //   string messages = string.Join(Environment.NewLine, ModelState.Values
                                        //.SelectMany(x => x.Errors)
                                        //.Select(x => x.ErrorMessage));

                //X.Msg.Show(new MessageBoxConfig
                //{
                //    Title = "Error",
                //    Message = messages, // " Insufficient data. Operation Aborted",
                //    Buttons = MessageBox.Button.OK,
                //    Icon = MessageBox.Icon.ERROR,
                //    Width = 350
                //});


                //}

                return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                logger.WriteLog(ex.Message);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Process failed -" + ex.Message,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }


        }
        public ActionResult DeleteRecord(string Cust_No)
        {
            
            try
            {
                if (string.IsNullOrEmpty(Cust_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select record to disapprove.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }

                //delete record
                if (employeeRepo.DeleteRecord(Cust_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Disapprove",
                        Message = "Successfully disapproved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    //clear controls
                    ClearControls();
                }
                Store store = X.GetCmp<Store>("employeeStore");
                store.Reload();

                return this.Direct();
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                logger.WriteLog(ex.Message);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Process failed -" + ex.Message,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }

        }
        public ActionResult Read()
        {
            try
            {
               return this.Store(employeeRepo.GetPendingEmployeeList());
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.Message);
                X.Msg.Show(new MessageBoxConfig
                {
                   Title = "Error",
                   Message = "Process failed -" + ex.Message,
                   Buttons = MessageBox.Button.OK,
                   Icon = MessageBox.Icon.INFO,
                    Width = 350


               });
                return this.Direct();
            }

        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("employee");
                x.Reset();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult Read2()
        {
            try
            {
               

                Store store = X.GetCmp<Store>("employeeStore");
                store.Reload();
                store.DataBind();

                return this.Store(employeeRepo.GetPendingEmployeeList());

            }
            catch (Exception ex)
            {
               // logger.WriteLog(ex.Message);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Process failed -" + ex.Message,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }

        }


    }
}