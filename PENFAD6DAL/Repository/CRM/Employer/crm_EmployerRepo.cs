
using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
//using PENFAD6DAL.Repository.Remittance.Contribution;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PENFAD6DAL.Repository.CRM.Employer
{
    public class crm_EmployerRepo

    {
        public string Employer_Id { get; set; }
        public string Employer_Id_New { get; set; }
        [Required]
        public string Employer_Name { get; set; }
        public string Tin_No { get; set; }
        public string Ssnit { get; set; }
        public string Npra_Number { get; set; }
        public string Business_Address { get; set; }
        public string Employer_Postal_Address { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Phone number format is not valid.")]
        public string Phone_Number { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Employer_Email_Address { get; set; }
        public string Web_Site { get; set; }
        public string Registration_Number { get; set; }
        public DateTime? Registration_Date { get; set; }   
       // [Required]  
        public string Contact_Person { get; set; }
       // [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Contact person's phone number format is not valid.")]
        public string Contact_Number { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Contact_Email { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Other number format is not valid.")]
        public string Employer_Other_Number { get; set; }
        //[Required(ErrorMessage = "Region is required")]
        public string Region_Id { get; set; }
        public string District_Id { get; set; }
        public string Office_Location { get; set; }
        //[Required(ErrorMessage = "Sector is required")]
        public string Sector_Id { get; set; }
        public string Sector_Name { get; set; }
        public string Region_Name { get; set; }
        public string District_Name { get; set; }
        public string Maker_Id { get; set; }
        public string Updated_Id { get; set; }
        public string Employer_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Auth_Status { get; set; }
        public string ES_Id { get; set; }
       // [Required(ErrorMessage = "Scheme is required")]
        public string Scheme_Id { get; set;}
        public string Scheme_Fund_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Fund_Name { get; set; }

        IDbConnection con;

        //Add Employer
        public void SaveRecord(crm_EmployerRepo empRepo)
        {
            var app = new AppSettings();
            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);

            using (OracleConnection con = new OracleConnection(app.conString()))  //
            {

                try
                {
                    //Get Connection
                    //AppSettings app = new AppSettings();
                    //con = app.GetConnection();
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "p_Employer_Id", value: empRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Employer_Name", value: empRepo.Employer_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Tin", value: empRepo.Tin_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Ssnit", value: empRepo.Ssnit, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Npra_Number", value: empRepo.Npra_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Business_Address", value: empRepo.Business_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Postal_Address", value: empRepo.Employer_Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Phone_Number", value: empRepo.Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Email_Address", value: empRepo.Employer_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Web_Site", value: empRepo.Web_Site, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Registration_Number", value: empRepo.Registration_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Registration_Date", value: empRepo.Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_Contact_Number", value: empRepo.Contact_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Contact_Email", value: empRepo.Contact_Email, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Contact_Person", value: empRepo.Contact_Person, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Other_Number", value: empRepo.Employer_Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Region_Id", value: empRepo.Region_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_District_Id", value: empRepo.District_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Office_Location", value: empRepo.Office_Location, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Sector_Id", value: empRepo.Sector_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Employer_Status", value: empRepo.Employer_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MAKER_ID", value: empRepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Auth_Status", value: empRepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_NEW_EMPLOYEE_ID", value: String.Empty, dbType: DbType.String, direction: ParameterDirection.Output);
                    con.Execute("ADD_CRM_EMPLOYER", param, commandType: CommandType.StoredProcedure);

                    var newemployerid = param.Get<string>("P_NEW_EMPLOYEE_ID");

                    DynamicParameters paramb = new DynamicParameters();
                    paramb.Add(name: "P_ES_ID", value: newemployerid + empRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "P_SCHEME_ID", value: empRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "P_EMPLOYER_ID", value: newemployerid, dbType: DbType.String, direction: ParameterDirection.Input);
                    paramb.Add(name: "P_MAKER_ID", value: empRepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                    con.Execute("MIX_REMIT_EMPLOYER_SCHEME", paramb, commandType: CommandType.StoredProcedure);
                    ts.Complete();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ts.Dispose();
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }
        // Edit Employer
        public void UpdateRecord(crm_EmployerRepo empRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Employer_Id", value: empRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Name", value: empRepo.Employer_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Tin", value: empRepo.Tin_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Ssnit", value: empRepo.Ssnit, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Npra_Number", value: empRepo.Npra_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Business_Address", value: empRepo.Business_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Postal_Address", value: empRepo.Employer_Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Phone_Number", value: empRepo.Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Email_Address", value: empRepo.Employer_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Web_Site", value: empRepo.Web_Site, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Registration_Number", value: empRepo.Registration_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Registration_Date", value: empRepo.Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_Contact_Number", value: empRepo.Contact_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Contact_Email", value: empRepo.Contact_Email, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Contact_Person", value: empRepo.Contact_Person, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Other_Number", value: empRepo.Employer_Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Region_Id", value: empRepo.Region_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_District_Id", value: empRepo.District_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Office_Location", value: empRepo.Office_Location, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Sector_Id", value: empRepo.Sector_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Status", value: empRepo.Employer_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MAKER_ID", value: empRepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Auth_Status", value: empRepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("UPD_CRM_EMPLOYER", param, commandType: CommandType.StoredProcedure);
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

		// Edit Employer status
		public void UpdateRecord_Status(crm_EmployerRepo empRepo)
		{
			try
			{
				//Get Connection
				AppSettings app = new AppSettings();
				con = app.GetConnection();
				DynamicParameters param = new DynamicParameters();
				param.Add(name: "p_Employer_Id", value: empRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input); 
				param.Add(name: "p_E_Status", value: empRepo.Employer_Status, dbType: DbType.String, direction: ParameterDirection.Input); 

				con.Execute("UPD_CRM_EMPLOYER_STATUS", param, commandType: CommandType.StoredProcedure);
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


		// Edit Employer
		public void DeleteEmployerRecord(crm_EmployerRepo empRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Employer_Id", value: empRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("DEL_CRM_EMPLOYER", param, commandType: CommandType.StoredProcedure);
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

        //transfer employee
        public void EmployeeTransfer(crm_EmployeeRepo empRepo)
        {
            try
            {
                //Get Connection
                 AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "p_Cust_No", value: empRepo.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Employer_Id", value: empRepo.Employer_Id_New, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Old_Employer_Id", value: empRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("CHG_CRM_EMPLOYER", param, commandType: CommandType.StoredProcedure);
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


		//status employee
		public void EmployeeStatus(crm_EmployeeRepo empRepo)
		{
			try
			{
				//Get Connection
				AppSettings app = new AppSettings();
				con = app.GetConnection();
				DynamicParameters param = new DynamicParameters();
				param.Add(name: "p_Cust_No", value: empRepo.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "p_Cust_Status", value: empRepo.Cust_Status, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "p_Cust_Status_New", value: empRepo.Cust_Status_New, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "p_Make_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
				con.Execute("CHG_CRM_EMPLOYEE_STATUS", param, commandType: CommandType.StoredProcedure);
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

		//Approved Empoyer...............................................................
		public void ApprovedNewRecord(string p_Employer_Id, string p_Employer_Status, string p_Auth_Id, string p_Auth_Status, string p_Es_Status)
        {
            var app = new AppSettings();
            //using (OracleConnection conp = new OracleConnection(app.GetConnection()))
            //    var app = new AppSettings();
            con = app.GetConnection();

            {

                OracleCommand oraCmd = new OracleCommand("UPD_CRM_EMPLOYERAPPOVAL");
                oraCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oraCmd.Parameters.Add(new OracleParameter("p_Employer_Id", OracleDbType.Varchar2)).Value = p_Employer_Id;
                oraCmd.Parameters.Add(new OracleParameter("p_EmployerStatus", OracleDbType.Varchar2)).Value = p_Employer_Status;
                oraCmd.Parameters.Add(new OracleParameter("p_Auth_Id", OracleDbType.Varchar2)).Value = p_Auth_Id;
                oraCmd.Parameters.Add(new OracleParameter("p_Auth_Status", OracleDbType.Varchar2)).Value = p_Auth_Status;
                oraCmd.Parameters.Add(new OracleParameter("p_Es_Status", OracleDbType.Varchar2)).Value = p_Es_Status;


                OracleConnection oConn = new OracleConnection(app.conString());
                try
                {
                    oConn.Open();
                    oraCmd.Connection = oConn;
                    oraCmd.ExecuteNonQuery();
                }
                finally
                {
                    oraCmd.Dispose();
                    oConn.Close();
                }
            }
        }

        public bool DisapprovedNewRecord(crm_EmployerRepo EmployerRepo)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Employer_Id", value: Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_CRM_EMPLOYER", param: param, commandType: CommandType.StoredProcedure);
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


        public void ApprovedEditedRecord(string p_Employer_Id, string p_Employer_Status, string p_Update_Id, string p_Auth_Status)
        {
            var app = new AppSettings();
            con = app.GetConnection();

            {
                OracleCommand oraCmd = new OracleCommand("UPD_CRM_EMPLOYER_EDIT_APPOVAL");
                oraCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oraCmd.Parameters.Add(new OracleParameter("p_Employer_Id", OracleDbType.Varchar2)).Value = p_Employer_Id;
                oraCmd.Parameters.Add(new OracleParameter("p_EmployerStatus", OracleDbType.Varchar2)).Value = p_Employer_Status;
                oraCmd.Parameters.Add(new OracleParameter("p_Update_Id", OracleDbType.Varchar2)).Value = p_Update_Id;
                oraCmd.Parameters.Add(new OracleParameter("p_Auth_Status", OracleDbType.Varchar2)).Value = p_Auth_Status;

                OracleConnection oConn = new OracleConnection(app.conString());
                try
                {
                    oConn.Open();
                    oraCmd.Connection = oConn;
                    oraCmd.ExecuteNonQuery();

                }
                finally
                {
                    oraCmd.Dispose();
                    oConn.Close();
                }
            }
        }

        public bool DisapprovedEditedRecord(crm_EmployerRepo EmployerRepo)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Employer_Id", value: Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_CRM_EMPLOYER_TEMP", param: param, commandType: CommandType.StoredProcedure);
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


        public bool DeleteRecord(string Cust_No)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Cust_No", value: Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_CRM_EMPLOYEE", param: param, commandType: CommandType.StoredProcedure);
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


        public bool DeleteRecordDisApp(string Cust_No)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Cust_No", value: Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_CRM_EMPLOYEE_DISAPP", param: param, commandType: CommandType.StoredProcedure);
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

                cmd.CommandText = "SEL_CRM_EMPLOYER_ACTIVE";
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
                throw;
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
                    Employer_Name = row.Field<string>("Employer_Name"),
                    Tin_No = row.Field<string>("TIN"),
                    Ssnit = row.Field<string>("SSNIT"),
                    Npra_Number = row.Field<string>("NPRA_NUMBER"),
                    Business_Address = row.Field<string>("BUSINESS_ADDRESS"),
                    Employer_Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    Employer_Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Web_Site = row.Field<string>("WEB_SITE"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Registration_Date = row.Field<DateTime?>("REGISTRATION_DATE"),
                    Contact_Number = row.Field<string>("CONTACT_NUMBER"),
                    Contact_Email = row.Field<string>("CONTACT_EMAIL"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Employer_Other_Number = row.Field<string>("OTHER_NUMBER"),
                    Region_Id = row.Field<string>("REGION_ID"),
                    District_Id = row.Field<string>("DISTRICT_ID"),
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

        //Aproved Employer Data..............................................................
        public DataSet EmployerApproveData2()
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
                cmd.CommandText =  "SEL_CRM_EMPLOYER_EDITING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;
                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "appemp");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IEnumerable<crm_EmployerRepo> GetEmployerAppList2()
        {
            try
            {
                DataSet dt = EmployerApproveData2();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerRepo
                {
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("Employer_Name"),
                    Tin_No = row.Field<string>("TIN"),
                    Ssnit = row.Field<string>("SSNIT"),
                    Npra_Number = row.Field<string>("NPRA_NUMBER"),
                    Business_Address = row.Field<string>("BUSINESS_ADDRESS"),
                    Employer_Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    Employer_Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Web_Site = row.Field<string>("WEB_SITE"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Registration_Date = row.Field<DateTime?>("REGISTRATION_DATE"),
                    Contact_Number = row.Field<string>("CONTACT_NUMBER"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Contact_Email = row.Field<string>("CONTACT_EMAIL"),
                    Employer_Other_Number = row.Field<string>("OTHER_NUMBER"),
                    Region_Id = row.Field<string>("REGION_ID"),
                    District_Id = row.Field<string>("DISTRICT_ID"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Sector_Id = row.Field<string>("SECTOR_ID"),
                    ////Scheme_Id = row.Field<string>("scheme_id"),
                    ////Scheme_Name = row.Field<string>("scheme_name"),


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

        //Aproved Employer Data..............................................................
        public DataSet EmployerApproveData()
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
                cmd.CommandText = "SEL_CRM_EMPLOYER_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;
                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "appemp");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IEnumerable<crm_EmployerRepo> GetEmployerAppList()
        {
            try
            {
                DataSet dt = EmployerApproveData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerRepo
                {
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("Employer_Name"),
                    Tin_No = row.Field<string>("TIN"),
                    Ssnit = row.Field<string>("SSNIT"),
                    Npra_Number = row.Field<string>("NPRA_NUMBER"),
                    Business_Address = row.Field<string>("BUSINESS_ADDRESS"),
                    Employer_Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    Employer_Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Web_Site = row.Field<string>("WEB_SITE"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Registration_Date = row.Field<DateTime?>("REGISTRATION_DATE"),
                    Contact_Number = row.Field<string>("CONTACT_NUMBER"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Contact_Email = row.Field<string>("CONTACT_EMAIL"),
                    Employer_Other_Number = row.Field<string>("OTHER_NUMBER"),
                    Region_Id = row.Field<string>("REGION_ID"),
                    District_Id = row.Field<string>("DISTRICT_ID"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Sector_Id = row.Field<string>("SECTOR_ID"),
                    Scheme_Id = row.Field<string>("scheme_id"),
                    Scheme_Name = row.Field<string>("scheme_name"),


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

     
        public bool EmployerExist(string emprname, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_Employer_Name", value: emprname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_CRM_EMPLOYEREXIST", param: param, commandType: CommandType.StoredProcedure);

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

        public DataSet GetEmployersDataList()
        {
            try
            {

                var ds = new DataSet();

                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var cmd = new SqlCommand();

                cmd.CommandText = "dbo.sel_crm_EmployerByEmployerName";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (SqlConnection)con;

                IDbDataParameter param = cmd.CreateParameter();

                //Input Param
                param.ParameterName = "p_EmployerName";
                param.DbType = DbType.String;
                param.Value = this.Employer_Name;
                param.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(param);

                try
                {
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(ds, "employer");
                }
                catch
                {
                    throw;
                }


                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetEmployerDataList()
        {
            try
            {

                var ds = new DataSet();

                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var cmd = new SqlCommand();

                cmd.CommandText = "dbo.sel_crm_EmployerByEmployerName";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (SqlConnection)con;

                IDbDataParameter param = cmd.CreateParameter();

                //Input Param
                param.ParameterName = "p_EmployerName";
                param.DbType = DbType.String;
                param.Value = this.Employer_Name;
                param.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(param);

                try
                {
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(ds, "employer");
                }
                catch
                {
                    throw;
                }


                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<crm_EmployerRepo> GetEmployerData()
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                IEnumerable<crm_EmployerRepo> result = con.Query<crm_EmployerRepo>(sql: "select * from vw_crm_employer");
                return result;
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

        public IEnumerable<crm_EmployerRepo> GetDistrictList()
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                IEnumerable<crm_EmployerRepo> result = con.Query<crm_EmployerRepo>(sql: "select * from setup_district");
                return result;
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

