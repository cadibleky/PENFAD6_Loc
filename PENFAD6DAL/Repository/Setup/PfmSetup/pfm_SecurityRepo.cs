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

namespace PENFAD6DAL.Repository.Setup.pfmSetup
{
    public class pfm_SecurityRepo
    {
        public string Security_Id { get; set; }
        [Required]
        public string Security_Name { get; set; }
        [Required]
        public string Asset_Class { get; set; }
        public string Maker_Id { get; set; }
        public string Update_Id { get; set; }

        IDbConnection con;
        public void SaveRecord(pfm_SecurityRepo securityRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_SECURITY_ID", value: securityRepo.Security_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SECURITY_NAME", value: securityRepo.Security_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ASSET_CLASS", value: securityRepo.Asset_Class, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_SECURITY", param, commandType: CommandType.StoredProcedure);
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

        public void updateRecord(pfm_SecurityRepo securityRepo)
        {

            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_SECURITY_ID", value: Security_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SECURITY_NAME", value: Security_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ASSET_CLASS", value: Asset_Class, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_SECURITY", param, commandType: CommandType.StoredProcedure);
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
        public bool DeleteRecord(string SECURITY_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SECURITY_ID", value: SECURITY_ID, dbType: DbType.String, direction: ParameterDirection.Input);   
                int result = con.Execute(sql: "DEL_PFM_SECURITY", param: param, commandType: CommandType.StoredProcedure);

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
                    cmd.CommandText = "SEL_PFM_SECURTY";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "PFM_SECURITY");
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
        public bool isSecurityUnique(string MSECURITY)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SECURITY_NAME", MSECURITY, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_PFM_SECURITY_EXIST", param, commandType: CommandType.StoredProcedure);
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

        public DataSet SecurityData()
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

                cmd.CommandText = "SEL_PFM_SECURITY";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "security");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_SecurityRepo> GetSecurityList()
        {
            try
            {
                DataSet dt = SecurityData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_SecurityRepo
                {
                    Security_Id = row.Field<string>("SECURITY_ID"),
                    Security_Name = row.Field<string>("SECURITY_NAME"),
                    Asset_Class = row.Field<string>("ASSET_CLASS")
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
