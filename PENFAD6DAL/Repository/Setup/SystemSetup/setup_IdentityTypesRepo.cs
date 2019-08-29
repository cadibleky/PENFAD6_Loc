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
    public class setup_IdentityTypesRepo
    {
        //Richard.................................
        public string Identity_Type_Id { get; set; }

        [Required(ErrorMessage ="Identity Type is a required data item.")]
        public string Identity_Type { get; set; }
        public string Maker_Id { get; set; }
       
        public string Update_Id { get; set; }
       

        IDbConnection con;


        public void SaveRecord(setup_IdentityTypesRepo IdentityTypesRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_IDENTITY_TYPE_ID", value: IdentityTypesRepo.Identity_Type_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IDENTITY_TYPE", value: IdentityTypesRepo.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_SETUP_IDENTITYTYPE", param, commandType: CommandType.StoredProcedure);
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

       


        public bool DeleteRecord(string Identity_Type_Id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_IDENTITY_TYPE_ID", value: Identity_Type_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_SETUP_IDENTITYTYPE ", param: param, commandType: CommandType.StoredProcedure);
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

       

        public DataSet IdentityData()
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

                cmd.CommandText = "SEL_SETUP_IDENTITYTYPE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "identity");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<setup_IdentityTypesRepo> GetIdentityTypeList()
        {
            try
            {
                DataSet dt = IdentityData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_IdentityTypesRepo
                {
                    Identity_Type_Id = row.Field<string>("IDENTITY_TYPE_ID"),
                    Identity_Type = row.Field<string>("IDENTITY_TYPE"),


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
        public bool IdentityExist(string Identitylnname, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_IDENTITY_TYPE", value: Identitylnname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_SETUP_IDENTITYTYPEEXIT", param: param, commandType: CommandType.StoredProcedure);

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
       



    }


}

