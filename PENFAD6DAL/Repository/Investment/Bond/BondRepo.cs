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

namespace PENFAD6DAL.Repository.Investment.Bond
{
    public class BondRepo
    {
        AppSettings db = new AppSettings();
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
        public string GL_Account_Name { get; set; }
        [Required]
        public string GL_Account_No { get; set; }
        public string Maker_Id { get; set; }

        public DateTime Make_Date { get; set; }

        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public DateTime Auth_Date { get; set; }

        public string Update_Id { get; set; }
        public DateTime Update_Date { get; set; }
        public string Authorizer { get; set; }
        public string Approve_Equity { get; set; }
        //[Required]
        public string User_Id { get; set; }
        public DateTime Scheme_Working_Date { get; set; }
       // [Required]
        public string Contract_No { get; set; }
        [Required]
        public decimal Amount_Invested { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "Invalid Duration")]
        public decimal Duration_In_Days { get; set; }
        [Required]
        [Range(0.001, 100, ErrorMessage = "Invalid Annual Coupon Rate")]
        public decimal Annual_Int_Rate { get; set; }
        public decimal Daily_Int_Rate { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Maturity_Date { get; set; }
        public decimal Expected_Int { get; set; }
        [Required]
        public string Issuer_Id { get; set; }
        public string Issuer_Name { get; set; }
        [Required]
        public decimal Interest_Day_Basic { get; set; }
        public string Rollover_YesNo { get; set; }
        public string Rollover_Instrution { get; set; }
        public decimal Principal_Bal { get; set; }
        public decimal Interest_Bal { get; set; }
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
        public decimal Cost { get; set; }
        public string Bond_Class { get; set; }
        public decimal Bond_Qutn { get; set; }
        public DateTime Last_Coupon_Payment_Date { get; set; }
        public DateTime Next_Coupon_Date_Aftertrade { get; set; }
        public DateTime Settlement_Date { get; set; }
        public decimal Trans_Duration { get; set; }
        public decimal Base_Norminal_Value { get; set; }
        public DateTime? Receipt_Date { get; set; }
        [Range(0, 9999999999999999, ErrorMessage = "Invalid Principal  Amount")]
        public decimal Principal_Amount { get; set; }
        [Range(0, 9999999999999999, ErrorMessage = "Invalid Interest Amount")]
        public decimal Interest_Amount { get; set; }
        public string Credit_Account_No { get; set; }
        public string Payment_Mode { get; set; }
        public string Instrument_No { get; set; }
        public decimal Interest_Accrued { get; set; }

        public decimal Interest_Payable_Bal { get; set; }
        public decimal Current_Yield { get; set; }
        [Range(0, 9999999999999999, ErrorMessage = "Invalid Brokerage Fee")]
        public decimal Brokerage_Fee { get; set; }
        public decimal Current_Year { get; set; }
        public decimal Carrying_Value_Year { get; set; }
        public decimal Interest_Year { get; set; }
        public decimal No_Days_Amortised { get; set; }
        public decimal Amortised_Cost_Year { get; set; }
        public decimal Amortised_Cost_Daily { get; set; }
        public DateTime? Reset_Amortised_Date { get; set; }
        public decimal CH_NUMBER { get; set; }

        IDbConnection conn;

        public bool Add_Submit_Trans(BondRepo MM_TBill)

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
                param.Add(name: "p_Expected_Int", value: MM_TBill.Interest_On_Maturity, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Settlement_Date", value: MM_TBill.Settlement_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
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
                param.Add(name: "p_Bond_Type", value: "PRIMARY", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Bond_Class", value: MM_TBill.Bond_Class, dbType: DbType.String, direction: ParameterDirection.Input);
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
      
         public bool Add_Submit_Trans_Sec(BondRepo MM_TBill)

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
                param.Add(name: "p_Expected_Int", value: MM_TBill.Interest_On_Maturity, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Issur_Id", value: MM_TBill.Issuer_Id , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Day_Basic", value: MM_TBill.Interest_Day_Basic, dbType: DbType.Decimal , direction: ParameterDirection.Input);
                param.Add(name: "p_ROLLOVER_INST_YES_NO ", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Rollover_Instrution", value: NKRollOver, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Principal_Bal", value: MM_TBill.Amount_Invested , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Bal", value: MM_TBill.Interest_On_Maturity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Principal_Paid", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Interest_Paid", value:0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Last_Date_Int_Accrued", value: MM_TBill.Last_Coupon_Payment_Date, dbType: DbType.DateTime , direction: ParameterDirection.Input);
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
                param.Add(name: "p_Cost", value: MM_TBill.Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Brokerage_Fee", value: MM_TBill.Brokerage_Fee, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Bond_Type", value: "SECONDARY", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Bond_Class", value: MM_TBill.Bond_Class, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "p_Current_Yield", value: MM_TBill.Current_Yield, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Bond_Qutn", value: MM_TBill.Bond_Qutn, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Settlement_Date", value: MM_TBill.Settlement_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_Next_Coupon_Date", value: MM_TBill.Next_Coupon_Date_Aftertrade, dbType: DbType.Date, direction: ParameterDirection.Input);

                param.Add(name: "p_Trans_Duration", value: MM_TBill.Trans_Duration, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                int result = conn.Execute(sql: "ADD_INVEST_TRANS_FIX_SEC", param: param, commandType: CommandType.StoredProcedure);
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
        public List<BondRepo> GetINVESTMENTRECORDList(string Invest_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from INVEST_TRANS_FIX WHERE INVEST_NO = '" + Invest_No + "'";
                return bn = conn.Query<BondRepo> (query).ToList();

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
        public bool Approve_MM_TBill(BondRepo MM_TBill)
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

                    ////Update Invest_Fixed_Inc_Statement table
                    //DynamicParameters paramb = new DynamicParameters();
                    //paramb.Add(name: "P_SCHEME_FUND_ID", value: MM_TBill.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "P_PRODUCT_ID", value: MM_TBill.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Invest_No", value: MM_TBill.Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Debit", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Credit", value: MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Balance", value: MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Princial_Bal", value: MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Interest_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Principal_Amt", value: MM_TBill.Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Interest_Amt", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Value_Date", value: MM_TBill.Start_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Narration", value: "INVESTMENT STARTED", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Rec_Status", value: "INITIAL", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Bill_Pay", value: "BILL", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Annual_Int_Rate", value: MM_TBill.Annual_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Daily_Rate", value: MM_TBill.Daily_Int_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Auth_Id", value: "GlobalValue.User_ID", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Pay_Type", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Pay_Type_Ref", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Last_Int_Accured_Date", value: MM_TBill.Start_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Rollover_Trans", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Payment_Mode", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Cheque_No", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                    //paramb.Add(name: "p_Reversal_Trans", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                    //conn.Execute("ADD_INVEST_FIXED_INC_STATEMENT", paramb, commandType: CommandType.StoredProcedure);

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
        public bool Reverse_MM_TBill(BondRepo MM_TBill)
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
        public void Get_GL_Balance(BondRepo  MM_TBILL)
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

       
     
        //FILTERING FUND MANAGER LIST FOR SCHEME
        public List<BondRepo> GetFMList(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from VW_SCHEME_FUND_MANAGER WHERE SCHEME_ID = '" + Scheme_Id + "'";
                return bn = conn.Query<BondRepo>(query).ToList();

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
        public List<BondRepo> GetGLASFList(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from GL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' AND MEMO_CODE = '" + "101" + "' AND REC_STATUS = '" + "ACTIVE" + "' and gl_default ='" + "NO" + "' ";
                return bn = conn.Query<BondRepo>(query).ToList();

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

        //FILTERING COLLECTION ACCOUNT
        public List<BondRepo> GetGLASFListBTE(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from VW_COLL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' ";
                return bn = conn.Query<BondRepo>(query).ToList();

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
        public List<BondRepo> GetGLAccFList(string GL_Account_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from GL_ACCOUNT WHERE GL_ACCOUNT_NO = '" + GL_Account_No + "' ";
                return bn = conn.Query<BondRepo>(query).ToList();

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
        public List<BondRepo> GetuserAList()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from SEC_AUTHORIZER WHERE APPROVE_EQUITY = '" + "YES" + "' ";
                return bn = conn.Query<BondRepo>(query).ToList();

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
        public List<BondRepo> GetECISPList(string Class_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from INVEST_PRODUCTS WHERE CLASS_ID = '" + Class_Id + "'";
                return bn = conn.Query<BondRepo>(query).ToList();

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
        public List<BondRepo> GetSchemeFundProductList()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = conn.Query<BondRepo>("Select * from VW_EQUITY_CIS_PRODUCT_SF ").ToList();
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

        public List<BondRepo> GetAssetList()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from INVEST_ASSET_CLASS where CLASS_ID = '03'").ToList();
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

        public List<BondRepo> GetFMList()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from PFM_FUND_MANAGER  ").ToList();
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


//get peddending Bond
        public List<BondRepo> GetInvest_FixedIncome_List()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'PENDING' AND INVEST_STATUS = 'PENDING' and class_id = '03' and primary_or_secondary = 'PRIMARY' " ).ToList();
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

        //get Bond
        public List<BondRepo> GetInvest_FixedIncome_List_Pay()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'AUTHORIZED' AND INVEST_STATUS = 'ACTIVE' and class_id = '03' AND PRINCIPAL_BAL > 0 AND INTEREST_PAYABLE_BAL > 0").ToList();
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

        //get all active Bond
        public List<BondRepo> GetInvest_FixedIncome_List_Pay2()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'AUTHORIZED' AND INVEST_STATUS = 'ACTIVE' and class_id = '03' AND PRINCIPAL_BAL > 0 ").ToList();
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

        //get all active Bond FOR ADJUST ACCRUED
        public List<BondRepo> GetInvest_FixedIncome_List_Pay21()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'AUTHORIZED' AND INVEST_STATUS != 'RESERVED' and class_id = '03' AND PRINCIPAL_BAL > 0").ToList();
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


        //get MATURED Bond
        public List<BondRepo> GetInvest_FixedIncome_List_MPay()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'AUTHORIZED' AND INVEST_STATUS = 'MATURED' and class_id = '03' AND PRINCIPAL_BAL > 0").ToList();
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



        //get pending Bond
        public List<BondRepo> GetInvest_TBill_List()
        {
   
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'PENDING' AND INVEST_STATUS = 'PENDING' and class_id = '03' and primary_or_secondary = 'SECONDARY' and sec_buy_sell = 'BUY' ").ToList();
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

        //get new Bond
        public List<BondRepo> GetInvest_FixedIncome_List_Rev()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'AUTHORIZED' AND INVEST_STATUS = 'ACTIVE' and class_id = '03' and primary_or_secondary = 'PRIMARY'  and interest_paid <=0 and principal_paid <= 0 and interest_accrued <= 0 ").ToList();
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

        //get new Sec Bond
        public List<BondRepo> GetInvest_FixedIncome_List_SecRev()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = db.GetConnection().Query<BondRepo>("Select * from VW_MM_TBILL  where AUTH_STATUS = 'AUTHORIZED' AND INVEST_STATUS = 'ACTIVE' and class_id = '03' and primary_or_secondary = 'SECONDARY'  and interest_paid <=0 and principal_paid <= 0 and interest_accrued <= 0").ToList();
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


        public void GetBondClassList(BondRepo MM_TBill)
        {
            try
            {
                if (string.IsNullOrEmpty( MM_TBill.Product_Id))
                {
                }
                else
                {
                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_PRODUCT_ID", MM_TBill.Product_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_BONDCLASS", param, commandType: CommandType.StoredProcedure);
                    MM_TBill.Bond_Class = param.Get<string>("VDATA");
                }
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

        public List<BondRepo> GetFMRecord(BondRepo BondRepo)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> bn = new List<BondRepo>();

                string query = "Select * from invest_trans_fix WHERE invest_no = '" + Invest_No + "'";
                return bn = conn.Query<BondRepo>(query).ToList();

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

        //confirm payment
        public bool Receipt_Bond(BondRepo BondRepo)
        {
            var app = new AppSettings();

            // get the pending record
            BondRepo.GetINVESTMENTRECORDList(Invest_No);


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

                    conn.Execute("UPD_PAY_INVEST_BOND_INT", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
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

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Bal - Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

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
            //disinvest bond
            public bool DisReceipt_Bond(BondRepo BondRepo)
            {
                var app = new AppSettings();

                // get the pending record
                BondRepo.GetINVESTMENTRECORDList(Invest_No);


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

                        conn.Execute("UPD_PAY_INVEST_BOND_INT_DIS", param_E, commandType: CommandType.StoredProcedure);

                        //Update INVEST_FIXED_INC_STATEMENT table
                        //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                        DynamicParameters param_B = new DynamicParameters();
                        param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
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

                        param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                        param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                        param_B.Add(name: "p_Acc_Interest", value: Interest_Bal - Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

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

        // get bond list
        public List<BondRepo> GetSchemeFundProductListbond()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<BondRepo> ObjFund = new List<BondRepo>();

                return ObjFund = conn.Query<BondRepo>("Select * from VW_FOR_BOND_CLASS where class_id = '03' and invest_status = 'ACTIVE' AND PRINCIPAL_BAL >0 ").ToList();
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
        public void ck_issuer(BondRepo Equity_CISRepo)
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


        public bool Add_Submit_Trans_Sec_Sell(BondRepo MM_TBill)

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


                //string gl_status_new = "PENDING".ToString().ToUpper();
                param.Add(name: "p_Invest_No", value: MM_TBill.Product_Id + MM_TBill.Scheme_Fund_Id + MM_TBill.Start_Date.ToString("ddMMyy"), dbType: DbType.String, direction: ParameterDirection.Input);
                
                int result = conn.Execute(sql: "SELL_INVEST_TRANS_FIX_SEC", param: param, commandType: CommandType.StoredProcedure);
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


        //confirm payment
        public bool Receipt_Bond_Sell(BondRepo BondRepo)
        {
            var app = new AppSettings();

            // get the pending record
            BondRepo.GetINVESTMENTRECORDList(Invest_No);


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
                    conn.Execute("ADD_SALE_INVEST_EQUITY_CIS", param_EA, commandType: CommandType.StoredProcedure);


                    // update Invest_Trans_Fix table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Accrued", value: Interest_Accrued, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Paid", value: Interest_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Paid", value: Principal_Paid, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_Interest_Amount", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    conn.Execute("UPD_PAY_INVEST_BOND_INT", param_E, commandType: CommandType.StoredProcedure);

                    //Update INVEST_FIXED_INC_STATEMENT table
                    //UPDATE GL_ACCOUNT / GL_TRANSACTION TABLE
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "p_Invest_No", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_SF_Id", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Product_Id", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Class_Id", value: Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Debit", value: Principal_Amount + Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Principal_Bal", value: Principal_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Bal", value: Interest_Bal, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Amount_Invested", value: Amount_Invested, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Expected_Int", value: Expected_Int, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Receipt_Date", value: Receipt_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Narration", value: "BOND SALE", dbType: DbType.String, direction: ParameterDirection.Input);
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

                    param_B.Add(name: "p_Principal_Paid", value: Principal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Interest_Paid", value: Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "p_Acc_Interest", value: Interest_Bal - Interest_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                    conn.Execute("SELL_FD_MM_STATEMENT_PAY", param_B, commandType: CommandType.StoredProcedure);

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

    }
}
