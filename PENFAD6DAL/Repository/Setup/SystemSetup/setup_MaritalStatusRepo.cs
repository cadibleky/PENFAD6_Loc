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

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public class setup_MaritalStatusRepo
    {
        //Richard........................
        public string Marital_Status_Id { get; set; }

        [Required(ErrorMessage = "Marital Status is a required data item.")]
        public string Marital_Status { get; set; }
        public string Maker_Id { get; set; }
       
        public string Update_Id { get; set; }
   

        IDbConnection con;
        AppSettings app = new AppSettings();

        public void SaveRecord(setup_MaritalStatusRepo MaritalStatuss)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_MARITAL_STATUS_ID", value: MaritalStatuss.Marital_Status_Id , dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MARITAL_STATUS", value: MaritalStatuss.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("mix_SETUP_MARITALSTATUS", param, commandType: CommandType.StoredProcedure);
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



    

        public bool DeleteRecord(string marital_status_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();

                DynamicParameters param = new DynamicParameters();

                //Input Param
                param.Add("p_MARITAL_STATUS_ID", marital_status_Id, DbType.String, ParameterDirection.Input);

                int result = con.Execute("DEL_SETUP_MARITALSTATUS", param, commandType: CommandType.StoredProcedure);
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

     

     

         public bool MaritalStatusExist(string maritalnname, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_MARITAL_STATUS", value: maritalnname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_SETUP_MARITALSTATUSEXIT", param: param, commandType: CommandType.StoredProcedure);

                tott = param.Get<Int32>("p_count");
                if (tott > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // throw ex;
                error = ex.ToString();
                return false;
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
        public DataSet MaritalStatusData()
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

                cmd.CommandText = "SEL_SETUP_MARITALSTATUS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "maritalstatus");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public IEnumerable<setup_MaritalStatusRepo> GetMaritalstatusList()
        {
            try
            {
                DataSet dt = MaritalStatusData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_MaritalStatusRepo
                {
                    Marital_Status_Id = row.Field<string>("MARITAL_STATUS_ID"),
                    Marital_Status = row.Field<string>("MARITAL_STATUS"),


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

       

    }

}


