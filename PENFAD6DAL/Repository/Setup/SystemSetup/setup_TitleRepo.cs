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
    public class setup_TitleRepo
    {
        public string Title_ID { get; set; }
        [Required(ErrorMessage = " Title is a required data item.")]
        public string Title { get; set; }

        IDbConnection con;
        public void SaveRecord(setup_TitleRepo titleRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_TITLE_ID", value: titleRepo.Title_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TITLE", value: titleRepo.Title, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("ADD_SETUP_TITLE", param, commandType: CommandType.StoredProcedure);
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

        public void updateRecord(setup_TitleRepo titleRepo)
        {

            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_TITLE_ID", value: Title_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_TITLE", value: Title, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("UPD_SETUP_TITLE", param, commandType: CommandType.StoredProcedure);
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
        public bool DeleteRecord(string TITLEID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_TITLE_ID", value: TITLEID, dbType: DbType.String, direction: ParameterDirection.Input);   
                int result = con.Execute(sql: "DEL_SETUP_TITLE", param: param, commandType: CommandType.StoredProcedure);

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
                    cmd.CommandText = "SEL_SETUP_TITLE";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "SETUP_TITLE");
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
        public bool isTitleUnique(string MTITLE)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_TITLE", MTITLE, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_SETUP_TITLE_EXIST", param, commandType: CommandType.StoredProcedure);
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
            //finally
            //{
            //    con.Dispose();
            //}

        }

        public DataSet TitleData()
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

                cmd.CommandText = "SEL_SETUP_TITLE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "title");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_TitleRepo> GetTitleList()
        {
            try
            {
                DataSet dt = TitleData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_TitleRepo
                {
                    Title_ID = row.Field<string>("TITLE_ID"),
                    Title = row.Field<string>("TITLE")                
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
