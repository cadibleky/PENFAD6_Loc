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
using PENFAD6DAL.Repository.Remittance.Contribution;

namespace PENFAD6UI.Areas.Investment.Controllers.Reports
{
    public class SchemeDashboardController : Controller
    {
        cLogger logger = new cLogger();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        Report_DashSchemeRepo ReportRepo = new Report_DashSchemeRepo();  
        public ActionResult AddDBTab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "ReportDBPartial",
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
                    string pa = Server.MapPath("~/Penfad_Reports/MM_TBILL_BOND_SDB.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_SF_ID"] = GlobalValue.Report_Param_1;
                    My_Report[":P_DATE"] = GlobalValue.Report_Param_2;
					My_Report[":P_USER"] = GlobalValue.Report_Param_2_string;

                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
               
               
                else
                {
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
            string con = "ReportDBStatementPartial";
            //int y = 0;
            for (int i = 1; i <= 1; i++)
            {
                Node treeNode = new Node();

                if (i == 1)
                {
                    treeNode.Text = "DASHBOARD";
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
            X.Mask.Hide();
            return PartialView("ReportDBStatementPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id, DateTime? Report_Date)
        {
            try
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

              
                   ReportRepo.Report_DashScheme(ReportRepo);

                    X.Mask.Hide();
            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();

             }
            catch (Exception ex)
            {
                X.Mask.Hide();
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Sorry! No record found";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Sorry! No record found";
                }
                else
                {
                    ora_code = "Sorry! No record found";
                }
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }

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