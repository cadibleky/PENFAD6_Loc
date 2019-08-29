using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{ 
   public class setup_DistrictRepo
    {
        //Richard........................
        //[Required(ErrorMessage = "Region Name is a required data item.")]
        public string Region_Id { get; set; }   
        public string Region_Name { get; set; }
        public string District_Id { get; set; }
        [Required(ErrorMessage = "District Name is a required data item.")]
        public string District_Name { get; set; }
        public string Maker_Id { get; set; }
        public string Update_Id { get; set; }


        IDbConnection con;
        AppSettings app = new AppSettings();

    public void SaveRecord(setup_DistrictRepo DistrictRepo)
    {
        try
        {
            //Get Connection
            AppSettings app = new AppSettings();
            con = app.GetConnection();
            DynamicParameters param = new DynamicParameters();
            param.Add(name: "p_DISTRICT_ID", value: DistrictRepo.District_Id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add(name: "p_DISTRICT_NAME", value: DistrictRepo.District_Name, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add(name: "p_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
           // param.Add(name: "p_REGION_ID", value: DistrictRepo.Region_Id, dbType: DbType.String, direction: ParameterDirection.Input);
     
            con.Execute("MIX_SETUP_DISTRICT", param, commandType: CommandType.StoredProcedure);
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

   
        public bool DeleteRecord(string District_Id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_DISTRICT_ID", value: District_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_SETUP_DISTRICT", param: param, commandType: CommandType.StoredProcedure);
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
        public DataSet DistrictData()
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

                cmd.CommandText = "SEL_SETUP_DISTRICT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "district");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public IEnumerable<setup_DistrictRepo> GetDistrictList()
        {
            try
            {
                DataSet dt = DistrictData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_DistrictRepo
                {
                    District_Id = row.Field<string>("DISTRICT_ID"),
                    District_Name = row.Field<string>("DISTRICT_NAME")
                   
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