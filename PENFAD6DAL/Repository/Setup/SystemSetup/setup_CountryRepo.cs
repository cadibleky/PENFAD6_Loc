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

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public class setup_CountryRepo
    {     
        public string Country_ID { get; set; }
        [Required(ErrorMessage = " Country is a required data item.")]
        public string Country_Name { get; set; }
        [Required(ErrorMessage = " Country Code is a required data item.")]
        public string Country_Code { get; set; }
        [Required(ErrorMessage = " Nationality is a required data item.")]
        public string Nationality { get; set; }


        IDbConnection con;
        
        // Save Country
        public void SaveRecord()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_COUNTRY_ID", value: Country_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_COUNTRY_NAME", value: Country_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_COUNTRY_CODE", value: Country_Code, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_NATIONALITY", value: Nationality, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                
                con.Execute("ADD_SETUP_COUNTRY", param, commandType: CommandType.StoredProcedure);
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

        public void updateRecord()
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add("P_COUNTRY_ID", Country_ID, DbType.String, ParameterDirection.Input);
                param.Add("P_COUNTRY_NAME", Country_Name, DbType.String, ParameterDirection.Input);
                param.Add("P_COUNTRY_CODE", Country_Code, DbType.String, ParameterDirection.Input);
                param.Add("P_NATIONALITY", Nationality, DbType.String, ParameterDirection.Input);
                con.Execute("UPD_SETUP_COUNTRY", param, commandType: CommandType.StoredProcedure);
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

        //Delete Country
        public bool DeleteRecord(string COUNTRYID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_COUNTRY_ID", value: COUNTRYID, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_SETUP_COUNTRY", param: param, commandType: CommandType.StoredProcedure);

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

        public DataSet CountryData()
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

                cmd.CommandText = "SEL_SETUP_COUNTRY";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "country");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_CountryRepo> GetCountryList()
        {
            try
            {
                DataSet dt = CountryData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_CountryRepo
                {
                    Country_ID = row.Field<string>("COUNTRY_ID"),
                    Country_Name = row.Field<string>("COUNTRY_NAME"),
                    Country_Code = row.Field<string>("COUNTRY_CODE"),
                    Nationality = row.Field<string>("NATIONALITY")
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
        public DataSet CountryNationality(string countryId)
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

                cmd.CommandText = "SEL_COUNTRY_Nationality";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                param = cmd.Parameters.Add(countryId, OracleDbType.Varchar2);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "nationality");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_CountryRepo> GetNationality(string countryId)
        {
            try
            {
                DataSet dt = CountryNationality(countryId);
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_CountryRepo
                {
                    Country_ID = row.Field<string>("COUNTRY_ID"),
                    Country_Name = row.Field<string>("COUNTRY_NAME"),
                    Country_Code = row.Field<string>("COUNTRY_CODE"),
                    Nationality = row.Field<string>("NATIONALITY")
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
        public bool isCountryUnique(string MCOUNTRY)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_COUNTRY_NAME", MCOUNTRY, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_SETUP_COUNTRY_EXIST", param, commandType: CommandType.StoredProcedure);
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

    }
}
