using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class crm_ViewEmployeesController : Controller
    {
         crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        IEnumerable<crm_EmployeeRepo> empRepoList = new List<crm_EmployeeRepo>();
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        cLogger logger = new cLogger();
        readonly crm_BeneNextRepo BeneNextRepo = new crm_BeneNextRepo();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddViewEmployeeTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ViewEmployeePartial",
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

        public ActionResult Read(string Employer_Id)
        {
            try
            {
                ClearControls();
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

        public ActionResult Read2(string Employer_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id))
                {
                    X.Mask.Hide();
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

                X.GetCmp<Store>("nokStore").Reload();
                X.GetCmp<Store>("BeneStore").Reload();
                X.GetCmp<Store>("view_employeeStore").Reload();

                Store store = X.GetCmp<Store>("view_employeeStore");
                store.Reload();
                store.DataBind();

                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList2(Employer_Id);
                if(obj.Count == 0)
                {
                    X.Mask.Hide();
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
                X.Mask.Hide();
                return this.Store(obj);

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

        public ActionResult Read22()
        {
            try
            {
               
                X.GetCmp<Store>("nokStore").Reload();
                X.GetCmp<Store>("BeneStore").Reload();
                X.GetCmp<Store>("view_employeeStore").Reload();

                Store store = X.GetCmp<Store>("view_employeeStore");
                store.Reload();
                store.DataBind();
               
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList22();
                X.Mask.Hide();
                if (obj.Count == 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "There are no employees.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }
                X.Mask.Hide();
                return this.Store(obj);

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

        public ActionResult ReadBeneficiary(string Cust_No)
        {
            try
            {

                Store store = X.GetCmp<Store>("BeneStore");
                store.Reload();
                store.DataBind();

                ClearControls();

                return this.Store(employeeRepo.GetBeneficiaryList(Cust_No));
                X.Mask.Hide();
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
        public ActionResult ReadNextOfkin(string Cust_No)
        {
            try
            {

                Store store = X.GetCmp<Store>("NextStore");
                store.Reload();
                store.DataBind();

                ClearControls();
                return this.Store(employeeRepo.GetNextofKinList(Cust_No));
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
                var x = X.GetCmp<FormPanel>("view_empDetails");
                x.Reset();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult DisplayPhoto(string Cust_No)
        {
            try
            {
                if (string.IsNullOrEmpty(Cust_No))
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

                empList = employeeRepo.GetEmpList(Cust_No);

                if (empList.Count > 0)
                {

                    byte[] pic = null;
                    byte[] signature = null;

                    // if (kycList[0].Pict_Image.Length > 0)
                    pic = empList[0].Employee_Photo;

                    //if (kycList[0].SIGNATURE.Length > 0)
                    signature = empList[0].Employee_Photo_Id;

                    if ((pic != null && pic.Length > 0) && (signature != null && signature.Length > 0))
                    {
                        this.GetCmp<Image>("empView_pic1").ImageUrl = "../ImageProcessor/ViewImageProcessor.ashx?Cust_No=" + Cust_No; // + ", " + Pict_Image; 

                        this.GetCmp<Image>("empView_pic2").ImageUrl = "../ImageProcessor/ViewImageIDProcessor.ashx?Cust_No=" + Cust_No; // + ", " + Pict_Image; 
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