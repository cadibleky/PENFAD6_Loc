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

namespace PENFAD6DAL.Repository.Setup.Utilities
{
    public class EndOfMonthRepo
    {
        [Required(ErrorMessage = " Scheme is a required data item.")]
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        [Required(ErrorMessage = " Scheme Date is a required data item.")]
        public DateTime Scheme_Today_Date { get; set; }
        public DateTime System_Date { get; set; }
        [Required]
        public DateTime Confirm_Date { get; set; }
        AppSettings db = new AppSettings();
        IDbConnection con;

        public bool RunEOD(EndOfMonthRepo EOMRepo)
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
                    // check for pending transactions
                            //Remittance
                            var paramR = new DynamicParameters();
                            paramR.Add("P_SCHEME_ID", Scheme_Id, DbType.String, ParameterDirection.Input);
                            paramR.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                            conn.Execute("EOD_PENDING_REMITTANCE", paramR, commandType: CommandType.StoredProcedure);
                            int paramoption = paramR.Get<int>("VDATA");
                            if (paramoption > 0)
                            return false;

                            //Payment
                            var paramRe = new DynamicParameters();
                            paramRe.Add("P_SCHEME_ID", Scheme_Id, DbType.String, ParameterDirection.Input);
                            paramRe.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                            conn.Execute("EOD_PENDING_PAYMENT", paramRe, commandType: CommandType.StoredProcedure);
                            int receipt = paramRe.Get<int>("VDATA");
                            if (receipt > 0)
                            return false;

                            //Purchases
                            var paramPu = new DynamicParameters();
                            paramPu.Add("P_SCHEME_ID", Scheme_Id, DbType.String, ParameterDirection.Input);
                            paramPu.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                            conn.Execute("EOD_PENDING_PURCHASE", paramPu, commandType: CommandType.StoredProcedure);
                            int purchase = paramPu.Get<int>("VDATA");
                            if (receipt > 0)
                                return false;

                    // GL(Journal)

                    //check for securities maturity

                    //Apply investment interest

                    // Apply surchage on contribution due for surchage
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_SCHEME_ID", value: Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("EOD_SURCHARGE_CON_LOG", param, commandType: CommandType.StoredProcedure);


                    //VALUATE ASSET

                    //ACCRUE FEES BEFORE NAV

                    // ACCRUE FEES AFTER NAV

                    //calculate unit price


                    // move today date to next date
                    DynamicParameters parama = new DynamicParameters();
                    parama.Add(name: "P_SCHEME_ID", value: Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    conn.Execute("EOD_NEXT_DATE", parama, commandType: CommandType.StoredProcedure);

                    // log unit price

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


        public  void GetSchemeList(EndOfMonthRepo EOMRepo)
        {
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_SCHEME_ID", EOMRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_SCHEME_NAME", param, commandType: CommandType.StoredProcedure);
                    EOMRepo.Scheme_Name = param.Get<string>("VDATA");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }

            }
        }


        public void GetSystemDate(EndOfMonthRepo EOMRepo)
        {
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_DATE_NOW", "", DbType.Date, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_DATE_NOW", param, commandType: CommandType.StoredProcedure);
                    EOMRepo.System_Date = param.Get<DateTime>("P_DATE_NOW");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }

            }
        }


    }
}
