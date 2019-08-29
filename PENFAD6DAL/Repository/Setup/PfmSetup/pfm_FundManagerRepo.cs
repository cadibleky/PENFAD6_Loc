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
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{  public class pfm_FundManagerRepo
    {
        public string Fund_Manager_Id { get; set; }
        [Required(ErrorMessage = "Fund Manager is required")]
        public string Fund_Manager { get; set; }
        [Required(ErrorMessage = "Contact Person is required")]
        public string Contact_Person { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered Moblie phone number format is not valid.")]
        public string Mobile_Number { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered Phone number format is not valid.")]
        public string Phone_Number { get; set; }
        public string Office_Location { get; set; }
        public string Postal_Address { get; set; }
        public string Email_Address { get; set; }
        public string Registration_Number { get; set; }
        public DateTime Registration_Date { get; set; }
        public string Maker_Id { get; set; }
        
        public string Updated_Id { get; set; }
     
        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public string Fund_Manager_Status { get; set; }
        


        IDbConnection con;

    
  


        public void SaveRecord(pfm_FundManagerRepo FundManagerRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Fund_Manager_Id", value: FundManagerRepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Fund_Manager", value: FundManagerRepo.Fund_Manager, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Contact_Person", value: FundManagerRepo.Contact_Person, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Mobile_Number", value: FundManagerRepo.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Phone_Number", value: FundManagerRepo.Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Office_Location", value: FundManagerRepo.Office_Location, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Postal_Address", value: FundManagerRepo.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Email_Address", value: FundManagerRepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Registration_Number", value: FundManagerRepo.Registration_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Registration_Date", value: FundManagerRepo.Registration_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Fund_Manager_Status", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value:"ACTIVE" , dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_FUND_MANAGER", param, commandType: CommandType.StoredProcedure);
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

      

        public bool DeleteRecord(string fundManId)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Fund_Manager_Id", value: fundManId, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_PFM_FUNDMANAGER", param: param, commandType: CommandType.StoredProcedure);
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

        public DataSet FundManagerData()
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

                cmd.CommandText = "SEL_PFM_FUND_MANAGER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "fundmana");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IEnumerable<pfm_FundManagerRepo> GetFundmanagerList()
        {
            try
            {
                DataSet dt = FundManagerData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_FundManagerRepo
                {
                    Fund_Manager_Id  = row.Field<string>("FUND_MANAGER_ID"),
                    Fund_Manager = row.Field<string>("FUND_MANAGER"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Mobile_Number = row.Field<string>("MOBILE_NUMBER"),
                    Phone_Number =row.Field <string >("PHONE_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Registration_Date = row.Field<DateTime>("REGISTRATION_DATE"),

                }).ToList();

                return eList;

            }
            catch (Exception EX)
            {

                throw EX;
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



        public bool FundManagerExist(string fundManagername, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_Fund_Manager", value: fundManagername, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_PFM_FUNDMANAGERNAMEEXIST", param: param, commandType: CommandType.StoredProcedure);

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
