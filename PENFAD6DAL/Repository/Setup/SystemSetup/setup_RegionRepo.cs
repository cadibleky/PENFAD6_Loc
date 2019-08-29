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
    public class setup_RegionRepo
    {
        public string Region_ID { get; set; }

        [Required(ErrorMessage = "Region is a required data item.")]
        public string Region_Name { get; set; }

        [Required(ErrorMessage = "Country is a required data item.")]
        public string Country_ID { get; set; }
        public string Country_Name { get; set; }

        IDbConnection con;
        public void SaveRecord(setup_RegionRepo regionRepo)
        {
            try
            {

                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_REGION_ID", value: regionRepo.Region_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_REGION_NAME", value: regionRepo.Region_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_COUNTRY_ID", value: regionRepo.Country_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("ADD_SETUP_REGION", param, commandType: CommandType.StoredProcedure);
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

        
        public bool DeleteRecord(string REGIONID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_REGION_ID", value: REGIONID, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_SETUP_REGION", param: param, commandType: CommandType.StoredProcedure);

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
                    cmd.CommandText = "SEL_SETUP_REGION";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "SETUP_REGION");
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
        public bool isRegionUnique(string MREGION)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_REGION_NAME", MREGION, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_SETUP_REGION_EXIST", param, commandType: CommandType.StoredProcedure);
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

        public DataSet RegionData()
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

                cmd.CommandText = "SEL_SETUP_REGION";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "region");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_RegionRepo> GetRegionList()
        {
            try
            {
                DataSet dt = RegionData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_RegionRepo
                {
                    Region_ID = row.Field<string>("REGION_ID"),
                    Region_Name = row.Field<string>("REGION_NAME"),
                    Country_ID = row.Field<string>("COUNTRY_ID"),
                    Country_Name = row.Field<string>("COUNTRY_NAME"),
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