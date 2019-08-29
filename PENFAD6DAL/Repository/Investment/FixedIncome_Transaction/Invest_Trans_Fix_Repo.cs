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

namespace PENFAD6DAL.Repository.Investment.FixedIncome_Transaction
{
    public class Invest_Trans_Fix_Repo
    {   
        public string Invest_No { get; set; }
        public string Invest_Description { get; set; }
        [Required]
        public string Scheme_Fund_Id { get; set; }
        public string Fund_Id { get; set; }
        public string Scheme_Id { get; set; }
        public string Fund_Name { get; set; }
        public string Scheme_Name { get; set; }
       [Required]
        public decimal GL_Balance { get; set; }
        
        public string Class_Id { get; set; }
        public string Description { get; set; }
        [Required]
        public string Fund_Manager_Id { get; set; }
        
        public string Fund_Manager { get; set; }
        [Required]
        public string Product_Id { get; set; }
        public string Product_Name { get; set; }
       
        public string Invest_Status { get; set; }
      
        public string Trans_Type { get; set; }
        public string Account_Name { get; set; }
        public string GL_Account_Name { get; set; }
        [Required]
        public string GL_Account_No { get; set; }
        public string Credit_Account_No { get; set; }
        public string Maker_Id { get; set; }

        public DateTime Make_Date { get; set; }

        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public DateTime Auth_Date { get; set; }

        public string Update_Id { get; set; }
        public DateTime Update_Date { get; set; }
        public string Authorizer { get; set; }
        public string Approve_Equity { get; set; }
       // [Required]
        public string User_Id { get; set; }
        public DateTime Scheme_Working_Date { get; set; }
        [Required]
        public string Contract_No { get; set; }
        [Required]
        [Range(1, 9999999999999999, ErrorMessage = "Invalid Amount Invested")]
        public decimal Amount_Invested { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "Invalid Duration")]
        public decimal Duration_In_Days { get; set; }
        [Required]
        [Range(0.001, 100, ErrorMessage = "Invalid Annual Interest Rate")]
        public decimal Annual_Int_Rate { get; set; }
        public decimal Daily_Int_Rate { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Maturity_Date { get; set; }
        public DateTime Settlement_Date { get; set; }
        public decimal Expected_Int { get; set; }
        [Required]
        public string Issuer_Id { get; set; }
        public string Issuer_Name { get; set; }
        [Required]
        public decimal Interest_Day_Basic { get; set; }
        public string Rollover_Yes_No { get; set; }
        public string Rollover_Instrution { get; set; }
        public decimal Principal_Bal { get; set; }
        public decimal Interest_Bal { get; set; }
        public decimal Interest_Accrued { get; set; }
        public decimal Principal_Paid { get; set; }
        public decimal Interest_Paid { get; set; }
        public decimal Avaliable_Balance { get; set; }
        public DateTime Last_Date_Int_Accrued { get; set; }
        public DateTime Actual_Date_Redemption { get; set; }
        public string NeworBF_Status { get; set; }
        public decimal Amount_on_Maturity { get; set; }
        public decimal Maturity_Value { get; set; }
        public decimal Interest_On_Maturity { get; set; }
        public string Interest_Type { get; set; }
        public string Security_Type { get; set; }
        public string Trans_CuCode { get; set; }
        public string Base_CusCode { get; set; }
        public string Reason_For_Editing { get; set; }
        public decimal BF_ACCRUED_INTEREST { get; set; }
        public string Payment_Confirmed { get; set; }
        public string Full_Name { get; set; }
        public string Updated_Id { get; set; }
        public DateTime Updated_Date { get; set; }
        public decimal Periodrate_Customer { get; set; }
        public decimal Total_Cost { get; set; }
        public string Invest_Type { get; set; }
        public DateTime? Receipt_Date { get; set; }
        [Range(0, 9999999999999999, ErrorMessage = "Invalid Principal  Amount")]
        public decimal Principal_Amount { get; set; }
        [Range(0, 9999999999999999, ErrorMessage = "Invalid Interest Amount")]
        public decimal Interest_Amount { get; set; }
        public decimal Interest_Payable_Bal { get; set; }
        public string TID { get; set; }
        public string Payment_Mode { get; set; }
        public string Instrument_No { get; set; }
        public DateTime Payment_Date { get; set; }
        public DateTime From_Date { get; set; }
        public DateTime To_Date { get; set; }
        public decimal CH_NUMBER { get; set; }

        public decimal CHECK_COM { get; set; }

        IDbConnection conn;

        public bool Add_Submit_Trans(Invest_Trans_Fix_Repo MM_TBill)

        {
            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);
            try
            {
                //Get connectoin
                var app = new AppSettings();
                conn = app.GetConnection();
                var param = new DynamicParameters();

                string NKRollOver = "DO NOT ROLLOVER";
           
                
                //string gl_status_new = "PENDING".ToString().ToUpper();
                param.Add(name: "p_Invest_No", value: MM_TBill.Product_Id + MM_TBill.Scheme_Fund_Id + MM_TBill.Start_Date.ToString("ddMMyy"), dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Contract_No", value: MM_TBill.Contract_No, dbType: DbType.String , direction: ParameterDirection.Input);
                param.Add(name: "p_SF_Id", value: MM_TBill.Scheme_Fund_Id , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Class_Id", value: MM_TBill.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Product_Id", value: MM_TBill.Product_Id, dbType: DbType.String , direction: ParameterDirection.Input);
                param.Add(name: "p_Amount_Invested", value: MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Duration_In_Days", value: MM_TBill.Duration_In_Days , dbType: DbType.Decimal , direction: ParameterDirection.Input);
                param.Add(name: "p_Annual_Int_Rate", value: MM_TBill.Annual_Int_Rate , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Daily_Int_Rate", value: MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Start_Date", value: MM_TBill.Start_Date , dbType: DbType.DateTime , direction: ParameterDirection.Input);
                param.Add(name: "p_Maturity_Date", value: MM_TBill.Maturity_Date , dbType: DbType.DateTime , direction: ParameterDirection.Input);
                param.Add(name: "p_Settlement_Date", value: MM_TBill.Settlement_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Expected_Int", value: MM_TBill.Interest_On_Maturity, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Issur_Id", value: MM_TBill.Issuer_Id , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Day_Basic", value: MM_TBill.Interest_Day_Basic, dbType: DbType.Decimal , direction: ParameterDirection.Input);
                param.Add(name: "p_ROLLOVER_INST_YES_NO ", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Rollover_Instrution", value: NKRollOver, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Principal_Bal", value: MM_TBill.Amount_Invested , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Bal", value: MM_TBill.Interest_On_Maturity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Principal_Paid", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Paid", value:0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Last_Date_Int_Accrued", value: MM_TBill.Start_Date/*.AddDays(-1)*/, dbType: DbType.DateTime , direction: ParameterDirection.Input);
                param.Add(name: "p_NeworBF_Status", value: "NEW", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Payment_Confirmed", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Amount_on_Maturity", value: MM_TBill.Amount_on_Maturity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Type", value: MM_TBill.Interest_Type , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trans_CuCode", value: 1 , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Base_CusCode", value: 1, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Invest_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Id", value: MM_TBill.Auth_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Fund_Manager_Id", value: MM_TBill.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_GL_Account_No", value: MM_TBill.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Cost", value: MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Bond_Type", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Bond_Class", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                int result = conn.Execute(sql: "ADD_INVEST_TRANS_FIX", param: param, commandType: CommandType.StoredProcedure);
                ts.Complete();

                if (result != 0)
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
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    if (conn != null) { conn = null; }
                }
            }

        }
      

        //CALL INVESTMENT PENDING RECORD
        public List<Invest_Trans_Fix_Repo> GetINVESTMENTRECORDList(string Invest_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from INVEST_TRANS_FIX WHERE INVEST_NO = '" + Invest_No + "'";
                return bn = conn.Query< Invest_Trans_Fix_Repo > (query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }

        }

        // approve investment
        public bool Approve_MM_TBill(Invest_Trans_Fix_Repo MM_TBill)
        {
            var app = new AppSettings();

            // get the pending record
            MM_TBill.GetINVESTMENTRECORDList(Invest_No);


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    // update INVEST_TRANS_FIX table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: MM_TBill.Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_INVEST_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("APP_INVEST_TRANS_FIXAPPROVAL", param_E, commandType: CommandType.StoredProcedure);
                
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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        // Reverse investment
        public bool Reverse_MM_TBill(Invest_Trans_Fix_Repo MM_TBill)
        {
            var app = new AppSettings();

            // get the pending record
            MM_TBill.GetINVESTMENTRECORDList(Invest_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    // update INVEST_TRANS_FIX table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: MM_TBill.Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_INVEST_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("REV_INVEST_TRANS_FIXAPPROVAL", param_E, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }

        //confirm payment - DISINVEST
        public bool Receipt_MM_TBill_Dis(Invest_Trans_Fix_Repo MM_TBill)
        {
            var app = new AppSettings();

            // get the pending record
            MM_TBill.GetINVESTMENTRECORDList(Invest_No);


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    //log investment payment
                    DynamicParameters param_EA = new DynamicParameters();
                    param_EA.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_PAY_INVEST_EQUITY_CIS", param_EA, commandType: CommandType.StoredProcedure);


                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Accrued", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Paid", value: Interest_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Paid", value: Principal_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    conn.Execute("UPD_PAY_INVEST_DISINVEST", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "PAYMENT RECEIVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Rec_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Bill_Pay ", value: "PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Annual_Int_Rate", value: Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Daily_Int_Rate", value: Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    // param_B.Add(name: "p_Interest_Bal", value: , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Last_Date_Int_Accrued", value: Last_Date_Int_Accrued, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Payment_Mode", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Instrument_No", value: Instrument_No, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);


                    conn.Execute("UPD_FD_MM_STATEMENT_PAY", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }

        //confirm payment - interest mm/t-bill
        public bool IntReceipt_MM_TBill_Dis(Invest_Trans_Fix_Repo MM_TBill)
        {
            var app = new AppSettings();

            // get the pending record
            MM_TBill.GetINVESTMENTRECORDList(Invest_No);


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    //log investment payment
                    DynamicParameters param_EA = new DynamicParameters();
                    param_EA.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_PAY_INVEST_EQUITY_CIS", param_EA, commandType: CommandType.StoredProcedure);


                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Accrued", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Paid", value: Interest_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Paid", value: Principal_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    conn.Execute("UPD_PAY_INVEST_INTEREST_MM", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "PAYMENT RECEIVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Rec_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Bill_Pay ", value: "PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Annual_Int_Rate", value: Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Daily_Int_Rate", value: Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    // param_B.Add(name: "p_Interest_Bal", value: , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Last_Date_Int_Accrued", value: Last_Date_Int_Accrued, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Payment_Mode", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Instrument_No", value: Instrument_No, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);


                    conn.Execute("UPD_FD_MM_STATEMENT_PAY", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        //Adjust Accrued- interest mm/t-bill
        public bool AccruedReceipt_MM_TBill_Dis(Invest_Trans_Fix_Repo MM_TBill)
        {
            var app = new AppSettings();

            // get the pending record
            MM_TBill.GetINVESTMENTRECORDList(Invest_No);


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                

                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Accrued", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    
                    conn.Execute("UPD_ACC_INVEST_INTEREST_MM", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "ACCRUED ADJUSTED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Rec_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Bill_Pay ", value: "PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Annual_Int_Rate", value: Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Daily_Int_Rate", value: Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    // param_B.Add(name: "p_Interest_Bal", value: , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Last_Date_Int_Accrued", value: Last_Date_Int_Accrued, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Payment_Mode", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Instrument_No", value: Instrument_No, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);


                    conn.Execute("UPD_ACCAD_MM_STATEMENT_PAY", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        //confirm payment 
        public bool Receipt_MM_TBill(Invest_Trans_Fix_Repo MM_TBill)
        {
            var app = new AppSettings();

            // get the pending record
            MM_TBill.GetINVESTMENTRECORDList(Invest_No);
            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    //log investment payment
                    DynamicParameters param_EA = new DynamicParameters();
                    param_EA.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_PAY_INVEST_EQUITY_CIS", param_EA, commandType: CommandType.StoredProcedure);


                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Accrued", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Paid", value: Interest_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Paid", value: Principal_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_PAY_INVEST_EQUITY_CIS", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value:Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "PAYMENT RECEIVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Rec_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Bill_Pay ", value: "PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Annual_Int_Rate", value: Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Daily_Int_Rate", value: Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    // param_B.Add(name: "p_Interest_Bal", value: , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Last_Date_Int_Accrued", value: Last_Date_Int_Accrued, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Payment_Mode", value: Payment_Mode , dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Instrument_No", value: Instrument_No, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);


                    conn.Execute("UPD_FD_MM_STATEMENT_PAY", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        //reverse payment
        public bool Receipt_MM_TBill_Reverse(Invest_Trans_Fix_Repo MM_TBill)
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
                    //log investment payment
                    DynamicParameters param_EA = new DynamicParameters();
                    param_EA.Add(name: "P_INVEST_NO", value: TID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Receipt_Date", value: Payment_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_PAY_INVEST_REVERSE", param_EA, commandType: CommandType.StoredProcedure);


                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                     param_E.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_PAY_INVEST_REVERSE", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Bal - Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "PAYMENT RECEIVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Rec_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Bill_Pay ", value: "PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Annual_Int_Rate", value: Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Daily_Int_Rate", value: Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    // param_B.Add(name: "p_Interest_Bal", value: , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Last_Date_Int_Accrued", value: Last_Date_Int_Accrued, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Payment_Mode", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Instrument_No", value: Instrument_No, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Credit_Account_No", value: GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);


                    conn.Execute("UPD_FD_MM_STATEMENT_PAY_REV", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        // Disapprove Purchase
        public void DisapproveOrderRecord(string Invest_No)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                conn.Execute("DEL_INVEST_FIXEDTRANSACTION", param, commandType: CommandType.StoredProcedure);
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
                    if (conn != null) { conn = null; }
                }
            }
        }

        //GET GL BALANCE FROM GL ACCOUNT TABLE
        public void Get_GL_Balance(Invest_Trans_Fix_Repo  MM_TBILL)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_GL_ACCOUNT_NO", MM_TBILL.GL_Account_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_BALANCE", param, commandType: CommandType.StoredProcedure);
                MM_TBILL.GL_Balance = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        ////GET CURRENT PRODUCT QUANTITY FROM EQUITY BALANCE TABLE TABLE
        //public void Get_Product_Current_Quantity(Invest_Trans_Fix_Repo MM_TBIll)
        //{
        //    try
        //    {
        //        //Get connection
        //        var con = new AppSettings();
        //        var param = new DynamicParameters();
        //        param.Add("P_SCHEME_FUND_ID", MM_TBIll.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
        //        param.Add("P_PRODUCT_ID", MM_TBIll.Product_Id, DbType.String, ParameterDirection.Input);
        //        param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
        //        con.GetConnection().Execute("SEL_CURRENT_PRODUCT_QUANTITY", param, commandType: CommandType.StoredProcedure);
        //        MM_TBIll.Current_Quantity = param.Get<decimal>("VDATA");

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}


        public bool isOrderUnique(string Invest_No)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_INVEST_NO", Invest_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_INVEST_MM_TBILL_EXIST", param, commandType: CommandType.StoredProcedure);
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

       
        // LIST FOR PENDING Money market / T_Bill
        //public DataSet MM_TBillPendingData()
        //{
        //    try
        //    {
        //        //Get connection
        //        var app = new AppSettings();
        //        conn = app.GetConnection();

        //        DataSet ds = new DataSet();

        //        OracleDataAdapter da = new OracleDataAdapter();
        //        OracleCommand cmd = new OracleCommand();
        //        OracleParameter param = cmd.CreateParameter();

        //        cmd.CommandText = "SEL_INVEST_TRANS_BY_PENDING";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = (OracleConnection)conn;

        //        param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
        //        param.Direction = ParameterDirection.Output;

        //        da = new OracleDataAdapter(cmd);
        //        da.Fill(ds, "ecis");
        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public IEnumerable<Invest_Trans_Fix_Repo> MM_TBillPendingList()
        //{
        //    try
        //    {
        //        DataSet dt = MM_TBillPendingData();
        //        var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_Trans_Fix_Repo
        //        {
        //            Invest_No = row.Field<string>("INVEST_NO"),
        //            Scheme_Id = row.Field<string>("SCHEME_ID"),
        //            Scheme_Name = row.Field<string>("SCHEME_NAME"),
        //            Fund_Id = row.Field<string>("FUND_ID"),
        //            Fund_Name = row.Field<string>("FUND_NAME"),
        //            Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
        //            Invest_Description = row.Field<string>("INVEST_DESCRIPTION"),
        //            Product_Id = row.Field<string>("PRODUCT_ID"),
        //            Product_Name = row.Field<string>("PRODUCT_NAME"),
        //            Fund_Manager = row.Field<string>("FUND_MANAGER"),
        //            Fund_Manager_Id = row.Field<string>("FUND_MANAGER_ID"),                    
        //            GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
        //            Contract_No = row.Field<string>("CONTRACT_NO"),
        //            Amount_Invested = row.Field<decimal>("AMOUNT_INVESTED"),
        //            Duration_In_Days = row.Field<decimal>("DURATION_IN_DAYS"),
        //            Annual_Int_Rate = row.Field<decimal>("ANNUAL_INT_RATE"),
        //            Daily_Int_Rate = row.Field<decimal>("Daily_Int_Rate"),
        //            Start_Date = Convert.ToDateTime(row.Field<DateTime>("Start_Date")),
        //            Maturity_Date = row.Field<DateTime>("Maturity_Date"),
        //            Expected_Int = row.Field<decimal>("EXPECTED_INT"),
        //            Issuer_Name = row.Field<string>("Issuer_Name"),
        //            Interest_Day_Basic = row.Field<decimal>("Interest_Day_Basic"),
        //            Amount_on_Maturity = row.Field<decimal>("AMOUNT_ON_MATURITY"),
        //            Interest_Type = row.Field<string>("INTEREST_TYPE"),
        //            //Trans_CuCode = row.Field<string>("TRANS_CUCODE"),
        //            Invest_Status = row.Field<string>("INVEST_STATUS"),
        //            Avaliable_Balance = Convert.ToDecimal(row.Field<decimal>("GL_Balance").ToString()),
        //            Auth_Id = row.Field<string>("Auth_Id"),
        //            Full_Name = row.Field<string>("FULLNAME"),
        //            Security_Type = row.Field<string>("DESCRIPTION")

        //        }).ToList();

        //        return eList;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}


        //FILTERING FUND MANAGER LIST FOR SCHEME
        public List<Invest_Trans_Fix_Repo> GetFMList(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from VW_SCHEME_FUND_MANAGER WHERE SCHEME_ID = '" + Scheme_Id + "'";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }


        //FILTERING GL ACOUNT LIST FOR SCHEME_FUND
        public List<Invest_Trans_Fix_Repo> GetGLASFList(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from GL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' AND MEMO_CODE = '" + "101" + "' AND REC_STATUS = '" + "ACTIVE" + "' and GL_DEFAULT = '" + "NO" + "'  or SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' AND MEMO_CODE = '" + "108" + "' AND REC_STATUS = '" + "ACTIVE" + "' ";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }

        //FILTERING GL ACOUNT LIST FOR SCHEME_FUND
        public List<Invest_Trans_Fix_Repo> GetGLASFListGL(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from GL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "'  AND REC_STATUS = '" + "ACTIVE" + "' ";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }


        //FILTERING GL ACOUNT LIST FOR SCHEME - merged
        public List<Invest_Trans_Fix_Repo> GetGLASFListGLScheme(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from MERGE_GL_ACCOUNT WHERE SCHEME_ID = '" + Scheme_Id + "' ";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }


        //FILTERING GL_BALANCE FOR GL BANK ACCOUNT
        public List<Invest_Trans_Fix_Repo> GetGLAccFList(string GL_Account_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from GL_ACCOUNT WHERE GL_ACCOUNT_NO = '" + GL_Account_No + "' ";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }

        //FILTERING USER_ID FOR AUTHORIZER
        public List<Invest_Trans_Fix_Repo> GetuserAList()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from SEC_AUTHORIZER WHERE APPROVE_EQUITY = '" + "YES" + "' ";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }

        }
      
        //FILTERING PRODUCT FOR ASSET CLASS
        public List<Invest_Trans_Fix_Repo> GetECISPList(string Class_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from INVEST_PRODUCTS WHERE CLASS_ID = '" + Class_Id + "'";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }

        // get product list
        public List<Invest_Trans_Fix_Repo> GetSchemeFundProductList()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = conn.Query<Invest_Trans_Fix_Repo>("Select * from VW_EQUITY_CIS_PRODUCT_SF ").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }

        }

        public List<Invest_Trans_Fix_Repo> GetAssetList()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from INVEST_ASSET_CLASS where CLASS_ID = '01' OR CLASS_ID = '02' ").ToList();
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

        public List<Invest_Trans_Fix_Repo> GetFMList()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from PFM_FUND_MANAGER  ").ToList();
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


//get pending Money market
        public List<Invest_Trans_Fix_Repo> GetInvest_FixedIncome_List()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'PENDING' AND INVEST_STATUS = 'PENDING' and class_id = '01'" ).ToList();
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


        //get pending T_Bill
        public List<Invest_Trans_Fix_Repo> GetInvest_TBill_List()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'PENDING' AND INVEST_STATUS = 'PENDING' and class_id = '02'").ToList();
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

        //get active Money market
        public List<Invest_Trans_Fix_Repo> GetInvest_FixedIncome_List_A()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where  INVEST_STATUS = 'ACTIVE' and class_id = '01'").ToList();
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


        //get active T_Bill
        public List<Invest_Trans_Fix_Repo> GetInvest_TBill_List_A()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where INVEST_STATUS = 'ACTIVE' and class_id = '02'").ToList();
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

        //get active Money market for reversal
        public List<Invest_Trans_Fix_Repo> GetInvest_FixedIncome_List_Re()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where  INVEST_STATUS = 'ACTIVE' and class_id = '01' and interest_paid <=0 and principal_paid <= 0 and interest_accrued <= 0 ").ToList();
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


        //get active T_Bill
        public List<Invest_Trans_Fix_Repo> GetInvest_TBill_List_Re()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where INVEST_STATUS = 'ACTIVE' and class_id = '02' and interest_paid <=0 and principal_paid <= 0 and interest_accrued <= 0").ToList();
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


        //get active Money market FOR PAYEMENT
        public List<Invest_Trans_Fix_Repo> GetInvest_FixedIncome_List_PAY()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where  INVEST_STATUS = 'ACTIVE' and class_id = '01' AND PRINCIPAL_BAL > 0 ").ToList();
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

         //get active Money market FOR PAYEMENT
        public List<Invest_Trans_Fix_Repo> GetInvest_FixedIncome_List_PAYA1()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where  INVEST_STATUS != 'REVERSED' and class_id = '01' AND PRINCIPAL_BAL > 0 ").ToList();
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

        //get active T_Bill FOR PAYEMENT
        public List<Invest_Trans_Fix_Repo> GetInvest_TBill_List_PAY()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where INVEST_STATUS = 'ACTIVE' and class_id = '02' AND PRINCIPAL_BAL > 0").ToList();
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

        //get active T_Bill FOR ADJUST
        public List<Invest_Trans_Fix_Repo> GetInvest_TBill_List_PAYA1()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where INVEST_STATUS != 'REVERSED' and class_id = '02' AND PRINCIPAL_BAL > 0").ToList();
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

        //get matured Money market FOR PAYEMENT
        public List<Invest_Trans_Fix_Repo> MGetInvest_FixedIncome_List_PAY()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where  INVEST_STATUS = 'MATURED' and class_id = '01' AND PRINCIPAL_BAL > 0").ToList();
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

        //get receipt Money market FOR reversal
        public List<Invest_Trans_Fix_Repo> MGetInvest_FixedIncome_List_reverse(Invest_Trans_Fix_Repo MM_TBILL)
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL_PAYMENT  where  REC_STATUS = 'ACTIVE' and class_id = '01' and (Payment_Date  between '" + MM_TBILL.From_Date.ToString("dd-MMM-yyyy") + "' AND '" + MM_TBILL.To_Date.ToString("dd-MMM-yyyy") + "') ").ToList();
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


        //get matured T_Bill FOR PAYEMENT
        public List<Invest_Trans_Fix_Repo> MGetInvest_TBill_List_PAY()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL  where INVEST_STATUS = 'MATURED' and class_id = '02' AND PRINCIPAL_BAL > 0").ToList();
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

        //get paid T_Bill FOR reversal 
        public List<Invest_Trans_Fix_Repo> MGetInvest_TBill_List_reverse(Invest_Trans_Fix_Repo MM_TBILL)
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL_PAYMENT  where REC_STATUS = 'ACTIVE' and class_id = '02'  and (Payment_Date  between  '" + MM_TBILL.From_Date.ToString("dd-MMM-yyyy") + "' AND '" + MM_TBILL.To_Date.ToString("dd-MMM-yyyy") + "' )").ToList();
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

        //get paid Bonds FOR reversal
        public List<Invest_Trans_Fix_Repo> ReverseReadPendingBonds_Transaction(Invest_Trans_Fix_Repo MM_TBILL)
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL_PAYMENT  where REC_STATUS = 'ACTIVE' and class_id = '03'  and (Payment_Date  between  '" + MM_TBILL.From_Date.ToString("dd-MMM-yyyy") + "' AND '" + MM_TBILL.To_Date.ToString("dd-MMM-yyyy") + "' )").ToList();
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

        //get SALE FOR REVERSAL
        public List<Invest_Trans_Fix_Repo> ReverseReadPendingBonds_TransactionSell(Invest_Trans_Fix_Repo MM_TBILL)
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Trans_Fix_Repo> ObjFund = new List<Invest_Trans_Fix_Repo>();

                return ObjFund = db.GetConnection().Query<Invest_Trans_Fix_Repo>("Select * from VW_MM_TBILL_SALE  where REC_STATUS = 'ACTIVE' and class_id = '03'  and (Payment_Date  between  '" + MM_TBILL.From_Date.ToString("dd-MMM-yyyy") + "' AND '" + MM_TBILL.To_Date.ToString("dd-MMM-yyyy") + "' )").ToList();
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
        public List<Invest_Trans_Fix_Repo> GetFMRecord(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from VW_MM_TBILL WHERE INVEST_NO = '" + Invest_No + "'";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }

        public List<Invest_Trans_Fix_Repo> GetFMRecord_Pay(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Trans_Fix_Repo> bn = new List<Invest_Trans_Fix_Repo>();

                string query = "Select * from VW_MM_TBILL_PAYMENT WHERE TID = '" + TID + "'";
                return bn = conn.Query<Invest_Trans_Fix_Repo>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Dispose();
            }
        }



        //check issuer id exist
        public void ck_issuer(Invest_Trans_Fix_Repo Equity_CISRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ISSUERID", Equity_CISRepo.Issuer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("CK_ISSUER", param, commandType: CommandType.StoredProcedure);
                Equity_CISRepo.CH_NUMBER = param.Get<Decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //reverse payment
        public bool Sale_MM_TBill_Reverse(Invest_Trans_Fix_Repo MM_TBill)
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
                    //log investment payment
                    DynamicParameters param_EA = new DynamicParameters();
                    param_EA.Add(name: "P_INVEST_NO", value: TID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Receipt_Date", value: Payment_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Credit_Account_No", value: Credit_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_EA.Add(name: "P_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_PAY_INVEST_REVERSE", param_EA, commandType: CommandType.StoredProcedure);


                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Amount", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_PAY_INVEST_REVERSE", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Bal - Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "PAYMENT RECEIVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Rec_status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Bill_Pay ", value: "PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Annual_Int_Rate", value: Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Daily_Int_Rate", value: Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    // param_B.Add(name: "p_Interest_Bal", value: , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Last_Date_Int_Accrued", value: Last_Date_Int_Accrued, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Payment_Mode", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Instrument_No", value: Instrument_No, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Credit_Account_No", value: GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);


                    conn.Execute("UPD_FD_MM_STATEMENT_PAY_SALE", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }

        }


        //check for investment compliance
        public void check_compliance_inv(Invest_Trans_Fix_Repo MM_TBILL)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SFID", MM_TBILL.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_PID", MM_TBILL.Product_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_AMOUNT", MM_TBILL.Amount_Invested, DbType.Decimal, ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("CHECK_COMPLIANCE_INV", param, commandType: CommandType.StoredProcedure);
                MM_TBILL.CHECK_COM = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
