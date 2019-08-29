using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Serilog;

using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class ApproveEditingEmployeeController : Controller
    {
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        IEnumerable<crm_EmployeeRepo> empRepoList = new List<crm_EmployeeRepo>();
        cLogger logger = new cLogger();

        public static byte[] mImage;
        public static byte[] mImageID;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ApproveEditingEmployeeTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ApproveEditingEmployeePartial",
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
        public ActionResult ApproveEditingEmployee(crm_EmployeeRepo crmEmployee)
        {

            try
            {
                //if (ModelState.IsValid)
                //{
                var empId = crmEmployee.Employee_Id;

                crmEmployee.Employee_Photo = mImage;
                crmEmployee.Employee_Photo_Id = mImageID;

                if (string.IsNullOrEmpty(empId))
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
                if (this.employeeRepo.UpdateEmployeeWithEmpTmp(crmEmployee))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employee: " + crmEmployee.Surname + " " + crmEmployee.First_Name + " approved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });
                    //clear controls
                    ClearControls();
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
                Store store = X.GetCmp<Store>("empEditingApprove_employeeStore");
                store.Reload();

                //}
                //else
                //{
                //    string messages = string.Join(Environment.NewLine, ModelState.Values
                //                        .SelectMany(x => x.Errors)
                //                        .Select(x => x.ErrorMessage));

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
        public ActionResult DisAppDeleteRecord(string Cust_No)
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
                if (employeeRepo.DeleteRecordDisApp(Cust_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Successfully disapproved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    //clear controls
                    ClearControls();
                }
                Store store = X.GetCmp<Store>("empEditingApprove_employeeStore");
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
                Store store = X.GetCmp<Store>("empEditingApprove_employeeStore");
                store.Reload();
                store.DataBind();

                return this.Store(employeeRepo.GetEditingEmployeeListAll());
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
                var x = X.GetCmp<FormPanel>("empEditingApprove");
                x.Reset();
                this.GetCmp<Image>("empAppEdit_pic1").ImageUrl = "";
                this.GetCmp<Image>("empAppEdit_pic2").ImageUrl = ""; 
            }
            catch (Exception)
            {

                throw;
            }

        }
        public ActionResult Read2(string employerId)
        {
            try
            {
                if (string.IsNullOrEmpty(employerId))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Employer name is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                Store store = X.GetCmp<Store>("empEditingApprove_employeeStore");
                store.Reload();
                store.DataBind();

                return this.Store(employeeRepo.GetEditingEmployeeList(employerId));

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

        public ActionResult DisplayPhoto(string cust_no)
        {
            try
            {
                if (string.IsNullOrEmpty(cust_no))
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

                empList = employeeRepo.GetEmpTempList(cust_no);

                if (empList.Count > 0)
                {

                    byte[] pic = null;
                    byte[] signature = null;

                    // if (kycList[0].Pict_Image.Length > 0)
                    pic = empList[0].Employee_Photo;
                    mImage = empList[0].Employee_Photo;
                    //if (kycList[0].SIGNATURE.Length > 0)
                    signature = empList[0].Employee_Photo_Id;
                    mImageID = empList[0].Employee_Photo_Id;

                    if ((pic != null && pic.Length > 0) )
                    {
                        this.GetCmp<Image>("empAppEdit_pic1").ImageUrl = "../ImageProcessor/ViewImageTemp.ashx?Cust_No=" + cust_no; // + ", " + Pict_Image; 
            }
                    if ((signature != null && signature.Length > 0))
                    {
                       
                        this.GetCmp<Image>("empAppEdit_pic2").ImageUrl = "../ImageProcessor/ViewImageIDTemp.ashx?Cust_No=" + cust_no; // + ", " + Pict_Image; 
                    }
                }

                return this.Direct();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}