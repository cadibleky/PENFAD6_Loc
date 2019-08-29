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
    public class Remit_Unit_TransferRepo
    {
        [Required]
        public string To_ESF_Id { get; set; }
		public string ESF_Id { get; set; }
		public string To_Surname { get; set; }
        public string To_First_Name { get; set; }
        public string To_SSNIT_No { get; set; }
        public DateTime? To_Date_Of_Birth { get; set; }
        [Required]
        public string From_ESF_Id { get; set; }
        public string From_Surname { get; set; }
        public string From_First_Name { get; set; }
        public string From_SSNIT_No { get; set; }
        public DateTime? From_Date_Of_Birth { get; set; }
        public string ES_Id { get; set; }
        public string Employer_Id { get; set; }
        public string Employer_Name { get; set; }
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
		public string Fund_Id { get; set; }
		public string Fund_Name { get; set; }
		public string Con_Log_Id { get; set; }
        public decimal Employer_Amt { get; set; }
        public decimal Employee_Amt { get; set; }
        public decimal Employer_Units { get; set; }
        public decimal Employee_Units { get; set; }
        public decimal Unit_Price { get; set; }
        public string Purchase_Type { get; set; }
        [Required]
        public string Reason_Transfer { get; set; }
        [Required]
        public decimal For_Year { get; set; }
        [Required]
        public decimal For_Month { get; set; }
        public decimal Total_Contribution { get; set; }
        public decimal Total_Purchase { get; set; }
        public decimal Con_Balance { get; set; }
        public decimal Total_Surcharge { get; set; }
        public decimal Total_Sur_Purchase { get; set; }
        public decimal Sur_Balance { get; set; }
        public decimal Total_Balance { get; set; }
        [Required]
        public string Purchase_Log_Id { get; set; }
        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public DateTime Auth_Date { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public decimal Temp_Con_Purchase { get; set; }
        public decimal Temp_Sur_Purchase { get; set; }
        public DateTime Working_Date { get; set; }
        public DateTime Today_Date { get; set; }
        public string From_Name { get; set; }
        public string To_Name { get; set; }
        [Required]
        public string Scheme_Fund_Id { get; set; }
        public int TID { get; set; }

        IDbConnection conn;
        public void SaveRecord(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_PURCHASE_LOG_ID", value: Unit_TransferRepo.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_LOG_ID", value: Unit_TransferRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FROM_ESF_ID", value: Unit_TransferRepo.From_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TO_ESF_ID", value: Unit_TransferRepo.To_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_AMT", value: Unit_TransferRepo.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_AMT", value: Unit_TransferRepo.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_UNITS", value: Unit_TransferRepo.Employer_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_UNITS", value: Unit_TransferRepo.Employee_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_UNIT_PRICE", value: Unit_TransferRepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_PURCHASE_TYPE", value: Unit_TransferRepo.Purchase_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FROM_NAME", value: Unit_TransferRepo.From_Surname + " " + Unit_TransferRepo.From_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TO_NAME", value: Unit_TransferRepo.To_Surname + " " + Unit_TransferRepo.To_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FOR_MONTH", value: Unit_TransferRepo.For_Month, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FOR_YEAR", value: Unit_TransferRepo.For_Year, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REASON_TRANSFER", value: Unit_TransferRepo.Reason_Transfer, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                conn.Execute("ADD_REMIT_UNIT_TRANSFER_EMP", param, commandType: CommandType.StoredProcedure);
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

        public bool Approve_Unit_Transfer(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            var app = new AppSettings();

            // get the pending purchase record
            Unit_TransferRepo.GetPendingUTList(Unit_TransferRepo);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {

                    // Update remit_unit_transfer table   
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_TID", value: TID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_REC_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    conn.Execute("APP_REMIT_UNIT_TRANSFER", param, commandType: CommandType.StoredProcedure);


                    //Update remit_unit_purchase_trans table
                    DynamicParameters param_conl = new DynamicParameters();
                    param_conl.Add(name: "P_PURCHASE_LOG_ID", value: Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_PURCHASE_TYPE", value: Purchase_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_FROM_ESF_ID", value: From_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_TO_ESF_ID", value: To_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_EMPLOYER_AMT", value: Unit_TransferRepo.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_conl.Add(name: "P_EMPLOYEE_AMT", value: Unit_TransferRepo.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_UNIT_PUR_TRANS_UT", param_conl, commandType: CommandType.StoredProcedure);


                    //Update Employee_Scheme_Fund table
                    DynamicParameters param_cash = new DynamicParameters();
                    param_cash.Add(name: "P_FROM_ESF_ID", value: From_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_TO_ESF_ID", value: To_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_EMPLOYER_AMT", value: Unit_TransferRepo.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_EMPLOYEE_AMT", value: Unit_TransferRepo.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_EMPLOYER_UNITS", value: Unit_TransferRepo.Employer_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param_cash.Add(name: "P_EMPLOYEE_UNITS", value: Unit_TransferRepo.Employee_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    conn.Execute("UPD_REMIT_ESF_TRANSFER", param_cash, commandType: CommandType.StoredProcedure);

                   
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

        // merge employee accounts
        public bool Approve_Unit_Merge(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            var app = new AppSettings();

            // get the pending purchase record
            Unit_TransferRepo.GetPendingUTList(Unit_TransferRepo);

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {

                try
                {

                
                    DynamicParameters param = new DynamicParameters();
                    
                    param.Add(name: "P_FROM_ESF_ID", value: Unit_TransferRepo.From_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TO_ESF_ID", value: Unit_TransferRepo.To_ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_REASON_TRANSFER", value: Unit_TransferRepo.Reason_Transfer, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FROM_NAME", value: Unit_TransferRepo.From_Surname + " " + Unit_TransferRepo.From_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TO_NAME", value: Unit_TransferRepo.To_Surname + " " + Unit_TransferRepo.To_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("ADD_REMIT_UNIT_MERGE_EMP", param, commandType: CommandType.StoredProcedure);


                   
                  
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


        // Disapprove unit transfer
        public void DisapproveUTRecord(string TID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_TID", value: TID, dbType: DbType.String, direction: ParameterDirection.Input);
                conn.Execute("DEL_REMIT_UNIT_TRANSFER", param, commandType: CommandType.StoredProcedure);
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

        // public bool isPurchaseUnique(string CON_LOG_ID)
        // {
        //     try
        //     {
        //         //Get connection
        //         var con = new AppSettings();
        //         var param = new DynamicParameters();
        //         param.Add("P_CON_LOG_ID", CON_LOG_ID, DbType.String, ParameterDirection.Input);
        //         param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
        //         con.GetConnection().Execute("SEL_REMIT_PURCHASE_EXIST", param, commandType: CommandType.StoredProcedure);
        //         int paramoption = param.Get<int>("VDATA");

        //         if (paramoption == 0)
        //             return false;
        //         else
        //             return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }

        // }

      


        // list for employee accounts
        public DataSet ESFData(string Scheme_Fund_Id)
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

                cmd.CommandText = "SEL_REMIT_ESF_ACTIVE_TRANSFER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)conn;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;


                param = cmd.Parameters.Add("P_SCHEME_FUND_ID", OracleDbType.Varchar2, Scheme_Fund_Id, ParameterDirection.Input);


                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "unit_transfer");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Remit_Unit_TransferRepo> GetESFList(string Scheme_Fund_Id)
        {
            try
            {
                DataSet dt = ESFData(Scheme_Fund_Id);
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_Unit_TransferRepo
                {
                    From_ESF_Id = row.Field<string>("ESF_ID"),
                    From_Surname = row.Field<string>("SURNAME"),
                    From_First_Name = row.Field<string>("FIRST_NAME"),
                    From_Date_Of_Birth = row.Field<DateTime?>("DATE_OF_BIRTH"),
                    From_SSNIT_No = row.Field<string>("SSNIT_NO"),
                    To_ESF_Id = row.Field<string>("ESF_ID"),
                    To_Surname = row.Field<string>("SURNAME"),
                    To_First_Name = row.Field<string>("FIRST_NAME"),
                    To_Date_Of_Birth = row.Field<DateTime?>("DATE_OF_BIRTH"),
                    To_SSNIT_No = row.Field<string>("SSNIT_NO"),

					//Scheme_Fund_Id = row.Field<string>("Scheme_Fund_Id"),
					//Scheme_Name = row.Field<string>("SCHEME_NAME"),
					//Fund_Name = row.Field<string>("Fund_Name"),


				}).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
            }



        }

		// list for employee accounts
		public DataSet ESFData_change2()
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

				cmd.CommandText = "SEL_REMIT_ESF_ACTIVE_TRANSFER";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Connection = (OracleConnection)conn;

				param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
				param.Direction = ParameterDirection.Output;


				param = cmd.Parameters.Add("P_SCHEME_FUND_ID", OracleDbType.Varchar2, Scheme_Fund_Id, ParameterDirection.Input);


				da = new OracleDataAdapter(cmd);
				da.Fill(ds, "unit_transfer");
				return ds;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public IEnumerable<Remit_Unit_TransferRepo> GetESFList2()
		{
			try
			{
				DataSet dt = ESFData_change2();
				var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_Unit_TransferRepo
				{
					From_ESF_Id = row.Field<string>("ESF_ID"),
					From_Surname = row.Field<string>("SURNAME"),
					From_First_Name = row.Field<string>("FIRST_NAME"),
					From_Date_Of_Birth = row.Field<DateTime?>("DATE_OF_BIRTH"),
					From_SSNIT_No = row.Field<string>("SSNIT_NO"),
					To_ESF_Id = row.Field<string>("ESF_ID"),
					To_Surname = row.Field<string>("SURNAME"),
					To_First_Name = row.Field<string>("FIRST_NAME"),
					To_Date_Of_Birth = row.Field<DateTime?>("DATE_OF_BIRTH"),
					To_SSNIT_No = row.Field<string>("SSNIT_NO"),

					Scheme_Fund_Id = row.Field<string>("Scheme_Fund_Id"),
					Scheme_Name = row.Field<string>("SCHEME_NAME"),
					Fund_Name = row.Field<string>("Fund_Name"),


				}).ToList();

				return eList;
			}
			catch (Exception)
			{
				throw;
			}



		}

		//// FOR ACTIVE RECEIPTS
		//public DataSet ReceiptActiveData()
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

		//        cmd.CommandText = "SEL_REMIT_RECEIPT_ACTIVE";
		//        cmd.CommandType = CommandType.StoredProcedure;
		//        cmd.Connection = (OracleConnection)con;

		//        param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
		//        param.Direction = ParameterDirection.Output;

		//        da = new OracleDataAdapter(cmd);
		//        da.Fill(ds, "receipt");
		//        return ds;
		//    }
		//    catch (Exception)
		//    {
		//        throw;
		//    }

		// public IEnumerable<Remit_ReceiptRepo> GetReceiptActiveList()
		// {
		//     try
		//     {
		//         DataSet dt = ReceiptActiveData();
		//         var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
		//         {
		//             ES_Id = row.Field<string>("ES_ID"),
		//             Scheme_Id = row.Field<string>("SCHEME_ID"),
		//             Scheme_Name = row.Field<string>("SCHEME_NAME"),
		//             Employer_Id = row.Field<string>("EMPLOYER_ID"),
		//             Employer_Name = row.Field<string>("EMPLOYER_NAME"),
		//             Receipt_Id = row.Field<string>("RECEIPT_ID"),
		//             Trans_Amount = row.Field<Decimal>("TRANS_AMOUNT"),
		//             Actual_Receipt_Date = row.Field<DateTime>("ACTUAL_RECEIPT_DATE"),
		//             Narration = row.Field<string>("NARRATION"),
		//             Narration_Syatem = row.Field<string>("NARRATION_SYSTEM"),
		//             Payment_Mode = row.Field<string>("PAYMENT_MODE"),
		//             Instrument_No = row.Field<string>("INSTRUMENT_NO"),
		//             Receipt_Status = row.Field<string>("RECEIPT_STATUS"),
		//             Auth_Status = row.Field<string>("AUTH_STATUS")
		//         }).ToList();

		//         return eList;
		//     }
		//     catch (Exception)
		//     {
		//         throw;
		//     }

		// }

		// // FOR PENDING REVERSED RECEIPTS
		// public DataSet ReverseReceiptData()
		// {
		//     try
		//     {
		//         //Get connection
		//         var app = new AppSettings();
		//         con = app.GetConnection();

		//         DataSet ds = new DataSet();

		//         OracleDataAdapter da = new OracleDataAdapter();
		//         OracleCommand cmd = new OracleCommand();
		//         OracleParameter param = cmd.CreateParameter();

		//         cmd.CommandText = "SEL_REMIT_RECEIPT_R_PENDING";
		//         cmd.CommandType = CommandType.StoredProcedure;
		//         cmd.Connection = (OracleConnection)con;

		//         param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
		//         param.Direction = ParameterDirection.Output;

		//         da = new OracleDataAdapter(cmd);
		//         da.Fill(ds, "receipt");
		//         return ds;
		//     }
		//     catch (Exception)
		//     {
		//         throw;
		//     }
		// }
		// public IEnumerable<Remit_ReceiptRepo> GetReverseReceiptList()
		// {
		//     try
		//     {
		//         DataSet dt = ReverseReceiptData();
		//         var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_ReceiptRepo
		//         {
		//             ES_Id = row.Field<string>("ES_ID"),
		//             Scheme_Id = row.Field<string>("SCHEME_ID"),
		//             Scheme_Name = row.Field<string>("SCHEME_NAME"),
		//             Employer_Id = row.Field<string>("EMPLOYER_ID"),
		//             Employer_Name = row.Field<string>("EMPLOYER_NAME"),
		//             Receipt_Id = row.Field<string>("RECEIPT_ID"),
		//             Trans_Amount = row.Field<Decimal>("TRANS_AMOUNT"),
		//             Actual_Receipt_Date = row.Field<DateTime>("ACTUAL_RECEIPT_DATE"),
		//             Narration = row.Field<string>("NARRATION"),
		//             Narration_Syatem = row.Field<string>("NARRATION_SYSTEM"),
		//             Payment_Mode = row.Field<string>("PAYMENT_MODE"),
		//             Instrument_No = row.Field<string>("INSTRUMENT_NO"),
		//             Receipt_Status = row.Field<string>("RECEIPT_STATUS"),
		//             Auth_Status = row.Field<string>("AUTH_STATUS")
		//         }).ToList();

		//         return eList;
		//     }
		//     catch (Exception)
		//     {
		//         throw;
		//     }

		// }

		// public DataSet PurchaseESData()
		// {
		//     try
		//     {
		//         //Get connection
		//         var app = new AppSettings();
		//         con = app.GetConnection();

		//         DataSet ds = new DataSet();

		//         OracleDataAdapter da = new OracleDataAdapter();
		//         OracleCommand cmd = new OracleCommand();
		//         OracleParameter param = cmd.CreateParameter();

		//         cmd.CommandText = "SEL_REMIT_RECEIPT_EMPLOYER";
		//         cmd.CommandType = CommandType.StoredProcedure;
		//         cmd.Connection = (OracleConnection)con;

		//         param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
		//         param.Direction = ParameterDirection.Output;

		//         da = new OracleDataAdapter(cmd);
		//         da.Fill(ds, "purchase");
		//         return ds;
		//     }
		//     catch (Exception)
		//     {
		//         throw;
		//     }
		// }
		// public IEnumerable<Remit_PurchaseRepo> GetPurchaseESList()
		// {
		//     try
		//     {
		//         DataSet dt = PurchaseESData();
		//         var eList = dt.Tables[0].AsEnumerable().Select(row => new Remit_PurchaseRepo
		//         {
		//             ES_Id = row.Field<string>("ES_ID"),
		//             Scheme_Id = row.Field<string>("SCHEME_ID"),
		//             Scheme_Name = row.Field<string>("SCHEME_NAME"),
		//             Employer_Id = row.Field<string>("EMPLOYER_ID"),
		//             Employer_Name = row.Field<string>("EMPLOYER_NAME"),
		//             Cash_Balance = row.Field<decimal>("CASH_BALANCE"),
		//             Today_Date = row.Field<DateTime>("TODAY_DATE")

		//         }).ToList();

		//         return eList;
		//     }
		//     catch (Exception)
		//     {
		//         throw;
		//     }

		// }

		// public bool isYearMonthValid(Remit_PurchaseRepo PurchaseRepo)
		// {
		//     try
		//     {
		//         //Get connection
		//         var con = new AppSettings();
		//         var param = new DynamicParameters();
		//         param.Add("P_FOR_YEAR", For_Year, DbType.Decimal, ParameterDirection.Input);
		//         param.Add("P_FOR_MONTH", For_Month, DbType.Decimal, ParameterDirection.Input);
		//         param.Add("P_ES_ID", ES_Id, DbType.String, ParameterDirection.Input);
		//         param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
		//         con.GetConnection().Execute("SEL_REMIT_YEAR_MONTH", param, commandType: CommandType.StoredProcedure);
		//         int paramoption = param.Get<int>("VDATA");

		//         if (paramoption == 0)
		//             return false;
		//         else
		//             return true;
		//     }
		//     catch (Exception ex)
		//     {
		//         throw ex;
		//     }

		//GET UNIT TRANSFER  PENDING
		public List<Remit_Unit_TransferRepo> GetPendingUTList(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {
                var app = new AppSettings();
                conn = app.GetConnection();
                List<Remit_Unit_TransferRepo> bn = new List<Remit_Unit_TransferRepo>();

                string query = "Select * from REMIT_UNIT_TRANSFER_LOG WHERE AUTH_STATUS = 'PENDING' ";
                return bn = conn.Query<Remit_Unit_TransferRepo>(query).ToList();

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



        //GET PURCHASE  RECORDS
        public List<Remit_Unit_TransferRepo> GetPurchasesList(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {
                var app = new AppSettings();
                conn = app.GetConnection();
                List<Remit_Unit_TransferRepo> bn = new List<Remit_Unit_TransferRepo>();

                string query = "Select * from VW_ESF_UNIT_TRANSFER WHERE SCHEME_FUND_ID = '" + Unit_TransferRepo.Scheme_Fund_Id + "' and ESF_ID = '" + Unit_TransferRepo.From_ESF_Id + "'  ";
                return bn = conn.Query<Remit_Unit_TransferRepo>(query).ToList();

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

		public List<Remit_Unit_TransferRepo> GetPurchasesList2(Remit_Unit_TransferRepo Unit_TransferRepo)
		{
			try
			{
				var app = new AppSettings();
				conn = app.GetConnection();
				List<Remit_Unit_TransferRepo> bn = new List<Remit_Unit_TransferRepo>();

				string query = "Select * from VW_ESF_UNIT_TRANSFER WHERE SCHEME_FUND_ID = '" + Unit_TransferRepo.Scheme_Fund_Id + "' and ESF_ID = '" + Unit_TransferRepo.ESF_Id + "'  ";
				return bn = conn.Query<Remit_Unit_TransferRepo>(query).ToList();

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

		public void DelRecord(Remit_Unit_TransferRepo Unit_TransferRepo)
		{
			try
			{
				//Get Connection
				AppSettings app = new AppSettings();
				conn = app.GetConnection();
				DynamicParameters param = new DynamicParameters();
				param.Add(name: "P_PURCHASE_LOG_ID", value: Unit_TransferRepo.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_CON_LOG_ID", value: Unit_TransferRepo.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_ESF_ID", value: Unit_TransferRepo.ESF_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
				conn.Execute("ADD_REMIT_UNIT_DEL_EMP", param, commandType: CommandType.StoredProcedure);
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

	}
}

