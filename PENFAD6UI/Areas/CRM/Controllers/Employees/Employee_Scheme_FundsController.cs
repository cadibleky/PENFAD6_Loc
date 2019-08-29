using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class Employee_Scheme_FundsController : Controller
    {
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly crm_EmployeeRepo EmployeeRepo = new crm_EmployeeRepo();
        readonly crm_EmployeeBatchLogRepo batch_log = new crm_EmployeeBatchLogRepo();
        readonly crm_EmployeeRepo employee_repo = new crm_EmployeeRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        IEnumerable<crm_EmployeeRepo> empRepoList = new List<crm_EmployeeRepo>();
        readonly crm_EmployerRepo employer = new crm_EmployerRepo();
        List<crm_EmployeeRepo> empList = new List<crm_EmployeeRepo>();
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("ESF");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public void ClearControls_change()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("changeESF");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public void ClearControls_changeemployer()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("changeESF_employer");
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
                var x = X.GetCmp<FormPanel>("ActivateESF");
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
                var x = X.GetCmp<FormPanel>("ApproveESF");
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
                var x = X.GetCmp<FormPanel>("CloseESF");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddESFTab(string containerId = "MainArea")
        {
            try { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployeeSchemeFundPartial",
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

        public ActionResult AddChangeESFTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployeeChangeSchemeFundPartial",
                    // Model = ESFRepo.GetESFList(),
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

        public ActionResult AddESFApproveTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployeeSchemeFundApprovePartial",
              //  Model = ESFRepo.GetESFList(),
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
                throw ex;
            }
        }
        public ActionResult AddESFViewTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployeeSchemeFundViewPartial",
               // Model = ESFRepo.GetESFList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception EX)
            {
                X.Mask.Hide();
                throw EX;
            }
        }

        public ActionResult AddESFCloseTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployeeSchemeFundClosePartial",
               // Model = ESFRepo.GetESFList(),
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

        public ActionResult AddESFActivateTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "EmployeeSchemeFundActivatePartial",
               // Model = ESFRepo.GetESFList(),
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

        public ActionResult AddChangeESFTab_Employer(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EmployerChangeSchemeFundPartial",
                    // Model = ESFRepo.GetESFList(),
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

        public ActionResult SaveRecord(crm_EmployeeSchemeFundRepo ESFRepo)
        {
            try
            {
                string esf_iddd = ESFRepo.Cust_No + ESFRepo.Scheme_Id;
            if (ESFRepo.isESUnique(esf_iddd) == true)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Duplicate",
                    Message = "Employee already exist on this scheme.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }
            //// validate if the employee has an existing ACTIVE fund account

            if (ModelState.IsValid)
            {         
               this.ESFRepo.SaveRecord(ESFRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Employee Fund_Scheme Successfully Added.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                ClearControls();
                Store store = X.GetCmp<Store>("esfStore");
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
            catch (System.Exception EX)
            {

                throw EX;
            }

        }

        public ActionResult ChangeRecord(crm_EmployeeSchemeFundRepo ESFRepo)
        {
            try
            {
                string esf_iddd = ESFRepo.Cust_No + ESFRepo.New_Scheme_Fund_Id;
                if (ESFRepo.isESUnique(esf_iddd) == true)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Duplicate",
                        Message = "Employee Scheme_Fund already exist.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }


                //// check if there is enougth money for transfer			
                //if (ESFRepo.Get_GL_Balance(ESFRepo) == true)
                //{
                //    X.Mask.Hide();
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Insufficient cash in collection account. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                // check if there is collection account for new scheme			
                if (ESFRepo.Get_GL_coll(ESFRepo) == true)
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "No collection account for new scheme account. Process aborted",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350
					});
					return this.Direct();
				}

            


				if (ModelState.IsValid)
                {
                   this.ESFRepo.ChangeRecord(ESFRepo);
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employee Scheme Fund Successfully Changed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    X.Mask.Hide();
                    ClearControls_change();
                    Store store = X.GetCmp<Store>("change_ESF_employeeStore");
                    store.Reload();
                    Store store1 = X.GetCmp<Store>("changeSFStore");
                    store1.Reload();

                    return this.Direct();

                }
                else
                {
                    X.Mask.Hide();
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                    X.Mask.Hide();
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
            catch (System.Exception EX)
            {
                X.Mask.Hide();
                throw EX;
            }

        }

        public ActionResult ChangeRecordemployer(crm_EmployeeSchemeFundRepo ESFRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(ESFRepo.Employer_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select employee",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }
                if (string.IsNullOrEmpty(ESFRepo.New_Employer_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select new employer",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }



				this.ESFRepo.ChangeRecordemployer(ESFRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employer Successfully Changed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
				//ClearControls_changeemployer();
				//Store store = X.GetCmp<Store>("change_ESF_employeeStore_employer");
				//store.Reload();
				//Store store1 = X.GetCmp<Store>("changeSFStore_employer");
				//store1.Reload();
				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "EmployerChangeSchemeFundPartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;
			}
            catch (System.Exception EX)
            {

                throw EX;
            }

        }



        public ActionResult ApproveRecord(string ESF_ID)
        {
            try
            { 
            if (ESF_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Employee Scheme_Fund' has been selected for approval.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            ESFRepo.ApproveESFRecord(ESF_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Employee Scheme_Fund Successfully Approved.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
            ClearControls_Approve();
            Store store = X.GetCmp<Store>("ApproveesfStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult CloseRecord(string ESF_ID, string Scheme_Id)
        {
            try
            { 
            if (ESF_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Employee Scheme_Fund' has been selected for closure.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

				if (!string.IsNullOrEmpty(Scheme_Id))
				{
					GlobalValue.Get_Scheme_Today_Date(Scheme_Id);
				}
				else
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Scheme cannot be verified.",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
					return this.Direct();

				}

				ESFRepo.CloseESFRecord(ESF_ID);

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Employee Scheme_Fund Successfully Closed.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls_Close();
            Store store = X.GetCmp<Store>("CloseesfStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ActivateRecord(string ESF_ID)
        {
            try
            { 
            if (ESF_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Employee Scheme_Fund' has been selected for activation.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            ESFRepo.ActivateESFRecord(ESF_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Employee Scheme_Fund Successfully Activated.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350

            });
            ClearControls_Activate();
            Store store = X.GetCmp<Store>("ActivateesfStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        //public ActionResult DeleteRecord(string SCHEME_FEE_ID)
        //{
        //    if (SCHEME_FEE_ID == string.Empty)
        //    {
        //        X.Msg.Show(new MessageBoxConfig
        //        {
        //            Title = "Error",
        //            Message = "Sorry! No 'Employee Fund_Scheme' has been selected.",
        //            Buttons = MessageBox.Button.OK,
        //            Icon = MessageBox.Icon.ERROR,
        //            Width = 350
        //        });
        //        return this.Direct();
        //    }

        //    ESFRepo.DeleteRecord(SCHEME_FEE_ID);
        //    X.Msg.Show(new MessageBoxConfig
        //    {
        //        Title = "Success",
        //        Message = "Employee Fund_Scheme Successfully Deleted.",
        //        Buttons = MessageBox.Button.OK,
        //        Icon = MessageBox.Icon.INFO,
        //        Width = 350


        //    });
        //    ClearControls();
        //    Store store = X.GetCmp<Store>("schemeFeeStore");
        //    store.Reload();

        //    return this.Direct();
        //}
        public ActionResult Read()
        {
            try
            { 
            return this.Store(ESFRepo.GetESFList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult GetESFEmployeeList()
        {
            try
            {
                return this.Store(EmployeeRepo.GetESFEmployeeList());
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public ActionResult ReadESFPending()
        {
            try
            { 
            return this.Store(ESFRepo.GetESFPendingList());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ReadESFNotClosed()
        {
            try
            { 
            return this.Store(ESFRepo.GetESFNotClosedList());
            }
            catch (System.Exception EX)
            {

                throw EX;
            }
        }
        public ActionResult ReadESFClosed()
        {
            try
            { 
            return this.Store(ESFRepo.GetESFClosedList());
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult ReadSF()
        {
            try { 
            return this.Store(SFRepo.GetSchemeFundList());
            }
            catch (System.Exception)
            {

                throw;
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

        //
        public ActionResult FilterESGrid2()
        {
            try
            {

                Store store = X.GetCmp<Store>("esfStore");
                store.Reload();
                store.DataBind();

                return this.Store(ESFRepo.GetESCHEMEList2());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }
        public ActionResult FilterESGrid3(string Scheme_Id)
        {
            try
            {

                Store store = X.GetCmp<Store>("changeSFStore");
                store.Reload();
                store.DataBind();

                return this.Store(ESFRepo.GetESCHEMEList3(Scheme_Id));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }


        public ActionResult FilterESGrid6(string Scheme_Id)
        {
            try
            {

                Store store = X.GetCmp<Store>("changeSFStore_employer");
                store.Reload();
                store.DataBind();

                return this.Store(ESFRepo.GetESCHEMEList6(Scheme_Id));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }


        public ActionResult Read2(string Employer_Id, string Employer_Name)
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
                X.GetCmp<Store>("ESF_employeeStore").Reload();

                Store store = X.GetCmp<Store>("ESF_employeeStore");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList2(Employer_Id, Employer_Name);
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


        public ActionResult Read5(string Employer_Id, string Employer_Name)
        {
            try
            {
                if (string.IsNullOrEmpty(Employer_Id) || Employer_Id == null)
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
                X.GetCmp<Store>("change_ESF_employeeStore_send").Reload();

                Store store = X.GetCmp<Store>("change_ESF_employeeStore_send");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList5(Employer_Id, Employer_Name);
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


        public ActionResult Read6(string Employer_Id, string Employer_Name)
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
                X.GetCmp<Store>("change_ESF_employeeStore_send").Reload();

                Store store = X.GetCmp<Store>("change_ESF_employeeStore_send");
                store.Reload();
                store.DataBind();
                List<crm_EmployeeSchemeFundRepo> obj = employeeRepo.GetEmployeeList6(Employer_Id, Employer_Name);
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

