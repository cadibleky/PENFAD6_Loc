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
    public class setup_StaffRepo
    {
        public string Staff_Id { get; set; }
        [Required(ErrorMessage = " Name  is a required field.")]
        public string Staff_Name { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Residential_Address { get; set; }
        public string Staff_Status { get; set; }
        public string Maker_Id { get; set; }
        public string Make_Date { get; set; }
        public string Update_Id { get; set; }
        public string Update_Date { get; set; }

        IDbConnection con;
        public void SaveRecord(setup_StaffRepo staffRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_STAFF_ID", value: staffRepo.Staff_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_STAFF_NAME", value: staffRepo.Staff_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MOBILENO", value: staffRepo.MobileNo, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PHONENO", value: staffRepo.PhoneNo, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMAIL", value: staffRepo.Email, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GENDER", value: staffRepo.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RESIDENTIAL_ADDRESS", value: staffRepo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_STAFF_STATUS", value: staffRepo.Staff_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: staffRepo.Make_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: staffRepo.Update_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                con.Execute("ADD_SETUP_STAFF", param, commandType: CommandType.StoredProcedure);
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


        public bool DeleteRecord(string Staff_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_STAFF_ID", value: Staff_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_SETUP_STAFF", param: param, commandType: CommandType.StoredProcedure);

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

        public bool DropRecord(string Staff_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_STAFF_ID", value: Staff_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DRO_SETUP_STAFF", param: param, commandType: CommandType.StoredProcedure);

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

        public DataSet StaffData()
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

                cmd.CommandText = "SEL_SETUP_STAFF";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "staff");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_StaffRepo> GetStaffList()
        {
            try
            {
                DataSet dt = StaffData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_StaffRepo
                {
                    Staff_Id = row.Field<string>("STAFF_ID"),
                    Staff_Name = row.Field<string>("STAFF_NAME")  ,
                    MobileNo = row.Field<string>("MOBILENO"),
                    PhoneNo = row.Field<string>("PHONENO"),
                    Email = row.Field<string>("EMAIL")  ,
                    Gender = row.Field<string>("GENDER"),
                    Staff_Status = row.Field<string>("STAFF_STATUS")  ,
                    Residential_Address = row.Field<string>("RESIDENTIAL_ADDRESS")
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
