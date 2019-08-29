using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    public class Remit_ReceiptRepo
    {
        public string Receipt_Id { get; set; }

        [Required(ErrorMessage = "Please select Employer Scheme Account")]
        public string ES_Id { get; set; }
        public string Employer_Id { get; set; }
        public string Employer_Name { get; set; }
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Scheme_Id_2 { get; set; }
        public string Scheme_Name_2 { get; set; }
        public decimal For_Month { get; set; }
        public decimal For_Year { get; set; }
        public string Receipt_Confirm { get; set; }
        public decimal Total_Contribution { get; set; }
        public decimal Total_Receipt { get; set; }
        public decimal Total_Arrears { get; set; }
        public decimal Surcharge_Balance { get; set; }
        public string Con_Auth_Status { get; set; }
        public string Log_Status { get; set; }
        [Required(ErrorMessage = "Please Date is required")]
        public DateTime Actual_Receipt_Date { get; set; }
        [Required(ErrorMessage = "Please Amount is required")]
        public Decimal Trans_Amount { get; set; }
        public string Trans_Amount_str { get; set; }
        [Required(ErrorMessage = "Please Narration is required")]
        public string Narration { get; set; }
        public string Narration_Syatem { get; set; }
        public string Auth_Status { get; set; }
        [Required(ErrorMessage = "Please select Payment mode")]
        public string Payment_Mode { get; set; }
        public string Instrument_No { get; set; }
        public string Receipt_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Maker_Id { get; set; }
        public string Reverse_Id { get; set; }
        public string Reverse_Reason { get; set; }
        public string Reverse_Date { get; set; }
        public string Reverse_Status { get; set; }
        public DateTime From_Date { get; set; }
        public DateTime To_Date { get; set; }
        public DateTime Today_Date { get; set; }
        public DateTime Scheme_Working_Date { get; set; }
        public decimal Cash_Balance { get; set; }
        public decimal Con_Total { get; set; }

        public string Contact_Email { get; set; }
        public string Contact_Person { get; set; }


        IDbConnection con;
        public void SaveRecord(Remit_ReceiptRepo ReceiptRepo)
        {
            GlobalValue.Get_Scheme_Today_Date(ReceiptRepo.Scheme_Id);
            //put trans scope here

            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_RECEIPT_ID", value: ReceiptRepo.Receipt_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ES_ID", value: ReceiptRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_AMOUNT", value: ReceiptRepo.Trans_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_ACTUAL_RECEIPT_DATE", value: ReceiptRepo.Actual_Receipt_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_NARRATION", value: ReceiptRepo.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_NARRATION_SYSTEM", value: ReceiptRepo.Narration, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PAYMENT_MODE", value: ReceiptRepo.Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_INSTRUMENT_NO", value: ReceiptRepo.Instrument_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                con.Execute("MIX_REMIT_RECEIPT", param, commandType: CommandType.StoredProcedure);
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


        // Approve Receipt
        public bool ApproveReceiptRecord(string RECEIPT_ID, Remit_ReceiptRepo ReceiptRepo)
        {
            AppSettings app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            try
            {
                //go off
                //Get Connection

                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_RECEIPT_ID", value: RECEIPT_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RECEIPT_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                con.Execute("APP_REMIT_RECEIPT", param, commandType: CommandType.StoredProcedure);
                ts.Complete();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //return true;
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


        // Disapprove Receipt
        public bool DisapproveReceiptRecord(string RECEIPT_ID)
        {
            AppSettings app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);
            try
            {
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_RECEIPT_ID", value: RECEIPT_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RECEIPT_STATUS", value: "DISAPPROVED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "DISAPPROVED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                con.Execute("DISAPPROVE_REMIT_RECEIPT", param, commandType: CommandType.StoredProcedure);
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }
        }


        // Reverse Receipt
        public bool ReverseReceiptRecord(Remit_ReceiptRepo Receipt)
        {
            AppSettings app = new AppSettings();

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);
            try
            {
                //go off
                //Get Connection
                 
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_RECEIPT_ID", value: Receipt.Receipt_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REVERSE_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REVERSE_REASON", value: Receipt.Reverse_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REVERSE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REVERSE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_RECEIPT_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("REV_REMIT_RECEIPT", param, commandType: CommandType.StoredProcedure);
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }
        }


        ////Approve Reverse Receipt
        //public void ApproveReverseReceiptRecord(Remit_ReceiptRepo Receipt)
        //{
        //    try
        //    {
        //        //Get Connection
        //        AppSettings app = new AppSettings();
        //        con = app.GetConnection();
        //        DynamicParameters param = new DynamicParameters();
        //        param.Add(name: "P_RECEIPT_ID", value: Receipt.Receipt_Id, dbType: DbType.String, direction: ParameterDirection.Input);
        //        param.Add(name: "P_REVERSE_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
        //        param.Add(name: "P_REVERSE_REASON", value: Receipt.Reverse_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
        //        param.Add(name: "P_REVERSE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
        //        param.Add(name: "P_REVERSE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        //        param.Add(name: "P_RECEIPT_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
        //        con.Execute("REV_REMIT_RECEIPT", param, commandType: CommandType.StoredProcedure);
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

        public bool isReceiptUnique(string RECEIPT_ID)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_RECEIPT_ID", RECEIPT_ID, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_REMIT_RECEIPT_EXIST", param, commandType: CommandType.StoredProcedure);
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

        public DataSet ReceiptData()
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

                cmd.CommandText = "SEL_REMIT_RECEIPT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "receipt");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_ReceiptRepo> GetReceiptList()
        {
            try
            {
                DataSet dt = ReceiptData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Receipt_Id = row.Field<string>("RECEIPT_ID"),
                    Trans_Amount = row.Field<Decimal>("TRANS_AMOUNT"),
                    Actual_Receipt_Date = row.Field<DateTime>("ACTUAL_RECEIPT_DATE"),
                    Narration = row.Field<string>("NARRATION"),
                    Narration_Syatem = row.Field<string>("NARRATION_SYSTEM"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Receipt_Status = row.Field<string>("RECEIPT_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
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

        // FOR PENDING RECEIPTS
        public DataSet ReceiptPendingData()
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

                cmd.CommandText = "SEL_REMIT_RECEIPT_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "receipt");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_ReceiptRepo> GetReceiptPendingList()
        {
            try
            {
                DataSet dt = ReceiptPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Receipt_Id = row.Field<string>("RECEIPT_ID"),
                    Trans_Amount = row.Field<Decimal>("TRANS_AMOUNT"),
                    Actual_Receipt_Date = row.Field<DateTime>("ACTUAL_RECEIPT_DATE"),
                    Narration = row.Field<string>("NARRATION"),
                    Narration_Syatem = row.Field<string>("NARRATION_SYSTEM"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Receipt_Status = row.Field<string>("RECEIPT_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // get list of employers
        public List<Remit_ReceiptRepo> GetEmployersList()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_ReceiptRepo> bn = new List<Remit_ReceiptRepo>();

                string query = "Select * from CRM_EMPLOYER";
                return bn = con.Query<Remit_ReceiptRepo>(query).ToList();

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

        // FOR ACTIVE RECEIPTS
        public List<Remit_ReceiptRepo> GetERList(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_ReceiptRepo> bn = new List<Remit_ReceiptRepo>();

                string query = "Select * from VW_RECEIPT_EMPLOYER WHERE EMPLOYER_ID = '" + Employer_Id + "' and  (ACTUAL_RECEIPT_DATE between '" + From_Date.ToString("dd-MMM-yyyy") + "' and '" + To_Date.ToString("dd-MMM-yyyy") + "')   AND RECEIPT_STATUS = 'ACTIVE'";
                return bn = con.Query<Remit_ReceiptRepo>(query).ToList();

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

        public DataSet ReceiptActiveData()
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

                cmd.CommandText = "SEL_REMIT_RECEIPT_ACTIVE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "receipt");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_ReceiptRepo> GetReceiptActiveList()
        {
            try
            {
                DataSet dt = ReceiptActiveData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Receipt_Id = row.Field<string>("RECEIPT_ID"),
                    Trans_Amount = row.Field<Decimal>("TRANS_AMOUNT"),
                    Actual_Receipt_Date = row.Field<DateTime>("ACTUAL_RECEIPT_DATE"),
                    Narration = row.Field<string>("NARRATION"),
                    Narration_Syatem = row.Field<string>("NARRATION_SYSTEM"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Receipt_Status = row.Field<string>("RECEIPT_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // FOR PENDING REVERSED RECEIPTS
        public DataSet ReverseReceiptData()
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

                cmd.CommandText = "SEL_REMIT_RECEIPT_R_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "receipt");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_ReceiptRepo> GetReverseReceiptList()
        {
            try
            {
                DataSet dt = ReverseReceiptData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Receipt_Id = row.Field<string>("RECEIPT_ID"),
                    Trans_Amount = row.Field<Decimal>("TRANS_AMOUNT"),
                    Actual_Receipt_Date = row.Field<DateTime>("ACTUAL_RECEIPT_DATE"),
                    Narration = row.Field<string>("NARRATION"),
                    Narration_Syatem = row.Field<string>("NARRATION_SYSTEM"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Receipt_Status = row.Field<string>("RECEIPT_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public DataSet ReceiptESData()
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

                cmd.CommandText = "SEL_REMIT_RECEIPT_EMPLOYER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "receipt");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Remit_ReceiptRepo> GetReceiptESList()
        {
            try
            {
                DataSet dt = ReceiptESData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Cash_Balance = row.Field<decimal>("CASH_BALANCE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IEnumerable<Remit_ReceiptRepo> GetReceiptESList2()
        {
            try
            {
                DataSet dt = ReceiptESData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id_2 = row.Field<string>("SCHEME_ID"),
                    Scheme_Name_2 = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Cash_Balance = row.Field<decimal>("CASH_BALANCE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        public List<Remit_ReceiptRepo> GetReceiptESListBTE(Remit_ReceiptRepo rr)
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Remit_ReceiptRepo> ObjFund = new List<Remit_ReceiptRepo>();

                return ObjFund = db.GetConnection().Query<Remit_ReceiptRepo>("Select * from VW_EMPLOYER_ES where scheme_id = '" + rr.Scheme_Id + "'").ToList();
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

        //public List<Remit_ReceiptRepo> GetReceiptESListBTE(Remit_ReceiptRepo rr)
        //{
        //    try
        //    {
        //        //Get connection
        //        var app = new AppSettings();
        //        con = app.GetConnection();

        //        DataSet ds = new DataSet();

        //        OracleDataAdapter da = new OracleDataAdapter();
        //        OracleCommand cmd = new OracleCommand();


        //        cmd.CommandText = "SEL_REMIT_BTE_EMPLOYER";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = (OracleConnection)con;

        //        //Input param
        //        OracleParameter paramemployer_Id = new OracleParameter("DID", OracleDbType.Varchar2, rr.Scheme_Id, ParameterDirection.Input);
        //        cmd.Parameters.Add(paramemployer_Id);
        //        OracleParameter param2 = new OracleParameter("cur", OracleDbType.RefCursor, ParameterDirection.Output);
        //        cmd.Parameters.Add(param2);

        //        da = new OracleDataAdapter(cmd);
        //        da.Fill(ds, "employee");


        //        var eList = ds.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
        //        {
        //            ES_Id = row.Field<string>("ES_ID"),
        //            Scheme_Id_2 = row.Field<string>("SCHEME_ID"),
        //            Scheme_Name_2 = row.Field<string>("SCHEME_NAME"),
        //            Employer_Id = row.Field<string>("EMPLOYER_ID"),
        //            Employer_Name = row.Field<string>("EMPLOYER_NAME"),
        //            Cash_Balance = row.Field<decimal>("CASH_BALANCE")

        //        }).ToList();

        //        return eList;

        //    }
        //    catch (Exception EX)
        //    {

        //        throw EX;
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
        public decimal getConBalance(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();             
                param.Add("P_ES_ID", ReceiptRepo.ES_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_CON_TOTAL", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_CON_TOTAL", param, commandType: CommandType.StoredProcedure);
                ReceiptRepo.Con_Total = param.Get<decimal>("P_CON_TOTAL");
                return this.Con_Total;              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        // FOR PENDING RECEIPTS
        public List<Remit_ReceiptRepo> GetERPENDList(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_ReceiptRepo> bn = new List<Remit_ReceiptRepo>();

                string query = "Select * from VW_RECEIPT_EMPLOYER WHERE EMPLOYER_ID = '" + Receipt_Id + "'   AND RECEIPT_STATUS = 'PENDING'";
                return bn = con.Query<Remit_ReceiptRepo>(query).ToList();

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


        // FOR PENDING RECEIPTS
        public List<Remit_ReceiptRepo> GetEMPLOYERList(Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_ReceiptRepo> bn = new List<Remit_ReceiptRepo>();

                string query = "Select * from crm_EMPLOYER";
                return bn = con.Query<Remit_ReceiptRepo>(query).ToList();

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
}
