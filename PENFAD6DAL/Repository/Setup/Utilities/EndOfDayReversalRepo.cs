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
    public class EndOfDayReversalRepo
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

        

        public  void GetSchemeList(EndOfDayReversalRepo EODRepo)
        {
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_SCHEME_NAME", param, commandType: CommandType.StoredProcedure);
                    EODRepo.Scheme_Name = param.Get<string>("VDATA");
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


        public void GetSystemDate(EndOfDayReversalRepo EODRepo)
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
                    EODRepo.System_Date = param.Get<DateTime>("P_DATE_NOW");
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
