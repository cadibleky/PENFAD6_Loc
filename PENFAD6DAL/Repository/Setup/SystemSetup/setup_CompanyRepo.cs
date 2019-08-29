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
    public class setup_CompanyRepo
    {
        public string SMTP { get; set; }
        public decimal PORT { get; set; }
        public string EMAIL_FROM { get; set; }
        public string EMAIL_PASSWORD { get; set; }
        public string COMPANY_NAME { get; set; }
        public string WEBSITE_ADDRESS { get; set; }
        public string POSTAL_ADDRESS { get; set; }
        public string TELEPHONE_NUMBER { get; set; }
        public string LOCATION { get; set; }
        public byte[] LOGO { get; set; }
        public string SMS_NAME { get; set; }
        public string SMS_ID { get; set; }
        public string SMS_PASSWORD { get; set; }

        IDbConnection con;

        public DataSet CompanyData()
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

                cmd.CommandText = "SEL_SETUP_COMPANY";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "Company");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<setup_CompanyRepo> GetCompanyList()
        {
            try
            {
                DataSet dt = CompanyData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_CompanyRepo
                {
                    SMTP = row.Field<string>("SMTP"),
                    PORT = row.Field<decimal>("PORT"),
                    EMAIL_FROM = row.Field<string>("EMAIL_FROM"),
                    EMAIL_PASSWORD = row.Field<string>("EMAIL_PASSWORD"),
                    COMPANY_NAME = row.Field<string>("COMPANY_NAME"),
                    WEBSITE_ADDRESS = row.Field<string>("WEBSITE_ADDRESS"),
                    POSTAL_ADDRESS = row.Field<string>("POSTAL_ADDRESS"),
                    TELEPHONE_NUMBER = row.Field<string>("TELEPHONE_NUMBER"),
                    LOCATION = row.Field<string>("LOCATION"),
                    SMS_NAME = row.Field<string>("SMS_NAME"),
                    SMS_ID = row.Field<string>("SMS_ID"),
                    SMS_PASSWORD = row.Field<string>("SMS_PASSWORD")


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


        public  List<setup_CompanyRepo> GetEmpList(string COMPANY_NAME)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<setup_CompanyRepo> bn = new List<setup_CompanyRepo>();

                string query = "Select * from SETUP_COMPANY WHERE COMPANY_NAME = '" + COMPANY_NAME + "' ";
                return bn = con.Query<setup_CompanyRepo>(query).ToList();

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

        public bool AddEmployeeRecordToTmp(setup_CompanyRepo CompanyRepo)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                param.Add(name: "p_company_name", value: CompanyRepo.COMPANY_NAME, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_smtp", value: CompanyRepo.SMTP, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_port", value: CompanyRepo.PORT, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "p_email_from", value: CompanyRepo.EMAIL_FROM, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_email_password", value: CompanyRepo.EMAIL_PASSWORD, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_website_address", value: CompanyRepo.WEBSITE_ADDRESS, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_postal_address", value: CompanyRepo.POSTAL_ADDRESS, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_telephone_number", value: CompanyRepo.TELEPHONE_NUMBER, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_location", value: CompanyRepo.LOCATION, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_sms_name", value: CompanyRepo.SMS_NAME, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_sms_id", value: CompanyRepo.SMS_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_sms_password", value: CompanyRepo.SMS_PASSWORD, dbType: DbType.String, direction: ParameterDirection.Input);
               
                param.Add(name: "P_LOGO", value: CompanyRepo.LOGO, dbType: DbType.Binary, direction: ParameterDirection.Input);
               
                int result = con.Execute(sql: "UPD_COMPANY_DET", param: param, commandType: CommandType.StoredProcedure);

                if (result < 0)
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




    }
}
