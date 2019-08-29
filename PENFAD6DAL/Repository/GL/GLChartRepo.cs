using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;
using System.Collections;

namespace PENFAD6DAL.Repository.GL
{
    public class GLChartRepo
    {       
        public string Con_Code { get; set; }
        public string Con_Name { get; set; }
        public string Memo_Code { get; set; }
        public string Memo_Name { get; set; }
        public string GL_Account_No { get; set; }
        public string Sheme_Fund_Id { get; set; }
        public string Scheme_Id { get; set; }
        public string Fund_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Fund_Name { get; set; }
        public string GL_Balance { get; set; }
        public string Rec_Status { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Auth_Id { get; set; }
        public DateTime Auth_Date { get; set; }
        public string Year_TB { get; set; }
		public DateTime? TB_Date { get; set; }


		// methods begin

		IDbConnection con;

        public void Add_GL_Control(string Con_Code, string Con_Name)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "P_CON_CODE", value: Con_Code, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_NAME", value: Con_Name, dbType: DbType.String, direction: ParameterDirection.Input);
               
                int result = con.Execute(sql: "ADD_GL_CONTROL", param: param, commandType: CommandType.StoredProcedure);
         
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }

        private void Add_GL_Class(string Memo_Code, string Memo_Name, string Con_Code)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "P_MEMO_CODE", value: Memo_Code, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MEMO_NAME", value: Memo_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_CODE", value: Con_Code, dbType: DbType.String, direction: ParameterDirection.Input);
              
                int result = con.Execute(sql: "ADD_GL_CLASS", param: param, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }

        public bool Add_GLChart(GLChartRepo GLRepo)
        {
            try 
            {
                //add to GL_Control
                Add_GL_Control( "1" , "ASSETS");
                Add_GL_Control( "2" , "LIABILITIES");
                Add_GL_Control( "3" , "DEALING WITH MEMBERS");
                Add_GL_Control( "4" , "EXPENSES");
                //Add_GL_Control( "5", "REPRESENTED BY");

                //add to GL_Class
                Add_GL_Class("101", "Bank Accounts", "1");
                Add_GL_Class("102", "Money Market", "1");
                Add_GL_Class("103", "T-Bills", "1");
                Add_GL_Class("104", "Bonds", "1");
               // Add_GL_Class("105", "Corporate Bonds", "1");
                Add_GL_Class("106", "Equities", "1");
                Add_GL_Class("107", "CIS", "1");
             

                Add_GL_Class("120", "Interest Receivables-Money Market", "1");
                Add_GL_Class("121", "Interest Receivables-T-Bills", "1");
                Add_GL_Class("122", "Interest Receivables-Bonds", "1");
                //Add_GL_Class("123", "Interest Receivables-Corporate Bonds", "1");
                Add_GL_Class("124", "Contribution Receivables", "1");

                Add_GL_Class("201", "Benefit Payables", "2");
                Add_GL_Class("202", "Admin Expense Payables", "2");
                Add_GL_Class("203", "Employers Control", "2");
                Add_GL_Class("204", "Tax Payables", "2");
               
                Add_GL_Class("301", "Contributions", "3");
                Add_GL_Class("302", "Investment Income", "3");
                Add_GL_Class("303", "GAIN/LOSSES ON VALUATION", "3");

                Add_GL_Class("401", "Benefits Paid", "4");
                Add_GL_Class("402", "Fees", "4");
                Add_GL_Class("403", "Other Expenses", "4");
                Add_GL_Class("404", "Tax Paid", "4");
                Add_GL_Class("405", "Brokerage Fees/ Levies/ Commission", "4");


                Add_GL_Class("500", "Nets Assets Availabe For Benefits", "1");
                Add_GL_Class("501", "Fair Value Reserve (Available For Sale)", "3");

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public List<GLChartRepo> GetGLList()
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<GLChartRepo> ObjFund = new List<GLChartRepo>();

                return ObjFund = db.GetConnection().Query<GLChartRepo>("Select * from VW_GL_CHART").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }

    } //end class gl repo

}
