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
using PENFAD6DAL.DbContext;
using System.Data;
using Dapper;
using PENFAD6DAL.Repository.GL;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class Report_EmployeeStatementSpecificController : Controller
    {
        // GET: CRM/Report_EmployeeStatement
        cLogger logger = new cLogger();
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();

        public ActionResult Report_Employee_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_EmployeePartial",
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
                if (GlobalValue.Report_Index_Id == "employee_report1")
                {
                    //load report table
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_ESFID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    con.GetConnection().Execute("REPORT_INSERT_STATEMENT_MONTH", param, commandType: CommandType.StoredProcedure);

                    string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionMonth_On_Month_Specific.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_ESF_ID"] = GlobalValue.Report_Param_1;
                    My_Report[":USER_ID"] = GlobalValue.User_ID;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else if (GlobalValue.Report_Index_Id == "employee_report2")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionYear_On_Year_2.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_ESF_ID"] = GlobalValue.Report_Param_1;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else if (GlobalValue.Report_Index_Id == "employee_report3")
                {
					//load report table
					var con = new AppSettings();
					var param = new DynamicParameters();
					param.Add("P_ESF_ID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
					param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
					con.GetConnection().Execute("REPORT_EMPLOYEE_STATEMENT", param, commandType: CommandType.StoredProcedure); 

					string pa = Server.MapPath("~/Penfad_Reports/Employee_BenefitStatementReport_3.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_ESF_ID"] = GlobalValue.Report_Param_1;
					My_Report[":P_USER_ID"] = GlobalValue.User_ID;
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






        public StoreResult GetChildren()
        {
            NodeCollection nodes = new Ext.Net.NodeCollection();
            // int i = 1;
            string node = "employee_report";
            string act = "Load_ReportForm";
            string con = "Report_EmployeeStatement";
            //int y = 0;
            for (int i = 1; i <= 1; i++)
            {
                Node treeNode = new Node();

                if (i == 1)
                {
                    treeNode.Text = "Annual Contribution Statement";
                    treeNode.NodeID = node + i;
                    //treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                    //treeNode.
                }
                else if (i == 2)
                {
                    treeNode.Text = "Contribution Statement - Year On Year";
                    treeNode.NodeID = node + i;
                    //treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                }
                else if (i == 3)
                {
                    treeNode.Text = "Benefits Statement";
                    treeNode.NodeID = node + i;
                    //treeNode.
                   // treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                }
               
                treeNode.Leaf = true;
                nodes.Add(treeNode);
            }
            

            return this.Store(nodes);
        }

 
        public ActionResult Load_Employee_Report(string containerId)
        {
            return PartialView("Report_EmployeeStatementPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string ESF_Id, string tree_node_id, GLTBtRepo TBREPO)
        {
            if (string.IsNullOrEmpty(ESF_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select an employee account.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            if (!TBREPO.TB_Date.HasValue)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select Date.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            GlobalValue.Report_Param_1 = ESF_Id;
            GlobalValue.Report_Param_2 = TBREPO.TB_Date;
            GlobalValue.Report_Index_Id = tree_node_id;
            GlobalValue.Report_Param_2_string = GlobalValue.User_ID;

            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employee_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();
             
        }


        // filter employer scheme for employeeFilterEmpolyeeSFGrid
        public ActionResult FilterESGrid(string Employer_Id)
        {
            var misdepartmentrepo = new crm_EmployeeRepo();
            var mydata = misdepartmentrepo.GetEmployerSFList(Employer_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Scheme_Fund_Id;
                string name = ddd.Scheme_Name + " / " + ddd.Fund_Name;

                data.Add(new { Id = id, Name = name });
            }

            return this.Store(data);
        }
        public ActionResult FilterEmpolyeeSFGrid(String Scheme_Fund_Id, string Employer_Id, string Employer_Name)
        {
            try
            {
                return this.Store(employeeRepo.GetEmployeeSFList(Scheme_Fund_Id, Employer_Id, Employer_Name));
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        

    }// end class

}