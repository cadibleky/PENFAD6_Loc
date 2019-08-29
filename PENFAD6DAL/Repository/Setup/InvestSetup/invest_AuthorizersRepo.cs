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

namespace PENFAD6DAL.Repository.Setup.InvestSetup
{
    public class Invest_AuthorizersRepo
    {
        [Required]
        public string User_Id { get; set; }

        public string Fullname { get; set; }
        [Required]
        public string Authority_Level { get; set; }
        [Required]
        public string Approve_Fixed_Income { get; set; }
        [Required]
        public string Approve_Equity { get; set; }
        [Required]
        public decimal Fixed_Income_Limit { get; set; }
        [Required]
        public decimal Equity_Limit { get; set; }
        public string Marker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public DateTime Update_Date { get; set; }
        public string Update_Id { get; set; }
        public string Record_Status { get; set; }

       

        IDbConnection con;
        public void SaveRecord(Invest_AuthorizersRepo AuthorizersRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_USERID", value: AuthorizersRepo.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTHORIZERLEVEL", value: AuthorizersRepo.Authority_Level, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_APPROVEFIXEDINCOME", value: AuthorizersRepo.Approve_Fixed_Income, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_APPROVEEQUITY", value: AuthorizersRepo.Approve_Equity, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FIXEDINCOMELIMIT", value: AuthorizersRepo.Fixed_Income_Limit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_EQUITYLIMIT", value: AuthorizersRepo.Equity_Limit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_RECORD_STATUS", value: AuthorizersRepo.Record_Status, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_INVEST_AUTHORIZER", param, commandType: CommandType.StoredProcedure);
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

        public bool DeleteRecord(string User_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_USERID", value: User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_INVEST_AUTHORIZERS", param: param, commandType: CommandType.StoredProcedure);

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
       
        public DataSet AuthData()
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

                cmd.CommandText = "SEL_INVEST_AUTHORIZERS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "authorizers");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Invest_AuthorizersRepo> GetAuthList()
        {
            try
            {
                DataSet dt = AuthData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_AuthorizersRepo
                {
                    User_Id = row.Field<string>("USER_ID"),
                    Authority_Level = row.Field<string>("AUTHORITY_LEVEL"),
                    Approve_Fixed_Income = row.Field<string>("APPROVE_FIXED_INCOME"),
                    Fixed_Income_Limit = row.Field<decimal>("FIXED_INCOME_LIMIT"),
                    Approve_Equity = row.Field<string>("APPROVE_EQUITY"),
                    Equity_Limit = row.Field<decimal>("EQUITY_LIMIT"),
                    Record_Status = row.Field<string>("RECORD_STATUS"),
                    Fullname = row.Field<string>("FULLNAME"),
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

        public List<Invest_AuthorizersRepo> GetUsersList()
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Invest_AuthorizersRepo> ObjFund = new List<Invest_AuthorizersRepo>();

                return ObjFund = db.GetConnection().Query<Invest_AuthorizersRepo>("Select * from sec_user").ToList();
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