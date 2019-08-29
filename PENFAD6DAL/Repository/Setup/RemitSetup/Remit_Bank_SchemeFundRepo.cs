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

namespace PENFAD6DAL.Repository.Setup.RemitSetup
{
    public class remit_BankSchemeFundRepo
    {
        public string Bank_RefNo_Prefix { get; set; }
        public string Bank_RefNo{ get; set; }
        [Required]
        public string Scheme_Fund_Id { get; set; }
        [Required]
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Fund_Id { get; set; }
        public string Fund_Name { get; set; }
        [Required]
        public string Actual_Bank_Acct_No { get; set; }

        public string GL_Account_Name { get; set; }
        [Required]
        public string GL_Account_No { get; set; }
        public string GL_Account_Id { get; set; }
        [Required]
        public string Actual_Bank_Acct_Name { get; set; }
        [Required]
        public string Custodian_Id { get; set; }
        public string Maker_Id { get; set; }
        public string Make_Date { get; set; }

        IDbConnection con;
        public void SaveRecord(remit_BankSchemeFundRepo BankSFRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_BANK_REFNO", value: BankSFRepo.Bank_RefNo, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_BANK_REFNO_PREFIX", value: BankSFRepo.GL_Account_No + BankSFRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ACTUAL_BANK_ACCT_NO", value: BankSFRepo.Actual_Bank_Acct_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NO", value: BankSFRepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ACTUAL_BANK_ACCT_NAME", value: BankSFRepo.Actual_Bank_Acct_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: BankSFRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CUSTODIAN_ID", value: BankSFRepo.Custodian_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                con.Execute("MIX_BANK_SCHEME_FUND", param, commandType: CommandType.StoredProcedure);
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





        public bool DeleteRecord(string BANK_REFNO)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_BANK_REFNO", value: BANK_REFNO, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_BANK_SCHEME_FUND", param: param, commandType: CommandType.StoredProcedure);

                if (result > 0)
                    return true;
                else
                    return false;
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


        public bool isESUnique(string BANK_REFNO)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_BANK_REFNO", BANK_REFNO, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_BANK_SF_EXIST", param, commandType: CommandType.StoredProcedure);
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


        public DataSet BankSFData()
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

                cmd.CommandText = "SEL_BANK_SCHEME_FUND";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "BankSF");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<remit_BankSchemeFundRepo> GetBankSFList()
        {
            try
            {
                DataSet dt = BankSFData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new remit_BankSchemeFundRepo
                {
                   
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fund_Name = row.Field<string>("FUND_NAME")
                  
                }).ToList();

                return eList;

            }
            catch (Exception)
            {
                throw;
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

        //FILTERING GL_ACCOUNT LIST FOR SCHEME_FUND
        public List<remit_BankSchemeFundRepo> GetGLASFList(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<remit_BankSchemeFundRepo> bn = new List<remit_BankSchemeFundRepo>();

                string query = "Select * from GL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' and MEMO_CODE = '" + "101" + "' ";
                return bn = con.Query<remit_BankSchemeFundRepo>(query).ToList();

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


        //FILTERING GL_ACCOUNT LIST FOR SCHEME_FUND
        public List<remit_BankSchemeFundRepo> GetGLASFListGL(string Scheme_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<remit_BankSchemeFundRepo> bn = new List<remit_BankSchemeFundRepo>();

                string query = "Select * from GL_ACCOUNT WHERE SUBSTR(SCHEME_FUND_ID,0,2) = '" + Scheme_Id + "' and MEMO_CODE = '" + "101" + "' and  SUBSTR(SCHEME_FUND_ID,-2,2) != '01' ";
                return bn = con.Query<remit_BankSchemeFundRepo>(query).ToList();

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

        //GET LIST FOR GRID - BANK-SCHEMEFUND
        public List<remit_BankSchemeFundRepo> GetBankSFListGrid()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<remit_BankSchemeFundRepo> bn = new List<remit_BankSchemeFundRepo>();

                string query = "Select * from VW_BANK_SF ";
                return bn = con.Query<remit_BankSchemeFundRepo>(query).ToList();

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


    }
}
