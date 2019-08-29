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
    public class Remit_Unit_Log
    {
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

    }

    public class Remit_Unit_Log_Details
    {
        public string Con_Log_Id { get; set; }
        public string Purchase_Log_Id { get; set; }
        public string Esf_Id { get; set; }
        public string Employee_Id { get; set; }
        public int For_Month { get; set; }
        public int For_Year { get; set; }
        public decimal Employer_Amt { get; set; }
        public decimal Employee_Amt { get; set; }
        public decimal Employer_Units { get; set; }
        public decimal Employee_Units { get; set; }
        public string Purchase_Type { get; set; }
        public decimal Unit_Price { get; set; }
        public DateTime Make_date { get; set; }
        public string SEL_Migrate_GetPurchase { get; set; }

        AppSettings db = new AppSettings();
        public string PurchaseSaveRecord(Remit_Unit_Log_Details remitConLogdetailsrepo)
        {
            try
            {
                //Get Connection
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_PURCHASE_LOG_ID", value: remitConLogdetailsrepo.Purchase_Log_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ESF_ID", value: remitConLogdetailsrepo.Esf_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_UNITS", value: remitConLogdetailsrepo.Employee_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_UNITS", value: remitConLogdetailsrepo.Employer_Units, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_PURCHASE_TYPE", value: "MIGRATION-UNITS", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UNIT_PRICE", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);

                db.GetConnection().Execute("MIGRATE_UNIT_PURCHASES", param, commandType: CommandType.StoredProcedure);
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


        //GET purchase log id
 
       // IDbConnection con;
        public bool GetPurchaseLog(string Esf_Idi, string Purchase_Log_Idi)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_PURCHASE_LOG_ID", Purchase_Log_Idi, DbType.String, ParameterDirection.Input);
                param.Add("P_ESF_ID", Esf_Idi, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_MIGRATE_GETPURCHASE", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");

                if (paramoption == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
          
        }

    }
}

    
