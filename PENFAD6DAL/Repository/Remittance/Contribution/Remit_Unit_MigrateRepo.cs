using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    public class Remit_Unit_MigrateRepo
    {
        public string Con_Log_Id { get; set; }
        public string Employer_Id { get; set; }
        public string Scheme_Id { get; set; }
        public string ES_Id { get; set; }
        public int For_Month { get; set; }
        public int For_Year { get; set; }
        public decimal? Total_Contribution { get; set; }
        public string Log_Status { get; set; }
        public DateTime? Make_Date { get; set; }
        public string Purchase_Log_Id { get; set; }
        public string Esf_Id { get; set; }
        public string Employee_Id { get; set; }
        public decimal? Employer_Units { get; set; }
        public decimal? Employee_Units { get; set; }
        public string Purchase_Type { get; set; }
        public decimal? Unit_Price { get; set; }
        public string Scheme_Fund_Id { get; set; }

        IDbConnection conn;
        public bool getPurchaseLog(Remit_Unit_MigrateRepo remitConLogdetailsrepo)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_PURCHASE_LOG_ID", remitConLogdetailsrepo.Purchase_Log_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL__MIG_PURCHASE_ID", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");

                if (paramoption <= 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Create_Units(Remit_Unit_MigrateRepo remitConLogdetailsrepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                conn = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "p_PURCHASE_LOG_ID", value: remitConLogdetailsrepo.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF_ID", value: remitConLogdetailsrepo.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Units", value: remitConLogdetailsrepo.Employer_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Employee_Units", value: remitConLogdetailsrepo.Employee_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Purchase_Type", value: "MIGRATED UNIT", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Unit_Price", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                conn.Execute("MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);
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

    
