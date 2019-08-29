
using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class crm_BeneNextController : Controller
    {
        readonly crm_BeneNextRepo BeneNextRepo = new crm_BeneNextRepo();
        readonly crm_EmployeeRepo EmplpoyeeRepo = new crm_EmployeeRepo();
        static List<crm_BeneNextRepo> BeneNextRepoList = new List<crm_BeneNextRepo>();
		readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();

		public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
          + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
          + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("ESF_bene");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddBeneNextTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "BeneNextPartial",
                    //Model = BeneNextRepo.GetBeneNextList(),
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

		public ActionResult Read5(string Employer_Id, string Employer_Name)
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
				X.GetCmp<Store>("change_ESF_employeeStore_employer_bene").Reload();

				Store store = X.GetCmp<Store>("change_ESF_employeeStore_employer_bene");
				store.Reload();
				store.DataBind();
				List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList8(Employer_Id, Employer_Name);
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
		public ActionResult AddBeneNextEditTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "BeneNextEditPartial",
                    //Model = BeneNextRepo.GetBeneNextList(),
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return pvr;
            }
            catch (System.Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }

        public ActionResult AddBeneBatchTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "BeneBatchPartial",
                    //Model = BeneNextRepo.GetBeneNextList(),
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

        public ActionResult SaveRecord(crm_BeneNextRepo BeneNextRepo)

        {
            try
            {
                if (!BeneNextRepo.Date_Of_Birth.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Date of Birth' is invalid.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }

                if (BeneNextRepo.Beneficiary_Rate > 100)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Beneficiary Rate' can not be 100%.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }
                if (BeneNextRepo.Beneficiary_Rate <= 0 && BeneNextRepo.Beneficiary_NextOfKin == "BENEFICIARY")
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Beneficiary Rate' can not be '0' or less.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }

                //check total beneficiary rate
                BeneNextRepo.checkrate(BeneNextRepo);

                if ((BeneNextRepo.Beneficiary_Rate + BeneNextRepo.Total_Rate) > 100)
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Total Beneficiary Rate Can Not Be More Than 100%",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });         
                    return this.Direct();
                }

                if (ModelState.IsValid)
                {
                    BeneNextRepo.SaveRecord(BeneNextRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Beneficiary/Next of kin Successfully Saved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    ClearControls();
                    Store store = X.GetCmp<Store>("BeneNextStore");
                    store.RemoveAll();

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


        public ActionResult EditRecord(crm_BeneNextRepo BeneNextRepo)

        {
            try
            {
                if (Microsoft.VisualBasic.Information.IsNumeric(BeneNextRepo.Beneficiary_NextOfKin_Id) != true)
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Please select Beneficiary or Next of Kin to Edit.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 400
                    });
                    return this.Direct();

                }

                if (!string.IsNullOrEmpty(BeneNextRepo.Phone_Number1))
                {
                    if (Microsoft.VisualBasic.Information.IsNumeric(BeneNextRepo.Phone_Number1) == false)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "'Primary Phone Number' is invalid.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();
                    }
                }

                if (!string.IsNullOrEmpty(BeneNextRepo.Phone_Number2))
                {
                    if (Microsoft.VisualBasic.Information.IsNumeric(BeneNextRepo.Phone_Number2) == false)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "'Secondary Phone Number' is invalid.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });
                        return this.Direct();
                    }
                }

                if (!string.IsNullOrEmpty(BeneNextRepo.Email_Address))
                {
                    if (Regex.IsMatch(BeneNextRepo.Email_Address, MatchEmailPattern) == false)
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

                if (!BeneNextRepo.Date_Of_Birth.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Date of Birth' is invalid.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }

                if (BeneNextRepo.Beneficiary_Rate > 100)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Beneficiary Rate' can not be 100%.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }
                if (BeneNextRepo.Beneficiary_Rate <= 0 && BeneNextRepo.Beneficiary_NextOfKin == "BENEFICIARY")
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "'Beneficiary Rate' can not be '0' or less.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();

                }

                //check total beneficiary rate
                BeneNextRepo.checkrate_edit(BeneNextRepo);

                if ((BeneNextRepo.Beneficiary_Rate + BeneNextRepo.Total_Rate - BeneNextRepo.Beneficiary_Rate_Temp) > 100)
                {

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Total Beneficiary Rate Can Not Be More Than 100%",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }


                if (ModelState.IsValid)
                {
                    this.BeneNextRepo.SaveRecord(BeneNextRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Beneficiary/Next of kin Successfully Saved.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    ClearControls();
                    Store store = X.GetCmp<Store>("BeneNextStore");
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
            catch (System.Exception ex)
            {

                throw ex;
            }

        }



        public ActionResult DeleteRecord(string BENEFICIARY_NEXTOFKIN_ID)
        {
            try
            {
                if (BENEFICIARY_NEXTOFKIN_ID == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No 'Beneficiary / Next of kin' has been selected.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                BeneNextRepo.DeleteRecord(BENEFICIARY_NEXTOFKIN_ID);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "'Beneficiary / Next of kin' Successfully Deleted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                ClearControls();
                Store store = X.GetCmp<Store>("BeneNextStore");
                store.RemoveAll();

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
                return this.Store(BeneNextRepo.GetBeneNextList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ReadEmployee()
        {
            try
            {
                return this.Store(BeneNextRepo.GetEmployeeList());
            }
            catch (System.Exception EX)
            {

                throw EX;
            }

        }

        // filter benefeciary and next of kin grid
        public ActionResult FilterBNGrid(string Cust_No)
        {
            try
            {

                Store store = X.GetCmp<Store>("BeneNextStore");
                store.RemoveAll();
                store.DataBind();

                return this.Store(BeneNextRepo.GetBNList(Cust_No));
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }


        
    }


}
    
