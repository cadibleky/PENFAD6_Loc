using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Security
{
  public   class sec_AuthorizerRepo
    {
        public string User_Id { get; set; }
        ////[Required(ErrorMessage = "Authorizer Name is a required data item.")]
        public string Full_Name { get; set; }
        [Required(ErrorMessage = "Authoriizer Type is a required data item.")]
        public string Authority_Level { get; set;}
        [Required(ErrorMessage = " Authorizer WithDrawal  is a required data item.")]
        public string With_Drawal_Authority { get; set;}
        [Required(ErrorMessage = " Authority  is a required data item.")]
        public string Vocher_Petty_Cash_Authority { get; set; }
        public string Major_Expense_Authority { get; set; }
        public string Batch_Deposit_Authority { get; set;}
        public decimal With_Drawal_Limit { get; set; }
        public decimal Vocher_petty_Limit { get; set; }
        public decimal   Vocher_Major_Limit{get;set;}
        public string Approve_Fixed_Income { get; set; }
        public decimal  No_Of_Time_Can_Approve { get; set; }
        public decimal Fixed_Income_Limit { get; set; }
        public string Approve_Equity { get; set; }
        public decimal Equity_Limit { get; set; }
        public string Maker_Id { get; set; }
        public string Updated_Id { get; set; }

        IDbConnection con;

        AppSettings app = new AppSettings();
        public bool CreateAuthorizer(sec_AuthorizerRepo Authorizer)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
           
               // DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_User_Id", value: Authorizer.User_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Authorizer_Level", value: Authorizer.Authority_Level, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_With_drawal_Authority", value: Authorizer.With_Drawal_Authority, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Voucher_petty_Cash_Authority", value: Authorizer.Vocher_Petty_Cash_Authority, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Major_Expenser_Authority", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Batch_disposit_Authority", value: Authorizer.Batch_Deposit_Authority, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_With_drawal_limit", value: Authorizer.With_Drawal_Limit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Voucher_petty_Limit", value: Authorizer.Vocher_petty_Limit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Voucher_major_Limit", value: 0, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Approve_Fixed_InCome", value: Authorizer.Approve_Fixed_Income, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_No_of_Time_Can_Approve", value: Authorizer.No_Of_Time_Can_Approve, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_Fixed_Income_Limit", value: Authorizer.Fixed_Income_Limit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                 param.Add(name:"p_Approve_Equity", value: Authorizer.Approve_Equity , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Equity_limit", value: Authorizer.Equity_Limit , dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATED_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                //con.Execute("", param, commandType: CommandType.StoredProcedure);

                int result = con.Execute(sql: "MIX_SEC_AUTHORIZER", param: param, commandType: CommandType.StoredProcedure);

                if (result != 0)
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
        //public bool SaveRecord(sec_AuthorizerRepo AuthorizerRepo)
        //{
        //    try
        //    {
        //        //Get Connection

        //       AppSettings app = new AppSettings();
        //        con = app.GetConnection();
              
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
        //Get Fixed Income Authorizer
        public DataSet FixedIncomeAuthorizerData()
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

                cmd.CommandText = "SEL_SEC_AUTHORIZER_FIXEDINCOME";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "fixed");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<sec_AuthorizerRepo> GetFixedIncomeAuthorizerList()
        {
            try
            {
                DataSet dt = FixedIncomeAuthorizerData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new sec_AuthorizerRepo
                {
                    User_Id = row.Field<string>("USER_ID"),
                    Full_Name = row.Field<string>("FULLNAME"),
                    Approve_Fixed_Income  = row.Field<string>("APPROVE_FIXED_INCOME"),
                    //No_Of_Time_Can_Approve  = row.Field<int>("NO_OF_TIME_CAN_APPROVE"),
                    ///Fixed_Income_Limit  = row.Field<decimal >("FIXED_INCOME_LIMIT "),
                   

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
        public DataSet AuthorizerData()
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

                cmd.CommandText = "SEL_SEC_USERAUTHORIZER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "auth");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
       
        public IEnumerable<sec_AuthorizerRepo> GetAuthorizer_List()
        {
            try
            {
                DataSet dt = AuthorizerData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new sec_AuthorizerRepo
                {
                    User_Id = row.Field<string>("User_Id"),
                    Full_Name = row.Field<string>("FULLNAME"),
                    Authority_Level = row.Field<string>("AUTHORITY_LEVEL"),
                    With_Drawal_Authority  = row.Field<string>("With_Drawal_Authority"),
                    Vocher_Petty_Cash_Authority  = row.Field<string>("VOUCHER_PETTY_CASH_AUTHORITY"),
                    Major_Expense_Authority  = row.Field<string>("MAJOR_EXPENSE_AUTHORITY"),
                    Batch_Deposit_Authority  = row.Field<string>("BATCH_DEPOSIT_AUTHORITY"),
                    With_Drawal_Limit  = row.Field<decimal >("WITH_DRAWAL_LIMIT"),
                    Vocher_petty_Limit  = row.Field<decimal >("VOUCHER_PETTY_LIMIT"),
                    Vocher_Major_Limit  = row.Field<decimal>("VOUCHER_MAJOR_LIMIT"),
                    Approve_Fixed_Income  = row.Field<string>("APPROVE_FIXED_INCOME"),
                    Fixed_Income_Limit  = row.Field<decimal>("FIXED_INCOME_LIMIT"),
                    No_Of_Time_Can_Approve  = row.Field<decimal >("NO_OF_TIME_CAN_APPROVE"),
                    Approve_Equity = row.Field<string>("APPROVE_EQUITY"),
                    Equity_Limit  = row.Field<decimal>("EQUITY_LIMIT"),
                    

                }).ToList();

                return eList;

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
        public bool DeleteRecord(string user_id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();

                DynamicParameters param = new DynamicParameters();

                //Input Param
                param.Add("p_USER_ID", user_id, DbType.String, ParameterDirection.Input);

                int result = con.Execute("DEL_SEC_AUTHORIZER", param, commandType: CommandType.StoredProcedure);
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


    }
}
