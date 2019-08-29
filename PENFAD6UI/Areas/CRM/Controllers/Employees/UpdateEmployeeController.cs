using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Collections.Generic;
using Serilog;

using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.GlobalObject;
using System.Web;
using System.Data;
using Dapper;
using PENFAD6DAL.DbContext;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class UpdateEmployeeController : Controller
    {
        readonly crm_EmployeeBatchLogRepo batch_log = new crm_EmployeeBatchLogRepo();
        readonly crm_EmployeeRepo employee_repo = new crm_EmployeeRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        IEnumerable<crm_EmployeeRepo> empRepoList = new List<crm_EmployeeRepo>();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        cLogger logger = new cLogger();
       // IDbConnection con;

        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
      + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
      + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
      + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public static byte[] mImage;
        public static byte[] mImage_ID;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddUpdateEmployeeTab(string containerId = "MainArea")
        {
            
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "UpdateEmployeePartial",
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
                return this.Direct();
            }

        }

        public ActionResult AddDeleteEmployeeTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "DeleteEmployeePartial",
                   // Model = empRepoList,
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
                // logger.WriteLog(ex.Message);
                return this.Direct();
            }

        }

        public ActionResult AddDeleteBatchEmployeeTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "DeleteEmployeeBatchPartial",
                    //Model = empRepoList,
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
                // logger.WriteLog(ex.Message);
                return this.Direct();
            }

        }

        public ActionResult AddTransferEmployeeTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeStatusPartial",
                    // Model = empRepoList,
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
                // logger.WriteLog(ex.Message);
                return this.Direct();
            }

        }

        public ActionResult UpdateRecord(crm_EmployeeRepo crmEmployee)
        {
            
            try
            {
                //if (ModelState.IsValid)
                //{

                crmEmployee.Employee_Photo = mImage;
                crmEmployee.Employee_Photo_Id = mImage_ID;

                if (string.IsNullOrEmpty(crmEmployee.Employee_Id))
                    {

                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select an employee to edit.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();

                    }

                if (crmEmployee.isValidUpdate(crmEmployee.Employee_Id) == true)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! There is a pending update for this employee. Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
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
                 

                    //update employee record
                    if (this.employeeRepo.AddEmployeeRecordToTmp(crmEmployee))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Updated Successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350


                        });
                        //clear controls
                       ClearControls();
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
                    Store store = X.GetCmp<Store>("employeeStore");
                    store.Reload();

              

                return this.Direct();
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
        public ActionResult DeleteRecord(string Cust_No)
        {
            
            try
            {
                if (string.IsNullOrEmpty(Cust_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select employee to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }

                //delete record
                if (employeeRepo.DeleteRecord(Cust_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employee Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    //clear controls
                    ClearControls_Delete();
                }
                Store store = X.GetCmp<Store>("employeedelete_empStore");
                store.Reload();

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
                    ora_code = "Record can not be deleted. Process aborted..";
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
        public ActionResult ReadEmployee(string Employer_Id)
        {
            try
            {
                return this.Store(employeeRepo.GetEmployeeList(Employer_Id));
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
        public ActionResult ReadEmployer()
        {
            
            try
            {
                return this.Store(employer.EmployerData());
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
                var x = X.GetCmp<FormPanel>("empEdit");
                x.Reset();
                this.GetCmp<Image>("empEdit_pic1").ImageUrl = "";
                this.GetCmp<Image>("empEdit_pic2").ImageUrl = "";
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void ClearControls_employeetransfer()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_Employeestatus");
                x.Reset();
      
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void ClearControls_Delete()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("empDelete");
                x.Reset();
                this.GetCmp<Image>("empEdit_pic1").ImageUrl = "";
                this.GetCmp<Image>("empEdit_pic2").ImageUrl = "";
            }
            catch (Exception)
            {

                throw;
            }

        }
      
        public ActionResult Read2(string Employer_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id))
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

               // X.GetCmp<Store>("nokStore").Reload();
               //X.GetCmp<Store>("BeneStore").Reload();
                X.GetCmp<Store>("employeeStore").Reload();

                Store store = X.GetCmp<Store>("employeeStore");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList2(Employer_Id);
                if (obj.Count == 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "There are no employees for this employer.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                return this.Store(obj);

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

        //public ActionResult Read2(string Employer_Id)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Employer_Id))
        //        {
        //            X.Msg.Show(new MessageBoxConfig
        //            {
        //                Title = "Error",
        //                Message = "Employer name is required",
        //                Buttons = MessageBox.Button.OK,
        //                Icon = MessageBox.Icon.INFO,
        //                Width = 350

        //            });
        //            return this.Direct();
        //        }

        //        Store store = X.GetCmp<Store>("employeeStore");
        //        store.Reload();
        //        store.DataBind();

        //        return this.Store(employeeRepo.GetEmployeeList2(Employer_Id));

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteLog(ex.Message);
        //        X.Msg.Show(new MessageBoxConfig
        //        {
        //            Title = "Error",
        //            Message = "Process failed -" + ex.Message,
        //            Buttons = MessageBox.Button.OK,
        //            Icon = MessageBox.Icon.INFO,
        //            Width = 350


        //        });
        //        return this.Direct();
        //    }

        //}

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

                empList = employeeRepo.GetEmpList(cust_no);

                if (empList.Count > 0)
                {

                    byte[] pic = null;
                    byte[] signature = null;

                    pic = empList[0].Employee_Photo;
                    mImage = empList[0].Employee_Photo;
                    //if (kycList[0].SIGNATURE.Length > 0)
                    signature = empList[0].Employee_Photo_Id;
                    mImage_ID = empList[0].Employee_Photo_Id;


                    if ((pic != null && pic.Length > 0) )
                    {
                        this.GetCmp<Image>("empEdit_pic1").ImageUrl = "../ImageProcessor/ViewImageProcessor.ashx?Cust_No=" + cust_no; // + ", " + Pict_Image; 
                      }
                    if ((signature != null && signature.Length > 0))
                    {
                        this.GetCmp<Image>("empEdit_pic2").ImageUrl = "../ImageProcessor/ViewImageIDProcessor.ashx?Cust_No=" + cust_no; // + ", " + Pict_Image; 
                    }


                }

                return this.Direct();
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
                if (X.GetCmp<FileUploadField>("empEdit_btnloadfile").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("empEdit_btnloadfile").PostedFile;

                    ImageWork.Current_Image_Path = ImageWork.ImagePhysicalPath(file_posted);

                    mImage = ImageWork.ImageToByte(ImageWork.Current_Image_Path);

                    //clear image field
                    this.GetCmp<Image>("empEdit_pic1").ImageUrl = "";
                    this.GetCmp<Image>("empEdit_pic1").ImageUrl = "../ImageProcessor/ImageProcessor.ashx?Filedata=" + ImageWork.Current_Image_Path;

                }

                if (X.GetCmp<FileUploadField>("empEdit_btnloadfile2").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("empEdit_btnloadfile2").PostedFile;

                    ImageWork.Current_Image_Path = ImageWork.ImagePhysicalPath(file_posted);

                    mImage_ID = ImageWork.ImageToByte(ImageWork.Current_Image_Path);

                    //clear image field
                    this.GetCmp<Image>("empEdit_pic2").ImageUrl = "";
                    this.GetCmp<Image>("empEdit_pic2").ImageUrl = "../ImageProcessor/ImageProcessor.ashx?Filedata=" + ImageWork.Current_Image_Path;

                }
                return this.Direct();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /////////////////////////////////////////////////////
        public ActionResult Get_BatchLogPending(string Employer_Id)
        {
           // var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(batch_log.GetBatch_EmployeeList_ByStatus_Delete(Employer_Id));
            }
            catch (Exception ex)
            {
               // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult Get_EmployeesInBatchLogPending(crm_EmployeeRepo emp)
        {
            //var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(employee_repo.GetEmployee_BatchList_ByStatus(emp.Batch_No));
            }
            catch (Exception ex)
            {
               // log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }


     
        public ActionResult ClearControls_batch()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_empbatchapproval_delete");
                x.Reset();

                this.GetCmp<Store>("batchstore_delete").RemoveAll();
                this.GetCmp<Store>("emp_list_store_delete").RemoveAll();
                return this.Direct();
            }
            catch (System.Exception ex)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ex.ToString(),
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350

                });
                return this.Direct();
            }
        }

        public ActionResult Delete_BatchRecord(string batch_no)
        {
            try
            {
                if (string.IsNullOrEmpty(batch_no))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select Batch to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
                //delete Record
                batch_log.Delete_BatchRecord_All(batch_no);

                ClearControls_batch();
                {
        
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Delete Batch",
                        Message = "Batch Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
           
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
                    ora_code = "Record can not be deleted. Process aborted..";
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
//status employee
        public ActionResult EmployeeStatus(crm_EmployeeRepo empRepo)
        {

            try
            {
                if (string.IsNullOrEmpty(empRepo.Cust_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select employee.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }
                if (string.IsNullOrEmpty(empRepo.Cust_Status_New))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select new status.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }

				//if (!string.IsNullOrEmpty(empRepo.Scheme_Id))
				//{
				//	GlobalValue.Get_Scheme_Today_Date(empRepo.Scheme_Id);					
				//}
				//else
				//{
				//	X.Msg.Show(new MessageBoxConfig
				//	{
				//		Title = "Error",
				//		Message = "Scheme cannot be verified.",
				//		Buttons = MessageBox.Button.OK,
				//		Icon = MessageBox.Icon.INFO,
				//		Width = 350

				//	});
				//	return this.Direct();

				//}


				employeeRepo.EmployeeStatus(empRepo);
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employee Status Successfully Changed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });

                    //clear controls
                    ClearControls_employeetransfer();
                }
				Store store = X.GetCmp<Store>("employeestatus_empStore");
				store.Reload();

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
                    ora_code = "Record can not be transferred. Process aborted..";
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
        
        public ActionResult GetESFEmployeeList()
        {
            try
            {
                return this.Store(employeeRepo.GetESFEmployeeList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult GetESFEmployeeListAll()
        {
            try
            {
                return this.Store(employeeRepo.GetESFEmployeeListAll());
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult Read5(string Employer_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id))
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



                // X.GetCmp<Store>("nokStore").Reload();
                //X.GetCmp<Store>("BeneStore").Reload();
                X.GetCmp<Store>("change_ESF_employeeStore").Reload();

                Store store = X.GetCmp<Store>("change_ESF_employeeStore");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList5(Employer_Id);
                if (obj.Count == 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "There are no employees for this employer.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                return this.Store(obj);

            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex.Message);
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


        public ActionResult Read5e(string Employer_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id))
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



                // X.GetCmp<Store>("nokStore").Reload();
                //X.GetCmp<Store>("BeneStore").Reload();
                X.GetCmp<Store>("change_ESF_employeeStore").Reload();

                Store store = X.GetCmp<Store>("change_ESF_employeeStore");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList5e(Employer_Id);
                if (obj.Count == 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "There are no employees for this employer.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                return this.Store(obj);

            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex.Message);
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