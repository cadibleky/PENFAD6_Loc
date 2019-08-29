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
    public class GLFeeRepo
    {
        public decimal TID { get; set; }
        public DateTime? Trans_Date { get; set; }
        [Range(0, 9999999999999999, ErrorMessage = "Invalid Principal  Amount")]
        public decimal Paid_Amount { get; set; }
        public decimal Apply_Amount { get; set; }
        public decimal Apply_Bal { get; set; }
        public DateTime Last_Apply_Date { get; set; }
        public DateTime Last_Paid_Date { get; set; }
        public string Fee_Id { get; set; }
        public string Fee_Description { get; set; }
        public string Fund_Manager_Id { get; set; }
        public string Fund_Manager { get; set; }
        public string Narration { get; set; }
        public string Journal_Status { get; set; }
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
        public decimal GL_Balance { get; set; }
        public string GL_Account_No { get; set; }
        public string GL_Account_Name { get; set; }

        // methods begin

        IDbConnection con;

        public void SaveRecord(GLFeeRepo GLFee)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
                param.Add(name: "P_TRANS_DATE", value: GLFee.Trans_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_AMOUNT", value: GLFee.Paid_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_SF_ID", value: GLFee.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FEE_ID", value: GLFee.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_APPLY_PAID", value: "PAID", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_NO", value: GLFee.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REC_STATUS", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_NARRATION", value: GLFee.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FM_ID", value: GLFee.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "ADD_FEEPAYMENT", param: param, commandType: CommandType.StoredProcedure);        
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
        public void Get_Balance(GLFeeRepo GLFee)
         {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_GL_NO",GLFee.GL_Account_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_BALANCE_FEE", param, commandType: CommandType.StoredProcedure);
                GLFee.GL_Balance = param.Get<decimal>("VDATA");

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

        //GET GL CREDIT ACCOUNT BALANCE FROM GL ACCOUNT TABLE
        public void Get_BalanCredit_Rev(GLJournalRepo GLRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_CREDIT_GL_NO", GLRepo.Debit_Gl_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_CREDITBALANCE", param, commandType: CommandType.StoredProcedure);
                GLRepo.Debit_Gl_Balance = param.Get<decimal>("VDATA");
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




        //public DataSet GLFeeApproveData()
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

        //        cmd.CommandText = ("SEL_GLFEE_PENDING");

        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = (OracleConnection)con;

        //        param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
        //        param.Direction = ParameterDirection.Output;

        //        da = new OracleDataAdapter(cmd);
        //        da.Fill(ds, "glfeepend");
        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public IEnumerable<GLFeeRepo> GLApproveList(GLFeeRepo GLFee)
        //{
        //    try
        //    {
        //        DataSet dt = GLFeeApproveData();
        //        var eList = dt.Tables[0].AsEnumerable().Select(row => new GLFeeRepo
        //        {
        //            TID = row.Field<decimal>("TID"),
        //            Scheme_Fund_Id = row.Field<string>("scheme_fund_id"),
        //            Fee_Id = row.Field<string>("fee_id"),
        //            Fee_Description = row.Field<string>("fee_description"),
        //            Trans_Date = row.Field<DateTime>("trans_date"),
        //            Apply_Amount = row.Field<decimal>("apply_amount"),
        //            Paid_Amount = row.Field<decimal>("Paid_Amount"),
        //            Narration = row.Field<string>("NARRATION"),
        //            Fund_Manager_Id = row.Field<string>("Fund_Manager_Id"),
        //            Fund_Manager = row.Field<string>("Fund_Manager"),
        //            GL_Account_No = row.Field<string>("GL_Account_No"),
        //            GL_Account_Name = row.Field<string>("GL_Account_Name"),
        //            GL_Balance = row.Field<decimal>("GL_Balance") * -1,
        //            Fund = row.Field<string>("FUND_NAME"),
        //            Scheme = row.Field<string>("SCHEME_NAME"),
        //        }).ToList();

        //        return eList;
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

        public void GLApproveList(GLFeeRepo GLFee)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                if (string.IsNullOrEmpty(GLFee.Fee_Id) || string.IsNullOrEmpty(GLFee.Scheme_Fund_Id))
                {
                }

                else
                {

                    param.Add("P_FEE_ID", GLFee.Fee_Id, DbType.String, ParameterDirection.Input);
                    param.Add("P_SCHEME_FUND_ID", GLFee.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                    param.Add("P_FM_ID", GLFee.Fund_Manager_Id, DbType.String, ParameterDirection.Input);
                    param.Add("V1", "", DbType.String, ParameterDirection.Output);
                    param.Add("V2", "", DbType.String, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_GLFEE_FUNDM_SEARCH", param, commandType: CommandType.StoredProcedure);
                    GLFee.Fund_Manager = param.Get<string>("V1");
                    GLFee.Fund_Manager_Id = param.Get<string>("V2");
                }
              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataSet GLFeeApproveData2()
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

                cmd.CommandText = ("SEL_GLFEE_PENDING");

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "glfeepend2");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<GLFeeRepo> GLApproveList2()
        {
            try
            {
                DataSet dt = GLFeeApproveData2();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new GLFeeRepo
                {
                    TID = row.Field<decimal>("TID"),
                    Scheme_Fund_Id = row.Field<string>("scheme_fund_id"),
                    Fee_Id = row.Field<string>("fee_id"),
                    Fee_Description = row.Field<string>("fee_description"),
                    Trans_Date = row.Field<DateTime>("trans_date"),
                    Apply_Amount = row.Field<decimal>("apply_amount"),
                    Paid_Amount = row.Field<decimal>("Paid_Amount"),
                    Narration = row.Field<string>("NARRATION"),
                    Fund_Manager_Id = row.Field<string>("Fund_Manager_Id"),
                    //Fund_Manager = row.Field<string>("Fund_Manager"),
                    GL_Account_No = row.Field<string>("GL_Account_No"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name"),
                    GL_Balance = row.Field<decimal>("GL_Balance") * -1,
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


        public IEnumerable<GLFeeRepo> GLReversalList(DateTime? From_Date, DateTime? To_Date)
        {
            try
            {
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_GLFEE_ACTIVE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                OracleParameter ParamfromDate = new OracleParameter("p_fromDate", OracleDbType.Date, From_Date, ParameterDirection.Input);
                cmd.Parameters.Add(ParamfromDate);

                OracleParameter ParamtoDate = new OracleParameter("p_toDate", OracleDbType.Date, To_Date, ParameterDirection.Input);
                cmd.Parameters.Add(ParamtoDate);

                param = cmd.Parameters.Add("p_Journal_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;


                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "glfee");
                var eList = ds.Tables[0].AsEnumerable().Select(row => new GLFeeRepo
                {
                    TID = row.Field<decimal>("TID"),
                    Scheme_Fund_Id = row.Field<string>("scheme_fund_id"),
                    Fee_Id = row.Field<string>("fee_id"),
                    Fee_Description = row.Field<string>("fee_description"),
                    Trans_Date = row.Field<DateTime>("trans_date"),
                    Apply_Amount = row.Field<decimal>("apply_amount"),
                    Paid_Amount = row.Field<decimal>("Paid_Amount"),
                    Narration = row.Field<string>("NARRATION"),
                    Fund_Manager_Id = row.Field<string>("Fund_Manager_Id"),
                   // Fund_Manager = row.Field<string>("Fund_Manager"),
                    GL_Account_No = row.Field<string>("GL_Account_No"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name"),
                    GL_Balance = row.Field<decimal>("GL_Balance") * -1,
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


        public IEnumerable<GLFeeRepo> GLReversalList2(DateTime? From_Date, DateTime? To_Date)
        {
            try
            {
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_GLFEE_ACTIVE2";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                OracleParameter ParamfromDate = new OracleParameter("p_fromDate", OracleDbType.Date, From_Date, ParameterDirection.Input);
                cmd.Parameters.Add(ParamfromDate);

                OracleParameter ParamtoDate = new OracleParameter("p_toDate", OracleDbType.Date, To_Date, ParameterDirection.Input);
                cmd.Parameters.Add(ParamtoDate);

                param = cmd.Parameters.Add("p_Journal_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;


                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "glfee");
                var eList = ds.Tables[0].AsEnumerable().Select(row => new GLFeeRepo
                {
                    TID = row.Field<decimal>("TID"),
                    Scheme_Fund_Id = row.Field<string>("scheme_fund_id"),
                    Fee_Id = row.Field<string>("fee_id"),
                    Fee_Description = row.Field<string>("fee_description"),
                    Trans_Date = row.Field<DateTime>("trans_date"),
                    Apply_Amount = row.Field<decimal>("apply_amount"),
                    Paid_Amount = row.Field<decimal>("Paid_Amount"),
                    Narration = row.Field<string>("NARRATION"),
                    GL_Account_No = row.Field<string>("GL_Account_No"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name"),
                    GL_Balance = row.Field<decimal>("GL_Balance") * -1,
                    Fund = row.Field<string>("FUND_NAME"),
                    Scheme = row.Field<string>("SCHEME_NAME"),
                    Fund_Manager_Id = "NA",
             
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


        //CALL FEE PENDING RECORD
        public List<GLFeeRepo> GetGLFeePendingList(decimal TID)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLFeeRepo> bn = new List<GLFeeRepo>();

                string query = "Select * from PFM_SCHEME_FEE_TRANS_APP_PAID WHERE TID = '" +  TID + "'";
                return bn = con.Query<GLFeeRepo>(query).ToList();

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


        public bool Approve_GLFee(GLFeeRepo GLFee)
        {
            var app = new AppSettings();

            // get the pending purchase record

            GLFee.GetGLFeePendingList(TID);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {


                    // UPDATE log TABLE

                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_TID", value: TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_REC_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("APP_GLFEE", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_GL_NO", value: GLFee.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AMOUNT", value: GLFee.Paid_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_NARRATION", value: GLFee.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TID", value: GLFee.TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TRANS_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SF_ID", value: GLFee.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_FEE_ID", value: GLFee.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_FM_ID", value: GLFee.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                    conn.Execute("APP_FEE_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

                

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

        public bool Disapprove_GLFee(GLFeeRepo GLFee)
        {
            var app = new AppSettings();

            // get the pending purchase record

            GLFee.GetGLFeePendingList(TID);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {


                    // UPDATE log TABLE

                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_TID", value: TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_REC_STATUS", value: "DISAPPROVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("APP_GLFEE", param, commandType: CommandType.StoredProcedure);

                  
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
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        public bool Reverse_Fee(GLFeeRepo GLFee)
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
						// UPDATE log TABLE

                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_TID", value: TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_REC_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("APP_GLFEE", param, commandType: CommandType.StoredProcedure);

					//UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

					DynamicParameters param_gl = new DynamicParameters();
					param_gl.Add(name: "P_GL_NO", value: GLFee.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_AMOUNT", value: GLFee.Paid_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_NARRATION", value: GLFee.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_TID", value: GLFee.TID, dbType: DbType.Decimal, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_TRANS_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_SF_ID", value: GLFee.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_FEE_ID", value: GLFee.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
					param_gl.Add(name: "P_FM_ID", value: GLFee.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);

					conn.Execute("APP_FEE_GL_TRANS_REV", param_gl, commandType: CommandType.StoredProcedure);



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
					if (conn.State == ConnectionState.Open)
					{
						conn.Close();
					}
				}

			}

        }

        //FILTERING FUND MANGERS LIST FOR FEE
        public List<GLFeeRepo> GetFeesList2(GLFeeRepo GLFee)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLFeeRepo> bn = new List<GLFeeRepo>();

                if (string.IsNullOrEmpty(GLFee.Fee_Id) || string.IsNullOrEmpty(GLFee.Scheme_Fund_Id))
                {
                }

                else if (GLFee.Fee_Id == "F0002")
                {
                   
                    string query = "Select * from VW_SCH_FEE_APPLY_FM WHERE FEE_ID = '" + GLFee.Fee_Id + "' and SCHEME_FUND_ID = '" + GLFee.Scheme_Fund_Id + "'";
                    return bn = con.Query<GLFeeRepo>(query).ToList();

                }
                else if (GLFee.Fee_Id != "F0002")
                {
              

                    string query = "Select * from VW_SCH_FEE_APPLY WHERE FEE_ID = '" + GLFee.Fee_Id + "' and SCHEME_FUND_ID = '" + GLFee.Scheme_Fund_Id + "' ";
                    return bn = con.Query<GLFeeRepo>(query).ToList();
                }
                return bn;
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

        public void GetFeesList(GLFeeRepo GLFee)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                if (string.IsNullOrEmpty(GLFee.Fee_Id) || string.IsNullOrEmpty(GLFee.Scheme_Fund_Id))
                {
                }

                else if (GLFee.Fee_Id =="F0002")
                {
               
                    param.Add("P_FEE_ID", GLFee.Fee_Id, DbType.String, ParameterDirection.Input);
                    param.Add("P_FM_ID", GLFee.Fund_Manager_Id, DbType.String, ParameterDirection.Input);
                    param.Add("P_SCHEME_FUND_ID", GLFee.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VABAL", null, DbType.Decimal, ParameterDirection.Output);
                    param.Add("VLADATE", null, DbType.DateTime, ParameterDirection.Output);
                    param.Add("VLPDATE", null, DbType.DateTime, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_SCHEME_APPLY_VAL", param, commandType: CommandType.StoredProcedure);
                    GLFee.Apply_Bal = param.Get<decimal>("VABAL");
                    GLFee.Last_Apply_Date = param.Get<DateTime>("VLADATE");
                    GLFee.Last_Paid_Date = param.Get<DateTime>("VLPDATE");
                }
                else if (GLFee.Fee_Id != "F0002")
                {

                    param.Add("P_FEE_ID", GLFee.Fee_Id, DbType.String, ParameterDirection.Input);
                    param.Add("P_FM_ID", GLFee.Fund_Manager_Id, DbType.String, ParameterDirection.Input);
                    param.Add("P_SCHEME_FUND_ID", GLFee.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VABAL", null, DbType.Decimal, ParameterDirection.Output);
                    param.Add("VLADATE", null, DbType.DateTime, ParameterDirection.Output);
                    param.Add("VLPDATE", null, DbType.DateTime, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_SCHEME_APPLY", param, commandType: CommandType.StoredProcedure);
                    GLFee.Apply_Bal = param.Get<decimal>("VABAL");
                    GLFee.Last_Apply_Date = param.Get<DateTime>("VLADATE");
                    GLFee.Last_Paid_Date = param.Get<DateTime>("VLPDATE");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public List<GLFeeRepo> GetAllFeesList()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLFeeRepo> bn = new List<GLFeeRepo>();

                string query = "Select * from PFM_FEES  ";
                return bn = con.Query<GLFeeRepo>(query).ToList();

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


    }
} //end class gl repo

