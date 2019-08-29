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

namespace PENFAD6DAL.Repository.Investment.Equity_CIS
{
    public class Invest_Equity_CISRepo
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
        public string Product_Id { get; set; }
        public string Product_Name { get; set; }
        [Required]
        [Range(1, 100000000000, ErrorMessage = "Order quantity must be above 0.")]
        public decimal Order_Quantity { get; set; }
        [Required]
        //[Range(0.0001, 100000000000, ErrorMessage = "Unit Price must be above 0.")]
        public decimal Order_Unit_Price { get; set; }
    
        public decimal Consideration { get; set; }
        [Required]
        public DateTime Order_Date { get; set; }
        public string Invest_Status { get; set; }
      
        public string Trans_Type { get; set; }
        [Required]
        [Range(0, 100000000000, ErrorMessage = "Total Levies must be above 0.")]
        public decimal Total_Levies { get; set; }
        [Required]
        public decimal Total_Cost { get; set; }
      
        public string GL_Account_Name { get; set; }
        [Required]
        public string GL_Account_No { get; set; }
       // [Required]
        public string Issuer_Id { get; set; }
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
        public string Equity_Limit { get; set; }
        public string Issuer { get; set; }
        public decimal Current_Quantity { get; set; }
        public decimal CURRENTAVERAGEUNITCOST { get; set; }
        public DateTime Scheme_Working_Date { get; set; }
        public decimal Current_Total_Cost { get; set; }
        public decimal Gain_Loss { get; set; }

        public decimal CH_NUMBER { get; set; }

        IDbConnection conn;
        public void SaveRecord(Invest_Equity_CISRepo EquityCISRepo) 
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                DynamicParameters param = new DynamicParameters();            
                param.Add(name: "P_INVEST_NO", value: EquityCISRepo.Product_Id + EquityCISRepo.Scheme_Fund_Id + EquityCISRepo.Order_Date.ToString("ddMMyy"), dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_INVEST_DESCRIPTION", value: EquityCISRepo.Invest_Description, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: EquityCISRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PRODUCT_ID", value: EquityCISRepo.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FUND_MANAGER_ID", value: EquityCISRepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ISSUER_ID", value: EquityCISRepo.Issuer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ORDER_QUANTITY", value: EquityCISRepo.Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_ORDER_UNIT_PRICE", value: EquityCISRepo.Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CONSIDERATION", value: EquityCISRepo.Consideration, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_ORDER_DATE", value: EquityCISRepo.Order_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_INVEST_STATUS", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_TYPE", value: "BUY", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TOTAL_LEVIES", value: EquityCISRepo.Total_Levies, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_TOTAL_COST", value: EquityCISRepo.Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTHORIZER", value: EquityCISRepo.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NO", value: EquityCISRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EQUITY_CIS", value: EquityCISRepo.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CAUC", value: EquityCISRepo.CURRENTAVERAGEUNITCOST, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CTC", value: EquityCISRepo.Current_Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                //param.Add(name: "P_GL", value: EquityCISRepo.Gain_Loss, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                conn.Execute("ADD_INVEST_EQUITY_CIS", param, commandType: CommandType.StoredProcedure);
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

        public void sellSaveRecord(Invest_Equity_CISRepo EquityCISRepo)
        {
            try
            {
                //EquityCISRepo.Gain_Loss = EquityCISRepo.Total_Cost - EquityCISRepo.Current_Total_Cost;
                

                //Get Connection
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_INVEST_NO", value: EquityCISRepo.Product_Id  + EquityCISRepo.Scheme_Fund_Id + EquityCISRepo.Order_Date.ToString("ddMMyy"), dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_INVEST_DESCRIPTION", value: EquityCISRepo.Invest_Description, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: EquityCISRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PRODUCT_ID", value: EquityCISRepo.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FUND_MANAGER_ID", value: EquityCISRepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ISSUER_ID", value: EquityCISRepo.Issuer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ORDER_QUANTITY", value: EquityCISRepo.Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_ORDER_UNIT_PRICE", value: EquityCISRepo.Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CONSIDERATION", value: EquityCISRepo.Consideration, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_ORDER_DATE", value: EquityCISRepo.Order_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_INVEST_STATUS", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_TYPE", value: "SELL", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TOTAL_LEVIES", value: EquityCISRepo.Total_Levies, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_TOTAL_COST", value: EquityCISRepo.Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTHORIZER", value: EquityCISRepo.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value:GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NO", value: EquityCISRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EQUITY_CIS", value: EquityCISRepo.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CAUC", value: EquityCISRepo.CURRENTAVERAGEUNITCOST, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CTC", value: EquityCISRepo.Current_Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
               // param.Add(name: "P_GL", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                conn.Execute("ADD_INVEST_EQUITY_CIS", param, commandType: CommandType.StoredProcedure);
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

        //CALL INVESTMENT PENDING RECORD
        public List<Invest_Equity_CISRepo> GetINVESTMENTRECORDList(string Invest_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> bn = new List<Invest_Equity_CISRepo>();

                string query = "Select * from INVEST_EQUITY_CIS WHERE INVEST_NO = '" + Invest_No + "'";
                return bn = conn.Query<Invest_Equity_CISRepo>(query).ToList();

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



        public bool Approve_Equity_CIS(Invest_Equity_CISRepo Equity_CISRepo)
        {
            var app = new AppSettings();

            // get the pending record
            Equity_CISRepo.GetINVESTMENTRECORDList(Invest_No);


            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
          
                    // update Invest_Equity table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_INVEST_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("APP_INVEST_EQUITY_CIS", param_E, commandType: CommandType.StoredProcedure);

                    //Update Invest_Equity_Balance table
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_PRODUCT_ID", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_CURRENT_QUANTITY", value: Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_TOTAL_COST", value: Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_UNIT_PRICE", value: Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_FUND_MANAGER_ID", value: Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    
                    conn.Execute("MIX_INVEST_EQUITY_AFTER", param_B, commandType: CommandType.StoredProcedure);

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
                    if (this.conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }
                
        }

        public bool Reverse_Equity_CIS(Invest_Equity_CISRepo Equity_CISRepo)
        {
            var app = new AppSettings();

            // get the pending record
            Equity_CISRepo.GetINVESTMENTRECORDList(Invest_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    // update Invest_Equity table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_INVEST_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("APP_INVEST_EQUITY_CIS", param_E, commandType: CommandType.StoredProcedure);

                    //Update Invest_Equity_Balance table
                    DynamicParameters param_B = new DynamicParameters();
                    param_B.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_PRODUCT_ID", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_CURRENT_QUANTITY", value: Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_TOTAL_COST", value: Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_UNIT_PRICE", value: Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_B.Add(name: "P_FUND_MANAGER_ID", value: Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("MIX_INVEST_EQUITY_REVERSE", param_B, commandType: CommandType.StoredProcedure);

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


        public bool Approve_Equity_CIS_Sell(Invest_Equity_CISRepo Equity_CISRepo)
        {
            var app = new AppSettings();

            // get the pending record
            Equity_CISRepo.GetINVESTMENTRECORDList(Invest_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //insert gain / loss into Invest_Equity table

                    // update Invest_Equity table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_INVEST_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("APP_INVEST_EQUITY_CIS", param_E, commandType: CommandType.StoredProcedure);

                    //Update Invest_Equity_Balance table
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PRODUCT_ID", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_CURRENT_QUANTITY", value: Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_TOTAL_COST", value: Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_UNIT_PRICE", value: Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("MIX_INVEST_EQUITY_SELL_AFTER", param, commandType: CommandType.StoredProcedure);

                    ts.Complete();

                    return true;
                }
                catch (Exception ex)
                {
                    //string xx = ex.ToString();
                    throw ex;
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

        public bool Reverse_Equity_CIS_Sell(Invest_Equity_CISRepo Equity_CISRepo)
        {
            var app = new AppSettings();

            // get the pending record
            Equity_CISRepo.GetINVESTMENTRECORDList(Invest_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //insert gain / loss into Invest_Equity table

                    // update Invest_Equity table
                    DynamicParameters param_E = new DynamicParameters();
                    param_E.Add(name: "P_INVEST_NO", value: Invest_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_INVEST_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_E.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("APP_INVEST_EQUITY_CIS_REV", param_E, commandType: CommandType.StoredProcedure);

                    //Update Invest_Equity_Balance table
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PRODUCT_ID", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_CURRENT_QUANTITY", value: Order_Quantity, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_TOTAL_COST", value: Total_Cost, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_UNIT_PRICE", value: Order_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("MIX_INVEST_EQUITY_SELL_REVERS", param, commandType: CommandType.StoredProcedure);

                    ts.Complete();

                    return true;
                }
                catch (Exception ex)
                {
                    //string xx = ex.ToString();
                    throw ex;
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
                conn.Execute("DEL_INVEST_EQUITY_CIS", param, commandType: CommandType.StoredProcedure);
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
        public void Get_GL_Balance(Invest_Equity_CISRepo ECIDRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_GL_ACCOUNT_NO", ECIDRepo.GL_Account_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_BALANCE", param, commandType: CommandType.StoredProcedure);
                ECIDRepo.GL_Balance = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //GET CURRENT PRODUCT QUANTITY FROM EQUITY BALANCE TABLE TABLE
        public void Get_Product_Current_Quantity(Invest_Equity_CISRepo ECIDRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEME_FUND_ID", ECIDRepo.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_PRODUCT_ID", ECIDRepo.Product_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_CURRENT_PRODUCT_QUANTITY", param, commandType: CommandType.StoredProcedure);
                ECIDRepo.Current_Quantity = param.Get<decimal>("VDATA");

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
                con.GetConnection().Execute("SEL_INVEST_EQUITY_CIS_EXIST", param, commandType: CommandType.StoredProcedure);
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

     

        // LIST FOR PENDING EQUITY / CIS - Buy
        public DataSet ECISPendingData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                conn = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_INVEST_ECIS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)conn;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ecis");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Invest_Equity_CISRepo> GetECISPendingList()
        {
            try
            {
                DataSet dt = ECISPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_Equity_CISRepo
                {
                    Invest_No = row.Field<string>("INVEST_NO"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Invest_Description = row.Field<string>("INVEST_DESCRIPTION"),
                    Product_Id = row.Field<string>("PRODUCT_ID"),
                    Product_Name = row.Field<string>("PRODUCT_NAME"),
                    Fund_Manager = row.Field<string>("FUND_MANAGER"),
                    Fund_Manager_Id = row.Field<string>("FUND_MANAGER_ID"),
                    Issuer = row.Field<string>("issuer_NAME"),
                    Issuer_Id = row.Field<string>("issuer_id"),
                    Order_Date = row.Field<DateTime>("ORDER_DATE"),
                    Order_Quantity = row.Field<decimal>("ORDER_QUANTITY"),
                    Order_Unit_Price = row.Field<decimal>("ORDER_UNIT_PRICE"),
                    Consideration = row.Field<decimal>("CONSIDERATION"),
                    Invest_Status = row.Field<string>("INVEST_STATUS"),
                    Trans_Type = row.Field<string>("TRANS_TYPE"),
                    Total_Cost = row.Field<Decimal>("TOTAL_COST"),
                    Total_Levies = row.Field<Decimal>("TOTAL_LEVIES"),
                    Authorizer = row.Field<string>("AUTHORIZER"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO")

                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // LIST FOR REVERSSAL EQUITY / CIS - Buy
        public DataSet ECISReverseData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                conn = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_INVEST_ECIS_REV";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)conn;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ecis_rev");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Invest_Equity_CISRepo> GetECISReverseList()
        {
            try
            {
                DataSet dt = ECISReverseData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_Equity_CISRepo
                {
                    Invest_No = row.Field<string>("INVEST_NO"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Invest_Description = row.Field<string>("INVEST_DESCRIPTION"),
                    Product_Id = row.Field<string>("PRODUCT_ID"),
                    Product_Name = row.Field<string>("PRODUCT_NAME"),
                    Fund_Manager = row.Field<string>("FUND_MANAGER"),
                    Fund_Manager_Id = row.Field<string>("FUND_MANAGER_ID"),
                    Issuer = row.Field<string>("issuer_NAME"),
                    Issuer_Id = row.Field<string>("issuer_id"),
                    Order_Date = row.Field<DateTime>("ORDER_DATE"),
                    Order_Quantity = row.Field<decimal>("ORDER_QUANTITY"),
                    Order_Unit_Price = row.Field<decimal>("ORDER_UNIT_PRICE"),
                    Consideration = row.Field<decimal>("CONSIDERATION"),
                    Invest_Status = row.Field<string>("INVEST_STATUS"),
                    Trans_Type = row.Field<string>("TRANS_TYPE"),
                    Total_Cost = row.Field<Decimal>("TOTAL_COST"),
                    Total_Levies = row.Field<Decimal>("TOTAL_LEVIES"),
                    Authorizer = row.Field<string>("AUTHORIZER"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO")

                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // LIST FOR PENDING EQUITY / CIS - Sell
        public DataSet ECIS_SellPendingData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                conn = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_INVEST_ECIS_SELL";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)conn;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ecis");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Invest_Equity_CISRepo> GetECIS_SellPendingList()
        {
            try
            {
                DataSet dt = ECIS_SellPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_Equity_CISRepo
                {
                    Invest_No = row.Field<string>("INVEST_NO"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Invest_Description = row.Field<string>("INVEST_DESCRIPTION"),
                    Product_Id = row.Field<string>("PRODUCT_ID"),
                    Product_Name = row.Field<string>("PRODUCT_NAME"),
                    Fund_Manager = row.Field<string>("FUND_MANAGER"),
                    Fund_Manager_Id = row.Field<string>("FUND_MANAGER_ID"),
                    Order_Date = row.Field<DateTime>("ORDER_DATE"),
                    Order_Quantity = row.Field<decimal>("ORDER_QUANTITY"),
                    Order_Unit_Price = row.Field<decimal>("ORDER_UNIT_PRICE"),
                    Consideration = row.Field<decimal>("CONSIDERATION"),
                    Invest_Status = row.Field<string>("INVEST_STATUS"),
                    Trans_Type = row.Field<string>("TRANS_TYPE"),
                    Total_Cost = row.Field<Decimal>("TOTAL_COST"),
                    Total_Levies = row.Field<Decimal>("TOTAL_LEVIES"),
                    Authorizer = row.Field<string>("AUTHORIZER"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
                    CURRENTAVERAGEUNITCOST = row.Field<decimal>("CURRENTAVERAGEUNITCOST"),
                    Current_Quantity = row.Field<decimal>("CURRENT_QUANTITY"),
                    Current_Total_Cost = row.Field<decimal>("CURRENT_TOTAL_COST"),
                    Gain_Loss = row.Field<decimal>("GAIN_LOSS")

                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        public DataSet ECIS_SellReverseData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                conn = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_INVEST_ECIS_SELL_REV";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)conn;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ecis");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Invest_Equity_CISRepo> GetECIS_SellReverseList()
        {
            try
            {
                DataSet dt = ECIS_SellReverseData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_Equity_CISRepo
                {
                    Invest_No = row.Field<string>("INVEST_NO"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Invest_Description = row.Field<string>("INVEST_DESCRIPTION"),
                    Product_Id = row.Field<string>("PRODUCT_ID"),
                    Product_Name = row.Field<string>("PRODUCT_NAME"),
                    Fund_Manager = row.Field<string>("FUND_MANAGER"),
                    Fund_Manager_Id = row.Field<string>("FUND_MANAGER_ID"),
                    Order_Date = row.Field<DateTime>("ORDER_DATE"),
                    Order_Quantity = row.Field<decimal>("ORDER_QUANTITY"),
                    Order_Unit_Price = row.Field<decimal>("ORDER_UNIT_PRICE"),
                    Consideration = row.Field<decimal>("CONSIDERATION"),
                    Invest_Status = row.Field<string>("INVEST_STATUS"),
                    Trans_Type = row.Field<string>("TRANS_TYPE"),
                    Total_Cost = row.Field<Decimal>("TOTAL_COST"),
                    Total_Levies = row.Field<Decimal>("TOTAL_LEVIES"),
                    Authorizer = row.Field<string>("AUTHORIZER"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
                    CURRENTAVERAGEUNITCOST = row.Field<decimal>("CURRENTAVERAGEUNITCOST"),
                    Current_Quantity = row.Field<decimal>("CURRENT_QUANTITY"),
                    Current_Total_Cost = row.Field<decimal>("CURRENT_TOTAL_COST"),
                    Gain_Loss = row.Field<decimal>("GAIN_LOSS")

                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }



        //FILTERING FUND MANAGER LIST FOR SCHEME
        public List<Invest_Equity_CISRepo> GetFMList(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> bn = new List<Invest_Equity_CISRepo>();

                string query = "Select * from VW_SCHEME_FUND_MANAGER WHERE SCHEME_ID = '" + Scheme_Id + "'";
                return bn = conn.Query<Invest_Equity_CISRepo>(query).ToList();

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
        public List<Invest_Equity_CISRepo> GetGLASFList(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> bn = new List<Invest_Equity_CISRepo>();

                string query = "Select * from GL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' AND MEMO_CODE = '" + "101" + "' AND REC_STATUS = '" + "ACTIVE" + "' or SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' AND MEMO_CODE = '" + "108" + "' AND REC_STATUS = '" + "ACTIVE" + "' ";
                return bn = conn.Query<Invest_Equity_CISRepo>(query).ToList();

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
        public List<Invest_Equity_CISRepo> GetGLAccFList(string GL_Account_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> bn = new List<Invest_Equity_CISRepo>();

                string query = "Select * from GL_ACCOUNT WHERE GL_ACCOUNT_NO = '" + GL_Account_No + "' ";
                return bn = conn.Query<Invest_Equity_CISRepo>(query).ToList();

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
        public List<Invest_Equity_CISRepo> GetuserAList()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> bn = new List<Invest_Equity_CISRepo>();

                string query = "Select * from SEC_AUTHORIZER WHERE APPROVE_EQUITY = '" + "YES" + "' ";
                return bn = conn.Query<Invest_Equity_CISRepo>(query).ToList();

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
        public List<Invest_Equity_CISRepo> GetECISPList(string Class_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> bn = new List<Invest_Equity_CISRepo>();

                string query = "Select * from INVEST_PRODUCTS WHERE CLASS_ID = '" + Class_Id + "'";
                return bn = conn.Query<Invest_Equity_CISRepo>(query).ToList();

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
        public List<Invest_Equity_CISRepo> GetSchemeFundProductList()
        {
            try
            {
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                List<Invest_Equity_CISRepo> ObjFund = new List<Invest_Equity_CISRepo>();

                return ObjFund = conn.Query<Invest_Equity_CISRepo>("Select * from VW_EQUITY_CIS_PRODUCT_SF where current_quantity > 0 ").ToList();
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

        public List<Invest_Equity_CISRepo> GetAssetList()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Equity_CISRepo> ObjFund = new List<Invest_Equity_CISRepo>();

                return ObjFund = db.GetConnection().Query<Invest_Equity_CISRepo>("Select * from INVEST_ASSET_CLASS where CLASS_ID = '04' OR CLASS_ID = '05' ").ToList();
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

        public List<Invest_Equity_CISRepo> GetFMList()
        {
            AppSettings db = new AppSettings();
            conn = db.GetConnection();
            try
            {
                List<Invest_Equity_CISRepo> ObjFund = new List<Invest_Equity_CISRepo>();

                return ObjFund = db.GetConnection().Query<Invest_Equity_CISRepo>("Select * from PFM_FUND_MANAGER  ").ToList();
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

        //check issuer id exist
        public void ck_issuer(Invest_Equity_CISRepo Equity_CISRepo)
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


    }
}
