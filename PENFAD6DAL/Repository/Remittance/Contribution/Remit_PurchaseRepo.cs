using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    
    public class Remit_PurchaseRepo

    {
        string plog;
        [Required]
        public string ES_Id { get; set; }
        public string Con_Type { get; set; }
        public string ES_Id_Cal { get; set; }
        public string Employer_Id { get; set; }
        public string Employer_Name { get; set; }
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        [Required]
        public decimal Cash_Balance { get; set; }
        public decimal Cash_Balance_Cal { get; set; }
        [Required]
        public string Con_Log_Id { get; set; }
        [Required]
        public decimal For_Year { get; set; }
        [Required]
        public decimal For_Month { get; set; }
        [Required]
        public decimal Total_Contribution { get; set; }
        public decimal Total_Purchase { get; set; }
        public decimal Con_Balance { get; set; }
        public decimal Total_Surcharge { get; set; }
        public decimal Total_Sur_Purchase { get; set; }
        public decimal Sur_Balance { get; set; }
        [Required]
        public decimal Total_Balance { get; set; }
        public string Purchase_Id { get; set; }
        [Required]
        public DateTime Trans_Date { get; set; }
        public string Trans_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public DateTime Auth_Date { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Reversal_Id { get; set; }
        public string Reason_For_Reversal { get; set; }
        public DateTime Reversal_Date { get; set; }
        public string Reversal_Status { get; set; }
        public decimal Temp_Con_Purchase { get; set; }
        public decimal Temp_Sur_Purchase { get; set; }
        public DateTime Working_Date { get; set;}
        public DateTime Today_Date { get; set; }

        IDbConnection con;
        public void SaveRecord(Remit_PurchaseRepo PurchaseRepo) 
        {
            try
            {
                
                ////////////////////////////////////////////////
                if (PurchaseRepo.For_Month.ToString().Length == 1)
                {
                    plog = "0" + PurchaseRepo.For_Month;
                }
                else
                {
                    plog = PurchaseRepo.For_Month.ToString();
                }
                ///////////////////////////////////////////////////

                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.ES_Id + PurchaseRepo.For_Year + plog, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_LOG_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_DATE", value: PurchaseRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date , dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_TYPE", value: PurchaseRepo.Con_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("ADD_REMIT_UNIT_PURCHASES_NEW", param, commandType: CommandType.StoredProcedure);
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


        public bool Approve_Unit_Purchases(Remit_PurchaseRepo PurchaseRepo)
        {
            var app = new AppSettings(); 

            // get the pending purchase record
            PurchaseRepo.GetPurchasePendingList(Purchase_Id);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(160);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
             
                try
                {


                    // UPDATE REMIT_CON_LOG TABLE
                    PurchaseRepo.Temp_Con_Purchase = 0;
                    PurchaseRepo.Temp_Sur_Purchase = 0;

                    if (PurchaseRepo.Con_Balance >= PurchaseRepo.Cash_Balance)
                    {
                        PurchaseRepo.Temp_Con_Purchase =Math.Round(PurchaseRepo.Cash_Balance,2);
                        PurchaseRepo.Con_Balance = Math.Round(PurchaseRepo.Con_Balance,2) - Math.Round(PurchaseRepo.Temp_Con_Purchase,2);
                        PurchaseRepo.Cash_Balance = 0;
                        PurchaseRepo.Total_Purchase = Math.Round(PurchaseRepo.Total_Purchase,2) + Math.Round(PurchaseRepo.Temp_Con_Purchase,2);
                    }
                    else if (PurchaseRepo.Con_Balance < PurchaseRepo.Cash_Balance)
                    {
                        PurchaseRepo.Temp_Con_Purchase = Math.Round(PurchaseRepo.Con_Balance,2);
                        PurchaseRepo.Cash_Balance = Math.Round(PurchaseRepo.Cash_Balance,2) - Math.Round(PurchaseRepo.Temp_Con_Purchase,2);
                        PurchaseRepo.Con_Balance = 0;
                        PurchaseRepo.Total_Purchase = Math.Round(PurchaseRepo.Total_Purchase,2) + Math.Round(PurchaseRepo.Temp_Con_Purchase,2);
                    }
                    else if (PurchaseRepo.Sur_Balance > 0 && PurchaseRepo.Cash_Balance > 0)
                    {
                        if (PurchaseRepo.Sur_Balance >= PurchaseRepo.Cash_Balance)
                        {
                            PurchaseRepo.Temp_Sur_Purchase = Math.Round(PurchaseRepo.Cash_Balance,2);
                            PurchaseRepo.Sur_Balance = Math.Round(PurchaseRepo.Sur_Balance,2) - Math.Round(PurchaseRepo.Temp_Sur_Purchase,2);
                            PurchaseRepo.Cash_Balance = 0;
                            PurchaseRepo.Total_Sur_Purchase = Math.Round(PurchaseRepo.Total_Sur_Purchase,2) + Math.Round(PurchaseRepo.Temp_Sur_Purchase,2);
                        }
                        else if (PurchaseRepo.Sur_Balance < PurchaseRepo.Cash_Balance)
                        {
                            PurchaseRepo.Temp_Sur_Purchase = Math.Round(PurchaseRepo.Sur_Balance,0);
                            PurchaseRepo.Cash_Balance = Math.Round(PurchaseRepo.Cash_Balance,2) - Math.Round(PurchaseRepo.Temp_Sur_Purchase,2);
                            PurchaseRepo.Sur_Balance = 0;
                            PurchaseRepo.Total_Sur_Purchase = Math.Round(PurchaseRepo.Total_Sur_Purchase,2) + Math.Round(PurchaseRepo.Temp_Sur_Purchase,2);
                        }
                    }

                    PurchaseRepo.Total_Balance = Math.Round(PurchaseRepo.Con_Balance,2) + Math.Round(PurchaseRepo.Sur_Balance,2);

                    // UPDATE REMIT_UNIT_PURCHASES_LOG TABLE
                    //PurchaseRepo.ApprovePurchaseRecord(PurchaseRepo.Purchase_Id);
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_PURCHASE_LOG_ID", value: Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_CON_PURCHASE_AMOUNT", value: PurchaseRepo.Temp_Con_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_SUR_PURCHASE_AMOUNT", value: PurchaseRepo.Temp_Sur_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_PURCHASE_NEW", param, commandType: CommandType.StoredProcedure);


                    //Update REMIT_CON_LOG Table
                    // PurchaseRepo.UpdateConLogRecord(PurchaseRepo);
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_CON_LOG_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_CONTRIBUTION", value: PurchaseRepo.Total_Contribution, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_PURCHASE", value: PurchaseRepo.Total_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_CON_BALANCE", value: PurchaseRepo.Con_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_SURCHARGE", value: PurchaseRepo.Total_Surcharge, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_SUR_PURCHASE", value: PurchaseRepo.Total_Sur_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_SUR_BALANCE", value: PurchaseRepo.Sur_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_BALANCE", value: PurchaseRepo.Total_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_PURCHASE_DATE", value: PurchaseRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_CON_PURCHASE", param_conl, commandType: CommandType.StoredProcedure);
                    

                    // UPDATE REMIT_CON_DETAILS TABLE
                    if (PurchaseRepo.Total_Contribution > 0)
                    {
                        // PurchaseRepo.UpdateConDetailsRecord(PurchaseRepo);
                        DynamicParameters param_con = new DynamicParameters();
                        param_con.Add(name: "P_CON_LOG_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_con.Add(name: "P_TEMP_CON_PURCHASE", value: PurchaseRepo.Temp_Con_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                        param_con.Add(name: "P_TOTAL_CONTRIBUTION", value: PurchaseRepo.Total_Contribution, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                        param_con.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_con.Add(name: "P_PURCHASE_DATE", value: PurchaseRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                        param_con.Add(name: "P_CON_TYPE", value: PurchaseRepo.Con_Type, dbType: DbType.String, direction: ParameterDirection.Input);

                        conn.Execute("UPD_REMIT_CON_DET_PURCHASE", param_con, commandType: CommandType.StoredProcedure);
                    }

                    if (PurchaseRepo.Total_Surcharge > 0)
                    {
                        //PurchaseRepo.UpdateSurDetailsRecord(PurchaseRepo);
                        DynamicParameters param_def = new DynamicParameters();
                        param_def.Add(name: "P_CON_LOG_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_def.Add(name: "P_TEMP_SUR_PURCHASE", value: PurchaseRepo.Temp_Sur_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                        param_def.Add(name: "P_TOTAL_SURCHARGE", value: PurchaseRepo.Total_Surcharge, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                        param_def.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_def.Add(name: "P_PURCHASE_DATE", value: PurchaseRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                        conn.Execute("UPD_REMIT_SUR_DET_PURCHASE", param_def, commandType: CommandType.StoredProcedure);
                    }
          

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
                        cmd_sft.CommandText = "SEL_REMIT_PURCH_BY_FUND_TOTAL";
                        cmd_sft.Parameters.Add("p_con_log_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = PurchaseRepo.Con_Log_Id;
                        cmd_sft.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd_sft.ExecuteReader();
                        while (dr.Read())
                        {
                            string scheme_fund_id = (dr["SCHEME_FUND_ID"].ToString()); // p_result.Value.ToString();SCHEME_FUND_ID
                            decimal con_purchased_amt = Convert.ToDecimal(dr["CON_PURCHASE_AMOUNT"].ToString());
                            decimal surcharge_purchased_amt = Convert.ToDecimal(dr["SUR_PURCHASE_AMOUNT"].ToString());

                            //
                            DynamicParameters param_gl = new DynamicParameters();
                            param_gl.Add(name: "P_ES_ID", value: PurchaseRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_SCHEME_FUND_ID", value: scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_CON_PURCHASE_AMOUNT", value: con_purchased_amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_SUR_PURCHASE_AMOUNT", value: surcharge_purchased_amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                            conn.Execute("ADD_REMIT_PURCHASE_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);
                        }
                    }

                    // CLEAR TEMP FIELDS in db---------------------
                    DynamicParameters param_tem = new DynamicParameters();
                    param_tem.Add(name: "P_CON_LOG_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_tem.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_PURCHASE_ZERO", param_tem, commandType: CommandType.StoredProcedure);

                    // UPDATE ES TABLE(CASH BALANCE)---------------------
                    DynamicParameters param_cash = new DynamicParameters();
                    param_cash.Add(name: "P_ES_ID", value: PurchaseRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_CASH_BALANCE", value: PurchaseRepo.Cash_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_ES_PURCHASE", param_cash, commandType: CommandType.StoredProcedure);

                    ///GET DEFFERED MEMBERS                   
                    //var param = new DynamicParameters();
                    //param.Add("P_SID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    //param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.String, ParameterDirection.Input);
                    //con.GetConnection().Execute("Z_DEFERRED", param, commandType: CommandType.StoredProcedure);


                    //// recalculate units for employees
                    //DynamicParameters param_cash2 = new DynamicParameters();
                    //param_cash2.Add(name: "P_E_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //conn.Execute("UPD_EMPLOYEE_RECAL", param_cash2, commandType: CommandType.StoredProcedure);


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

        public bool Reverse_Unit_Purchases(Remit_PurchaseRepo PurchaseRepo)
        {
            var app = new AppSettings();

            // get the pending purchase record
            PurchaseRepo.GetPurchasePendingList(Purchase_Id);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(60);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    //get scheme_fund_totals
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd_sft = new OracleCommand())
                    {
                        cmd_sft.Connection = conn;
                        cmd_sft.CommandType = CommandType.StoredProcedure;
                        cmd_sft.CommandText = "SEL_REV_PURCH_BY_FUND_TOTAL";
                        cmd_sft.Parameters.Add("p_con_log_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = PurchaseRepo.Purchase_Id;
                        cmd_sft.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd_sft.ExecuteReader();
                        while (dr.Read())
                        {
                            string scheme_fund_id = (dr["SCHEME_FUND_ID"].ToString()); // p_result.Value.ToString();SCHEME_FUND_ID
                            decimal con_purchased_amt = Convert.ToDecimal(dr["CON_PURCHASE_AMOUNT"].ToString());
                            decimal surcharge_purchased_amt = 0; // Convert.ToDecimal(dr["SUR_PURCHASE_AMOUNT"].ToString());
                            //
                            DynamicParameters param_gl = new DynamicParameters();
                            param_gl.Add(name: "P_ES_ID", value: PurchaseRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_SCHEME_FUND_ID", value: scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_CON_PURCHASE_AMOUNT", value: con_purchased_amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_SUR_PURCHASE_AMOUNT", value: surcharge_purchased_amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                            param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                            conn.Execute("REV_REMIT_PURCHASE_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);
                        }
                    }

                    // UPDATE DEFERRED MEMBERSHIP
                    DynamicParameters param_DEF = new DynamicParameters();
                    param_DEF.Add(name: "P_S_ID", value: PurchaseRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_DEF.Add(name: "P_EMPLOYER", value: PurchaseRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_DEF.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_E_DEFERRED_REV", param_DEF, commandType: CommandType.StoredProcedure);


                    //// UPDATE REMIT_UNIT_PURCHASES_LOG TABLE
                    ////PurchaseRepo.ApprovePurchaseRecord(PurchaseRepo.Purchase_Id);
                    //DynamicParameters param = new DynamicParameters();
                    //param.Add(name: "P_PURCHASE_LOG_ID", value: Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "P_TRANS_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "P_REV_REASON", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    //conn.Execute("REV_REMIT_PURCHASE", param, commandType: CommandType.StoredProcedure);


                    //Update REMIT_CON_LOG Table
                    // PurchaseRepo.UpdateConLogRecord(PurchaseRepo);
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_CON_LOG_ID", value: PurchaseRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_CONTRIBUTION", value: PurchaseRepo.Total_Contribution, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_PURCHASE", value: PurchaseRepo.Temp_Con_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_CON_BALANCE", value: PurchaseRepo.Con_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_SURCHARGE", value: PurchaseRepo.Total_Surcharge, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_SUR_PURCHASE", value: PurchaseRepo.Temp_Sur_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_SUR_BALANCE", value: PurchaseRepo.Sur_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_BALANCE", value: PurchaseRepo.Total_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_CON_PURCHASE", param_conl, commandType: CommandType.StoredProcedure);

                    // delete from purchase trans table and trigger update con_log_details table
                   
                        DynamicParameters param_con = new DynamicParameters();
                        param_con.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param_con.Add(name: "P_PURCHASE_DATE", value: PurchaseRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                        conn.Execute("REV_REMIT_TRANS_DET_PURCHASE", param_con, commandType: CommandType.StoredProcedure);

                    // delete from purchase LOG 

                    DynamicParameters paramLOG_con = new DynamicParameters();
                    paramLOG_con.Add(name: "P_PURCHASE_LOG_ID", value: PurchaseRepo.Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_LOG_PURCHASE", paramLOG_con, commandType: CommandType.StoredProcedure);




                    // UPDATE ES TABLE(CASH BALANCE)---------------------
                    DynamicParameters param_cash = new DynamicParameters();
                    param_cash.Add(name: "P_ES_ID", value: PurchaseRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_CASH_BALANCE", value: PurchaseRepo.Temp_Con_Purchase + PurchaseRepo.Temp_Sur_Purchase, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_ES_PURCHASE", param_cash, commandType: CommandType.StoredProcedure);

                   

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



        // Disapprove Purchase
        public void DisapprovePurchaseRecord(string Purchase_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_PURCHASE_LOG_ID", value: Purchase_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("DEL_REMIT_PURCHASE", param, commandType: CommandType.StoredProcedure);
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

        public bool isPurchaseUnique(string CON_LOG_ID)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_CON_LOG_ID", CON_LOG_ID, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_REMIT_PURCHASE_EXIST", param, commandType: CommandType.StoredProcedure);
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

        //GET CASH BALANCE FROM EMPLOYER SCHEME TABLE
        public void Get_Cash_Balance(Remit_PurchaseRepo PurchaseRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ES_ID", PurchaseRepo.ES_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_ES_CASHBALANCE", param, commandType: CommandType.StoredProcedure);
                PurchaseRepo.Cash_Balance = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // FOR PENDING PURCHASE
        public DataSet PurchasePendingData()
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

                cmd.CommandText = "SEL_REMIT_PURCHASE_PENDING_NEW";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "purchase");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_PurchaseRepo> GetPurchasePendingList()
        {
            try
            {
                DataSet dt = PurchasePendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_PurchaseRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Cash_Balance = row.Field<decimal>("CASH_BALANCE"),
                    Con_Log_Id = row.Field<string>("CON_LOG_ID"),
                    For_Month = row.Field<decimal>("FOR_MONTH"),
                    For_Year = row.Field<decimal>("FOR_YEAR"),
                    Total_Contribution = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Total_Purchase = row.Field<decimal>("TOTAL_PURCHASE"),
                    Con_Balance = row.Field<decimal>("CON_BALANCE"),
                    Total_Surcharge = row.Field<decimal>("TOTAL_SURCHARGE"),
                    Total_Sur_Purchase = row.Field<decimal>("TOTAL_SUR_PURCHASE"),
                    Sur_Balance = row.Field<decimal>("SUR_BALANCE"),
                    Total_Balance = row.Field<decimal>("TOTAL_BALANCE"),
                    Purchase_Id = row.Field<string>("PURCHASE_LOG_ID"),
                    Trans_Date = row.Field<DateTime>("TRANS_DATE"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Con_Type = row.Field<string>("CON_TYPE")

                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // LIST FOR PURCHASE REVERSAL
        public DataSet PurchaseReverseData()
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

                cmd.CommandText = "SEL_REMIT_PURCHASE_REVERSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "purchaseR");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_PurchaseRepo> GetPurchaseReverseList()
        {
            try
            {
                DataSet dt = PurchaseReverseData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_PurchaseRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Cash_Balance = row.Field<decimal>("CASH_BALANCE"),
                    Con_Log_Id = row.Field<string>("CON_LOG_ID"),
                    For_Month = row.Field<decimal>("FOR_MONTH"),
                    For_Year = row.Field<decimal>("FOR_YEAR"),
                    Total_Contribution = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Total_Purchase = row.Field<decimal>("TOTAL_PURCHASE"),
                    Con_Balance = row.Field<decimal>("CON_BALANCE"),
                    Total_Surcharge = row.Field<decimal>("TOTAL_SURCHARGE"),
                    Total_Sur_Purchase = row.Field<decimal>("TOTAL_SUR_PURCHASE"),
                    Sur_Balance = row.Field<decimal>("SUR_BALANCE"),
                    Total_Balance = row.Field<decimal>("TOTAL_BALANCE"),
                    Purchase_Id = row.Field<string>("PURCHASE_LOG_ID"),
                    Trans_Date = row.Field<DateTime>("TRANS_DATE"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Temp_Con_Purchase= row.Field<decimal>("CON_PURCHASE_AMOUNT"),
                    Temp_Sur_Purchase = row.Field<decimal>("SUR_PURCHASE_AMOUNT")

                }).ToList();

                return eList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        // FOR ACTIVE RECEIPTS
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
                cmd.Connection = (OracleConnection)con;

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
                cmd.Connection = (OracleConnection)con;

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

        public DataSet PurchaseESData()
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
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "purchase");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_PurchaseRepo> GetPurchaseESList()
        {
            try
            {
                DataSet dt = PurchaseESData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_PurchaseRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Cash_Balance = row.Field<decimal>("CASH_BALANCE"),
                    Today_Date = row.Field<DateTime>("TODAY_DATE")
                    
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool isYearMonthValid(Remit_PurchaseRepo PurchaseRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_FOR_YEAR", For_Year, DbType.Decimal, ParameterDirection.Input);
                param.Add("P_FOR_MONTH", For_Month, DbType.Decimal, ParameterDirection.Input);
                param.Add("P_ES_ID", ES_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_REMIT_YEAR_MONTH", param, commandType: CommandType.StoredProcedure);
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
        //check for purchase date in unit table
        public bool isDate(Remit_PurchaseRepo PurchaseRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SID", Scheme_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_TD", Trans_Date, DbType.Date, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_UNIT_DATE", param, commandType: CommandType.StoredProcedure);
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

        //check if con is valid for purchase
        public bool isValidP(Remit_PurchaseRepo PurchaseRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SID", Con_Log_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_UNIT_PUR_VAL", param, commandType: CommandType.StoredProcedure);
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
        //FILTERING LIST OF SCHEME-FUND FOR EMPLOYER
        public List<Remit_PurchaseRepo> GetESCHEMEList(string ES_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_PurchaseRepo> bn = new List<Remit_PurchaseRepo>();

                string query = "Select * from VW_REMIT_CON_PURSHACE WHERE ES_Id = '" + ES_Id + "' and con_type != 'PORT-IN'";
                return bn = con.Query<Remit_PurchaseRepo>(query).ToList();

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

        //CALL PURCHASE RECORD
        public List<Remit_PurchaseRepo> GetPurchasePendingList(string Purchase_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_PurchaseRepo> bn = new List<Remit_PurchaseRepo>();

                string query = "Select * from REMIT_UNIT_PURCHASES_LOG WHERE PURCHASE_LOG_ID = '" + Purchase_Id + "'";
                return bn = con.Query<Remit_PurchaseRepo>(query).ToList();

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


        //public bool report_dash(pfm_Scheme_FundRepo SFRepo)
        //{
        //    var app = new AppSettings();


        //    TransactionOptions tsOp = new TransactionOptions();
        //    tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
        //    TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
        //    tsOp.Timeout = TimeSpan.FromMinutes(60);

        //    using (OracleConnection conn = new OracleConnection(app.conString()))  //
        //    {

        //        try
        //        {



        //            var con = new AppSettings();
        //            var param = new DynamicParameters();
        //            param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
        //            param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
        //            param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
        //            conn.Execute("REPORT_INSERT_PORT_SUM_SDB", param, commandType: CommandType.StoredProcedure);
        //            ts.Complete();


        //            ts.Complete();

        //            return true;
        //        }
        //        catch (Exception ex)
        //        {

        //            throw ex;
        //        }
        //        finally
        //        {
        //            ts.Dispose();
        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }
        //        }

        //    }

        //}


    }
}
