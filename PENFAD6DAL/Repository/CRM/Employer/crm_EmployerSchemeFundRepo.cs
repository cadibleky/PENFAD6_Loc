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

namespace PENFAD6DAL.Repository.CRM.Employer
{
    public class crm_EmployerSchemeFundRepo
    {

        [Required(ErrorMessage = "Please select Employer")]
        public string Employer_Name { get; set; }
        public string EMSF_Id { get; set; }
        [Required(ErrorMessage = "Please select Scheme-Fund")]
        public string Scheme_Fund_Id { get; set; }
        public string Scheme_Id { get; set; }
        public string Fund_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Fund_Name { get; set; }
        public string Employer_Id { get; set; }
        public string Registration_Number { get; set; }
        public string Office_Location { get; set; }

        public string Phone_Number { get; set; }      
        public string Auth_Status { get; set; }
        public string EMSF_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Maker_Id { get; set; }


        IDbConnection con;
        public void SaveRecord(crm_EmployerSchemeFundRepo ESRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_EMSF_ID", value: ESRepo.Employer_Id + ESRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: ESRepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_ID", value: ESRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_CRM_EMPLOYER_SCHEME_FUND", param, commandType: CommandType.StoredProcedure);
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


        // Approve Employer Scheme Fund
        public void ApproveESRecord(string EMSF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_EMSF_ID", value: EMSF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMSF_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("APP_CRM_EMSF", param, commandType: CommandType.StoredProcedure);
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

        // Approve Employer Scheme
        public void CloseESRecord(string EMSF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_EMSF_ID", value: EMSF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMSF_STATUS", value: "CLOSED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("UPD_CRM_EMSF", param, commandType: CommandType.StoredProcedure);
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

        public void ActivateESRecord(string EMSF_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_EMSF_ID", value: EMSF_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMSF_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("UPD_CRM_EMSF", param, commandType: CommandType.StoredProcedure);
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


        //public bool DeleteRecord(string ES_ID)
        //{
        //    try
        //    {
        //        //Get Connection
        //        AppSettings app = new AppSettings();
        //        con = app.GetConnection();
        //        DynamicParameters param = new DynamicParameters();
        //        //Input Param
        //        param.Add(name: "P_ES_ID", value: ES_ID, dbType: DbType.String, direction: ParameterDirection.Input);
        //        int result = con.Execute(sql: "DEL_REMIT_EMPLOYER_FUND", param: param, commandType: CommandType.StoredProcedure);

        //        if (result > 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            if (con != null) { con = null; }
        //        }
        //    }
        //}

        
        public DataSet ESData()
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

                cmd.CommandText = "SEL_CRM_EMSF";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ES");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeFundRepo> GetESList()
        {
            try
            {
                DataSet dt = ESData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeFundRepo
                {
                    EMSF_Id = row.Field<string>("EMSF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),                  
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    EMSF_Status = row.Field<string>("EMSF_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
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

        // FOR PENDING EMPLOYER SCHEME FUND
        public DataSet ESPendingData()
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

                cmd.CommandText = "SEL_CRM_EMSF_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "es");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeFundRepo> GetESPendingList()
        {
            try
            {
                DataSet dt = ESPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeFundRepo
                {
                    EMSF_Id = row.Field<string>("EMSF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    EMSF_Status = row.Field<string>("EMSF_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
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

        // FOR NOT CLOSED EMPLOYER SCHEME FUND
        public DataSet ESNotClosedData()
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

                cmd.CommandText = "SEL_CRM_EMSF_NOTCLOSED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "es");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeFundRepo> GetESNotClosedList()
        {
            try
            {
                DataSet dt = ESNotClosedData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeFundRepo
                {
                    EMSF_Id = row.Field<string>("EMSF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    EMSF_Status = row.Field<string>("EMSF_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
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

        // FOR  CLOSED EMPLOYER SCHEME FUND
        public DataSet ESClosedData()
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

                cmd.CommandText = "SEL_CRM_EMSF_CLOSED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "es");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeFundRepo> GetESClosedList()
        {
            try
            {
                DataSet dt = ESClosedData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeFundRepo
                {
                    EMSF_Id = row.Field<string>("EMSF_ID"),
                    Scheme_Fund_Id = row.Field<string>("SCHEME_FUND_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Fund_Id = row.Field<string>("FUND_ID"),
                    Fund_Name = row.Field<string>("FUND_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    EMSF_Status = row.Field<string>("EMSF_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
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


        // FOR EMPLOYER GRID
        public DataSet EmployerData()
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

                cmd.CommandText = "SEL_CRM_EMPLOYER_ES";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "emp");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public IEnumerable<crm_EmployerRepo> GetEmployerList()
        {
            try
            {
                DataSet dt = EmployerData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerRepo
                {
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Business_Address = row.Field<string>("BUSINESS_ADDRESS"),
                    Employer_Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    Employer_Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Web_Site = row.Field<string>("WEB_SITE"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Contact_Number = row.Field<string>("CONTACT_NUMBER"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Employer_Other_Number = row.Field<string>("OTHER_NUMBER"),
                    Region_Id = row.Field<string>("REGION_ID"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Sector_Id = row.Field<string>("SECTOR_ID"),


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
