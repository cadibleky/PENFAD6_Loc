
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.Remittance.Contribution;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Unit_Change
{
    public class Remit_Unit_ChangeController: Controller
    {
        Remit_Unit_TransferRepo Unit_ChangeRepo = new Remit_Unit_TransferRepo();
        crm_EmployeeRepo EmployeeRepo = new crm_EmployeeRepo();
        pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
		readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
		public ActionResult AddUnit_ChangeTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Unit_ChangePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddUnit_ChangeApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Unit_ChangeApprovePartial",
                // Model = PurchaseRepo.GetPurchasePendingList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddUnit_MergeChangeTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Unit_MergePartial",
                // Model = PurchaseRepo.GetPurchasePendingList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmUnit_ChangeApprovePartial");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmUnit_ChangePartial");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult DelRecord(Remit_Unit_TransferRepo Unit_ChangeRepo)
        {
            try
            {
				if (string.IsNullOrEmpty(Unit_ChangeRepo.ESF_Id))
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please select employee account",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
					return this.Direct();
				}

				if (string.IsNullOrEmpty(Unit_ChangeRepo.Purchase_Log_Id))
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please select contribution/purchase",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
					return this.Direct();
				}

				this.Unit_ChangeRepo.DelRecord(Unit_ChangeRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Contribution successfully deleted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "Unit_ChangePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;

			}
            catch (Exception ex)
            {
                return this.Direct();
            }
        }


       
       

      
        public ActionResult ReadPurchases(Remit_Unit_TransferRepo Unit_ChangeRepo)
        {
            try
            {
                return this.Store(Unit_ChangeRepo.GetPurchasesList2(Unit_ChangeRepo));

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ReadSF()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult ReadPendingUT(Remit_Unit_TransferRepo Unit_ChangeRepo)
        {
            try
            {
                return this.Store(Unit_ChangeRepo.GetPendingUTList(Unit_ChangeRepo));
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        
        //public ActionResult FilterEmpolyeeSFGrid(string Scheme_Fund_Id)
        //{
        //    try
        //    {
        //        return this.Store(Unit_ChangeRepo.GetESFList(Scheme_Fund_Id));
        //    }
        //    catch (System.Exception)
        //    {

        //        throw;
        //    }
        //}

		public ActionResult FilterEmpolyeeSFGrid2()
		{
			try
			{
				return this.Store(Unit_ChangeRepo.GetESFList2());
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
				X.GetCmp<Store>("change_ESF_employeeStore_employer_remit").Reload();

				Store store = X.GetCmp<Store>("change_ESF_employeeStore_employer_remit");
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

	}
}



