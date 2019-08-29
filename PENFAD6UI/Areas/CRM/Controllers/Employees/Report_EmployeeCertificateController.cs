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
    public class Report_EmployeeCertificateController : Controller
    {
        // GET: CRM/Report_EmployeeStatement
        cLogger logger = new cLogger();
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();

        public ActionResult Report_EmployeeCertificate_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_EmployeeCertPartial",
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
                    string pa = Server.MapPath("~/Penfad_Reports/Employee_Certificate.dll");

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
                    treeNode.Text = "Employee Certificate";
                    treeNode.NodeID = node + i;
                    //treeNode.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                    //treeNode.
                }
               
               
                treeNode.Leaf = true;
                nodes.Add(treeNode);
            }
            

            return this.Store(nodes);
        }

 
        public ActionResult Load_Employee_Report(string containerId)
        {
            return PartialView("Report_EmployeeCertificatePartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string ESF_Id, string tree_node_id)
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
            GlobalValue.Report_Param_1 = ESF_Id;
             
            GlobalValue.Report_Index_Id = tree_node_id; 

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
        public ActionResult FilterEmpolyeeSFGrid(String Scheme_Fund_Id, string Employer_Id)
        {
            try
            {
                return this.Store(employeeRepo.GetEmployeeSFList(Scheme_Fund_Id, Employer_Id));
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        

    }// end class

}