using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Serilog;

using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.GlobalObject;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.GL;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class Report_Ledger_AccountController : Controller
    {
        // GET: report
        cLogger logger = new cLogger();
        readonly  GLChartRepo GLRepo = new GLChartRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly Invest_Trans_Fix_Repo MM_TBill = new Invest_Trans_Fix_Repo();

        public ActionResult Report_Ledger_Account_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_LedgerPartial",
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



        public ActionResult GetReportSnapshot()
        {
            try
            {
                if (GlobalValue.Report_Index_Id == "employee_ledger")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/LEDGER_ACCOUNT.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":GL_NO"] = GlobalValue.Report_Param_1;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;
                    
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }

                else if (GlobalValue.Report_Index_Id == "employee_ledgerAll")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/LEDGER_ACCOUNT_ALL.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":SF_ID"] = GlobalValue.Report_Param_1;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else
                {
                    return this.Direct();
                }
            }
            catch (Exception ex)
            {
                string ssss = ex.ToString();
                return this.Direct();
            }

        }

        public ActionResult ViewerEvent()
        {
            try
            {
                if (GlobalValue.Report_Index_Id != "na")
                {
                    return StiMvcViewer.ViewerEventResult();
                }
                else
                {
                    return this.Direct();
                }
            }
            catch (Exception)
            {
                return this.Direct();
            }

        }



        public ActionResult Load_Employee_Report(string containerId)
        {
            return PartialView("Report_LedgerAccountPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string GL_Account_No)
        {
            if (string.IsNullOrEmpty(GL_Account_No))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select GL Account.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }
            GlobalValue.Report_Param_1 = GL_Account_No;
            GlobalValue.Report_Index_Id = "employee_ledger";

            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employee_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();
             
        }

        public ActionResult Load_Employee_Report_IFrameAll(string containerId, string Scheme_Fund_Id)
        {
            if (string.IsNullOrEmpty(Scheme_Fund_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select GL Account.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }
            GlobalValue.Report_Param_1 = Scheme_Fund_Id;
            GlobalValue.Report_Index_Id = "employee_ledgerAll";

            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employee_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();

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


        // filter GL Account for scheme account
        public ActionResult GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFListGL(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { gId = id, gName = name });
            }

            return this.Store(data);
        }



    }// end class

}