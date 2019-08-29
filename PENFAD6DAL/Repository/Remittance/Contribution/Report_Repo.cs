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
    
    public class Report_DashSchemeRepo

    {
        string plog;
        [Required]
        public string Scheme_Id { get; set; }
        public DateTime Report_Date { get; set; }
   
       

        IDbConnection con;
       
        public bool Report_DashScheme(Report_DashSchemeRepo ReportRepo)
        {
            var app = new AppSettings(); 

          

            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(120000);

            using (OracleConnection conn = new OracleConnection(app.conString()))  //
            {
             
                try
                {


                    var param = new DynamicParameters();
                    param.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    param.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    param.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    conn.Execute("REPORT_INSERT_PORT_SUM_SDB", param, commandType: CommandType.StoredProcedure);

                    var param1 = new DynamicParameters();
                    param1.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    param1.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    param1.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    conn.Execute("REPORT_INSERT_PORT_SUM_SDB1", param1, commandType: CommandType.StoredProcedure);

                    var param11 = new DynamicParameters();
                    param11.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    param11.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    param11.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    conn.Execute("REPORT_INSERT_PORT_SUM_SDB11", param11, commandType: CommandType.StoredProcedure);


                    var param2 = new DynamicParameters();
                    param2.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    param2.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    param2.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    conn.Execute("REPORT_INSERT_PORT_SUM_SDB2", param2, commandType: CommandType.StoredProcedure);

                    var param3 = new DynamicParameters();
                    param3.Add("P_SCHEMEFUNDID", GlobalValue.Report_Param_1, DbType.String, ParameterDirection.Input);
                    param3.Add("P_DATE", GlobalValue.Report_Param_2, DbType.DateTime, ParameterDirection.Input);
                    param3.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                    conn.Execute("REPORT_INSERT_PORT_SUM_SDB3", param3, commandType: CommandType.StoredProcedure);


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

      


    }
}
