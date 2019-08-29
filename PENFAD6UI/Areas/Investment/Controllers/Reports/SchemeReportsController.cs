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
using Dapper;
using System.Data;
using System.Transactions;
using Oracle.ManagedDataAccess.Client;

namespace PENFAD6UI.Areas.Investment.Controllers.Reports
{
    public class SchemeReportsController : Controller
    {
        cLogger logger = new cLogger();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();

        public ActionResult AddPVTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ReportPVPartial",
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

               
    //            //load report table
    //            var con = new AppSettings();
				//var param = new DynamicParameters();
				//param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
				//param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
				//param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
				//con.GetConnection().Execute("REPORT_INSERT_PORT_SUM_SCH", param, commandType: CommandType.StoredProcedure);


				if (GlobalValue.Report_Index_Id == "employee_report1")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/MM_TBILL_BOND_PV_Scheme.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();                   
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
                    My_Report[":P_SF_ID"] = GlobalValue.Report_Param_1;
                    My_Report[":P_DATE"] = GlobalValue.Report_Param_2;
					My_Report[":P_USER"] = GlobalValue.Report_Param_2_string;
                    //((StiOracleSource)My_Report.Dictionary.DataSources["con"]).CommandTimeout = 6000;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                   
                }
               
                else
                {
                    X.Mask.Hide();
                    return this.Direct();
                }
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
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

                X.Mask.Hide();
                return this.Direct();
            }

        }






        public StoreResult GetChildren()
        {
            NodeCollection nodes = new Ext.Net.NodeCollection();
            // int i = 1;
            string node = "employee_report";
            string act = "Load_ReportForm";
            string con = "ReportPVStatementPartial";
            //int y = 0;
            for (int i = 1; i <= 1; i++)
            {
                Node treeNode = new Node();

                if (i == 1)
                {
                    treeNode.Text = "Portfolio Valuation";
                    treeNode.NodeID = node + i;
                    //treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                    //treeNode.
                }
                //else if (i == 2)
                //{
                //    treeNode.Text = "Equities & CIS";
                //    treeNode.NodeID = node + i;
                //    //treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                //}
                //else if (i == 3)
                //{
                //    treeNode.Text = "Benefits Statement";
                //    treeNode.NodeID = node + i;
                //    //treeNode.
                //   // treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                //}
               
                treeNode.Leaf = true;
                nodes.Add(treeNode);
            }
            

            return this.Store(nodes);
        }

 
        public ActionResult Load_Report(string containerId)
        {
            return PartialView("ReportPVStatementPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id, DateTime? Report_Date)
        {


            if (string.IsNullOrEmpty(Scheme_Id))
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select a scheme.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            if (!Report_Date.HasValue)
            {
                X.Mask.Hide();
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
            GlobalValue.Report_Param_1 = Scheme_Id;
            GlobalValue.Report_Param_2 = Report_Date;
			GlobalValue.Report_Param_2_string = GlobalValue.User_ID;
			GlobalValue.Report_Index_Id = tree_node_id;

            var app = new AppSettings();
            //load report table
            var con = new AppSettings();
            var param = new DynamicParameters();
            param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
            param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
            param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
            con.GetConnection().Execute("REPORT_INSERT_PORT_SUM_SCH", param, commandType: CommandType.StoredProcedure);

            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            //tsOp.Timeout = TimeSpan.FromMinutes(160);

            //using (OracleConnection conn = new OracleConnection(app.conString()))  //
            //{
            //    var param = new DynamicParameters();
            //    param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
            //    param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
            //    param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
            //    conn.Execute("REPORT_INSERT_PORT_SUM_SCH", param, commandType: CommandType.StoredProcedure);



            //    //var param1 = new DynamicParameters();
            //    //param1.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
            //    //param1.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
            //    //param1.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
            //    //conn.Execute("REPORT_INSERT_PORT_SUM_SDB1", param1, commandType: CommandType.StoredProcedure);

            //    //var param11 = new DynamicParameters();
            //    //param11.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
            //    //param11.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
            //    //param11.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
            //    //conn.Execute("REPORT_INSERT_PORT_SUM_SDB11", param11, commandType: CommandType.StoredProcedure);

            //    //var param21 = new DynamicParameters();
            //    //param21.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
            //    //param21.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
            //    //param21.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
            //    //conn.Execute("REPORT_INSERT_PORT_SUM_SDB2", param21, commandType: CommandType.StoredProcedure);


            //    //var param2 = new DynamicParameters();
            //    //param2.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
            //    //param2.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
            //    //param2.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
            //    //conn.Execute("REPORT_INSERT_PORT_SUM_SCH2", param, commandType: CommandType.StoredProcedure);

            //    ts.Complete();

            //}

            X.Mask.Hide();
            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            X.Mask.Hide();
            return this.Direct();
             
        }



        public ActionResult ReadSF()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundListscheme());
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }


    }// end class

}