using Dapper;
using Ext.Net;
using Ext.Net.MVC;
using IBankWebService.Utils;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.Investment.Bond;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

//namespace PENFAD6UI.Areas.CRM.Controllers.Employees
namespace PENFAD6UI.Areas.Remittance.Controllers.BatchPortOut
{
    public class BatchPortOutController : Controller
    {
        readonly crm_EmployeeBatchLogRepo batch_log = new crm_EmployeeBatchLogRepo();
        readonly crm_EmployeeRepo employee_repo = new crm_EmployeeRepo();
        IDbConnection con;

        // GET: CRM/EmployeeBatchUploadApproval
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddPortOutEmployerTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "BatchPortOut",
                    //Model = empList,
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

        public ActionResult Get_Batchemp(crm_EmployeeBatchLogRepo batch_log)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(batch_log.GetBatch_EmployeeList_ByStatus_portout(batch_log.Employer_Id));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

        public ActionResult Get_EmployeesInBatchLogPending(crm_EmployeeRepo emp)
        {
            var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
            try
            {
                return this.Store(employee_repo.GetEmployee_BatchList_ByStatus_portout(emp));
            }
            catch (Exception ex)
            {
                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }

        }

		// filter GL Account for scheme
		public ActionResult GetGLAB(string Scheme_Fund_Id)
		{
			var misdepartmentrepo = new BondRepo();
			var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

			List<object> data = new List<object>();
			foreach (var ddd in mydata)
			{
				string id = ddd.GL_Account_No;
				string name = ddd.GL_Account_Name;

				data.Add(new { PortOutPay_GLB_gId_portout = id, gName = name });
			}

			return this.Store(data);
		}

		// filter GL Account for scheme
		public ActionResult GetGLacc(string GL_Account_No)
		{
			var misdepartmentrepo = new Invest_Equity_CISRepo();
			var mydata = misdepartmentrepo.GetGLAccFList(GL_Account_No);

			List<object> data = new List<object>();
			foreach (var ddd in mydata)
			{
				decimal id = ddd.GL_Balance * -1;
				decimal name = ddd.GL_Balance * -1;

				data.Add(new { ID_BatchPort = id, mName = name });
			}

			return this.Store(data);
		}

		public ActionResult Approve_Pending_Batch(crm_EmployeeBatchLogRepo repo_emplog)
        {
            try
            {
				if (repo_emplog.Unit_Price <=0)
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please enter a valid unit price",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});

					return this.Direct();
				}


				if (string.IsNullOrEmpty(repo_emplog.Fund_Id) || string.IsNullOrEmpty(repo_emplog.Scheme_Id))
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
						{
							Title = "Error",
							Message = "Please select employer account",
							Buttons = MessageBox.Button.OK,
							Icon = MessageBox.Icon.INFO,
							Width = 350

						});

						return this.Direct();
					}

				if (string.IsNullOrEmpty(repo_emplog.New_Trustee))
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please select new trustee",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});

					return this.Direct();
				}

				if (!repo_emplog.Pay_Date_Benefit.HasValue)
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please select Port-Out Date",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});

					return this.Direct();
				}


				if (string.IsNullOrEmpty(repo_emplog.GL_Account_No))
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please bank account",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});

					return this.Direct();
				}

				if (!string.IsNullOrEmpty(repo_emplog.Scheme_Id))
				{
					GlobalValue.Get_Scheme_Today_Date(repo_emplog.Scheme_Id);
					if (repo_emplog.Pay_Date_Benefit != GlobalValue.Scheme_Today_Date)
					{
						X.Mask.Hide();
						X.Msg.Show(new MessageBoxConfig
						{
							Title = "Error",
							Message = "Sorry! Transaction date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
							Buttons = MessageBox.Button.OK,
							Icon = MessageBox.Icon.INFO,
							Width = 350

						});

						return this.Direct();
					}
				}
				else
				{
					X.Mask.Hide();
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Scheme cannot be verified.",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
				}

                //check if amount is enough
                //do

				repo_emplog.Portout (repo_emplog);
				X.Mask.Hide();
				X.Msg.Show(new MessageBoxConfig
					{
						Title = "Success",
						Message = "Batch  Port-Out Processed Successfully.",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350

					});
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "BatchPortOut",
					//Model = empList,
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,

				};

				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();

				return pvr;

			}
			catch (Exception ex)
            {
                X.Mask.Hide();
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

        public ActionResult ClearControls()
        {
            try
            {

				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "BatchPortOut",
					//Model = empList,
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,

				};

				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();

				return pvr;
			}
            catch (System.Exception ex)
            {
                X.Mask.Hide();
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

    }//end s class
}