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

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class Report_EmployeeRetirementController : Controller
    {
        // GET: CRM/Report_EmployeeStatement
        cLogger logger = new cLogger();
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();

        public ActionResult Report_EmployeeRetirement_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_EmployeeRePartial",
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
                if (GlobalValue.Report_Index_Id == "employee_retirement")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Retirement_List.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_YEAR"] = GlobalValue.Report_Param_1;
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
            return PartialView("Report_EmployeeRetirementPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string Year)
        {
            if (string.IsNullOrEmpty(Year))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select Year.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }
            GlobalValue.Report_Param_1 = Year;
            GlobalValue.Report_Index_Id = "employee_retirement";

            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employee_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();
             
        }


        

    }// end class

}