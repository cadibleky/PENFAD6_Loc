
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Linq;
using System.Web.Mvc;
using PENFAD6DAL.Services;
using Serilog;

namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    public class Employer_SchemesController : Controller
    {
       // ILogger Log = TeksolLogger.MyLogger();      
             
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        readonly crm_EmployerRepo EmployerRepo = new crm_EmployerRepo();
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {

            try
            {
                var x = X.GetCmp<FormPanel>("ES");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public void ClearControls_Activate()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("ActivateES");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("ApproveES");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public void ClearControls_Close()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("CloseES");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public ActionResult AddESTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployerSchemePartial",
                Model = ESRepo.GetESList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                //Log.Information("Employer Scheme Accessed","username");
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                // Log.Error("Error loading Employer Scheme Account Form","username");
                throw;
            }
        }

        public ActionResult AddESApproveTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployerSchemeApprovePartial",
                Model = ESRepo.GetESList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }
        public ActionResult AddESCloseTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployerSchemeClosePartial",
                Model = ESRepo.GetESList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }

        public ActionResult AddESActivateTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployerSchemeActivatePartial",
                Model = ESRepo.GetESList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }

        public ActionResult AddESViewTab(string containerId = "MainArea")
        {
            try
                { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployerSchemeViewPartial",
                Model = ESRepo.GetESList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }
        public ActionResult SaveRecord(crm_EmployerSchemeRepo ESRepo)
        {
            try
            { 
            if (ESRepo.isESUnique(ESRepo.Employer_Id + ESRepo.Scheme_Id) == true)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Duplicate",
                    Message = "Employer Scheme already exist.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }
            if (ModelState.IsValid)
            {         
               this.ESRepo.SaveRecord(ESRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Employer Scheme Successfully Added.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                ClearControls();
                Store store = X.GetCmp<Store>("esStore");
                store.Reload();

                return this.Direct();

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
                    return this.Direct();
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ApproveRecord(crm_EmployerSchemeRepo ESRepo)
        {
            try
            { 
            if (ESRepo.ES_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Employer Scheme' has been selected for approval.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            ESRepo.ApproveESRecord(ESRepo);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Employer Scheme Successfully Approved.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
            ClearControls_Approve();
            Store store = X.GetCmp<Store>("ApproveesStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception EX)
            {

                throw EX;
            }
        }

        public ActionResult CloseRecord(string ES_ID)
        {
            try
            { 
            if (ES_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Employer Scheme' has been selected for closure.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            ESRepo.CloseESRecord(ES_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Employer Scheme Successfully Closed.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
            ClearControls_Close();
            Store store = X.GetCmp<Store>("CloseesStore");
            store.Reload();

            return this.Direct();

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ActivateRecord(string ES_ID)
        {
            try
            { 
            if (ES_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Employer Scheme' has been selected for activation.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            ESRepo.ActivateESRecord(ES_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Employer Scheme Successfully Activated.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
            ClearControls_Activate();
            Store store = X.GetCmp<Store>("ActivateesStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        public ActionResult Read()
        {
            try
            { 
            return this.Store(ESRepo.GetESList());
            }
            catch (System.Exception)
            {

                throw;
            }

        }
        
        public ActionResult ReadEmployer() 
        {
            try
            { 
            return this.Store(ESRepo.GetEmployerList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult ReadESPending()
        {
            try
            { 
            return this.Store(ESRepo.GetESPendingList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ReadES_NotClosed()
        {
            try
            { 
            return this.Store(ESRepo.GetESNotClosedList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ReadES_Closed()
        {
            try
            { 
            return this.Store(ESRepo.GetESClosedList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}

