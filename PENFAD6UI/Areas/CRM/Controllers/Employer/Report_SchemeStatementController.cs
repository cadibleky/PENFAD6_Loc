using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Serilog;
using PENFAD6DAL.GlobalObject;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Remittance.Contribution;
using System.Web;
using PENFAD6DAL.Repository.Setup.PfmSetup;

namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    public class Report_SchemeStatementController : Controller
    {
        // GET: CRM/Report_EmployerStatement

        cLogger logger = new cLogger();
        readonly crm_EmployerRepo employerRepo = new crm_EmployerRepo();
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        readonly Remit_ReceiptRepo ReceiptRepo = new Remit_ReceiptRepo();
        readonly pfm_SchemeRepo SchemeRepo = new pfm_SchemeRepo();

        public string ES_ID = "na";
        public ActionResult Report_Employer_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_SchemePartial",
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
                if (GlobalValue.Report_Index_Id == "employer_report1")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Scheme_Con_Monthly_1.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_ES_ID"] = GlobalValue.Report_Param_1; // GlobalValue.Rep_Cust_No;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else if (GlobalValue.Report_Index_Id == "employer_report2")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Scheme_Con_Annual_2.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_ES_ID"] = GlobalValue.Report_Param_1;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else if (GlobalValue.Report_Index_Id == "employer_report3")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Scheme_Payment_3.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_ES_ID"] = GlobalValue.Report_Param_1;
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
            string node = "employer_report";
            string act = "Load_ReportForm";
            string con = "Report_EmployerStatement";
            //int y = 0;
            for (int i = 1; i <= 3; i++)
            {
                Node treeNode = new Node();

                if (i == 1)
                {
                    treeNode.Text = "Contribution Statement - Month On Month";
                    treeNode.NodeID = node + i;
                    
                }
                else if (i == 2)
                {
                    treeNode.Text = "Contribution Statement - Year On Year";
                    treeNode.NodeID = node + i;
                    
                }
                else if (i == 3)
                {
                    treeNode.Text = "Payment Statement";
                    treeNode.NodeID = node + i;
                }
                //else if (i == 4)
                //{
                //    treeNode.Text = "summary statement";
                //    treeNode.NodeID = node + i;
                //    treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                //}

                treeNode.Leaf = true;
                nodes.Add(treeNode);
            }


            return this.Store(nodes);
        }


        public ActionResult Load_Employer_Report(string containerId)
        {
            return PartialView("Report_SchemeStatementPartial");
        }

        public ActionResult Load_Employer_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id)
        {
            if(string.IsNullOrEmpty(Scheme_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select an Scheme.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }
            //GlobalValue.Rep_Cust_No = cust_no;
            GlobalValue.Report_Param_1 = Scheme_Id;

            GlobalValue.Report_Index_Id = tree_node_id;

            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employer_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            return this.Direct();

        }


        // fill employee grid
        public ActionResult Read2(string Scheme_Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Scheme_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Scheme is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

               // X.GetCmp<Store>("Report_SchemePartial_employeeStore").Reload();
                Store store = X.GetCmp<Store>("SchemeempStore");
                store.Reload();
                store.DataBind();

                //List<crm_EmployeeRepo> obj = employeeRepo.GetEmployeeList2(Employer_Id);
                //if (obj.Count == 0)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "There are no employees for this employer.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //    return this.Direct();
                //}
                return this.Direct();
                //return this.Store(obj);

            }
            catch (Exception ex)
            {
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
        //end fill employee grid


        public ActionResult ReadEmployerScheme()
        {
            try
            {
                return this.Store(SchemeRepo.GetSchemeList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

    }// end class
}