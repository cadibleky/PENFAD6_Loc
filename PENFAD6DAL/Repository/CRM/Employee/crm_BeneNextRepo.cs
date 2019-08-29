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
using System.Transactions;

namespace PENFAD6DAL.Repository.CRM.Employee
{
    public class crm_BeneNextRepo
    {
        [Required(ErrorMessage = "Beneficiary or Next of kin  is required")]
        public string Beneficiary_NextOfKin { get; set; }
        public string Beneficiary_NextOfKin_Id { get; set; }
        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string First_Name { get; set; }  
        public string Other_Name { get; set; }
        public string Maiden_Name { get; set; }       
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered primary phone number format is not valid.")]
        public string Phone_Number1 { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered secondary phone number format is not valid.")]
        public string Phone_Number2 { get; set; }
        public string Residential_Address { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email_Address { get; set; }
       
        public string Relationship_Id { get; set; }
        [Required]
        public string Relationship_Name { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime? Date_Of_Birth { get; set; }

        public decimal Beneficiary_Rate { get; set; }

        public string Cust_No { get; set; }
        public string Employee_Id { get; set; }
        public string Employee_Name { get; set; }
        public string EGender { get; set; }
        public string ETitle { get; set; }
        public string ESurname { get; set; }
        public string EFirst_Name { get; set; }
        public string EOther_Name { get; set; }
        public string EMaiden_Name { get; set; }
        public DateTime? EDate_Of_Birth { get; set; }
        public string Beneficiary_NextOfKin_Status { get; set; }     
        public string Maker_Id { get; set; }
        public string Update_Id { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public decimal Total_Rate { get; set; }
        public decimal Beneficiary_Rate_Temp { get; set; }
        public string Employer_Name { get; set; }
        public string Employer_Id { get; set; }
        IDbConnection con;
        public void SaveRecord(crm_BeneNextRepo BeneNextRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_BENEFICIARY_NEXTOFKIN_ID", value: BeneNextRepo.Beneficiary_NextOfKin_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_BENEFICIARY_NEXTOFKIN", value: BeneNextRepo.Beneficiary_NextOfKin, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SURNAME", value: BeneNextRepo.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FIRST_NAME", value: BeneNextRepo.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_OTHER_NAME", value: BeneNextRepo.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAIDEN_NAME", value: BeneNextRepo.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PHONE_NUMBER1", value: BeneNextRepo.Phone_Number1, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PHONE_NUMBER2", value: BeneNextRepo.Phone_Number2, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RESIDENTIAL_ADDRESS", value: BeneNextRepo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMAIL_ADDRESS", value: BeneNextRepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RELATIONSHIP_NAME", value: BeneNextRepo.Relationship_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_DATE_OF_BIRTH", value: BeneNextRepo.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_BENEFICIARY_RATE", value: BeneNextRepo.Beneficiary_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CUST_NO", value: BeneNextRepo.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                
                con.Execute("MIX_CRM_BENE_NEXT", param, commandType: CommandType.StoredProcedure);
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

        public bool DeleteRecord(string BENEFICIARY_NEXTOFKIN_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_BENEFICIARY_NEXTOFKIN_ID", value: BENEFICIARY_NEXTOFKIN_ID, dbType: DbType.Int32, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_CRM_BENE_NEXT", param: param, commandType: CommandType.StoredProcedure);

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

        public void checkrate(crm_BeneNextRepo BeneNextRepo)
        {
            try
            {
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ID", BeneNextRepo.Cust_No, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("CHECK_RATE", param, commandType: CommandType.StoredProcedure);
                if (param.Get<decimal>("VDATA") == 0)
                {
                    BeneNextRepo.Total_Rate = 0;
                }

                BeneNextRepo.Total_Rate = param.Get<decimal>("VDATA");   
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }

        public void checkrate_edit(crm_BeneNextRepo BeneNextRepo)
        {
            try
            {
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ID", BeneNextRepo.Cust_No, DbType.String, ParameterDirection.Input);
                param.Add("P_BID", BeneNextRepo.Beneficiary_NextOfKin_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("CHECK_RATE2", param, commandType: CommandType.StoredProcedure);
                BeneNextRepo.Total_Rate = param.Get<decimal>("VDATA");

            }
           
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataSet BeneNextData()
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

                cmd.CommandText = "SEL_CRM_BENE_NEXT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "beneNext");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_BeneNextRepo> GetBeneNextList()
        {
            try
            {
                DataSet dt = BeneNextData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_BeneNextRepo
                {
                    Beneficiary_NextOfKin_Id = row.Field<string>("BENEFICIARY_NEXTOFKIN_ID"),
                    Beneficiary_NextOfKin = row.Field<string>("BENEFICIARY_NEXTOFKIN"),
                    Surname = row.Field<string>("SURNAME"),
                    First_Name = row.Field<string>("FIRST_NAME"),
                    Other_Name = row.Field<string>("OTHER_NAME"),
                    Maiden_Name = row.Field<string>("MAIDEN_NAME"),
                    Phone_Number1 = row.Field<string>("PHONE_NUMBER1"),
                    Phone_Number2 = row.Field<string>("PHONE_NUMBER2"),
                    Residential_Address = row.Field<string>("RESIDENTIAL_ADDRESS"),
                    Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Relationship_Name = row.Field<string>("RELATIONSHIP_NAME"),
                    Date_Of_Birth = row.Field<DateTime>("DATE_OF_BIRTH"),
                    Beneficiary_Rate = row.Field<decimal>("BENEFICIARY_RATE"),
                    //Beneficiary_Rate_Temp = row.Field<decimal>("BENEFICIARY_RATE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Name = row.Field<string>("EMPLOYEE_NAME"),
                    Beneficiary_NextOfKin_Status = row.Field<string>("BENEFICIARY_NEXTOFKIN_STATUS"),
                    Name = row.Field<string>("SURNAME") + " " + row.Field<string>("FIRST_NAME") + " " + row.Field<string>("OTHER_NAME")
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
  //FILTERING LIST FOR BENEFICIARY AND NEXT OF KIN GRID
        public List<crm_BeneNextRepo> GetBNList(string Cust_No)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_BeneNextRepo> bn = new List<crm_BeneNextRepo>();

                string query = "Select * from VW_BENEFICIARY_NEXT WHERE CUST_NO = '" + Cust_No + "' ";                
                return bn = con.Query<crm_BeneNextRepo>(query).ToList();

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

        public void Create_Bene(crm_BeneNextRepo BeneNextRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_BENEFICIARY_NEXTOFKIN_ID", value: BeneNextRepo.Beneficiary_NextOfKin_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_BENEFICIARY_NEXTOFKIN", value: BeneNextRepo.Beneficiary_NextOfKin, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SURNAME", value: BeneNextRepo.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FIRST_NAME", value: BeneNextRepo.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_OTHER_NAME", value: BeneNextRepo.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAIDEN_NAME", value: BeneNextRepo.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PHONE_NUMBER1", value: BeneNextRepo.Phone_Number1, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PHONE_NUMBER2", value: BeneNextRepo.Phone_Number2, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RESIDENTIAL_ADDRESS", value: BeneNextRepo.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMAIL_ADDRESS", value: BeneNextRepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_RELATIONSHIP_NAME", value: BeneNextRepo.Relationship_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_DATE_OF_BIRTH", value: BeneNextRepo.Date_Of_Birth, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_BENEFICIARY_RATE", value: BeneNextRepo.Beneficiary_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_CUST_NO", value: BeneNextRepo.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute("MIX_CRM_BENE_NEXT", param, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    if (con != null) { con = null; }
                //}
            }
        }

       
        // Get employee list
        public DataSet EmployeeData()
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

                cmd.CommandText = "sel_crm_Employee_Active";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("p_employee_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "employee");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<crm_BeneNextRepo> GetEmployeeList()
        {
            try
            {
                DataSet dt = EmployeeData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_BeneNextRepo
                {
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employer_Name = row.Field<string>("Employer_Name"),
                    Employer_Id = row.Field<string>("Employer_Id"),
                    EFirst_Name = row.Field<string>("First_Name"),
                    ESurname = row.Field<string>("Surname"),
                    EOther_Name = row.Field<string>("Other_Name"),
                    ETitle = row.Field<string>("Title"),
                    EGender = row.Field<string>("Gender"),
                    EMaiden_Name = row.Field<string>("Maiden_Name"),
                    EDate_Of_Birth = row.Field<DateTime?>("Date_Of_Birth"),
                    Employee_Name = row.Field<string>("Title") + " " + row.Field<string>("Surname") + " " + row.Field<string>("First_Name") + " " + row.Field<string>("Other_Name"),

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
