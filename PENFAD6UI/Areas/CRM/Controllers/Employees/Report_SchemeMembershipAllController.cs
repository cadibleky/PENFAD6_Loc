using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Serilog;

using PENFAD6DAL.Repository.Remittance.Contribution;
using PENFAD6DAL.GlobalObject;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using PENFAD6DAL.Repository.Setup.PfmSetup;

namespace PENFAD6UI.Areas.CRM.Controllers.Employees
{
    public class Report_SchemeMembershipAllController : Controller
    {
        // GET:
        cLogger logger = new cLogger();
        readonly pfm_SchemeRepo schemeRepo = new pfm_SchemeRepo();
		readonly Remit_Contribution_Upload_LogRepo  emp_log_repo = new Remit_Contribution_Upload_LogRepo();
		public ActionResult Report_SchemeMembership_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_SchemeAllPartial",
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
                    string pa = Server.MapPath("~/Penfad_Reports/Employee_List_per_Scheme_All.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_SCHEME_ID"] = GlobalValue.Report_Param_1;
					My_Report[":P_DATE"] = GlobalValue.Report_Param_2;

					return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
       
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
                }
              
               
                treeNode.Leaf = true;
                nodes.Add(treeNode);
            }
            

            return this.Store(nodes);
        }

 
        public ActionResult Load_Employee_Report(string containerId)
        {
            return PartialView("Report_SchemeMembershipAllPartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id, String con_month, DateTime? TB_Date)
        {
            if (string.IsNullOrEmpty(Scheme_Id))
            {
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

			if (!TB_Date.HasValue)
			{
				X.Msg.Show(new MessageBoxConfig
				{
					Title = "Error",
					Message = "Please Select Date.",
					Buttons = MessageBox.Button.OK,
					Icon = MessageBox.Icon.INFO,
					Width = 350

				});
				return this.Direct();
			}

			GlobalValue.Report_Param_1 = Scheme_Id;
			GlobalValue.Report_Param_2_string = con_month;
			GlobalValue.Report_Param_2 = TB_Date;
			GlobalValue.Report_Index_Id = tree_node_id; 

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

                throw;
            }
        }


		// filter con list for scheme
		public ActionResult GetGLAB(string Scheme_Id)
		{
			var misdepartmentrepo = new Remit_Contribution_Upload_LogRepo();
			var mydata = misdepartmentrepo.GetGLASFList(Scheme_Id);

			List<object> data = new List<object>();
			foreach (var ddd in mydata)
			{
				string id = Convert.ToString(ddd.For_Month) + Convert.ToString(ddd.For_Year); 
				string name = Convert.ToString(ddd.monthname) + ", " + Convert.ToString(ddd.For_Year);

				data.Add(new { conlist_GLB_gId = id, conName = name });
			}

			return this.Store(data);
		}



	}// end class

}