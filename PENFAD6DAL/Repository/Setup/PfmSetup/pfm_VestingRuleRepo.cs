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

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_VestingRuleRepo
    {
        public string Vesting_Rule_Id { get; set; }
        [Required]
        public string Vesting_Rule { get; set; }
       [Required]
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Marker_Id { get; set; }
        public string Update_Id { get; set; }

        IDbConnection con;
        public void SaveRecord(pfm_VestingRuleRepo vestingRuleRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_VESTING_RULE_ID", value: vestingRuleRepo.Vesting_Rule_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_VESTING_RULE", value: vestingRuleRepo.Vesting_Rule, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: vestingRuleRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("MIX_PFM_VESTING_RULE", param, commandType: CommandType.StoredProcedure);
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
      
        public bool DeleteRecord(string Vesting_Rule_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_VESTING_RULE_ID", value: Vesting_Rule_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_PFM_VESTING_RULE", param: param, commandType: CommandType.StoredProcedure);

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
        public DataSet GetTIDataset()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = (SqlConnection)con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SEL_PFM_VESTING_RULE";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "PFM_VESTING_RULE");
                        return ds;
                    }
                }
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
        public bool isVestingRuleUnique(string MVestingRule)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_VESTING_RULE", MVestingRule, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_PFM_VESTING_RULE_EXIST", param, commandType: CommandType.StoredProcedure);
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

        public DataSet VestingRuleData()
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

                cmd.CommandText = "SEL_PFM_VESTING_RULE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "vestingRule");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_VestingRuleRepo> GetVestingRuleList()
        {
            try
            {
                DataSet dt = VestingRuleData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_VestingRuleRepo
                {
                    Vesting_Rule_Id = row.Field<string>("VESTING_RULE_ID"),
                    Vesting_Rule = row.Field<string>("VESTING_RULE"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
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

        //LIST FOR SCHEME
        public DataSet SchemeData()
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

                cmd.CommandText = "SEL_PFM_SCHEME";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "scheme");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_VestingRuleRepo> GetSchemeList()
        {
            try
            {
                DataSet dt = SchemeData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_VestingRuleRepo
                {
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),             
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

    }
}