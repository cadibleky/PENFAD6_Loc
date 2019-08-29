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
    public class setup_GenderRepo
    {
        public string Gender_ID { get; set; }
        [Required(ErrorMessage = " Gender is a required data item.")]
        public string Gender { get; set; }

        IDbConnection con;
        public void SaveRecord(setup_GenderRepo genderRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_GENDER_ID", value: genderRepo.Gender_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GENDER", value: genderRepo.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("ADD_SETUP_GENDER", param, commandType: CommandType.StoredProcedure);
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

      
        public bool DeleteRecord(string GENDER_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_GENDER_ID", value: GENDER_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_SETUP_GENDER", param: param, commandType: CommandType.StoredProcedure);

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
                    cmd.CommandText = "SEL_SETUP_GENDER";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "SETUP_GENDER");
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
        public bool isGenderUnique(string MGENDER)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_GENDER", MGENDER, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_SETUP_GENDER_EXIST", param, commandType: CommandType.StoredProcedure);
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

        public DataSet GenderData()
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

                cmd.CommandText = "SEL_SETUP_GENDER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "gender");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_GenderRepo> GetGenderList()
        {
            try
            {
                DataSet dt = GenderData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_GenderRepo
                {
                    Gender_ID = row.Field<string>("GENDER_ID"),
                    Gender = row.Field<string>("GENDER")
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
