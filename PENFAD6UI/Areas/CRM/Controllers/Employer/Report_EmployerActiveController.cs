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
using PENFAD6DAL.DbContext;
using System.Data;
using Dapper;

namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    public class Report_EmployerActiveController : Controller
    {
        // GET: CRM/Report_Employer List

        cLogger logger = new cLogger();

       
        public ActionResult GetReportSnapshot()
        {
            try
            {
                if (GlobalValue.Report_Index_Id == "employer_List")
                {
                    string pa = Server.MapPath("~/Penfad_Reports/Employer_List.dll");

                    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                    //////asign constring
                    My_Report.Dictionary.DataStore.Clear();
                    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("con", GlobalValue.ConString));

                    //My_Report[":P_ES_ID"] = GlobalValue.Report_Param_1; // GlobalValue.Rep_Cust_No;
                    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                   
                }
                //else if (GlobalValue.Report_Index_Id == "employee_List")
                //{
                //    string pa = Server.MapPath("~/Penfad_Reports/Employee_List.dll");

                //    System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.LoadFrom(pa);
                //    StiReport My_Report = StiReport.GetReportFromAssembly(assembly_1);

                //    //////asign constring
                //    My_Report.Dictionary.DataStore.Clear();
                //    //My_Report.Dictionary.Databases.Add(new StiSqlDatabase("TekSolPenfad", GlobalValue.ConString));
                //    My_Report.Dictionary.Databases.Add(new StiOracleDatabase("TekSolPenfad", GlobalValue.ConString));

                //    //My_Report[":P_ES_ID"] = GlobalValue.Report_Param_1; // GlobalValue.Rep_Cust_No;
                //    //My_Report.Dictionary.DataSources["con"].Parameters["P_CUSTNO"].Value = GlobalValue.Rep_Cust_No;

                //    return StiMvcViewer.GetReportSnapshotResult(My_Report);
                //}
                X.Mask.Hide();
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


        public ActionResult Load_Report(string containerId = "z_mainform_center_panel_tab_main_1")
        {
            X.Mask.Hide();
            return PartialView("Report_ActivePartial");
        }

        public ActionResult Load_Employer_Report_IFrame( string containerId = "z_mainform_center_panel_tab_main_1")
        {

            GlobalValue.Report_Index_Id = "employer_List";

            //Get employee strength
            var con = new AppSettings();
            var param = new DynamicParameters();
            con.GetConnection().Execute("ES_EMP_STRENGTH", param, commandType: CommandType.StoredProcedure);

            this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
            {
                Url = Url.Action("Load_Report"),
                DisableCaching = true,
                Mode = LoadMode.Frame
            });
            X.Mask.Hide();
            return this.Direct();

        }

        //public ActionResult Load_Employee_Report_IFrame(string containerId = "z_mainform_center_panel_tab_main_1")
        //{

        //    GlobalValue.Report_Index_Id = "employee_List";

        //    this.GetCmp<Panel>(containerId).LoadContent(new ComponentLoader
        //    {
        //        Url = Url.Action("Load_Report"),
        //        DisableCaching = true,
        //        Mode = LoadMode.Frame
        //    });
        //    return this.Direct();

        //}


    }// end class
}