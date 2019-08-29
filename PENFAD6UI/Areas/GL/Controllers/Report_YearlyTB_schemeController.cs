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
using PENFAD6DAL.Repository.GL;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;
using PENFAD6DAL.DbContext;
using Dapper;
using System.Data;

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class Report_YearlyTB_schemeController : Controller
    {
        // GET: report
        cLogger logger = new cLogger();
        readonly  GLChartRepo GLRepo = new GLChartRepo();
		
		readonly GLTBtRepo TBREPO = new GLTBtRepo();
		readonly crm_EmployeeRepo employeeRepo = new crm_EmployeeRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly Invest_Trans_Fix_Repo MM_TBill = new Invest_Trans_Fix_Repo();

        public ActionResult Report_TB_Tab(string containerId = "MainArea")
        {

            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Report_TBPartial",
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

					////load report table
					//var con = new AppSettings();
					//var param = new DynamicParameters();
					//param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
					//param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
					//param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
					//con.GetConnection().Execute("REPORT_INSERT_CINAAFB_S", param, commandType: CommandType.StoredProcedure);					

					//load report
					string pa = Server.MapPath("~/Penfad_Reports/CINAAFB_S.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
					My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                }
				else if (GlobalValue.Report_Index_Id == "employer_report2")
				{
					////load report table
					//var con = new AppSettings();
					//var param = new DynamicParameters();
					//param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
					//param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
					//param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
					//con.GetConnection().Execute("REPORT_INSERT_NAAFB_S", param, commandType: CommandType.StoredProcedure);

					string pa = Server.MapPath("~/Penfad_Reports/NAAFB_S.dll");

					System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
					StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

					//////asign constring
					My_Report.Dictionary.DataStore.Clear();
					//My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
					My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

					My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
					My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
				}
				else if (GlobalValue.Report_Index_Id == "employer_report3")
				{
					////load report table
					//var con = new AppSettings();
					//var param = new DynamicParameters();
					//param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
					//param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
					//param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
					//con.GetConnection().Execute("REPORT_INSERT_TB_MONTHLY_S", param, commandType: CommandType.StoredProcedure);


					string pa = Server.MapPath("~/Penfad_Reports/MTB_S.dll");

					System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
					StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

					//////asign constring
					My_Report.Dictionary.DataStore.Clear();
					//My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
					My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
					My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
					My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
				}
				else if (GlobalValue.Report_Index_Id == "employer_report4")
				{
					////load report table
					//var con = new AppSettings();
					//var param = new DynamicParameters();
					//param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
					//param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
					//param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
					//con.GetConnection().Execute("REPORT_INSERT_TB_S", param, commandType: CommandType.StoredProcedure);

					string pa = Server.MapPath("~/Penfad_Reports/YTB_S.dll");

					System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
					StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

					//////asign constring
					My_Report.Dictionary.DataStore.Clear();
					//My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
					My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
					My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
					My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                    X.Mask.Hide();
                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
				}
                else if (GlobalValue.Report_Index_Id == "employer_report5")
                {
                    ////load report table
                    //var con = new AppSettings();
                    //var param = new DynamicParameters();
                    //param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    //param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    //param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    //con.GetConnection().Execute("REPORT_INSERT_CASHFLOW_S", param, commandType: CommandType.StoredProcedure);

                    string pa = Server.MapPath("~/Penfad_Reports/CASHFLOW_S.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
                    My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
                    My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
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
                    X.Mask.Hide();
                    return StiMvcViewer.ViewerEventResult();
                }
                else
                {
                    X.Mask.Hide();
                    return this.Direct();
                }
            }
            catch (Exception)
            {
                X.Mask.Hide();
                return this.Direct();
            }

        }

		//public ActionResult Load_Employer_Report_IFrame(string containerId, string Scheme_Fund_Id, string tree_node_id,DateTime? TB_Date)
		//{
		//	if (string.IsNullOrEmpty(Scheme_Fund_Id))
		//	{
		//		X.Msg.Show(new MessageBoxConfig
		//		{
		//			Title = "Error",
		//			Message = "Select Scheme Account.",
		//			Buttons = MessageBox.Button.OK,
		//			Icon = MessageBox.Icon.INFO,
		//			Width = 350

		//		});
		//		return this.Direct();
		//	}
		//	//GlobalValue.Rep_Cust_No = cust_no;
		//	GlobalValue.Report_Param_1 = Scheme_Fund_Id;
		//	GlobalValue.Report_Param_2 = TB_Date;
		//	GlobalValue.Report_Index_Id = tree_node_id;

		//	this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
		//	{
		//		Url = Url.Action("Load_Employee_Report"),
		//		DisableCaching = true,
		//		Mode = LoadMode.Frame
		//	});
		//	return this.Direct();

		//}


		public ActionResult Load_Employee_Report(string containerId)
        {
            X.Mask.Hide();
            return PartialView("Report_YearlyTBPartial");
        }

        public ActionResult Load_Employer_Report_IFrame(string containerId, string Scheme_Id, string tree_node_id, GLTBtRepo TBREPO)
        {
            if (string.IsNullOrEmpty(Scheme_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select Scheme Account.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                X.Mask.Hide();
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
                X.Mask.Hide();
                return this.Direct();
			}


			GlobalValue.Report_Param_1 = Scheme_Id;
            GlobalValue.Report_Param_2 = TBREPO.TB_Date;
			GlobalValue.Report_Param_2_string = GlobalValue.User_ID;
			GlobalValue.Report_Index_Id = tree_node_id;

            if (GlobalValue.Report_Index_Id == "employer_report1")
            {

                //load report table
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                con.GetConnection().Execute("REPORT_INSERT_CINAAFB_S", param, commandType: CommandType.StoredProcedure);

                ////load report
                //string pa = Server.MapPath("~/Penfad_Reports/CINAAFB_S.dll");

                //System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                ////////asign constring
                //My_Report.Dictionary.DataStore.Clear();
                ////My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                //My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
                //My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;

                //return StiMvcViewer.GetReportSnapshotResult(My_Report);
                X.Mask.Hide();
            }
            else if (GlobalValue.Report_Index_Id == "employer_report2")
            {
                //load report table
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                con.GetConnection().Execute("REPORT_INSERT_NAAFB_S", param, commandType: CommandType.StoredProcedure);

                //string pa = Server.MapPath("~/Penfad_Reports/NAAFB_S.dll");

                //System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                ////////asign constring
                //My_Report.Dictionary.DataStore.Clear();
                ////My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                //My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
                //My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                //return StiMvcViewer.GetReportSnapshotResult(My_Report);
                X.Mask.Hide();
            }
            else if (GlobalValue.Report_Index_Id == "employer_report3")
            {
                //load report table
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                con.GetConnection().Execute("REPORT_INSERT_TB_MONTHLY_S", param, commandType: CommandType.StoredProcedure);


                //string pa = Server.MapPath("~/Penfad_Reports/MTB_S.dll");

                //System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                ////////asign constring
                //My_Report.Dictionary.DataStore.Clear();
                ////My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
                //My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
                //My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                //return StiMvcViewer.GetReportSnapshotResult(My_Report);
                X.Mask.Hide();
            }
            else if (GlobalValue.Report_Index_Id == "employer_report4")
            {
                //load report table
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                con.GetConnection().Execute("REPORT_INSERT_TB_S", param, commandType: CommandType.StoredProcedure);

                //string pa = Server.MapPath("~/Penfad_Reports/YTB_S.dll");

                //System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                ////////asign constring
                //My_Report.Dictionary.DataStore.Clear();
                ////My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
                //My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
                //My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                //return StiMvcViewer.GetReportSnapshotResult(My_Report);
                X.Mask.Hide();
            }
            else if (GlobalValue.Report_Index_Id == "employer_report5")
            {
                //load report table
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                con.GetConnection().Execute("REPORT_INSERT_CASHFLOW_S", param, commandType: CommandType.StoredProcedure);

                //string pa = Server.MapPath("~/Penfad_Reports/CASHFLOW_S.dll");

                //System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                ////////asign constring
                //My_Report.Dictionary.DataStore.Clear();
                ////My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));
                //My_Report[":P_SFID"] = GlobalValue.Report_Param_1;
                //My_Report[":P_ID"] = GlobalValue.Report_Param_2_string;
                //return StiMvcViewer.GetReportSnapshotResult(My_Report);
                X.Mask.Hide();
            }
            else
            {
                X.Mask.Hide();
                return this.Direct();
            }


            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Employee_Report"),
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
                X.Mask.Hide();
                Store store = X.GetCmp<Store>("TBAccountSFStore_scheme");
				store.RemoveAll();
                return this.Store(SFRepo.GetSchemeFundListscheme());

            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }

		public StoreResult GetChildren()
		{
			NodeCollection nodes = new Ext.Net.NodeCollection();
			// int i = 1;
			string node = "employer_report";

			//int y = 0;
			for (int i = 1; i <= 5; i++)
			{
				Node treeNode = new Node();

				if (i == 1)
				{
					treeNode.Text = "Change In Net Asset Available For Benefits";
					treeNode.NodeID = node + i;

				}
				else if (i == 2)
				{
					treeNode.Text = "Net Asset Available For Benefit";
					treeNode.NodeID = node + i;

				}
				else if (i == 3)
				{
					treeNode.Text = "Monthly Trial Balance";
					treeNode.NodeID = node + i;
				}
				else if (i == 4)
				{
					treeNode.Text = "Year To Date Trial Balance";
					treeNode.NodeID = node + i;
				}
				else if (i == 5)
				{
					treeNode.Text = "Cash Flow";
					treeNode.NodeID = node + i;
				}

				treeNode.Leaf = true;
				nodes.Add(treeNode);
			}


			return this.Store(nodes);
		}

	}// end class

}