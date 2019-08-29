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



namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    public class Report_EmployerBenController : Controller
    {
        // GET: CRM/Report_EmployerStatement

        cLogger logger = new cLogger();
        readonly crm_EmployerRepo employerRepo = new crm_EmployerRepo();
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        readonly Remit_ReceiptRepo ReceiptRepo = new Remit_ReceiptRepo();

        public string ES_ID = "na";
        public ActionResult Report_EmployerBen_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_EmployerBPartial",
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
                    string pa = Server.MapPath("~/Penfad_Reports/EMPLOYER_BENEFIT_SUMMARY.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_EMP_ID"] = GlobalValue.Report_Param_1; // GlobalValue.Rep_Cust_No;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else if (GlobalValue.Report_Index_Id == "employer_report2")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Emp_Client_List.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_EMP_ID"] = GlobalValue.Report_Param_1;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
                else if (GlobalValue.Report_Index_Id == "employer_report3")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/EMPLOYER_BENEFIT_SUMMARY.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_EMP_ID"] = GlobalValue.Report_Param_1;
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
   
            //int y = 0;
            for (int i = 1; i <= 2; i++)
            {
                Node treeNode = new Node();

                if (i == 1)
                {
                    treeNode.Text = "Benefit Summary";
                    treeNode.NodeID = node + i;

                }
                else if (i == 2)
                {
                    treeNode.Text = "Membership List";
                    treeNode.NodeID = node + i;

                 }
                    //else if (i == 3)
                    //{
                    //    treeNode.Text = "Employer payment Statement";
                    //    treeNode.NodeID = node + i;
                    //}
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
            return PartialView("Report_EmployerBenPartial");
        }

        public ActionResult Load_Employer_Report_IFrame(string containerId, string es_id, string tree_node_id)
        {
            if (string.IsNullOrEmpty(es_id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select an employer account.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }
            //GlobalValue.Rep_Cust_No = cust_no;
            GlobalValue.Report_Param_1 = es_id;

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
        public ActionResult Read2(string Employer_Id)
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

                X.GetCmp<Store>("Report_EmployerBenPartial_employeeStore").Reload();
                Store store = X.GetCmp<Store>("Report_EmployerBenPartial_employeeStore");
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
                return this.Store(ReceiptRepo.GetReceiptESList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

    }// end class
}