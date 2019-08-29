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

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    public class Remit_WithdrawalRepo
    {
        [Required]
        public string ESF_Id { get; set; }
        public string ES_Id { get; set; }
        public string Employer_Id { get; set; }

        public string Employer_Name{ get; set; }
        public string Employer_Id1 { get; set; }

        public string Employer_Name1 { get; set; }
        public string Employee_Id { get; set; }
        public DateTime? Trans_Request_Date { get; set; }
        public string Withdrawal_No { get; set; }
        public decimal Employer_Con_Balance { get; set; }
        public decimal Employee_Con_Balance { get; set; }
        public decimal Employer_Unit_Balance { get; set; }
        public decimal Employee_Unit_Balance { get; set; }
        [Required]
        public decimal Total_Withdrawal_Unit { get; set; }
        public decimal Total_Withdrawal_Amount { get; set; }
        public string Scheme_Name { get; set; }
        public string Scheme_Id { get; set; }

        public string Scheme_Name_2{ get; set; }
        public string Scheme_Id_2 { get; set; }
        //public string ES_ID { get; set; }

        public string Fund_Name { get; set; }
        public string Fund_Id { get; set; }
        public string Scheme_Fund_Id { get; set; }
        public decimal Unit_Price { get; set; }
        [Range(0, 15, ErrorMessage = "Tax rate must fall between zero (0) and 15%.")]
        public decimal Tax { get; set; }
        public string Payment_Mode { get; set; }

        public string Instrument_No { get; set; }
        //[Required]
        public string Withdrawal_Reason { get; set; }
        public string Trans_Status { get; set; }
        public string Maker_Id { get; set; }
        public DateTime? Make_Date { get; set; }
   
        public string Cust_No { get; set; }
        public string Title { get; set; }
        public string Employee_Name { get; set; }
        public string Surname { get; set; }
        public string First_Name { get; set; }
        public decimal Total_Unit_Balance { get; set; }
        public decimal Total_Con_Balance { get; set; }
        public string Other_Name { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public DateTime Auth_Date { get; set; }
        public string Reverse_Id { get; set; }
        public string Reverse_Reason { get; set; }
        public DateTime Reverse_Date { get; set; }
        public string Reversal_Status { get; set; }
        public string ESF_Status { get; set; }
        public DateTime Working_Date { get; set;}
        public DateTime Today_Date { get; set; }
        public decimal Total_Benefit { get; set; }
        public decimal Total_Withdrawal_Temp { get; set; }   
        public string Type { get; set; }
        public string GL_Account_Name { get; set; }
       //[Required]
        public string GL_Account_No { get; set; }
       // [Required]
        public decimal GL_Balance { get; set; }
        public decimal Accrued_Benefit { get; set; }
        public DateTime? Pay_Date_Benefit { get; set; }
        public string Pay_Benefit { get; set; }

        public string Pay_Tax { get; set; }
        // for Porting in
        public string PortIn_No { get; set; }
        public DateTime? Trans_Date { get; set; }
        public decimal Employee_Amount { get; set; }
        public decimal Employer_Amount { get; set; }
        public decimal Employee_Unit_Purchased { get; set; }
        public decimal Employer_Unit_Purchased { get; set; }
        public string Previous_Trustee { get; set; }
        public string Trustee_Name { get; set; }

        //for porting out
        public string PortOut_No { get; set; }
        public string New_Trustee { get; set; }
        public decimal To_Employer { get; set; }
        public decimal To_Employer_Amount { get; set; }

        decimal taxAmount;
        IDbConnection con;

        readonly Remit_Contribution_Upload_LogRepo RemitInitialRepo = new Remit_Contribution_Upload_LogRepo();
        readonly crm_EmployerRepo EmployerRepo = new crm_EmployerRepo();
        readonly Remit_BatchLogRepo repo_Remit_Batch = new Remit_BatchLogRepo();
        public void SaveRecord(Remit_WithdrawalRepo WithdrawalRepo) 
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                taxAmount = (WithdrawalRepo.Tax / 100) * WithdrawalRepo.Total_Withdrawal_Amount;
                param.Add(name: "p_Trans_request_Date", value: WithdrawalRepo.Trans_Request_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_Id", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: WithdrawalRepo.Employer_Con_Balance + WithdrawalRepo.Employee_Con_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: WithdrawalRepo.Total_Unit_Balance * WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Total_Withdrawal", value: WithdrawalRepo.Total_Withdrawal_Unit , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Total_Withdrawal_Amount", value: WithdrawalRepo.Total_Withdrawal_Amount - taxAmount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Unit", value: WithdrawalRepo.Employer_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Unit", value: WithdrawalRepo.Employee_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Unit_Price", value: WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Tax", value: taxAmount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_WithDrawal_Reason", value: WithdrawalRepo.Withdrawal_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_To_Employer", value: WithdrawalRepo.To_Employer, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_Date", value: System.DateTime.Now, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trans_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Unit", value: WithdrawalRepo.Employee_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
				param.Add(name: "p_Employer_Id", value: WithdrawalRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				con.Execute("ADD_REMIT_WITH_DRAWAL_REQUEST", param, commandType: CommandType.StoredProcedure);
				
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




        public void SaveRecord_TE(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                taxAmount = (WithdrawalRepo.Tax / 100) * WithdrawalRepo.Total_Withdrawal_Amount;
                param.Add(name: "p_Trans_request_Date", value: WithdrawalRepo.Trans_Request_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_Id", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: WithdrawalRepo.Employer_Con_Balance + WithdrawalRepo.Employee_Con_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: WithdrawalRepo.Total_Unit_Balance * WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Total_Withdrawal", value: WithdrawalRepo.Total_Withdrawal_Temp, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Total_Withdrawal_Amount", value: WithdrawalRepo.Total_Withdrawal_Temp * WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Unit", value: WithdrawalRepo.Employer_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Unit", value: WithdrawalRepo.Employee_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Unit_Price", value: WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_WithDrawal_Reason", value: WithdrawalRepo.Withdrawal_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_Date", value: System.DateTime.Now, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trans_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Id", value: WithdrawalRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Bank", value: WithdrawalRepo.GL_Account_No, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                con.Execute("ADD_REMIT_TRANS_EMPLOYER", param, commandType: CommandType.StoredProcedure);

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

        public void SaveRecord_Port(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Trans_Date", value: WithdrawalRepo.Trans_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_Id", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeAmount", value: WithdrawalRepo.Employee_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeUnit_Purchased", value: WithdrawalRepo.Employee_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployerAmount", value: WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployerUnit_Purchased", value: WithdrawalRepo.Employer_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Unit_Price", value: WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_PreviousTrustee", value: WithdrawalRepo.Previous_Trustee, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_GL_Account_No", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_Date", value: System.DateTime.Now, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trans_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("ADD_REMIT_PORTIN_REQUEST", param, commandType: CommandType.StoredProcedure);
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

        public void SaveRecord_PortOut(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Trans_request_Date", value: WithdrawalRepo.Trans_Request_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_Id", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: WithdrawalRepo.Employer_Con_Balance + WithdrawalRepo.Employee_Con_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: WithdrawalRepo.Total_Unit_Balance * WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Total_Withdrawal", value: WithdrawalRepo.Total_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Total_Withdrawal_Amount", value: WithdrawalRepo.Total_Unit_Balance * WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Unit", value: WithdrawalRepo.Employer_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Unit", value: WithdrawalRepo.Employee_Unit_Balance, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Unit_Price", value: WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_Date", value: System.DateTime.Now, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trans_Status", value: "PENDING", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trustee", value: WithdrawalRepo.New_Trustee, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("ADD_REMIT_PORTOUT_REQUEST", param, commandType: CommandType.StoredProcedure);
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


        public bool Approve_Unit_Withdrawal(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                   
                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: System.DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_WITHDRAWAL_REQUEST", param, commandType: CommandType.StoredProcedure);


                    ////Update SCHEME_FUND Table
                    //DynamicParameters param_conl = new DynamicParameters();
                    //param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //conn.Execute("UPD_REMIT_WITH_SF", param_conl, commandType: CommandType.StoredProcedure);

                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "p_Tax", value: WithdrawalRepo.Tax, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_WITHDRAWAL_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

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


        public bool DApprove_Unit_Withdrawal(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {

                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "DISAPPROVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "DISAPPROVED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: System.DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_WITHDRAWAL_REQUEST", param, commandType: CommandType.StoredProcedure);


                    ////Update SCHEME_FUND Table
                    //DynamicParameters param_conl = new DynamicParameters();
                    //param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //conn.Execute("UPD_REMIT_WITH_SF", param_conl, commandType: CommandType.StoredProcedure);

                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: WithdrawalRepo.Trans_Request_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "p_Tax", value: WithdrawalRepo.Tax, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("DADD_REMIT_WITHDRAWAL_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

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

        public bool Approve_Unit_WithdrawalTE(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {

                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "PAID", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: System.DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_TRANS_EMPLOYER", param, commandType: CommandType.StoredProcedure);


                   
                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "p_GL", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_EMP_GL_TRANSTE", param_gl, commandType: CommandType.StoredProcedure);



                    //UPDATE EMPLOYER_ SCHEME TABLE

                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_ES_ID", value: WithdrawalRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_TRANS_EMPLOYER", param_conl, commandType: CommandType.StoredProcedure);



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

        public bool Approve_Unit_PortOut(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending PORTOUT record
            WithdrawalRepo.GetPortOutPendingList(PortOut_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {

                    // UPDATE REMIT_PORTOUT TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: System.DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_PORTOUT", param, commandType: CommandType.StoredProcedure);


                    ////Update SCHEME_FUND Table
                    //DynamicParameters param_conl = new DynamicParameters();
                    //param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //conn.Execute("UPD_REMIT_WITH_SF", param_conl, commandType: CommandType.StoredProcedure);


                    //// UPDATE CRM_EMPLOYEE_SCHEME_FUND TABLE
                    //DynamicParameters param_con = new DynamicParameters();
                    //param_con.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param_con.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //conn.Execute("UPD_REMIT_ESF_WITHDRAWAL", param_con, commandType: CommandType.StoredProcedure);

                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Benefit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_PORTOUT_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

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


        public bool Approve_Unit_PortIn(Remit_WithdrawalRepo WithdrawalRepo)
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
                    // UPDATE REMIT_PORTIN TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: System.DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_PORTIN", param, commandType: CommandType.StoredProcedure);

                    //Update SCHEME_FUND Table
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Employee_Unit_Purchased + WithdrawalRepo.Employer_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_PORT_SF", param_conl, commandType: CommandType.StoredProcedure);

                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Employee_Amount + WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_GL_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_PORTIN_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);
                    
                 
                    ///create log for the upload Remit Log
                    var paramb = new DynamicParameters();                 
                    paramb.Add(name: "p_Employer_Id", value: WithdrawalRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_ES_Id", value: WithdrawalRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_DeadLine_Date", value: WithdrawalRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_Unit_Purchased_YesNo", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_Total_Contribution", value: WithdrawalRepo.Employee_Amount + WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_Make_date", value: Trans_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
					paramb.Add(name: "p_Batch_Id", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
					paramb.Add(name: "p_Con_Log_Id", value: string.Empty, dbType: DbType.String, direction: ParameterDirection.Output);
                    conn.Execute(sql: "ADD_REMIT_CON_PORTIN", param: paramb, commandType: CommandType.StoredProcedure);
                    string batchno = paramb.Get<string>("p_Con_Log_Id");

                    var parambb = new DynamicParameters();
                    parambb.Add(name: "p_Employee_Id", value: WithdrawalRepo.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Con_Log_Id", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employer_Con", value: WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employee_Con", value: WithdrawalRepo.Employee_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employer_Amt_Used", value: WithdrawalRepo.Employee_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employee_Amt_Used", value: WithdrawalRepo.Employee_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Make_date", value: DateTime.Now, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employee_Salary", value: WithdrawalRepo.Employee_Amount + WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Req_Con", value: WithdrawalRepo.Employee_Amount + WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_Con_Type", value: "PORT-IN", dbType: DbType.String, direction: ParameterDirection.Input);
                    parambb.Add(name: "p_BatchNo", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    int result = conn.Execute(sql: "ADD_REMIT_CON_DETAILS", param: parambb, commandType: CommandType.StoredProcedure);

                    DynamicParameters paramS = new DynamicParameters();
                    paramS.Add(name: "P_PURCHASE_LOG_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramS.Add(name: "P_CON_LOG_ID", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramS.Add(name: "P_TRANS_DATE", value: WithdrawalRepo.Trans_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    paramS.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramS.Add(name: "P_MAKE_DATE", value: DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
					paramS.Add(name: "p_Batch_Id", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
					paramS.Add(name: "p_P_Log_Id", value: string.Empty, dbType: DbType.String, direction: ParameterDirection.Output);
                    conn.Execute("ADD_REMIT_UNIT_PUR_PORTIN", paramS, commandType: CommandType.StoredProcedure);
                    string Purbatchno = paramS.Get<string>("p_P_Log_Id");


					DynamicParameters paramSS = new DynamicParameters();
                    paramSS.Add(name: "P_PURCHASE_LOG_ID", value: Purbatchno, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_PURCHASE_TYPE", value: "PORT-IN", dbType: DbType.String, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYER_AMT", value: WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYEE_AMT", value: WithdrawalRepo.Employee_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYEE_UNITS", value: WithdrawalRepo.Employee_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYER_UNITS", value: WithdrawalRepo.Employer_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
					paramSS.Add(name: "p_Batch_Id", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
					paramSS.Add(name: "P_UNIT_PRICE", value: WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_PORT_PTRANS", paramSS, commandType: CommandType.StoredProcedure);


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

        public bool Reverse_Unit_PortIn(Remit_WithdrawalRepo WithdrawalRepo)
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
                    // UPDATE REMIT_PORTIN TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REV_REASON", value: WithdrawalRepo.Reverse_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_PORTIN", param, commandType: CommandType.StoredProcedure);

                    //Update SCHEME_FUND Table
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Employee_Unit_Purchased + WithdrawalRepo.Employer_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_PORT_SF", param_conl, commandType: CommandType.StoredProcedure);

                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Employee_Amount + WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_GL_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_PORTIN_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

                  
                    DynamicParameters paramSS = new DynamicParameters();
                   // paramSS.Add(name: "P_PURCHASE_LOG_ID", value: Purbatchno, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_PURCHASE_TYPE", value: "PORT-IN", dbType: DbType.String, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYER_AMT", value: WithdrawalRepo.Employer_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYEE_AMT", value: WithdrawalRepo.Employee_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYEE_UNITS", value: WithdrawalRepo.Employee_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_EMPLOYER_UNITS", value: WithdrawalRepo.Employer_Unit_Purchased, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    paramSS.Add(name: "P_UNIT_PRICE", value: WithdrawalRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_PORT_PTRANS", paramSS, commandType: CommandType.StoredProcedure);

                   


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


        public bool Pay_Unit_Withdrawal(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record for payment
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

			//Get connection
			var con = new AppSettings();
			var paramA = new DynamicParameters();
			paramA.Add("P_EMPLOYEEID", WithdrawalRepo.Employer_Id, DbType.String, ParameterDirection.Input);
			paramA.Add("VDATA", "", DbType.String, ParameterDirection.Output);
			con.GetConnection().Execute("SEL_EMPLOYER_CUST_NO", paramA, commandType: CommandType.StoredProcedure);
			String ECUSTNO = paramA.Get<String>("VDATA");

			TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "PAID", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_BENEFIT", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_ID_BENEFIT", value: GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_DATE_BENEFIT", value: Pay_Date_Benefit, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAYMENT_MODE", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_INSTRUMENT_NUMBER", value: Instrument_No, dbType: DbType.String, direction: ParameterDirection.Input);                  
                    conn.Execute("UPD_REMIT_PAY_REQUEST", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_DATE", value: Pay_Date_Benefit, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_PAYREQUEST_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

					//Update SCHEME_FUND Table
					DynamicParameters param_conl = new DynamicParameters();
					param_conl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
					param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Unit + To_Employer, dbType: DbType.Decimal, direction: ParameterDirection.Input);
					conn.Execute("UPD_REMIT_WITHD_SF", param_conl, commandType: CommandType.StoredProcedure);


					if (WithdrawalRepo.To_Employer > 0)
					{
						
						///create log for the upload Remit Log
						var paramb = new DynamicParameters();
						paramb.Add(name: "p_Employer_Id", value: Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_ES_Id", value: Employer_Id+Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_DeadLine_Date", value: Pay_Date_Benefit, dbType: DbType.Date, direction: ParameterDirection.Input);
						paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Unit_Purchased_YesNo", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Total_Contribution", value: To_Employer * Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Make_date", value: Pay_Date_Benefit, dbType: DbType.DateTime, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Batch_Id", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
						paramb.Add(name: "p_Con_Log_Id", value: string.Empty, dbType: DbType.String, direction: ParameterDirection.Output);						
						conn.Execute(sql: "ADD_REMIT_CON_PORTIN", param: paramb, commandType: CommandType.StoredProcedure);
						string batchno = paramb.Get<string>("p_Con_Log_Id");

						var parambb = new DynamicParameters();
						parambb.Add(name: "p_Employee_Id", value: Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_ESF_ID", value: ECUSTNO + Scheme_Id+"02", dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Con_Log_Id", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employer_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employee_Con", value:To_Employer * Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employer_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employee_Amt_Used", value: To_Employer * Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Make_date", value: DateTime.Now, dbType: DbType.DateTime, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employee_Salary", value: To_Employer * Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
						parambb.Add(name: "p_BatchNo", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
						int result = conn.Execute(sql: "ADD_REMIT_CON_DETAILS", param: parambb, commandType: CommandType.StoredProcedure);

						DynamicParameters paramS = new DynamicParameters();
						paramS.Add(name: "P_PURCHASE_LOG_ID", value: ECUSTNO + Scheme_Id + "02", dbType: DbType.String, direction: ParameterDirection.Input);
						paramS.Add(name: "P_CON_LOG_ID", value: batchno, dbType: DbType.String, direction: ParameterDirection.Input);
						paramS.Add(name: "P_TRANS_DATE", value: Pay_Date_Benefit, dbType: DbType.Date, direction: ParameterDirection.Input);
						paramS.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
						paramS.Add(name: "P_MAKE_DATE", value: DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
						paramS.Add(name: "p_Batch_Id", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
						paramS.Add(name: "p_P_Log_Id", value: string.Empty, dbType: DbType.String, direction: ParameterDirection.Output);
						conn.Execute("ADD_REMIT_UNIT_PUR_PORTIN", paramS, commandType: CommandType.StoredProcedure);
						string Purbatchno = paramS.Get<string>("p_P_Log_Id");

						DynamicParameters paramSS = new DynamicParameters();
						paramSS.Add(name: "P_PURCHASE_LOG_ID", value: Purbatchno, dbType: DbType.String, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_ESF_ID", value: ECUSTNO + Scheme_Id + "02", dbType: DbType.String, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_PURCHASE_TYPE", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_EMPLOYER_AMT", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_EMPLOYEE_AMT", value: To_Employer * Unit_Price , dbType: DbType.Decimal, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_EMPLOYEE_UNITS", value: To_Employer, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_EMPLOYER_UNITS", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						paramSS.Add(name: "P_UNIT_PRICE", value: Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
						paramSS.Add(name: "p_Batch_Id", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
						conn.Execute("ADD_REMIT_PORT_PTRANS", paramSS, commandType: CommandType.StoredProcedure);

					}
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

        public bool Reverse_Unit_Withdrawal(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the  withdrawal record for reversal
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSE_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSE_REASON", value: Reverse_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_REVERSE_REQUEST", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_BENEFIT", value: WithdrawalRepo.Pay_Tax, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TAX", value: WithdrawalRepo.Tax, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_REVERSEREQUEST_GL", param_gl, commandType: CommandType.StoredProcedure);

                    //Update SCHEME_FUND Table
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_UNIT_REMIT_WITH_SF", param_conl, commandType: CommandType.StoredProcedure);



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


        //reverse benefit transfer
        public bool Reverse_Unit_WithdrawalTE(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the  withdrawal record for reversal
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {


                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: System.DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("REV1_REMIT_TRANS_EMPLOYER", param, commandType: CommandType.StoredProcedure);



                    //go off
                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    //Total_Withdrawal_Amount = Total_Withdrawal_Unit * Unit_Price;
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "p_GL", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_WITHDRAWAL_GL_TRANSTE", param_gl, commandType: CommandType.StoredProcedure);



                    //UPDATE EMPLOYER_ SCHEME TABLE

                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_ES_ID", value: WithdrawalRepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_REMIT_TRANS_EMPLOYER", param_conl, commandType: CommandType.StoredProcedure);





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

        //revrese portout
        public bool Reverse_Unit_PortOut(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record for payment
            //WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    // UPDATE REMIT_PORTOUT_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSE_STATUS", value: "REVERSED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REVERSE_REASON", value: Reverse_Reason, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_REVERSE_PORTOUT", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Withdrawal_Amount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_BENEFIT", value: WithdrawalRepo.Pay_Tax, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TAX", value: WithdrawalRepo.Tax, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_REVERSEPORTOUT_GL", param_gl, commandType: CommandType.StoredProcedure);

                    //Update SCHEME_FUND Table
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("REV_UNIT_REMIT_WITH_SF", param_conl, commandType: CommandType.StoredProcedure);


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

        public bool Pay_Unit_PortOut(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record for payment
            //WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    //// UPDATE CRM_EMPLOYEE_SCHEME_FUND TABLE
                    //DynamicParameters param_con = new DynamicParameters();
                    //param_con.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param_con.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //param_con.Add(name: "P_TOTAL_BENEFIT", value: WithdrawalRepo.Total_Benefit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //conn.Execute("UPD_REMIT_ESF_WITHDRAWAL", param_con, commandType: CommandType.StoredProcedure);

                    //Update SCHEME_FUND Table
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_WITH_SF", param_conl, commandType: CommandType.StoredProcedure);


                    // UPDATE REMIT_PORTOUT TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_PORTOUT_NO", value: PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "PAID", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_BENEFIT", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_ID_BENEFIT", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_DATE_BENEFIT", value: Pay_Date_Benefit, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_PAY_PORTOUT", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Benefit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_DATE", value: Pay_Date_Benefit, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_PAYPORTOUT_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

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


        //pay tax
        public bool PayTax_Unit_Withdrawal(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record for payment
            WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {
                   
                    // UPDATE REMIT_WITHDRAWAL_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TRANS_STATUS", value: "PAID", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_BENEFIT", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_ID_BENEFIT", value: GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_DATE_BENEFIT", value: Pay_Date_Benefit, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAYMENT_MODE", value: Payment_Mode, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_PAY_TAX", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE

                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Tax, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_DATE", value: Pay_Date_Benefit, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_PAYTAX_GL_TRANS", param_gl, commandType: CommandType.StoredProcedure);

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
        
        // Disapprove TRANSFER TO EMPLOYER
        public void DisapproveWithdrawalRecordTE(string Withdrawal_No)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("DEL_REMIT_TRANS_EMPLOYER", param, commandType: CommandType.StoredProcedure);
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

        // Disapprove Withdrawal
        public void DisapproveWithdrawalRecord(string Withdrawal_No)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_WITHDRAWAL_NO", value: Withdrawal_No, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("DEL_REMIT_WITHDRAWAL", param, commandType: CommandType.StoredProcedure);
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

        

        //// Disapprove Withdrawal
        //public void DisapproveWithdrawalRecord(Remit_WithdrawalRepo WithdrawalRepo)
        //{
        //    try
        //    {
        //        //Get Connection
        //        AppSettings app = new AppSettings();
        //        con = app.GetConnection();
        //        DynamicParameters param = new DynamicParameters();
        //        param.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
        //        con.Execute("DEL_REMIT_WITHDRAWAL", param, commandType: CommandType.StoredProcedure);
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

        // Disapprove PORTOUT
        public void DisapprovePortOutRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_WITHDRAWAL_NO", value: WithdrawalRepo.PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("DEL_REMIT_PORTOUT", param, commandType: CommandType.StoredProcedure);
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

        // Disapprove PORTOUT PAY

        public bool DisapprovePortOutRecord_Pay(Remit_WithdrawalRepo WithdrawalRepo)
        {
            var app = new AppSettings();

            // get the pending withdrawal record for payment
            //WithdrawalRepo.GetWPendingList(Withdrawal_No);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
                try
                {
                    // UPDATE REMIT_PORTOUT_REQUEST TABLE
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_WITHDRAWAL_NO", value: PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("DEL_REMIT_PORTOUT_PAY", param, commandType: CommandType.StoredProcedure);

                    //UPDATE GL_ACCOUNT TABLE AND GL_TRANSACTION TABLE
                    DynamicParameters param_gl = new DynamicParameters();
                    param_gl.Add(name: "P_ESF_ID", value: WithdrawalRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_SCHEME_FUND_ID", value: Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TOTAL_WITHDRAWAL", value: Total_Benefit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_WITHDRAWAL_NO", value: PortOut_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_DATE", value: Trans_Request_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_ACCOUNT_NO", value: WithdrawalRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_PAY_BENEFIT", value: WithdrawalRepo.Pay_Tax, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_gl.Add(name: "P_TAX", value: WithdrawalRepo.Tax, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_REVERSEREQUEST_PAY", param_gl, commandType: CommandType.StoredProcedure);

                    ////Update SCHEME_FUND Table
                    //DynamicParameters param_conl = new DynamicParameters();
                    //param_conl.Add(name: "P_SCHEME_FUND_ID", value: WithdrawalRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param_conl.Add(name: "P_TOTAL_WITHDRAWAL", value: WithdrawalRepo.Total_Withdrawal_Unit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    //conn.Execute("REV_UNIT_REMIT_WITH_SF2", param_conl, commandType: CommandType.StoredProcedure);


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
       
        // Disapprove Port-in
        public void DisapprovePortInRecord(string PortIn_No)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_WITHDRAWAL_NO", value: PortIn_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("DEL_REMIT_PORTIN", param, commandType: CommandType.StoredProcedure);
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


        //GET CURRENT UNIT PRICE FROM SCHEME TABLE
        public void Get_UnitPrice(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id) || string.IsNullOrEmpty(WithdrawalRepo.Fund_Id))
                {
                }
                else
                {
                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_SCHEME_FUND_ID", WithdrawalRepo.Scheme_Id + WithdrawalRepo.Fund_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_SCHEME_UNITPRICE", param, commandType: CommandType.StoredProcedure);
                    WithdrawalRepo.Unit_Price = param.Get<decimal>("VDATA");             
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //get Employee Scheme-Fund  
        public List<Remit_WithdrawalRepo> GetESFList(string ESF_Id)
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Remit_WithdrawalRepo> ObjFund = new List<Remit_WithdrawalRepo>();

                return ObjFund = db.GetConnection().Query<Remit_WithdrawalRepo>("Select * from VW_REMIT_WITHDRAWAL_ESF WHERE ESF_ID = '" + ESF_Id + "'").ToList();
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


        //get Employee Scheme-Fund  
        public List<Remit_WithdrawalRepo> GetESFPortOutList(string PortOut_No)
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Remit_WithdrawalRepo> ObjFund = new List<Remit_WithdrawalRepo>();

                return ObjFund = db.GetConnection().Query<Remit_WithdrawalRepo>("Select * from REMIT_PORTOUT WHERE portout_no = '" + PortOut_No + "'").ToList();
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


        //check if there is a pending request for employee scheme fund 
        public bool GetcheckESFList(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ESF_ID", ESF_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_WITHDRAWAL_ESF_EXIST", param, commandType: CommandType.StoredProcedure);
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

        //check if there is a pending request for employee scheme fund - TRANSFER TO EMPLOYER
        public bool GetcheckESFListEmp(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ESF_ID", ESF_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_BTOEMP_ESF_EXIST", param, commandType: CommandType.StoredProcedure);
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


        //get list for employer 
        public List<Remit_WithdrawalRepo> GetSFList(string Employer_Id, string Employer_Name)
        {
            //var db = new AppSettings();
            //con = db.GetConnection();
            try
            {

                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {
                    var db = new AppSettings();
                    con = db.GetConnection();
                    List<Remit_WithdrawalRepo> ObjFund = new List<Remit_WithdrawalRepo>();

                    return ObjFund = db.GetConnection().Query<Remit_WithdrawalRepo>("Select * from VW_REMIT_WITHDRAWAL_ESF WHERE PERSONAL_PENSIONS = 'YES'and  ESF_STATUS = 'ACTIVE' and PERSONAL_PENSIONS1 = 'YES'").ToList();


                }
                else
                {
                    var db = new AppSettings();
                    con = db.GetConnection();
                    List<Remit_WithdrawalRepo> ObjFund = new List<Remit_WithdrawalRepo>();

                    return ObjFund = db.GetConnection().Query<Remit_WithdrawalRepo>("Select * from VW_REMIT_WITHDRAWAL_ESF WHERE EMPLOYER_ID = '" + Employer_Id + "' and  ESF_STATUS = 'ACTIVE'").ToList();

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        // FOR PENDING WITHDRAWAL
        public DataSet WPendingData()
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

                cmd.CommandText = "SEL_REMIT_WITHDRAWAL_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "withdrawal");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetWithdrawalPendingList()
        {
            try
            {
                DataSet dt = WPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    Withdrawal_No = row.Field<string>("WITHDRAWAL_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Tax = row.Field<decimal>("Tax"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Withdrawal_Reason = row.Field<string>("WITHDRAWAL_REASON"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Employer_Id = row.Field<String>("EMPLOYER_ID"),
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON"),
                    To_Employer = row.Field<decimal>("to_employer"),
                    To_Employer_Amount = row.Field<decimal>("to_employer") * row.Field<decimal>("UNIT_PRICE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // FOR PENDING BENEFIT TRANSFER
        public DataSet WPendingDataTE()
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

                cmd.CommandText = "SEL_REMIT_TO_EMPLOYER_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "withdrawal");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetWithdrawalPendingListTE()
        {
            try
            {
                DataSet dt = WPendingDataTE();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    Withdrawal_No = row.Field<string>("WITHDRAWAL_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
                    GL_Account_Name = row.Field<string>("GL_ACCOUNT_NAME"),
                    Withdrawal_Reason = row.Field<string>("WITHDRAWAL_REASON"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Employer_Id = row.Field<String>("EMPLOYER_ID"),
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON"),
                    ES_Id = row.Field<String>("ES_ID"),
                    Scheme_Id_2 = row.Field<string>("SCHEME_ID1"),
                    Scheme_Name_2 = row.Field<string>("SCHEME_NAME1"),
                    Employer_Id1 = row.Field<string>("Employer_Id1"),
                    Employer_Name1 = row.Field<string>("Employer_Name1"),


                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // FOR PENDING PORTOUT
        public DataSet PORTOUTPendingData()
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

                cmd.CommandText = "SEL_REMIT_PORTOUT_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "portout");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetPortOutPendingList()
        {
            try
            {
                DataSet dt = PORTOUTPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    PortOut_No = row.Field<string>("PORTOUT_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    New_Trustee = row.Field<string>("TRUSTEE_NAME"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    // Auth_Date = row.Field<DateTime>("AUTH_DATE")
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON")
                    //Reverse_Date = row.Field<DateTime>("REVERSE_DATE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // FOR REVERSE PORTOUT
        public DataSet PORTOUTReverseData()
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

                cmd.CommandText = "SEL_REMIT_PORTOUT_REVERSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "portout");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetPortOutReverseList()
        {
            try
            {
                DataSet dt = PORTOUTReverseData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    PortOut_No = row.Field<string>("PORTOUT_NO"),
                    Trans_Request_Date = row.Field<DateTime?>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    New_Trustee = row.Field<string>("TRUSTEE_NAME"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime?>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON")

                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // FOR PENDING PORTIN
        public DataSet PPendingData()
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

                cmd.CommandText = "SEL_REMIT_PORTIN_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "portin");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetPortInPendingList()
        {
            try
            {
                DataSet dt = PPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    PortIn_No = row.Field<string>("PORTIN_NO"),
                    ESF_Id = row.Field<string>("ESF_ID"),
                    ES_Id = row.Field<string>("ES_ID"),
                    Employer_Id = row.Field<string>("employer_id"),
                    Employee_Id = row.Field<string>("EMPLOYEE_id"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employee_Amount = row.Field<decimal>("EMPLOYEE_AMOUNT"),
                    Employer_Amount = row.Field<decimal>("EMPLOYER_AMOUNT"),
                    Employee_Unit_Purchased = row.Field<decimal>("employee_unit_purchased"),
                    Employer_Unit_Purchased = row.Field<decimal>("employer_unit_purchased"),
                    Previous_Trustee = row.Field<string>("previous_trustee"),
                    Trustee_Name = row.Field<string>("trustee_name"),
                    Trans_Date = row.Field<DateTime>("TRANS_DATE"),
                    GL_Account_No = row.Field<string>("GL_Account_No"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name"),                  
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    // Auth_Date = row.Field<DateTime>("AUTH_DATE")
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON")
                    //Reverse_Date = row.Field<DateTime>("REVERSE_DATE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // FOR REVERSAL -  PORTIN
        public DataSet ReversePPendingData()
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

                cmd.CommandText = "SEL_REMIT_PORTIN_REVERSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "portin");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetPortInReverseList()
        {
            try
            {
                DataSet dt = ReversePPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    PortIn_No = row.Field<string>("PORTIN_NO"),
                    ESF_Id = row.Field<string>("ESF_ID"),
                    ES_Id = row.Field<string>("ES_ID"),
                    Employer_Id = row.Field<string>("employer_id"),
                    Employee_Id = row.Field<string>("EMPLOYEE_id"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employee_Amount = row.Field<decimal>("EMPLOYEE_AMOUNT"),
                    Employer_Amount = row.Field<decimal>("EMPLOYER_AMOUNT"),
                    Employee_Unit_Purchased = row.Field<decimal>("employee_unit_purchased"),
                    Employer_Unit_Purchased = row.Field<decimal>("employer_unit_purchased"),
                    Previous_Trustee = row.Field<string>("previous_trustee"),
                    Trustee_Name = row.Field<string>("trustee_name"),
                    Trans_Date = row.Field<DateTime>("TRANS_DATE"),
                    GL_Account_No = row.Field<string>("GL_Account_No"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    // Auth_Date = row.Field<DateTime>("AUTH_DATE")
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON")
                    //Reverse_Date = row.Field<DateTime>("REVERSE_DATE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // FOR PENDING BENEFIT PAYMENT
        public DataSet WpPendingData()
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

                cmd.CommandText = "SEL_REMIT_WITHDRAWAL_APPROVED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "withdrawal");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetWithdrawalApprovedList()
        {
            try
            {
                DataSet dt = WpPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    Withdrawal_No = row.Field<string>("WITHDRAWAL_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Tax = row.Field<decimal>("Tax"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Withdrawal_Reason = row.Field<string>("WITHDRAWAL_REASON"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                     To_Employer = row.Field<decimal>("To_Employer"),
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON"),
                    Employer_Id = row.Field<string>("Employer_Id")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        // FOR REVERSAL- BENEFIT PAYMENT
        public DataSet ReverseWpPendingData()
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

                cmd.CommandText = "SEL_REMIT_WITHDRAWAL_REVERSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "withdrawalreverse");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetWithdrawalReverseList()
        {
            try
            {
                DataSet dt = ReverseWpPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    Withdrawal_No = row.Field<string>("WITHDRAWAL_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Accrued_Benefit = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Tax = row.Field<decimal>("Tax"),
                    Pay_Benefit = row.Field<string>("pay_benefit"),
                    Pay_Tax = row.Field<string>("PAY_TAX"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Withdrawal_Reason = row.Field<string>("WITHDRAWAL_REASON"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Pay_Date_Benefit = row.Field<DateTime>("PAY_DATE_BENEFIT"),
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name")


                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // FOR REVERSAL- BENEFIT TRANSFER
        public DataSet ReverseWpPendingDataTE()
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

                cmd.CommandText = "SEL_REMIT_WITHDRAWAL_REVERSETE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "withdrawalreverse");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetWithdrawalReverseListTE()
        {
            try
            {
                DataSet dt = ReverseWpPendingDataTE();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    Withdrawal_No = row.Field<string>("WITHDRAWAL_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    GL_Account_No = row.Field<string>("GL_ACCOUNT_NO"),
                    GL_Account_Name = row.Field<string>("GL_ACCOUNT_NAME"),
                    Withdrawal_Reason = row.Field<string>("WITHDRAWAL_REASON"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Employer_Id = row.Field<String>("EMPLOYER_ID"),
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON"),
                    ES_Id = row.Field<String>("ES_ID"),
                    Scheme_Id_2 = row.Field<string>("SCHEME_ID1"),
                    Scheme_Name_2 = row.Field<string>("SCHEME_NAME1"),
                    Employer_Id1 = row.Field<string>("Employer_Id1"),
                    Employer_Name1 = row.Field<string>("Employer_Name1"),


                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // FOR PENDING PORTOUT PAYMENT
        public DataSet WportoutPendingData()
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

                cmd.CommandText = "SEL_REMIT_PORTOUT_APPROVED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "portout");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetPortOutApprovedList()
        {
            try
            {
                DataSet dt = WportoutPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    PortOut_No = row.Field<string>("PORTOUT_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Trustee_Name = row.Field<string>("TRUSTEE_NAME"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    // Auth_Date = row.Field<DateTime>("AUTH_DATE")
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON")
                    //Reverse_Date = row.Field<DateTime>("REVERSE_DATE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        // FOR PENDING TAX PAYMENT
        public DataSet WpPendingDataTax()
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

                cmd.CommandText = "SEL_REMIT_WITHDRAWAL_APP_TAX";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "withdrawal");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_WithdrawalRepo> GetWithdrawalApprovedListTax()
        {
            try
            {
                DataSet dt = WpPendingDataTax();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_WithdrawalRepo
                {
                    ESF_Id = row.Field<string>("ESF_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Unit_Price = row.Field<decimal>("UNIT_PRICE"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Total_Unit_Balance = row.Field<decimal>("TOTAL_UNIT_BALANCE"),
                    Total_Con_Balance = row.Field<decimal>("TOTAL_CON_BALANCE"),
                    Withdrawal_No = row.Field<string>("WITHDRAWAL_NO"),
                    Trans_Request_Date = row.Field<DateTime>("TRANS_REQUEST_DATE"),
                    Employer_Con_Balance = row.Field<decimal>("TOTAL_CONTRIBUTION"),
                    Employee_Con_Balance = row.Field<decimal>("ACCRUED_BENEFIT"),
                    Total_Withdrawal_Unit = row.Field<decimal>("TOTAL_WITHDRAWAL_UNIT"),
                    Total_Withdrawal_Amount = row.Field<decimal>("TOTAL_WITHDRAWAL_AMOUNT"),
                    Employer_Unit_Balance = row.Field<decimal>("EMPLOYER_UNIT"),
                    Employee_Unit_Balance = row.Field<decimal>("EMPLOYEE_UNIT"),
                    Tax = row.Field<decimal>("Tax"),
                    Payment_Mode = row.Field<string>("PAYMENT_MODE"),
                    Instrument_No = row.Field<string>("INSTRUMENT_NO"),
                    Withdrawal_Reason = row.Field<string>("WITHDRAWAL_REASON"),
                    Trans_Status = row.Field<string>("TRANS_STATUS"),
                    Maker_Id = row.Field<string>("MAKER_ID"),
                    Make_Date = row.Field<DateTime>("MAKE_DATE"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    // Auth_Date = row.Field<DateTime>("AUTH_DATE")
                    Reversal_Status = row.Field<string>("REVERSE_STATUS"),
                    Reverse_Id = row.Field<string>("REVERSE_ID"),
                    Reverse_Reason = row.Field<string>("REVERSE_REASON")
                    //Reverse_Date = row.Field<DateTime>("REVERSE_DATE")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }

        }


        //CALL PURCHASE PENDING RECORD
        public List<Remit_WithdrawalRepo> GetWPendingList(string Withdrawal_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_WithdrawalRepo> bn = new List<Remit_WithdrawalRepo>();

                string query = "Select * from REMIT_WITHDRAWAL_REQUEST WHERE WITHDRAWAL_NO = '" + Withdrawal_No + "'";
                return bn = con.Query<Remit_WithdrawalRepo>(query).ToList();

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

        //CALL PORT OUT PENDING RECORD
        public List<Remit_WithdrawalRepo> GetPortOutPendingList(string PortOut_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<Remit_WithdrawalRepo> bn = new List<Remit_WithdrawalRepo>();

                string query = "Select * from REMIT_PORTOUT WHERE PORTOUT_NO = '" + PortOut_No + "'";
                return bn = con.Query<Remit_WithdrawalRepo>(query).ToList();

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


        //GET GL BALANCE FROM GL ACCOUNT TABLE
        public void Get_GL_Balance(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_GL_ACCOUNT_NO", WithdrawalRepo.GL_Account_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_GL_BALANCE", param, commandType: CommandType.StoredProcedure);
                WithdrawalRepo.GL_Balance = param.Get<decimal>("VDATA");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
