using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace PENFAD6DAL.Repository.CRM.Employee
{
    public class crm_EmployeeSchemeFundRepo
    {   
        public string ESF_Id { get; set; }
        [Required(ErrorMessage = "Please select Fund")]
        public string Scheme_Fund_Id { get; set; }
        [Required(ErrorMessage = "Please select Employer")]
        public string Scheme_Id { get; set; }
        public string New_Scheme_Fund_Id { get; set; }
        public string New_Scheme_Id { get; set; }
        public string New_Fund_Id { get; set; }
        public string New_Scheme_Name { get; set; }
        public string New_Fund_Name { get; set; }
        public string Scheme_Name { get; set; }
        public string Fund_Id { get; set; }
        public string Fund_Name { get; set; }
        public string Employee_Name { get; set; }
		public string Employer_Name { get; set; }
		public string New_Employer_Id { get; set; }
        public string New_Employer_Name { get; set; }
        public decimal? Employer_Con_Balance { get; set; }
        public decimal? Employee_Con_Balance { get; set; }
        public decimal? Employer_Unit_Balance { get; set; }
        public decimal? Employee_Unit_Balance { get; set; }

        public decimal? Total_Unit_Balance { get; set; }
        public decimal? Total_Con_Balance { get; set; }
        public string Cust_No { get; set; }
      
        public string Title { get; set; }
        public string Surname { get; set; }
        [Required(ErrorMessage = "First Name is requires")]
        public string First_Name { get; set; }     
        public string Other_Name { get; set; }      
        public DateTime? Date_Of_Birth { get; set; }      
        public string Auth_Status { get; set; }
        public string ESF_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Maker_Id { get; set; }
        public string Employer_Id { get; set; }
        public string Employee_Id { get; set; }
        public decimal? Total_CH { get; set; }

		public decimal? ESF_BAL { get; set; }

        public string Email_Address { get; set; }
        public string Mobile_Number { get; set; }

       public string Employee_Type { get; set; }

        IDbConnection con;
        public void SaveRecord(crm_EmployeeSchemeFundRepo ESFRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_ESF_ID", value: ESFRepo.Cust_No + ESFRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: ESFRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_UNIT_BALANCE", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_UNIT_BALANCE", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CUST_NO", value: ESFRepo.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_REMIT_EMPLOYEE_SCHEME_FUND", param, commandType: CommandType.StoredProcedure);
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


        // Approve Employee Scheme_Fund
        public void ApproveESFRecord(string ESF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ESF_ID", value: ESF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ESF_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("APP_REMIT_ESF", param, commandType: CommandType.StoredProcedure);
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

        // Close Employee Scheme_Fund
        public void CloseESFRecord(string ESF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ESF_ID", value: ESF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ESF_STATUS", value: "CLOSED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
				param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
				con.Execute("UPD_REMIT_ESF", param, commandType: CommandType.StoredProcedure);
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

        // Close Employee Scheme_Fund
        public void ActivateESFRecord(string ESF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ESF_ID", value: ESF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ESF_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("UPD_REMIT_ESF_ACT", param, commandType: CommandType.StoredProcedure);
               
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

        public bool DeleteRecord(string ESF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_ESF_ID", value: ESF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_REMIT_EMPLOYEE_SCHEME_FUND", param: param, commandType: CommandType.StoredProcedure);

                if (result > 0)
                    return true;
                else
                    return false;
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


        public bool isESUnique(string EMSF_ID)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_EMSF_ID", EMSF_ID, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_CRM_EMSF_EXIST_CHANGE", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");

                if (paramoption == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // change fund
        public bool ChangeRecord(crm_EmployeeSchemeFundRepo ESFRepo)
        {

            var app = new AppSettings();


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("p_new_esf_id", ESFRepo.Cust_No + ESFRepo.New_Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                    param.Add("p_old_esf_id", ESFRepo.Cust_No + ESFRepo.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
					param.Add("p_ESF_Bal", ESFRepo.Total_Unit_Balance, DbType.Decimal, ParameterDirection.Input);
					param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
					conn.Execute(sql: "CHANGE_EMP_SF", param: param, commandType: CommandType.StoredProcedure);
                    ts.Complete();

                    return true;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    ts.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }

        // change employer
        public bool ChangeRecordemployer(crm_EmployeeSchemeFundRepo ESFRepo)
        {

            var app = new AppSettings();


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    DynamicParameters param = new DynamicParameters();
					param.Add("p_esf_id", ESFRepo.Employer_Id, DbType.String, ParameterDirection.Input);
					param.Add("p_new_esf_id", ESFRepo.New_Employer_Id, DbType.String, ParameterDirection.Input);
                    param.Add("p_cust_no", ESFRepo.Cust_No, DbType.String, ParameterDirection.Input);
					param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
					conn.Execute(sql: "CHG_CRM_EMPLOYER", param: param, commandType: CommandType.StoredProcedure);
					
					ts.Complete();

                    return true;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    ts.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        public DataSet ESFData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_EMPLOYEE_SCHEME_FUND";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ESF");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployeeSchemeFundRepo> GetESFList()
        {
            try
            {
                DataSet dt = ESFData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployeeSchemeFundRepo
                {
                    Employee_Id = row.Field<string>("EMPLOYEE_ID"),
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employee_Unit_Balance = row.Field<Decimal?>("EMPLOYEE_UNIT_BALANCE"),
                    Employer_Unit_Balance = row.Field<Decimal?>("EMPLOYER_UNIT_BALANCE"),
                    Employee_Con_Balance = row.Field<Decimal?>("EMPLOYEE_CON_BALANCE"),
                    Employer_Con_Balance = row.Field<Decimal?>("EMPLOYER_CON_BALANCE"),
                    Total_Con_Balance = row.Field<Decimal?>("TOTAL_CON_BALANCE"),
                    Total_Unit_Balance = row.Field<Decimal?>("TOTAL_UNIT_BALANCE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Title = row.Field<String>("TITLE"),
                    Surname = row.Field<String>("SURNAME"),
                    First_Name = row.Field<String>("FIRST_NAME"),
                    Other_Name = row.Field<String>("OTHER_NAME"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    ESF_Status = row.Field<string>("ESF_STATUS"),
                    Date_Of_Birth = row.Field<DateTime?>("DATE_OF_BIRTH"),
                    Employee_Name = row.Field<String>("TITLE") + " " + row.Field<String>("SURNAME") + " " + row.Field<String>("FIRST_NAME") + " " + row.Field<String>("OTHER_NAME"),
                }).ToList();

                return eList;

            }
            catch (Exception)
            {
                throw;
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

        // FOR PENDING EMPLOYEE SCHEME FUND
        public DataSet ESFPendingData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_ESF_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "esf");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployeeSchemeFundRepo> GetESFPendingList()
        {
            try
            {
                DataSet dt = ESFPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployeeSchemeFundRepo
                {
                    Employee_Id = row.Field<string>("EMPLOYEE_ID"),
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employee_Unit_Balance = row.Field<Decimal?>("EMPLOYEE_UNIT_BALANCE"),
                    Employer_Unit_Balance = row.Field<Decimal?>("EMPLOYEE_UNIT_BALANCE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Surname = row.Field<String>("SURNAME"),
                    First_Name = row.Field<String>("FIRST_NAME"),
                    Other_Name = row.Field<String>("OTHER_NAME"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    ESF_Status = row.Field<string>("ESF_STATUS"),
                    Date_Of_Birth = row.Field<DateTime>("DATE_OF_BIRTH")
                }).ToList();

                return eList;

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

        // FOR UNCLOSED EMPLOYEE SCHEME FUND
        public DataSet ESFNotClosedData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_ESF_NOTCLOSED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "esf");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployeeSchemeFundRepo> GetESFNotClosedList()
        {
            try
            {
                DataSet dt = ESFNotClosedData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployeeSchemeFundRepo
                {
                    Employee_Id = row.Field<string>("EMPLOYEE_ID"),
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employee_Unit_Balance = row.Field<Decimal?>("EMPLOYEE_UNIT_BALANCE"),
                    Employer_Unit_Balance = row.Field<Decimal?>("EMPLOYEE_UNIT_BALANCE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Surname = row.Field<String>("SURNAME"),
                    First_Name = row.Field<String>("FIRST_NAME"),
                    Other_Name = row.Field<String>("OTHER_NAME"),
					Employer_Name = row.Field<String>("Employer_Name"),
					Auth_Status = row.Field<string>("AUTH_STATUS"),
                    ESF_Status = row.Field<string>("ESF_STATUS"),
                    Date_Of_Birth = row.Field<DateTime?>("DATE_OF_BIRTH")
                }).ToList();

                return eList;

            }
            catch (Exception)
            {
                throw;
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

        // FOR CLOSED EMPLOYEE SCHEME FUND
        public DataSet ESFClosedData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_ESF_CLOSED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "esf");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployeeSchemeFundRepo> GetESFClosedList()
        {
            try
            {
                DataSet dt = ESFClosedData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployeeSchemeFundRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employee_Unit_Balance = row.Field<Decimal>("EMPLOYEE_UNIT_BALANCE"),
                    Employer_Unit_Balance = row.Field<Decimal>("EMPLOYEE_UNIT_BALANCE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Surname = row.Field<String>("SURNAME"),
                    First_Name = row.Field<String>("FIRST_NAME"),
                    Other_Name = row.Field<String>("OTHER_NAME"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    ESF_Status = row.Field<string>("ESF_STATUS"),
                    Date_Of_Birth = row.Field<DateTime>("DATE_OF_BIRTH")
                }).ToList();

                return eList;

            }
            catch (Exception)
            {
                throw;
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
        
        //FILTERING LIST OF SCHEME-FUND FOR EMPLOYER
        public List<crm_EmployeeSchemeFundRepo> GetESCHEMEList(string Employer_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeSchemeFundRepo> bn = new List<crm_EmployeeSchemeFundRepo>();

                string query = "Select * from VW_ESF_SCHEME_FUND WHERE Employer_Id = '" + Employer_Id + "' ";
                return bn = con.Query<crm_EmployeeSchemeFundRepo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Dispose();
            }

        }

        //FILTERING LIST OF SCHEME-FUND FOR EMPLOYER
        public List<crm_EmployeeSchemeFundRepo> GetESCHEMEList3(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeSchemeFundRepo> bn = new List<crm_EmployeeSchemeFundRepo>();

                string query = "Select * from VW_ESF_SCHEME_FUND2 WHERE New_Scheme_Id = '" + Scheme_Id + "' ";
                return bn = con.Query<crm_EmployeeSchemeFundRepo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Dispose();
            }

        }

        //FILTERING LIST OF  EMPLOYER
        public List<crm_EmployeeSchemeFundRepo> GetESCHEMEList6(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeSchemeFundRepo> bn = new List<crm_EmployeeSchemeFundRepo>();

                string query = "Select * from VW_EMPLOYER_FOR_REP WHERE  Scheme_Id = '" + Scheme_Id + "' AND EMPLOYER_STATUS = 'ACTIVE' ";
                return bn = con.Query<crm_EmployeeSchemeFundRepo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Dispose();
            }

        }


        //get scheme funds
        public List<crm_EmployeeSchemeFundRepo> GetESCHEMEList2()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeSchemeFundRepo> bn = new List<crm_EmployeeSchemeFundRepo>();

                string query = "Select * from VW_SF_LIST";
                return bn = con.Query<crm_EmployeeSchemeFundRepo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Dispose();
            }

        }

		//check cash available
	

		public bool Get_GL_Balance(crm_EmployeeSchemeFundRepo ESFRepo)
		{
			try
			{
				//Get connection
				var con = new AppSettings();
				var param = new DynamicParameters();
				param.Add("P_SFID", ESFRepo.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
				param.Add("P_ESFID", ESFRepo.Cust_No + ESFRepo.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
				param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
				param.Add("GB", null, DbType.Decimal, ParameterDirection.Output);
				con.GetConnection().Execute("SEL_CH_CASH_FOR_SF", param, commandType: CommandType.StoredProcedure);
				decimal ESBENEFIT = param.Get<decimal>("VDATA");
				decimal CASH_BALANCE = param.Get<decimal>("GB") * -1;
				ESFRepo.ESF_BAL = ESBENEFIT;
				if (ESBENEFIT < CASH_BALANCE)
					return false;
				else
					return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}


		//check if collection account is available

		public bool Get_GL_coll(crm_EmployeeSchemeFundRepo ESFRepo)
		{
			try
			{
				//Get connection
				var con = new AppSettings();
				var param = new DynamicParameters();
				param.Add("P_SFID",  ESFRepo.New_Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
				param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
				con.GetConnection().Execute("SEL_CH_COLL_ACC", param, commandType: CommandType.StoredProcedure);
				decimal ESBENEFIT = param.Get<decimal>("VDATA");
				if (ESBENEFIT > 0)
					return false;
				else
					return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

	}
}
