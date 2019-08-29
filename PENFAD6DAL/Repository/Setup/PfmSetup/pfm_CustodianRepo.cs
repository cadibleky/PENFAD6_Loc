using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_CustodianRepo
    {
        public string Custodian_Id { get; set; }
        public string Custodian_Name{ get; set; }
        public string Contact_Person { get; set; }
        public string Mobile_Number { get; set; }
        public string Phone_Number { get; set; }
        public string Office_Location { get; set; }
        public string Postal_Address { get; set; }
        public string Email_Address { get; set; }
        public string Registration_Number { get; set; }
        public DateTime Registration_Date { get; set; }
        public string Custodian_Status { get; set; }
        public string Maker_Id { get; set; }
       
        public  string Updated_Id { get; set; }
      
        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
      

        IDbConnection con;

  
        public void SaveRecord(pfm_CustodianRepo CustodianRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Custodian_Id", value: CustodianRepo.Custodian_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Custodian_Name", value: CustodianRepo.Custodian_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Contact_Person", value: CustodianRepo.Contact_Person, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Mobile_Number", value: CustodianRepo.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Office_Location", value: CustodianRepo.Office_Location, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Postal_Address", value: CustodianRepo.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Email_Address", value: CustodianRepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Registration_Number", value: CustodianRepo.Registration_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Registration_Date", value: CustodianRepo.Registration_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Custodian_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MAKER_ID", value: CustodianRepo.Custodian_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_CUSTODIAN", param, commandType: CommandType.StoredProcedure);
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


   

        public DataSet CustodianData()
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

                cmd.CommandText = "SEL_PFM_CUSTODIAN";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "custod");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<pfm_CustodianRepo> GetCustodianList()
        {
            try
            {
                DataSet dt = CustodianData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_CustodianRepo
                {
                    Custodian_Id = row.Field<string>("CUSTODIAN_ID"),
                    Custodian_Name = row.Field<string>("CUSTODIAN_NAME"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Mobile_Number = row.Field<string>("MOBILE_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Registration_Date = row.Field<DateTime>("REGISTRATION_DATE"),
               


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


        public bool DeleteRecord(string Custodian_Id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Custodian_Id", value: Custodian_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_PFM_CUSTODIAN", param: param, commandType: CommandType.StoredProcedure);
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

        public bool isCustodianUnique(string MCUSTODIAN)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_Custodian_Name'", MCUSTODIAN, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_PFM_CUSTODIANNAMEEXIST", param, commandType: CommandType.StoredProcedure);
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
        public bool CustodianExist(string custodianname, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_Custodian_Name", value: custodianname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_PFM_CUSTODIANNAMEEXIST", param: param, commandType: CommandType.StoredProcedure);

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

