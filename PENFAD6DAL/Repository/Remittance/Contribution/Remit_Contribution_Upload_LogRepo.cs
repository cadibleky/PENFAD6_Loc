using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    public class Remit_Contribution_Upload_LogRepo
    {
        readonly crm_EmployerSchemeRepo ESRepo = new crm_EmployerSchemeRepo();
        //Contribution Log
        public string Con_Log_Id { get; set; }
        [Required(ErrorMessage = "Select  Employer ID is a required data item.")]
        [Display(Name = "Employer Name")]
        public string Employer_Id { get; set; }
        //public string Empoyer_Id { get; set; }
        [Required(ErrorMessage = "Select  Scheme Name is a required data item.")]
        [Display(Name = "Scheme Name")]
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set;}
        public string Fund_Name { get; set; }
        public string ES_Id { get; set; }

        [Required(ErrorMessage = "Select  Month Name is a required data item.")]
        [Display(Name = "Month Name")]
        public int For_Month { get; set; }
        [Required(ErrorMessage = "Select  Year is a required data item.")]
        [Display(Name = "Year")]
        public int For_Year { get; set; }
        public string Surname { get; set;}
        public string first_Name { get; set; }
     
        public DateTime DeadLine_Date { get; set; }
        public string Receipt_Confirm_YesNo { get; set; }
         public string Unit_Purchased_YesNo { get; set; }
        public double Total_Contribution { get; set;}
        public double Total_Receiption { get; set;}
        public double Total_Arrears { get; set;}
        public double Surcharge_Balance { get; set;}
        public DateTime Next_Surcharge_Date { get; set;}
         //public string Maker_Id { get; set; }
        public string Auth_status { get; set; }
        public string Log_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        //   Contribution
        public string Employee_Id { get; set;}
        public string Cust_No { get; set; }
        public string Esf_Id { get; set; }
        public string SchemeFund_Id { get; set; }
        public string Scheme_Fund_Id { get; set;}
       // [Range(typeof(decimal), "0.00", "49.99")]
        public decimal Employer_Con { get; set;}
        public decimal Employee_Con { get; set; }
        public decimal Employer_Amt_Used { get; set;}
        public decimal Employee_Amt_Used { get; set; }
        public decimal Employer_Bal { get; set;}
        public decimal Employee_Bal { get; set;}
        public decimal Employer_Amt_Touse_Temp { get; set;}
       public decimal Employee_Amt_Touse_Temp { get; set; }
        public decimal Employee_Salary { get; set; }
        public decimal Employee_Sal_Rate { get; set; }
        public decimal Req_Con  { get; set; }
         public decimal Difference { get; set; }
        public string  Req_Status { get; set; }
        public int Grace_Period { get; set; }
         public string Con_Type { get; set; }
        public string Employee_Name { get; set; }
        public string Employer_Name { get; set; }
        public string Business_Address { get; set; }
        public string Postal_Address { get; set; }
        public string Phone_Number { get; set; }

        public string Employee_Scheme_Fund_Id { get; set; }
        public double Employee_Con_Balance { get; set; }
        public double Employer_Con_Balance { get; set; }
        public decimal Unit_Price { get; set; }
        public double Quantity { get; set; }
        public decimal Employee_Unit_Price_Bal { get; set; }
        
             public decimal Total_Contribute { get; set; }
        public decimal Employer_Unit_Price_Bal { get; set; }
        public string Narration { get; set; }
        public string Trans_Type { get; set; }
        public DateTime Trans_Date { get; set; }
        public string Trans_Status { get; set; }
        public string Esmf_Id { get; set;}
        public string Contact_Person { get; set; }
        public string Contact_Email { get; set; }
        public string For_MonthN { get; set; }
        public string For_YearN { get; set; }
        public string SEND_SMS { get; set; }
		public string con_month { get; set; }

		public string monthname { get; set; }

		IDbConnection con;

        public DataSet EmployerData()
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

                cmd.CommandText = "SEL_CRM_EMPLOYER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "emp");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
      
        public IEnumerable<Remit_Contribution_Upload_LogRepo> GetEmployerList()
        {
            try
            {
                DataSet dt = EmployerData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_Contribution_Upload_LogRepo
                {
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                   Employer_Name = row.Field<string>("Employer_Name"),
                    Business_Address = row.Field<string>("BUSINESS_ADDRESS"),
                    Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                 


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
        //Contribution Approved;

        public DataSet ContributionData()
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

                cmd.CommandText = "SEL_REMIT_CON_PENDINGAPPROVAL";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "con");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public List<Remit_Contribution_Upload_LogRepo> GetEmployeeCon_BatchList_ByStatus(string batchLogno)
        {
            var batch_list = new List<Remit_Contribution_Upload_LogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //if (string.IsNullOrEmpty(cust_status))
                    //    cust_status = "0";
                    if (string.IsNullOrEmpty(batchLogno))
                        batchLogno = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_Remit_EmployeesByConLogId";
                        cmd.Parameters.Add("p_Con_Log_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = batchLogno;
                        //cmd.Parameters.Add("p_cust_status", OracleDbType.Varchar2, ParameterDirection.Input).Value = cust_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new Remit_Contribution_Upload_LogRepo
                            {
                                Surname  = row.Field<string>("SURNAME"),
                                first_Name = row.Field<string>("FIRST_NAME"),
                                Employee_Id = row.Field<string>("EMPLOYEE_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Fund_Name = row.Field<string>("FUND_NAME"),
                                Employee_Con = Convert.ToDecimal(row.Field<decimal>("EMPLOYEE_CON")),
                                Employer_Con = Convert.ToDecimal(row.Field<decimal >("EMPLOYER_CON")),
                               
                                Employee_Salary = Convert.ToDecimal(row.Field<decimal>("EMPLOYEE_SALARY")),
                               /// Employee_Sal_Rate = Convert.ToDecimal(row.Field<decimal>("EMPLOYEE_SAL_RATE"))
                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }
       

        //employer Scheme;
        public DataSet EmployerSchemeData()
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

                cmd.CommandText = "SEL_REMIT_EMPLOYERBYSCHEME";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;
                param = cmd.Parameters.Add("p_Employer_Id", OracleDbType.Varchar2);
                param = cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2);
                param = cmd.Parameters.Add("p_ESF_ID", OracleDbType.Varchar2);
                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "emp");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    

public IEnumerable GetRemit_EmployerScheme_Dataset(string emplid, string Schemeid)
{
    //EmployerSchemeData();
    var Escheme_list = new List<crm_EmployerSchemeRepo>();
    //List<object> data = new List<object>();
    var app = new AppSettings();
    using (OracleConnection conp = new OracleConnection(app.conString()))
    {
        try
        {
            //Get connection
            conp.Open();
            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.Connection = conp;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SEL_REMIT_EMPLOYERBYSCHEME";
                cmd.Parameters.Add("p_Employer_Id", OracleDbType.Varchar2, ParameterDirection.Output).Value = emplid;
                cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2, ParameterDirection.Output).Value = Schemeid;
                cmd.Parameters.Add("cur", OracleDbType.RefCursor, ParameterDirection.Output);

                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);



                    Escheme_list = dt.AsEnumerable().Select(row => new crm_EmployerSchemeRepo
                    {
                        ES_Id = row.Field<string>("ES_ID"),
                        //Scheme_Id= row.Field<string>("SCHEME_ID"),
                    }).ToList();


                }

            }

            return Escheme_list;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (conp.State == ConnectionState.Open)
            {
                conp.Close();
                conp.Dispose();
            }
        }
    }
}
        public IEnumerable GetEmployerSchemeByEmployerId(string employer_id)
        {
            //var role_list = new List<sec_UserGroupRepo>();
            List<object> data = new List<object>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    if (string.IsNullOrEmpty(employer_id))
                        employer_id = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_REMIT_EMPLOYER_SCHEME_BYID";
                        cmd.Parameters.Add("p_employer_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = employer_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach (DataRow roww in dt.Rows)
                            {
                                string id = roww["SCHEME_ID"].ToString(); // cityNode.SelectSingleNode("id").InnerText;
                                string name = roww["SCHEME_NAME"].ToString();  //cityNode.SelectSingleNode("name").InnerText;
                                data.Add(new { Id = id, Name = name });
                            }

                        }
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        /// validity of employee against employer
        public bool isEmployeeValid(string Employee_Id, string Employer_Id)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_EMP_ID", Employee_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_EMPLOYER_ID", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_EMPLOYER_EMP_EXIST", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");

                if (paramoption > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //Employee Scheme Fund;

        public string GetRemit_EmployeeSchemeFunds_DataReaderFund(string emploid,string  SchemefundId)
        
        {

            string result = "";
            var app = new AppSettings();
            using (OracleConnection conn = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_REMIT_EMPLOYEEBYSCHEMEFUND";
                        cmd.Parameters.Add("p_Employee_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = emploid;
                        cmd.Parameters.Add("p_Scheme_Fund_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = SchemefundId;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            result = dr["ESF_Id"].ToString(); // p_result.Value.ToString();
                        }


                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }

        }
       
        public bool RemitInitialUpLoadExist(string empl_Id,string eS_Id, string formonth, string foryear, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_Employer_Id", value: empl_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ES_Id", value: eS_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_For_Month", value: formonth, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_For_Year", value: foryear, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_REMIT_CON_EXISTFORMONTH", param: param, commandType: CommandType.StoredProcedure);

                tott = param.Get<Int32>("p_count");
                if (tott > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // throw ex;
                error = ex.ToString();
                return false;
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
    
        public string GetRemit_EmployerSchemes_Datasets_NotInUse(string emplid, string Schemeid)
        {
            string SEmp_Id = "";
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
                try
                {
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        //OracleDataReader dr = default(OracleDataReader);
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "SEL_REMIT_EMPLOYERBYSCHEME";
                        cmd.Parameters.Add("p_Employer_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = emplid;
                        cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Schemeid;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            SEmp_Id = dr["ES_ID"].ToString().Trim();
                        }

                    }
                    return SEmp_Id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //finally
                //{
                //    if (conp.State == ConnectionState.Open)
                //    {
                //        conp.Close();
                //        conp.Dispose();
                //    }
                //}
        }

        //  Approve Contribution

        public bool ApprovedRecord(Remit_Contribution_Upload_LogRepo conLogRepo)
        {
            //get scheme_today_date 
            GlobalValue.Get_Scheme_Today_Date(conLogRepo.Scheme_Id);

            var app = new AppSettings(); 

          
            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
             
                try
                {
                    Total_Contribution = Math.Round(Total_Contribution, 2);

                    DynamicParameters paramS = new DynamicParameters();
                   
                   
                    DynamicParameters param = new DynamicParameters();
                    param.Add("p_Con_Id", conLogRepo.Con_Log_Id, DbType.String, ParameterDirection.Input);
                    param.Add("p_Total_Contribution", conLogRepo.Total_Contribution, DbType.Decimal, ParameterDirection.Input);
                    param.Add("p_Con_Balance", conLogRepo.Total_Contribution, DbType.Decimal, ParameterDirection.Input);
                    param.Add("p_Total_Balance", conLogRepo.Total_Contribution, DbType.Decimal, ParameterDirection.Input);
                    param.Add("p_Auth_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add("p_Auth_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add("p_LOG_STATUS", "ACTIVE", DbType.String, ParameterDirection.Input);
                    param.Add("p_AUTH_STATUS", "AUTHORIZED", DbType.String, ParameterDirection.Input);
                    param.Add("p_REQ_STATUS", "ACTIVE", DbType.String, ParameterDirection.Input);
                    param.Add("p_EmpNo", conLogRepo.Total_Contribute, DbType.Decimal, ParameterDirection.Input);
                    //param.Add("p_Scheme_Id", conLogRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                   // param.Add("p_ES_Id", conLogRepo.ES_Id, DbType.String, ParameterDirection.Input);
                    conn.Execute(sql: "UPD_REMIT_CONLOG_FORAPPOVAL", param: param, commandType: CommandType.StoredProcedure);
                 //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    //get scheme_fund_totals
                    if (conn.State == ConnectionState.Open )
                    {
                        conn.Close();
                    }
                    if (conn.State == ConnectionState.Closed )
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd_sft = new OracleCommand())
                    {
                        cmd_sft.Connection = conn;
                        cmd_sft.CommandType = CommandType.StoredProcedure;
                        cmd_sft.CommandText = "SEL_REMIT_CON_BY_FUND_TOTAL";
                        cmd_sft.Parameters.Add("p_con_log_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = conLogRepo.Con_Log_Id;
                        cmd_sft.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd_sft.ExecuteReader();
                        while (dr.Read())
                        {
                       
                            string scheme_fund_id = (dr["SCHEME_FUND_ID"].ToString()); // p_result.Value.ToString();SCHEME_FUND_ID
                            decimal con_amt = Convert.ToDecimal(dr["CON_AMOUNT"].ToString());
                            //
                            DynamicParameters param_gl = new DynamicParameters();
                            param_gl.Add(name: "P_ES_ID", value: conLogRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_SCHEME_FUND_ID", value: scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_CON_AMOUNT", value: con_amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_CON_LOG_ID", value: conLogRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_DATE", value:  GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                            conn.Execute("ADD_REMIT_CON_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);
                        }

                        //paramS.Add("p_ES_Id", conLogRepo.ES_Id, DbType.String, ParameterDirection.Input);
                        //conn.Execute(sql: "UPD_REMIT_CONLOG_FORAPPOVAL2", param: paramS, commandType: CommandType.StoredProcedure);

                    }


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

        //public bool ApprovedRecord(Remit_Contribution_Upload_LogRepo conLogRepo)
        //{
        //    try
        //    {
        //        //get scheme_today_date 
        //        GlobalValue.Get_Scheme_Today_Date(conLogRepo.Scheme_Id);

        //        //Get Connection
        //        AppSettings app = new AppSettings();
        //        con = app.GetConnection();
        //        DynamicParameters param = new DynamicParameters();
                
        //        param.Add("p_Con_Id", conLogRepo.Con_Log_Id, DbType.String, ParameterDirection.Input);
        //        param.Add("p_Total_Contribution", conLogRepo.Total_Contribution , DbType.Decimal , ParameterDirection.Input);
        //        param.Add("p_Con_Balance", conLogRepo.Total_Contribution, DbType.Decimal, ParameterDirection.Input);
        //        param.Add("p_Total_Balance", conLogRepo.Total_Contribution, DbType.Decimal, ParameterDirection.Input);
        //        param.Add("p_Auth_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
        //        param.Add("p_Auth_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
        //        param.Add("p_LOG_STATUS", "ACTIVE", DbType.String, ParameterDirection.Input);
        //        param.Add("p_AUTH_STATUS", "AUTHORIZED", DbType.String, ParameterDirection.Input);
        //        param.Add("p_REQ_STATUS", "ACTIVE", DbType.String, ParameterDirection.Input);
        //        int result = con.Execute(sql: "UPD_REMIT_CONLOG_FORAPPOVAL", param: param, commandType: CommandType.StoredProcedure);
      

        //    if (result < 0)
        //            return true;
        //        else
        //            return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            if (con != null) { con = null; }
        //        }
        //    }
        //}



      
     
        public bool DisapprovedRecord(string Con_Log_Id)
        {
            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_CON_LOG_ID", value: Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_REMIT_CON", param: param, commandType: CommandType.StoredProcedure);

                ts.Complete();

                if (result < 0)
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
                ts.Dispose();
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }

        public string GetRemit_EmployerSchemes_Datasets(string emplid, string Schemeid)
        {
            string result = "";
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_REMIT_EMPLOYERBYSCHEME";
                        cmd.Parameters.Add("p_Employer_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = emplid;
                        cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Schemeid;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        
                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            result = dr["ES_ID"].ToString(); // p_result.Value.ToString();
                        }


                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }


        //Salary Rate
        public decimal  Get_EmployeeSalSchemes_Rate(string Scheme_id)
        {
            decimal  result2 = 0;
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_PFM_SCHEMERATEBY_ID";
                       
                        cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                     
                        //p_result.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(p_result);
                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            result2 = Convert.ToDecimal(dr["SALARY_RATE"].ToString()); // p_result.Value.ToString();

                    }


                    }
                    return result2;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        /////Grace Period
       
        public decimal Get_EmployeeScheme_GracePeriod(string Scheme_id)
        {
            decimal result3 = 0;
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_PMF_SCHEMEGRACEPERIODEXIST";

                        cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        //p_result.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(p_result);
                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            result3 = Convert.ToDecimal(dr["SURCHARGE_GRACE_PERIOD"].ToString()); // p_result.Value.ToString();



                        }


                    }
                    return result3;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }
        //Get Employer Scheme Account By Employer Id
        public List<Remit_Contribution_Upload_LogRepo> Get_Crm_Employer_SchemeByStatus(string Es_status)
        {
            string status = "ACTIVE";
            Es_status = status;

            var batch_list = new List<Remit_Contribution_Upload_LogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_CRM_EMPLOYER_SCHEMEBYEMPID";
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2, ParameterDirection.Input).Value = Es_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                      
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new Remit_Contribution_Upload_LogRepo
                            {
                                Employer_Name = row.Field<string>("Employer_Name"),
                                Employer_Id = row.Field<string>("EMPLOYER_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Scheme_Id = row.Field<string>("SCHEME_ID"),
                                ES_Id = row.Field<string>("EMPLOYER_ACCOUNT_NO"),
                                DeadLine_Date = Convert.ToDateTime(row.Field<DateTime>("NEXT_DEADLINE_DATE")),
                             

                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }


        
        //Get  Scheme Accounts
        public List<Remit_Contribution_Upload_LogRepo> Get_Crm_SchemeByStatus()
        {
           
            var batch_list = new List<Remit_Contribution_Upload_LogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_SCHEME_ALL_REP";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new Remit_Contribution_Upload_LogRepo
                            {

                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Scheme_Id = row.Field<string>("SCHEME_ID"),
      


                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        //  Delete Contribution


        public bool DeleteRecord(Remit_Contribution_Upload_LogRepo conLogRepo)
        {
            //get scheme_today_date 
            GlobalValue.Get_Scheme_Today_Date(conLogRepo.Scheme_Id);

            var app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    //get scheme_fund_totals

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd_sft = new OracleCommand())
                    {
                        cmd_sft.Connection = conn;
                        cmd_sft.CommandType = CommandType.StoredProcedure;
                        cmd_sft.CommandText = "SEL_REMIT_CON_BY_FUND_TOTAL";
                        cmd_sft.Parameters.Add("p_con_log_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = conLogRepo.Con_Log_Id;
                        cmd_sft.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd_sft.ExecuteReader();
                        while (dr.Read())
                        {
                            string scheme_fund_id = (dr["SCHEME_FUND_ID"].ToString()); // p_result.Value.ToString();SCHEME_FUND_ID
                            decimal con_amt = Convert.ToDecimal(dr["CON_AMOUNT"].ToString());
                            //
                            DynamicParameters param_gl = new DynamicParameters();
                            param_gl.Add(name: "P_ES_ID", value: conLogRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_SCHEME_FUND_ID", value: scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_CON_AMOUNT", value: con_amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_CON_LOG_ID", value: conLogRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                            conn.Execute("ADD_REMIT_CON_GL_TRANS_REV", param_gl, commandType: CommandType.StoredProcedure);
                        }
                    }

                    DynamicParameters param = new DynamicParameters();
                    param.Add("p_Con_Id", conLogRepo.Con_Log_Id, DbType.String, ParameterDirection.Input);
                    conn.Execute(sql: "DEL_REMIT_CONLOG_FORDELETE", param: param, commandType: CommandType.StoredProcedure);

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

        //public bool DeleteRecord(Remit_Contribution_Upload_LogRepo conLogRepo)
        //{
        //    try
        //    {             
        //        //Get Connection
        //        AppSettings app = new AppSettings();
        //        con = app.GetConnection();
        //        DynamicParameters param = new DynamicParameters();
        //        param.Add("p_Con_Id", conLogRepo.Con_Log_Id, DbType.String, ParameterDirection.Input);
        //        int result = con.Execute(sql: "DEL_REMIT_CONLOG_FORDELETE", param: param, commandType: CommandType.StoredProcedure);
        //        if (result < 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            if (con != null) { con = null; }
        //        }
        //    }
        //}

      
        public bool DeleteBatchRecord(Remit_Contribution_Upload_LogRepo conLogRepo)
        {
            try
            {

                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add("p_Employer_Id", conLogRepo.Employer_Id + conLogRepo.Scheme_Id + "01", DbType.String, ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_REMIT_CONLOG_BATCHDELETE", param: param, commandType: CommandType.StoredProcedure);


                if (result < 0)
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

        

        public List<Remit_Contribution_Upload_LogRepo> GetEmployeeCon_BatchList_delete(string batchLogno)
        {
            var batch_list = new List<Remit_Contribution_Upload_LogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //if (string.IsNullOrEmpty(cust_status))
                    //    cust_status = "0";
                    if (string.IsNullOrEmpty(batchLogno))
                        batchLogno = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_REMIT_EMPLOYEESBY_DELETE";
                        cmd.Parameters.Add("p_Con_Log_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = batchLogno;
                        //cmd.Parameters.Add("p_cust_status", OracleDbType.Varchar2, ParameterDirection.Input).Value = cust_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new Remit_Contribution_Upload_LogRepo
                            {
                                Surname = row.Field<string>("SURNAME"),
                                first_Name = row.Field<string>("FIRST_NAME"),
                                Employee_Id = row.Field<string>("EMPLOYEE_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Fund_Name = row.Field<string>("FUND_NAME"),
                                Employee_Con = Convert.ToDecimal(row.Field<decimal>("EMPLOYEE_CON")),
                                Employer_Con = Convert.ToDecimal(row.Field<decimal>("EMPLOYER_CON")),

                                Employee_Salary = Convert.ToDecimal(row.Field<decimal>("EMPLOYEE_SALARY")),
                                /// Employee_Sal_Rate = Convert.ToDecimal(row.Field<decimal>("EMPLOYEE_SAL_RATE"))
                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

		//FILTERING GL ACOUNT LIST FOR SCHEME_FUND
		public List<Remit_Contribution_Upload_LogRepo> GetGLASFList(string Scheme_Id)
		{
			try
			{
				AppSettings app = new AppSettings();
				con = app.GetConnection();
				List<Remit_Contribution_Upload_LogRepo> bn = new List<Remit_Contribution_Upload_LogRepo>();

				string query = "Select * from VW_REPORT_CON_LIST WHERE SCHEME_ID = '" + Scheme_Id + "'   group by SCHEME_ID,FOR_MONTH,FOR_YEAR,monthname,ES_ID order by   FOR_YEAR desc ,FOR_MONTH desc";
				return bn = con.Query<Remit_Contribution_Upload_LogRepo>(query).ToList();

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



		//FILTERING LIST
		public List<Remit_Contribution_Upload_LogRepo> GetGLASFList2(string ES_Id)
		{
			try
			{
				AppSettings app = new AppSettings();
				con = app.GetConnection();
				List<Remit_Contribution_Upload_LogRepo> bn = new List<Remit_Contribution_Upload_LogRepo>();

				string query = "Select * from VW_REPORT_CON_LIST WHERE ES_ID = '" + ES_Id + "'   group by SCHEME_ID,FOR_MONTH,FOR_YEAR,monthname,ES_ID order by   FOR_YEAR desc ,FOR_MONTH desc";
				return bn = con.Query<Remit_Contribution_Upload_LogRepo>(query).ToList();

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


        //FILTERING LIST
        public List<Remit_Contribution_Upload_LogRepo> GetGLASFList22(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_Contribution_Upload_LogRepo> bn = new List<Remit_Contribution_Upload_LogRepo>();

                string query = "Select * from VW_REPORT_CON_LIST WHERE Scheme_Id = '" + Scheme_Id + "'   group by SCHEME_ID,FOR_MONTH,FOR_YEAR,monthname,ES_ID order by   FOR_YEAR desc ,FOR_MONTH desc";
                return bn = con.Query<Remit_Contribution_Upload_LogRepo>(query).ToList();

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



        public bool isValidDelete(string Con_Log_Id)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_CON_ID", Con_Log_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_ISVALID_CON", param, commandType: CommandType.StoredProcedure);
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

    }

}


