using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class AddNewEmployeeController : Controller
    {
        public static byte[] mImage;
        public static byte[] mImage_ID;
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        cLogger logger = new cLogger();
        pfm_Scheme_FundRepo scheme = new pfm_Scheme_FundRepo();
       
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
      + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
      + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
      + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddEmployeeTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "AddNewEmployeePartial",
                    Model = empList,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
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
        public ActionResult SaveRecord(crm_EmployeeRepo crmEmployee)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    
                    crmEmployee.Employee_Photo = mImage;
                    crmEmployee.Employee_Photo_Id = mImage_ID;

                    //-------------------------------------------------------------------
                    //Validate Employee Identity Number and SSNIT if its a new record
                    //-------------------------------------------------------------------
                    if (string.IsNullOrEmpty(crmEmployee.Employee_Id))
                    {
                        //check if employee Identity Number exist
                        if (employeeRepo.EmployeeIdentityIdNumberExist(crmEmployee.Identity_Type, crmEmployee.Identity_Number))
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Employee Identity number has already been assigned to another employee.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }

                        if (employeeRepo.SocialSecurityNumberExist(crmEmployee.SSNIT_NO))
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Employee SSNIT No. has already been assigned to another employee.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                }
                if (string.IsNullOrEmpty(crmEmployee.Employer_Id))
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
                    if (string.IsNullOrEmpty(crmEmployee.Scheme_Fund_Id))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Scheme Account is required",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();
                    }
                    if (!string.IsNullOrEmpty(crmEmployee.Mobile_Number))
                    {
                        if (Microsoft.VisualBasic.Information.IsNumeric(crmEmployee.Mobile_Number) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Primary phone number must be numbers.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (!string.IsNullOrEmpty(crmEmployee.Father_Phone_Number))
                    {
                        if (Microsoft.VisualBasic.Information.IsNumeric(crmEmployee.Father_Phone_Number) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Father's phone number must be numbers.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (!string.IsNullOrEmpty(crmEmployee.Mother_Phone_Number))
                    {
                        if (Microsoft.VisualBasic.Information.IsNumeric(crmEmployee.Mother_Phone_Number) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Mother's phone number must be numbers.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (!string.IsNullOrEmpty(crmEmployee.Other_Number))
                    {
                        if (Microsoft.VisualBasic.Information.IsNumeric(crmEmployee.Other_Number) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Secondary phone number must be numbers.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (!string.IsNullOrEmpty(crmEmployee.Email_Address))
                    {
                        if (Regex.IsMatch(crmEmployee.Email_Address, MatchEmailPattern) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Primary email address is not valid.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (!string.IsNullOrEmpty(crmEmployee.Other_Email_Address))
                    {
                        if (Regex.IsMatch(crmEmployee.Other_Email_Address, MatchEmailPattern) == false)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Secondary email address is not valid.",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    if (DateTime.Compare(crmEmployee.Date_Of_Birth, DateTime.Today) >= 0)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "DOB can not be today or in the future.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();

                    }

                    if (string.IsNullOrEmpty(crmEmployee.Mobile_Number))
                    {
                        crmEmployee.Mobile_Number = "0";
                    }

                    

                    ///check if Employee_ID exist

                    //Save new employee
                    if (this.employeeRepo.AddNewEmployeeRecord(crmEmployee))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Saved Successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });
                        //clear controls
                        ClearControls();
                        this.GetCmp<Image>("AddEmp_new_pic1").ImageUrl = "";
                        this.GetCmp<Image>("AddEmp_new_pic2").ImageUrl = "";
                    }
                    else
                    {
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
                   
                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = messages, // " Insufficient data. Operation Aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    
                }

                return this.Direct();
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.Message);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Process failed -"+ ex.Message,
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
                Store store = X.GetCmp<Store>("employerStore");
                store.Reload();
                store.DataBind();

                return this.Store(employeeRepo.GetESList());
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

        // filter employer scheme for employee
        public ActionResult FilterESGrid(string Employer_Id)
        {
            try
            {

                Store store = X.GetCmp<Store>("esfStore");
                store.Reload();
                store.DataBind();

                return this.Store(ESFRepo.GetESCHEMEList(Employer_Id));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }
       
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("employeeIndividual");
                x.Reset();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult SavePhoto()
        {
            try
            {
                if (X.GetCmp<FileUploadField>("AddEmp_new_btnloadfile").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("AddEmp_new_btnloadfile").PostedFile;

                    ImageWork.Current_Image_Path = ImageWork.ImagePhysicalPath(file_posted);

                    mImage = ImageWork.ImageToByte(ImageWork.Current_Image_Path);

                    //clear image field
                    this.GetCmp<Image>("AddEmp_new_pic1").ImageUrl = "";
                    this.GetCmp<Image>("AddEmp_new_pic1").ImageUrl = "../ImageProcessor/ImageProcessor.ashx?Filedata=" + ImageWork.Current_Image_Path;

                }

                if (X.GetCmp<FileUploadField>("AddEmp_new_btnloadfile2").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("AddEmp_new_btnloadfile2").PostedFile;

                    ImageWork.Current_Image_Path = ImageWork.ImagePhysicalPath(file_posted);

                    mImage_ID = ImageWork.ImageToByte(ImageWork.Current_Image_Path);

                    //clear image field
                    this.GetCmp<Image>("AddEmp_new_pic2").ImageUrl = "";
                    this.GetCmp<Image>("AddEmp_new_pic2").ImageUrl = "../ImageProcessor/ImageProcessor.ashx?Filedata=" + ImageWork.Current_Image_Path;

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