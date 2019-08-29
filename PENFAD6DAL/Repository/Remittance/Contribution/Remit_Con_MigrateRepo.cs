using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Data;

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    public class Remit_Con_Log
    {
        string plog;
        string clog;
        public string Con_Log_Id { get; set; }
        public string Employer_Id { get; set; }
        public string Scheme_Id { get; set; }
        public string ES_Id { get; set; }
        public int For_Month { get; set; }
        public int For_Year { get; set; }
        public double Total_Contribution { get; set; }
        public string Log_Status { get; set; }
        public DateTime? Make_Date { get; set; }
        public string Purchase_Log_Id { get; set; }
        public string batchnoA { get; set; }
        AppSettings db = new AppSettings();
       
        public string Create_Con_Log(Remit_Con_Log remitconlog)
        {
            

            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlog.For_Month;
                }
                else
                {
                    clog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////

                ///create log for the upload Remit Log
                var paramb = new DynamicParameters();
                string batchno = "";
                paramb.Add(name: "p_Con_Log_Id ", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Employer_Id", value: remitconlog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ES_Id", value: remitconlog.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Month", value: clog, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Year", value: remitconlog.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_DeadLine_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_ALL_CON_LOG", param: paramb, commandType: CommandType.StoredProcedure);

                batchno = paramb.Get<string>("p_Con_Log_Id ");
                return batchno;
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

        public string Create_Con_Log_ER(Remit_Con_Log remitconlog)
        {


            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlog.For_Month;
                }
                else
                {
                    clog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////

                ///create log for the upload Remit Log
                var paramb = new DynamicParameters();
                string batchno = "";
                paramb.Add(name: "p_Con_Log_Id ", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Employer_Id", value: remitconlog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ES_Id", value: remitconlog.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Month", value: clog, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Year", value: remitconlog.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_DeadLine_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                db.GetConnection().Execute(sql: "ADD_MIGRATE_ALL_CON_LOG_ER", param: paramb, commandType: CommandType.StoredProcedure);

                batchno = paramb.Get<string>("p_Con_Log_Id ");
                return batchno;
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


        // for back pay
        public string Create_BP_Log_ER(Remit_Con_Log remitconlog)
        {


            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlog.For_Month;
                }
                else
                {
                    clog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////

                ///create log for the upload Remit Log
                var paramb = new DynamicParameters();
                string batchno = "";
                paramb.Add(name: "p_Con_Log_Id ", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Employer_Id", value: remitconlog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ES_Id", value: remitconlog.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Month", value: clog, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Year", value: remitconlog.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_DeadLine_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Con_Type", value: "BACK-PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                db.GetConnection().Execute(sql: "ADD_MIGRATE_ALL_BP_LOG_ER", param: paramb, commandType: CommandType.StoredProcedure);

                batchno = paramb.Get<string>("p_Con_Log_Id ");
                return batchno;
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


        public string Create_Con_Log2(Remit_Con_Log remitconlog)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlog.For_Month;
                }
                else
                {
                    clog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////

                ///create log for the upload Remit Log
                var paramb = new DynamicParameters();
                string batchno = "";
                paramb.Add(name: "p_Con_Log_Id", value: "CON" + remitconlog.ES_Id + remitconlog.For_Year + clog + "10", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Employer_Id", value: remitconlog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ES_Id", value: remitconlog.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Month", value: clog, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Year", value: remitconlog.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_DeadLine_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Con_Type", value: "TPFA", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_ALL_CON_LOG3", param: paramb, commandType: CommandType.StoredProcedure);

               // batchno = paramb.Get<string>("o_Con_Log_Id ");
                return batchno;
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

        public string Create_Con_Log_BP(Remit_Con_Log remitconlog)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlog.For_Month;
                }
                else
                {
                    clog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////

                ///create log for the upload Remit Log
                var paramb = new DynamicParameters();
                string batchno = "";
                paramb.Add(name: "p_Con_Log_Id", value: "CON" + remitconlog.ES_Id + remitconlog.For_Year + clog + "05", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Employer_Id", value: remitconlog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ES_Id", value: remitconlog.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Month", value: clog, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_For_Year", value: remitconlog.For_Year, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_DeadLine_Date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                paramb.Add(name: "p_ISINARREAS_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Unit_Purchased_YesNo", value: "NO", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Total_Contribution", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Log_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_GracePeriod", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
                paramb.Add(name: "p_Con_Type", value: "BACKPAY-CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                paramb.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_ALL_CON_LOG3", param: paramb, commandType: CommandType.StoredProcedure);

                // batchno = paramb.Get<string>("o_Con_Log_Id ");
                return batchno;
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

        public string PurchaseSaveRecord(Remit_Con_Log remitconlog)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    plog = "0" + remitconlog.For_Month;
                }
                else
                {
                    plog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////


                //Get Connection
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_PURCHASE_LOG_ID", value: remitconlog.ES_Id + remitconlog.For_Year + plog, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_LOG_ID", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute("ADD_MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);
                return this.Purchase_Log_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            
                {
                    db.Dispose();
                }
            }
        }

        //for back pay
        public string BPPurchaseSaveRecord(Remit_Con_Log remitconlog)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    plog = "0" + remitconlog.For_Month;
                }
                else
                {
                    plog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////


                //Get Connection
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_PURCHASE_LOG_ID", value: remitconlog.ES_Id + remitconlog.For_Year + plog, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_LOG_ID", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_DATE", value: DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "BACK-PAY", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute("ADD_MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);
                return this.Purchase_Log_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                {
                    db.Dispose();
                }
            }
        }


        public string PurchaseSaveRecord2(Remit_Con_Log remitconlog)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    plog = "0" + remitconlog.For_Month;
                }
                else
                {
                    plog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////


                //Get Connection
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_PURCHASE_LOG_ID", value: remitconlog.ES_Id + remitconlog.For_Year + plog + "10", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_LOG_ID", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_DATE", value: DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "TPFA", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute("ADD_MIGRATE_UNIT_PURCHASES10", param, commandType: CommandType.StoredProcedure);
                return this.Purchase_Log_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                {
                    db.Dispose();
                }
            }
        }

        public string PurchaseSaveRecord_BP(Remit_Con_Log remitconlog)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlog.For_Month.ToString().Length == 1)
                {
                    plog = "0" + remitconlog.For_Month;
                }
                else
                {
                    plog = remitconlog.For_Month.ToString();
                }
                ///////////////////////////////////////////////////


                //Get Connection
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_PURCHASE_LOG_ID", value: remitconlog.ES_Id + remitconlog.For_Year + plog + "05", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CON_LOG_ID", value: remitconlog.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TRANS_DATE", value: DateTime.Now, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "BACKPAY-CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlog.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute("ADD_MIGRATE_UNIT_PURCHASES10", param, commandType: CommandType.StoredProcedure);
                return this.Purchase_Log_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                {
                    db.Dispose();
                }
            }
        }

        public void PaymentSaveRecord(Remit_Con_Log remit_con_logrepo)
        {
            try
            {
                //Get Connection
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_ES_ID", value: remit_con_logrepo.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ACTUAL_RECEIPT_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                db.GetConnection().Execute("MIGRATE_REMIT_RECEIPT", param, commandType: CommandType.StoredProcedure);
               // return this.ES_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                {
                    db.Dispose();
                }
            }
        }


    }

    public class Remit_Con_Log_Details
    {
        string clog;
        public string Con_Log_Id { get; set;}
        public string Esf_Id { get; set; }
        public string ES_Id { get; set; }
        public string Employee_Id { get; set; }
        public int For_Month { get; set; }
        public int For_Year { get; set; }
        public decimal Employer_Con { get; set; }
        public decimal Employee_Con { get; set; }
        public decimal Employee_Amt_Used { get; set; }
        public decimal Employer_Amt_Used { get; set; }
        public decimal Employee_Sal_Rate { get; set; }
        public string Req_Status { get; set; }
        public string Con_Type { get; set; }
        public decimal Employer_Amt { get; set; }
        public decimal Employee_Amt { get; set; }
        public string Purchase_Log_Id { get; set; }
        public string Trustee_Name { get; set; }

        AppSettings db = new AppSettings();
        
        public void Create_Con_Log_Details(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_Employee_Id", value: remitconlogDetails.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Log_Id", value: remitconlogDetails.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Con", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Con", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt_Used", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Salary", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlogDetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_REMIT_CON_DETAILS", param: param, commandType: CommandType.StoredProcedure);

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

        public void Create_Con_Log_Details2(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlogDetails.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlogDetails.For_Month;
                }
                else
                {
                    clog = remitconlogDetails.For_Month.ToString();
                }
                ///////////////////////////////////////////////////


                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_Employee_Id", value: remitconlogDetails.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Log_Id", value: remitconlogDetails.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Con", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Con", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt_Used", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Salary", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Type", value: "TPFA", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trustee_Name", value: remitconlogDetails.Trustee_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlogDetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_REMIT_CON_DETAILS_TRANS", param: param, commandType: CommandType.StoredProcedure);

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

        public void Create_Con_Log_Details_BP(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                ////////////////////////////////////////////////
                if (remitconlogDetails.For_Month.ToString().Length == 1)
                {
                    clog = "0" + remitconlogDetails.For_Month;
                }
                else
                {
                    clog = remitconlogDetails.For_Month.ToString();
                }
                ///////////////////////////////////////////////////


                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_Employee_Id", value: remitconlogDetails.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Log_Id", value: remitconlogDetails.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Con", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Con", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt_Used", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt_Used", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Salary", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Type", value: "BACKPAY-CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Trustee_Name", value: "", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlogDetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_REMIT_CON_DETAILS_TRANS", param: param, commandType: CommandType.StoredProcedure);

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

        public void Create_Con_Log_Details_employer(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_Employee_Id", value: remitconlogDetails.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Log_Id", value: remitconlogDetails.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Con", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Con", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt_Used", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt_Used", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Salary", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_CON_DETAILS", param: param, commandType: CommandType.StoredProcedure);

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

        // FOR BACK PAY
        public void Create_BP_Log_Details_employer(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_Employee_Id", value: remitconlogDetails.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Log_Id", value: remitconlogDetails.Con_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Con", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Con", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Bal", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt_Used", value: remitconlogDetails.Employer_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt_Used", value: remitconlogDetails.Employee_Con, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Salary", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Sal_Rate", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Con", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Difference", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Req_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Con_Type", value: "BACK-PAY", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_CON_DETAILS", param: param, commandType: CommandType.StoredProcedure);

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

        public void Create_Unit_Log_Details_Employer(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: remitconlogDetails.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: remitconlogDetails.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Log_Id", value: remitconlogDetails.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
    
                db.GetConnection().Execute(sql: "ADD_MIGRATE_CON_EMPLOYER", param: param, commandType: CommandType.StoredProcedure);
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
        public void Create_Unit_Log_Details(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: remitconlogDetails.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: remitconlogDetails.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Log_Id", value: remitconlogDetails.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "MAIN_CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlogDetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_CON", param: param, commandType: CommandType.StoredProcedure);
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
        //////
        public void Create_Unit_Log_Details2(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: remitconlogDetails.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: remitconlogDetails.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Log_Id", value: remitconlogDetails.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "TPFA", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlogDetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_CON", param: param, commandType: CommandType.StoredProcedure);
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

        public void Create_Unit_Log_Details_BP(Remit_Con_Log_Details remitconlogDetails)
        {
            try
            {
                var param = new DynamicParameters();
                ///create log for the upload Remit Log
                param.Add(name: "p_ESF_ID", value: remitconlogDetails.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Amt", value: remitconlogDetails.Employer_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Amt", value: remitconlogDetails.Employee_Amt, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Log_Id", value: remitconlogDetails.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "BACKPAY-CONTRIBUTION", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: remitconlogDetails.ES_Id + "01", dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "ADD_MIGRATE_CON", param: param, commandType: CommandType.StoredProcedure);
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
    }

}





