using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace PENFAD6DAL.Repository.GL
{
    public class GLCallInterestRepo
    {
        public decimal TID { get; set; }
        [Required]
        public DateTime Trans_Date { get; set; }
        [Required]
        [Range(0.1, 999999999999999999, ErrorMessage = "AMOUNT must be greather than zero.")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Please select Account.")]
        public string GL_Account_No { get; set; }
        public string GL_Account_Name { get; set; }
        public decimal Gl_Balance { get; set; }
   
        public string Narration { get; set; }
        public string Rec_Status { get; set; }
        public string Auth_Status { get; set; }
        public string Auth_Date { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime Update_Date { get; set; }
        public string Reversal_Id { get; set; }
        public DateTime Reversal_Date { get; set; }
        public string Reversal_Reason { get; set; }
        public string Trans_Type { get; set; }
        public string Scheme_Fund_Id { get; set; }
        public string Scheme { get; set; }
        public string Fund { get; set; }
        public DateTime? From_Date { get; set; }
        public DateTime? To_Date { get; set; }

        // methods begin

        IDbConnection con;

        public void SaveRecord(GLJournalRepo GLJournal)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
                param.Add(name: "P_TRANS_DATE", value: GLJournal.Trans_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_AMOUNT", value: GLJournal.Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_SF_ID", value: GLJournal.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_DEBIT_GL_NO", value: GLJournal.Debit_Gl_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CREDIT_GL_NO", value: GLJournal.Credit_Gl_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_JOURNAL_STATUS", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_TYPE", value: GLJournal.Trans_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_NARRATION", value: GLJournal.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "ADD_JOURNAL", param: param, commandType: CommandType.StoredProcedure);        
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

        //GET GL DEBIT ACCOUNT BALANCE FROM GL ACCOUNT TABLE
        public void Get_Balance(GLJournalRepo GLRepo)
         {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_DEBIT_GL_NO", GLRepo.Debit_Gl_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_DEBITBALANCE", param, commandType: CommandType.StoredProcedure);
                GLRepo.Debit_Gl_Balance = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //GET GL CREDIT ACCOUNT BALANCE FROM GL ACCOUNT TABLE
        public void Get_BalanCredit(GLJournalRepo GLRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_CREDIT_GL_NO", GLRepo.Credit_Gl_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_CREDITBALANCE", param, commandType: CommandType.StoredProcedure);
                GLRepo.Credit_Gl_Balance = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //get debit gl list

        public List<GLJournalRepo> debitList(string scheme_fund_id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "SEL_GL_DEBIT_ACCOUNT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                OracleParameter paramemployer_Id = new OracleParameter("p_scheme_fund_id", OracleDbType.Varchar2, scheme_fund_id, ParameterDirection.Input);
                cmd.Parameters.Add(paramemployer_Id);
                OracleParameter param2 = new OracleParameter("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(param2);

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "journal");

                var eList = ds.Tables[0].AsEnumerable().Select(row => new GLJournalRepo
                {
                    Debit_Gl_No = row.Field<string>("GL_Account_No"),
                    Debit_Gl_Name = row.Field<string>("GL_Account_Name"),
                    Debit_Gl_Balance = row.Field<decimal>("GL_Balance") * -1,

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


        public List<GLJournalRepo> CreditList(string scheme_fund_id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "SEL_GL_DEBIT_ACCOUNT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                //Input param
                OracleParameter paramemployer_Id = new OracleParameter("p_scheme_fund_id", OracleDbType.Varchar2, scheme_fund_id, ParameterDirection.Input);
                cmd.Parameters.Add(paramemployer_Id);
                OracleParameter param2 = new OracleParameter("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(param2);

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "journalcredit");

                var eList = ds.Tables[0].AsEnumerable().Select(row => new GLJournalRepo
                {
                    Credit_Gl_No = row.Field<string>("GL_Account_No"),
                    Credit_Gl_Name = row.Field<string>("GL_Account_Name"),
                    Credit_Gl_Balance = row.Field<decimal>("GL_Balance") * -1,

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

        //public DataSet DebitData()
        //{
        //    try
        //    {
        //        //Get connection
        //        var app = new AppSettings();
        //        con = app.GetConnection();

        //        DataSet ds = new DataSet();

        //        OracleDataAdapter da = new OracleDataAdapter();
        //        OracleCommand cmd = new OracleCommand();
        //        OracleParameter param = cmd.CreateParameter();

        //        cmd.CommandText = ("SEL_GL_DEBIT_ACCOUNT");

        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = (OracleConnection)con;

        //        param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
        //        param.Direction = ParameterDirection.Output;

        //        da = new OracleDataAdapter(cmd);
        //        da.Fill(ds, "bankact");
        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public IEnumerable<GLJournalRepo> debitList()
        //{
        //    try
        //    {
        //        DataSet dt = DebitData();
        //        var eList = dt.Tables[0].AsEnumerable().Select(row => new GLJournalRepo
        //        {
        //            Debit_Gl_No  = row.Field<string>("GL_Account_No"),
        //            Debit_Gl_Name = row.Field<string>("GL_Account_Name"),
        //            Debit_Gl_Balance = row.Field<decimal>("GL_Balance") * -1,

        //        }).ToList();

        //        return eList;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
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




        public DataSet JournalApproveData()
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

                cmd.CommandText = ("SEL_JOURNAL_PENDING");

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "bankact");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<GLJournalRepo> GLApproveList()
        {
            try
            {
                DataSet dt = JournalApproveData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new GLJournalRepo
                {
                    TID = row.Field<decimal>("TID"),
                    Debit_Gl_No = row.Field<string>("DEBIT_GL_NO"),
                    Debit_Gl_Name = row.Field<string>("GL_Account_Name1"),
                    Debit_Gl_Balance = row.Field<decimal>("GL_Balance1") * -1,
                    Credit_Gl_No = row.Field<string>("CREDIT_GL_NO"),
                    Credit_Gl_Name = row.Field<string>("GL_Account_Name"),
                    Credit_Gl_Balance = row.Field<decimal>("GL_Balance") * -1,
                    Narration = row.Field<string>("NARRATION"),
                    Amount = row.Field<decimal>("AMOUNT"),
                    Trans_Date = row.Field<DateTime>("TRANS_DATE"),
                    Trans_Type = row.Field<string>("TRANS_TYPE"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund  = row.Field<string>("FUND_NAME"),
                    Scheme = row.Field<string>("SCHEME_NAME"),
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



        public IEnumerable<GLJournalRepo> GLReversalList(DateTime? From_Date, DateTime? To_Date)
        {
            try
            {
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_JOURNAL_ACTIVE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                OracleParameter ParamfromDate = new OracleParameter("p_fromDate", OracleDbType.Date, From_Date, ParameterDirection.Input);
                cmd.Parameters.Add(ParamfromDate);

                OracleParameter ParamtoDate = new OracleParameter("p_toDate", OracleDbType.Date, To_Date, ParameterDirection.Input);
                cmd.Parameters.Add(ParamtoDate);

                param = cmd.Parameters.Add("p_Journal_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;


                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "journal");
                var eList = ds.Tables[0].AsEnumerable().Select(row => new GLJournalRepo
                {
                    TID = row.Field<decimal>("TID"),
                    Debit_Gl_No = row.Field<string>("DEBIT_GL_NO"),
                    Debit_Gl_Name = row.Field<string>("GL_Account_Name1"),
                    Debit_Gl_Balance = row.Field<decimal>("GL_Balance1") * -1,
                    Credit_Gl_No = row.Field<string>("CREDIT_GL_NO"),
                    Credit_Gl_Name = row.Field<string>("GL_Account_Name"),
                    Credit_Gl_Balance = row.Field<decimal>("GL_Balance") * -1,
                    Narration = row.Field<string>("NARRATION"),
                    Amount = row.Field<decimal>("AMOUNT"),
                    Trans_Date = row.Field<DateTime>("TRANS_DATE"),
                    Trans_Type = (row["TRANS_TYPE"]).ToString(),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund = row.Field<string>("FUND_NAME"),
                    Scheme = row.Field<string>("SCHEME_NAME"),

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
        //public DataSet CreditData()
        //{
        //    try
        //    {
        //        //Get connection
        //        var app = new AppSettings();
        //        con = app.GetConnection();

        //        DataSet ds = new DataSet();

        //        OracleDataAdapter da = new OracleDataAdapter();
        //        OracleCommand cmd = new OracleCommand();
        //        OracleParameter param = cmd.CreateParameter();

        //        cmd.CommandText = ("SEL_GL_DEBIT_ACCOUNT");

        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = (OracleConnection)con;

        //        param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
        //        param.Direction = ParameterDirection.Output;

        //        da = new OracleDataAdapter(cmd);
        //        da.Fill(ds, "bankact");
        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public IEnumerable<GLJournalRepo> creditList()
        //{
        //    try
        //    {
        //        DataSet dt = CreditData();
        //        var eList = dt.Tables[0].AsEnumerable().Select(row => new GLJournalRepo
        //        {
        //            Credit_Gl_No = row.Field<string>("GL_Account_No"),
        //            Credit_Gl_Name = row.Field<string>("GL_Account_Name"),
        //            Credit_Gl_Balance = row.Field<decimal>("GL_Balance") * -1,

        //        }).ToList();

        //        return eList;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
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

             //CALL JOURNAL PENDING RECORD
        public List<GLJournalRepo> GetJournalPendingList(decimal TID)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLJournalRepo> bn = new List<GLJournalRepo>();

                string query = "Select * from GL_JOURNAL_LOG WHERE TID = '" +  TID + "'";
                return bn = con.Query<GLJournalRepo>(query).ToList();

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


        public bool Approve_Journal(GLJournalRepo GLRepo)
        {
            var app = new AppSettings(); 

            // get the pending purchase record
            GLRepo.GetJournalPendingList(TID);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {


                    // UPDATE GL_JOURNAL_LOG TABLE

                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_TID", value: TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_JOURNAL_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_JOURNAL", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_DEBIT_NO", value: GLRepo.Debit_Gl_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_CREDIT_NO", value: GLRepo.Credit_Gl_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AMOUNT", value: GLRepo.Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_NARRATION", value: GLRepo.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TID", value: GLRepo.TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TRANS_DATE", value: GLRepo.Trans_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                    conn.Execute("APP_JOURNAL_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

                

                ts.Complete();

                return true;
                }
                catch (Exception ex)
                {
                    string xx = ex.ToString();
                    throw ;
                }
                finally
                {
                    ts.Dispose();
                    if (con.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }
                
        }


        public bool Reverse_Journal(GLJournalRepo GLRepo)
        {
            var app = new AppSettings();

            // get the pending purchase record
            GLRepo.GetJournalPendingList(TID);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {


                    // UPDATE GL_JOURNAL_LOG TABLE

                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_TID", value: TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_JOURNAL_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSAL_REASON", value: Reversal_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSAL_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSAL_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("REV_JOURNAL", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_DEBIT_NO", value: GLRepo.Debit_Gl_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_CREDIT_NO", value: GLRepo.Credit_Gl_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AMOUNT", value: GLRepo.Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_NARRATION", value: GLRepo.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TID", value: GLRepo.TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TRANS_DATE", value: GLRepo.Trans_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                    conn.Execute("REV_JOURNAL_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);



                    ts.Complete();

                    return true;
                }
                catch (Exception ex)
                {
                    string xx = ex.ToString();
                    throw;
                }
                finally
                {
                    ts.Dispose();
                    if (con.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


    }
} //end class gl repo

