using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    public class ApprovedEmployerController : Controller
    {
        // GET: ApprovedEmployer
        readonly crm_EmployerRepo EmployerRepo = new crm_EmployerRepo();
        //...Richard
        string error = "";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddApproveNewEmployerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ApproveNewEmployerPartial",
                Model = EmployerRepo.GetEmployerAppList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddApproveEditedEmployerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ApproveEditedEmployerPartial",

                Model = EmployerRepo.GetEmployerAppList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("approveeditedemployer_employer");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public void ClearAppControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("approvenewemployer_employer");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult ApprovedNewRecord(crm_EmployerRepo EmployerRepo)

        {
            try
            {
                //if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(EmployerRepo.Employer_Id))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Please select a Employer to approved.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350


                        });
                        return this.Direct();
                    }

                    this.EmployerRepo.ApprovedNewRecord(EmployerRepo.Employer_Id, "ACTIVE", GlobalValue.User_ID, "AUTHORIZED", "ACTIVE");

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Approved Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 380
                    });
                    ClearAppControls();
                    Store store = X.GetCmp<Store>("appemployerStore");
                    store.Reload();
                    return this.Direct();
                }

                //return this.Direct();
            }
            catch (System.Exception ex)
            {
                X.Mask.Hide();
                // Log.Error("Error loading Employer Scheme Account Form","username");
                throw ex;
            }
        }
        

        public ActionResult DisapprovedNewRecord(crm_EmployerRepo EmployerRepo)
        {
            if (string.IsNullOrEmpty(EmployerRepo.Employer_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Employer Name to disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            EmployerRepo.DisapprovedNewRecord(EmployerRepo);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Disapproved Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearAppControls();
            Store store = X.GetCmp<Store>("appemployerStore");
            store.Reload();
            return this.Direct();
        }

        public ActionResult ApprovedEditedRecord(crm_EmployerRepo EmployerRepo)

        {
            if (string.IsNullOrEmpty(EmployerRepo.Employer_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Employer to approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            //if (ModelState.IsValid)
            {
                this.EmployerRepo.ApprovedEditedRecord(EmployerRepo.Employer_Id, "ACTIVE", GlobalValue.User_ID, "AUTHORIZED");
           
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Approved Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("appeditedemployerStore");
                store.Reload();
                return this.Direct();
            }

            //return this.Direct();
        }

        public ActionResult DisapprovedEditedRecord(crm_EmployerRepo EmployerRepo)
        {
            if (string.IsNullOrEmpty(EmployerRepo.Employer_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Employer Name to disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            EmployerRepo.DisapprovedEditedRecord(EmployerRepo);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Disapproved Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("appeditedemployerStore");
            store.Reload();
            return this.Direct();
        }

        public ActionResult ReadNew()
        {
            return this.Store(EmployerRepo.GetEmployerAppList());
        }

        public ActionResult ReadEdited()
        {
            return this.Store(EmployerRepo.GetEmployerAppList2());
        }
    }
}