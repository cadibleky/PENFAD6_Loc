﻿using Ext.Net;
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
    public class Report_SchemeInvestmentReceiptController : Controller
    {
        // GET:
        cLogger logger = new cLogger();
        readonly pfm_SchemeRepo schemeRepo = new pfm_SchemeRepo();

        public ActionResult Report_SchemeInvestmentReceipt_Tab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_SchemeInvestmentRePartial",
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
                    string pa = Server.MapPath("~/Penfad_Reports/Investment_Receipt.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_SID"] = GlobalValue.Report_Param_1;
                    My_Report[":P_DATE"] = GlobalValue.Report_Param_2;
                    My_Report[":P_DATE2"] = GlobalValue.Report_Param_1_date;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                //else if (GlobalValue.Report_Index_Id == "employee_report2")
                //{
                //    string pa = Server.MapPath("~/Penfad_Reports/Employee_ContributionYear_On_Year_2.dll");

                //    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                //    //////asign constring
                //    My_Report.Dictionary.DataStore.Clear();
                //    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("TekSolPenfad", GlobalValue.ConString));

                //    My_Report[":P_ESF_ID"] = GlobalValue.Report_Param_1;
                //    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                //    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                //}
                //else if (GlobalValue.Report_Index_Id == "employee_report3")
                //{
                //    string pa = Server.MapPath("~/Penfad_Reports/Employee_BenefitStatementReport_3.dll");

                //    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                //    //////asign constring
                //    My_Report.Dictionary.DataStore.Clear();
                //    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("TekSolPenfad", GlobalValue.ConString));

                //    My_Report[":P_ESF_ID"] = GlobalValue.Report_Param_1;
                //    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                //    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                //}
               
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

            //int y = 0;
            for (int i = 1; i <= 1; i++)
            {
                Node treeNode = new Node();

                if (i == 1)
                {
                    treeNode.Text = "Preview";
                    treeNode.NodeID = node + i;
                    //treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                    //treeNode.
                }
                //else if (i == 2)
                //{
                //    treeNode.Text = "Contribution Statement - Year On Year";
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

 
        public ActionResult Load_Employee_Report(string containerId)
        {
            return PartialView("Report_SchemeInvestmentReceiptPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id, DateTime? Report_Date, DateTime? Report_Date2)
        {
            if (string.IsNullOrEmpty(Scheme_Id))
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select Scheme.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }
            GlobalValue.Report_Param_1 = Scheme_Id;         
            GlobalValue.Report_Param_2 = Report_Date;
            GlobalValue.Report_Index_Id = tree_node_id;
            GlobalValue.Report_Param_1_date = Report_Date2;
            X.Mask.Hide();
            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employee_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();
             
        }


        public ActionResult FilterESGrid(string Scheme_Id)
        {
            try
            {
                return this.Store(schemeRepo.GetSchemeList());
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }


    }// end class

}