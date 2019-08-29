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
    public class Report_SchemeMembershipAgeController : Controller
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
                    ViewName = "Report_SchemeAgePartial",
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
                    string pa = Server.MapPath("~/Penfad_Reports/Employee_List_per_Scheme_Age.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_SCHEME"] = GlobalValue.Report_Param_1;
					My_Report[":P_DATE"] = GlobalValue.Report_Param_2;
                    My_Report[":FROM_AGE"] = GlobalValue.Report_Param_Age1_date;
                    My_Report[":TO_AGE"] = GlobalValue.Report_Param_Age2_date;

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
            catch (Exception ex)
            {
                throw ex;
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
            return PartialView("Report_SchemeMembershipAgePartial");
        }

        public ActionResult Load_Employee_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id, DateTime? TB_Date, Int32 From_Age, Int32 To_Age)
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

            if (Microsoft.VisualBasic.Information.IsNumeric(From_Age) == false)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Age must be numbers.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            if (From_Age < 1 )
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please Enter a Valid Age.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            if (Microsoft.VisualBasic.Information.IsNumeric(To_Age) == false)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Age must be numbers.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            if (To_Age < From_Age)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry, Please From Age can not be more than To Age.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                return this.Direct();
            }

            DateTime d_date = Convert.ToDateTime(TB_Date);


            GlobalValue.Report_Param_1 = Scheme_Id;
			GlobalValue.Report_Param_2 = TB_Date;
			GlobalValue.Report_Index_Id = tree_node_id;


            DateTime d1 = d_date.AddYears(-(From_Age));
            DateTime d2 = d_date.AddYears(-(To_Age));

            GlobalValue.Report_Param_Age1_date = d1;
            GlobalValue.Report_Param_Age2_date = d2;

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