using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Transactions;

namespace PENFAD6DAL.Repository.CRM.Employee
{
    //---------------------------------
    //Author: Steve
    //Date: 31/3/2016
    //---------------------------------
    public class crm_EmployeeRepo : crm_EmployerRepo
    {
        public string Cust_No { get; set; }
		public string Fund_Id { get; set; }
        public new string Scheme_Id { get; set; }
        public string Cust_Status { get; set; }
        public string Cust_Status_New { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee Id is required")]
        public string Employee_Id { get; set; }
		public string ESurname { get; set; }
		public string EFirst_Name { get; set; }
		public string EOther_Name { get; set; }
		public string EMaiden_Name { get; set; }
		public string EGender { get; set; }
		public string EDate_Of_Birth { get; set; }
		public string Employee_Name { get; set; }
		public string Title { get; set; }
        public string Employee_Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        public string First_Name { get; set; }
        public string Other_Name { get; set; }
        public string Maiden_Name { get; set; }
        //public string Gender_Id { get; set; }
        public string Gender { get; set; }
        [Required]
        public DateTime Date_Of_Birth { get; set; }
        public string HomeTown { get; set; }
        public string Town_Of_Birth { get; set; }
        //public string Region_Name { get; set; }

        public string Country_Id { get; set; }
        public string Nationtionality { get; set; }
        public string Resident_Country { get; set; }
        public string Postal_Address { get; set; }
        public string Residential_Address { get; set; }
        public string Resident_City { get; set; }
        public string Resident_District { get; set; }
        public string Resident_Region { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Contact person's phone number format is not valid.")]
        public string Mobile_Number { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Contact person's phone number format is not valid.")]
        public string Other_Number { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email_Address { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Other_Email_Address { get; set; }
       // [Required(AllowEmptyStrings = false, ErrorMessage = "Identity Type is required")]
        public string Identity_Type { get; set; }
        public string Identity_Number { get; set; }

        public DateTime? Identity_Issue_Date { get; set; }

        public DateTime? Identity_Expiry_Date { get; set; }

        public string SSNIT_NO { get; set; }
  
        //public string Marital_Status_Id { get; set; }
        public string Profession { get; set; }
        public string Marital_Status { get; set; }
  
        public string AddedById { get; set; }

        public string Position { get; set; }
        public string Position_Name { get; set; }
       
        //public string Employee_Type { get; set; }
        public string Employee_Type { get; set; }

        public DateTime? Date_Of_Employment { get; set; }
 
        //public string Employer_Id { get; set; }
        public string Town_Of_Birth_District { get; set; }
        public string Town_Of_Birth_City { get; set; }
        //[Required]
        public string Town_Of_Birth_Region { get; set; }
        public string Employment_Status { get; set; }
        public string MakerId { get; set; }
        public string AuthId { get; set; }
        public string Individual_Batch { get; set; }
        public string EmpId_SystemUser { get; set; }
        public string Batch_No { get; set; }
        public string TIN { get; set; }
        //[Required]
        public string Father_First_Name { get; set; }
        //[Required]
        public string Father_Middle_Name { get; set; }
        //[Required]
        public string Father_Last_Name { get; set; }

        //[Required]
        public DateTime? Father_Birth_Date { get; set; }
        //[Required]
        public string Mother_First_Name { get; set; }
       // [Required]
        public string Mother_Middle_Name { get; set; }
        //[Required]
        public string Mother_Last_Name { get; set; }
        //[Required]
        public string Mother_Maiden_Name { get; set; }
        //[Required]
        public DateTime? Mother_Birth_Date { get; set; }
        //[Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Mother's phone number format is not valid.")]
        public string Mother_Phone_Number { get; set; }
        //[Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Father's phone number format is not valid.")]
        public string Father_Phone_Number { get; set; }
        public DateTime Employee_Registration_Date { get; set; }

        public string Description { get; set; }
        public string Nationality { get; set; }
        public DateTime Update_Date { get; set; }
        public string Update_Id { get; set; }


        public string Beneficiary_NextOfKin { get; set; }
        public string Beneficiary_NextOfKin_Id { get; set; }

        public string bSurname { get; set; }

        public string bFirst_Name { get; set; }
        public string bOther_Name { get; set; }
        public string bMaiden_Name { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Beneficiary's phone number format is not valid.")]

        public string bPhone_Number1 { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Beneficiary's phone number format is not valid.")]
        public string bPhone_Number2 { get; set; }

        public string bResidential_Address { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string bEmail_Address { get; set; }
        public decimal bBeneficiary_Rate { get; set; }
        public string bRelationship_Id { get; set; }

        public string bRelationship_Name { get; set; }
        public string bName { get; set; }
        public string bBeneficiary_NextOfKin_Status { get; set; }

        public string ESF_Id { get; set; }

        public decimal Total_Unit_Balance { get; set; }
        

        public string SEND_SMS { get; set; }

        public string user_Password { get; set; }
        public byte[] Employee_Photo { get; set; }
        public byte[] Employee_Photo_Id { get; set; }
        public string Message { get; set; }

        public string SSurname { get; set; }
        public string SFirst_Name { get; set; }
        public string SEmployee_Id { get; set; }
        public string SSSNIT { get; set; }

     

        IDbConnection con;

        private crm_EmployeeBatchLogRepo batch_log = new crm_EmployeeBatchLogRepo();


        public bool AddNewEmployeeRecord(crm_EmployeeRepo crmEmployee)
        {
            try
            {
                crmEmployee.user_Password = crmEmployee.Cust_No + "@" + GetRandomvalue().ToString();
                crmEmployee.user_Password = cSecurityRepo.AES_Encrypt(crmEmployee.user_Password);

                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                if (crmEmployee.Employer_Name == "PERSONAL PENSIONS")
                {
                    param.Add(name: "p_CustNo", value: crmEmployee.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmployeeId", value: crmEmployee.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmployerId", value: crmEmployee.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Title", value: crmEmployee.Employee_Title, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Surname", value: crmEmployee.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FirstName", value: crmEmployee.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_OtherName", value: crmEmployee.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MaidenName", value: crmEmployee.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Gender", value: crmEmployee.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_DateofBirth", value: crmEmployee.Date_Of_Birth, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param.Add(name: "p_HomeTown", value: crmEmployee.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownOfBirth", value: crmEmployee.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownofBirthDistrict", value: crmEmployee.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownofBirthCity", value: crmEmployee.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownofBirthRegion", value: crmEmployee.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Nationality", value: crmEmployee.Nationality, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_PostalAddress", value: crmEmployee.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResAddress", value: crmEmployee.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MobileNumber", value: crmEmployee.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_OtherNumber", value: crmEmployee.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmailAddress", value: crmEmployee.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_OtherEmail", value: crmEmployee.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityType", value: crmEmployee.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityIssueDate", value: crmEmployee.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityExpiryDate", value: crmEmployee.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityNumber", value: crmEmployee.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_SSNIT_NO", value: crmEmployee.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TIN", value: crmEmployee.TIN, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MaritalStatus", value: crmEmployee.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "p_Branch", value: crmEmployee.Branch_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Position", value: crmEmployee.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmployeeType", value: crmEmployee.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_IndividualBatch", value: "Individual", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_BatchNo", value: crmEmployee.Batch_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "p_EmpIdSystemUser", value: crmEmployee.EmpId_SystemUser, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_DateOfEmployment", value: crmEmployee.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                    //param.Add(name: "p_EmploymentStatus", value: crmEmployee.Employment_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherLastName", value: crmEmployee.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherFirstName", value: crmEmployee.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherMiddleName", value: crmEmployee.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherBirthDate", value: crmEmployee.Father_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherFirstName", value: crmEmployee.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherLastName", value: crmEmployee.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherMiddleName", value: crmEmployee.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherMaidenName", value: crmEmployee.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherBirthDate", value: crmEmployee.Mother_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param.Add(name: "p_Profession", value: crmEmployee.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherPhoneNumber", value: crmEmployee.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherPhoneNo", value: crmEmployee.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    //param.Add(name: "p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_RegistrationDate", value: crmEmployee.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentCity", value: crmEmployee.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentDistrict", value: crmEmployee.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentRegion", value: crmEmployee.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                    param.Add(name: "p_UserPassword", value: crmEmployee.user_Password, dbType: DbType.String, direction: ParameterDirection.Input);

                    param.Add(name: "P_SCHEME_FUND_ID", value: crmEmployee.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_EMPLOYEE_PHOTO", value: crmEmployee.Employee_Photo, dbType: DbType.Binary, direction: ParameterDirection.Input);
                    param.Add(name: "P_EMPLOYEE_PHOTO_ID", value: crmEmployee.Employee_Photo_Id, dbType: DbType.Binary, direction: ParameterDirection.Input);
                    param.Add(name: "P_PERSONAL_PENSIONS", value: "YES", dbType: DbType.String, direction: ParameterDirection.Input);
                    int result = con.Execute(sql: "MIX_CRM_EMPLOYEE_P", param: param, commandType: CommandType.StoredProcedure);
                

                if (result < 0)
                    return true;
                else
                    return false;

                }
                else
                {
                    param.Add(name: "p_CustNo", value: crmEmployee.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmployeeId", value: crmEmployee.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmployerId", value: crmEmployee.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Title", value: crmEmployee.Employee_Title, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Surname", value: crmEmployee.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FirstName", value: crmEmployee.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_OtherName", value: crmEmployee.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MaidenName", value: crmEmployee.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Gender", value: crmEmployee.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_DateofBirth", value: crmEmployee.Date_Of_Birth, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param.Add(name: "p_HomeTown", value: crmEmployee.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownOfBirth", value: crmEmployee.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownofBirthDistrict", value: crmEmployee.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownofBirthCity", value: crmEmployee.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TownofBirthRegion", value: crmEmployee.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Nationality", value: crmEmployee.Nationality, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_PostalAddress", value: crmEmployee.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResAddress", value: crmEmployee.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MobileNumber", value: crmEmployee.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_OtherNumber", value: crmEmployee.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmailAddress", value: crmEmployee.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_OtherEmail", value: crmEmployee.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityType", value: crmEmployee.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityIssueDate", value: crmEmployee.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityExpiryDate", value: crmEmployee.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_IdentityNumber", value: crmEmployee.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_SSNIT_NO", value: crmEmployee.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_TIN", value: crmEmployee.TIN, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MaritalStatus", value: crmEmployee.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "p_Branch", value: crmEmployee.Branch_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_Position", value: crmEmployee.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_EmployeeType", value: crmEmployee.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_IndividualBatch", value: "Individual", dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_BatchNo", value: crmEmployee.Batch_No, dbType: DbType.String, direction: ParameterDirection.Input);
                    //param.Add(name: "p_EmpIdSystemUser", value: crmEmployee.EmpId_SystemUser, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_DateOfEmployment", value: crmEmployee.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                    //param.Add(name: "p_EmploymentStatus", value: crmEmployee.Employment_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherLastName", value: crmEmployee.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherFirstName", value: crmEmployee.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherMiddleName", value: crmEmployee.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherBirthDate", value: crmEmployee.Father_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherFirstName", value: crmEmployee.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherLastName", value: crmEmployee.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherMiddleName", value: crmEmployee.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherMaidenName", value: crmEmployee.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherBirthDate", value: crmEmployee.Mother_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    param.Add(name: "p_Profession", value: crmEmployee.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_FatherPhoneNumber", value: crmEmployee.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MotherPhoneNo", value: crmEmployee.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    //param.Add(name: "p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_RegistrationDate", value: crmEmployee.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentCity", value: crmEmployee.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentDistrict", value: crmEmployee.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "p_ResidentRegion", value: crmEmployee.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                    param.Add(name: "p_UserPassword", value: crmEmployee.user_Password, dbType: DbType.String, direction: ParameterDirection.Input);

                    param.Add(name: "P_SCHEME_FUND_ID", value: crmEmployee.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_EMPLOYEE_PHOTO", value: crmEmployee.Employee_Photo, dbType: DbType.Binary, direction: ParameterDirection.Input);
                    param.Add(name: "P_EMPLOYEE_PHOTO_ID", value: crmEmployee.Employee_Photo_Id, dbType: DbType.Binary, direction: ParameterDirection.Input);

                    int result = con.Execute(sql: "MIX_CRM_EMPLOYEE", param: param, commandType: CommandType.StoredProcedure);
                    if (result < 0)
                        return true;
                    else
                        return false;

                }
            
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
        public bool AddEmployeeRecordToTmp(crm_EmployeeRepo crmEmployee)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "p_CustNo", value: crmEmployee.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeId", value: crmEmployee.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployerId", value: crmEmployee.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Title", value: crmEmployee.Title, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Surname", value: crmEmployee.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FirstName", value: crmEmployee.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_OtherName", value: crmEmployee.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MaidenName", value: crmEmployee.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Gender", value: crmEmployee.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_DateofBirth", value: crmEmployee.Date_Of_Birth, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_HomeTown", value: crmEmployee.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownOfBirth", value: crmEmployee.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownofBirthDistrict", value: crmEmployee.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownofBirthCity", value: crmEmployee.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownofBirthRegion", value: crmEmployee.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Nationality", value: crmEmployee.Nationality, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_PostalAddress", value: crmEmployee.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResAddress", value: crmEmployee.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MobileNumber", value: crmEmployee.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_OtherNumber", value: crmEmployee.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmailAddress", value: crmEmployee.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_OtherEmail", value: crmEmployee.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityType", value: crmEmployee.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityIssueDate", value: crmEmployee.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityExpiryDate", value: crmEmployee.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityNumber", value: crmEmployee.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_SSNIT_NO", value: crmEmployee.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TIN", value: crmEmployee.TIN, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MaritalStatus", value: crmEmployee.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Branch", value: crmEmployee.Branch_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Position", value: crmEmployee.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeType", value: crmEmployee.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IndividualBatch", value: "Individual", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: crmEmployee.Batch_No, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmpIdSystemUser", value: crmEmployee.EmpId_SystemUser, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_DateOfEmployment", value: crmEmployee.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmploymentStatus", value: crmEmployee.Employment_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherLastName", value: crmEmployee.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherFirstName", value: crmEmployee.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherMiddleName", value: crmEmployee.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherBirthDate", value: crmEmployee.Father_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherFirstName", value: crmEmployee.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherLastName", value: crmEmployee.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherMiddleName", value: crmEmployee.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherMaidenName", value: crmEmployee.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherBirthDate", value: crmEmployee.Mother_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Profession", value: crmEmployee.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherPhoneNumber", value: crmEmployee.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherPhoneNo", value: crmEmployee.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UpdateDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_RegistrationDate", value: crmEmployee.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentCity", value: crmEmployee.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentDistrict", value: crmEmployee.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentRegion", value: crmEmployee.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "P_EMPLOYEE_PHOTO", value: crmEmployee.Employee_Photo, dbType: DbType.Binary, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_PHOTO_ID", value: crmEmployee.Employee_Photo_Id, dbType: DbType.Binary, direction: ParameterDirection.Input);


                int result = con.Execute(sql: "ADD_CRM_EMPLOYEETMP", param: param, commandType: CommandType.StoredProcedure);

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
                cmd.Connection = (OracleConnection) con;

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
		
		//FILTERING LIST FROM EMPLOYEE BY EMPLOYER ID
		public List<crm_EmployeeRepo> GetEmployeeList2(string Employer_Id, string Employer_Name)
        {
            try
            {

                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select  * from VW_ALL_CRM_EMPLOYEES WHERE PERSONAL_PENSIONS = 'YES' and Cust_Status = 'HIRED'";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }
                else
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE Employer_Id = '" + Employer_Id + "' and Cust_Status = 'HIRED'";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }

               

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

        //FILTERING ALL LIST
        public List<crm_EmployeeRepo> GetEmployeeList22(string SFirst_Name, string SSurname, string SEmployee_Id, string SSSNIT)
        {
            try
            {

                if (!string.IsNullOrEmpty(SFirst_Name) && string.IsNullOrEmpty(SSurname) && string.IsNullOrEmpty(SEmployee_Id) && string.IsNullOrEmpty(SSSNIT))
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE  FIRST_NAME like '%'||'" + SFirst_Name.ToUpper() + "'||'%'  ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }

                if (!string.IsNullOrEmpty(SSurname) && string.IsNullOrEmpty(SFirst_Name) && string.IsNullOrEmpty(SEmployee_Id) && string.IsNullOrEmpty(SSSNIT))
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE  surname like '%'||'" + SSurname.ToUpper() + "'||'%'  ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }
                if (!string.IsNullOrEmpty(SEmployee_Id) && string.IsNullOrEmpty(SFirst_Name) && string.IsNullOrEmpty(SSSNIT) && string.IsNullOrEmpty(SSurname))
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE  employee_id like '%'||'" + SEmployee_Id.ToUpper() + "'||'%'  ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }

                if (!string.IsNullOrEmpty(SSSNIT) && string.IsNullOrEmpty(SFirst_Name) && string.IsNullOrEmpty(SSurname) && string.IsNullOrEmpty(SSSNIT))
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE  SSNIT_NO like '%'||'" + SSSNIT.ToUpper() + "'||'%'  ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }


                if (!string.IsNullOrEmpty(SFirst_Name) && !string.IsNullOrEmpty(SSurname))
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE  FIRST_NAME like '%'||'" + SFirst_Name.ToUpper() + "'||'%' AND surname like '%'||'" + SSurname.ToUpper() + "'||'%'  ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }

                else
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_ALL_CRM_EMPLOYEES WHERE  SSNIT_NO like '%'||'" + SSSNIT.ToUpper() + "'||'%'  ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }
                
         
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

        public List<crm_EmployeeRepo> GetEmployeeList5(string Employer_Id, string Employer_Name)
        {
            try
            {
                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select  * from crm_employee WHERE PERSONAL_PENSIONS = 'YES' and  Employer_yes_no = 'NO'";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }
                else
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from crm_employee WHERE Employer_Id = '" + Employer_Id + "' and Employer_yes_no = 'NO' ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();

                }

                

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

        public List<crm_EmployeeRepo> GetEmployeeList5e(string Employer_Id, string Employer_Name)
        {
            try
            {

                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select  * from vw_all_crm_employees WHERE PERSONAL_PENSIONS = 'YES' and  Employer_yes_no = 'NO'";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }
                else
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from vw_all_crm_employees WHERE Employer_Id = '" + Employer_Id + "' and Employer_yes_no = 'NO' ";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();

                }


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

        public List<crm_EmployeeSchemeFundRepo> GetEmployeeList6(string Employer_Id, string Employer_Name) 
        {
            try
            {
                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {

                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeSchemeFundRepo> bn = new List<crm_EmployeeSchemeFundRepo>();

                    string query = "Select  * from VW_REP_ESF_EMP WHERE PERSONAL_PENSIONS = 'YES' and Employer_yes_no = 'NO' and ESF_STATUS = 'ACTIVE' AND PERSONAL_PENSIONS1 ='YES'";
                    return bn = con.Query<crm_EmployeeSchemeFundRepo>(query).ToList();

                    
                }
                else
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeSchemeFundRepo> bn = new List<crm_EmployeeSchemeFundRepo>();

                    string query = "Select * from VW_REP_ESF_EMP WHERE Employer_Id = '" + Employer_Id + "' and Employer_yes_no = 'NO' and ESF_STATUS = 'ACTIVE' ";
                    return bn = con.Query<crm_EmployeeSchemeFundRepo>(query).ToList();


                }


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


        public List<crm_EmployeeRepo> GetEmployeeList8(string Employer_Id, string Employer_Name)
		{
			try
			{
                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select  * from VW_REP_ESF_EMP2 WHERE PERSONAL_PENSIONS = 'YES' and ESF_STATUS = 'ACTIVE' AND PERSONAL_PENSIONS1 ='YES'";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }
                else
                {
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                    string query = "Select * from VW_REP_ESF_EMP2 WHERE Employer_Id = '" + Employer_Id + "' and ESF_STATUS = 'ACTIVE'";
                    return bn = con.Query<crm_EmployeeRepo>(query).ToList();
                }

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



		public IEnumerable<crm_EmployeeRepo> GetEmployeeList(string Employer_Id)
        {
            try
            {
                DataSet dt = EmployeeData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
                {
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Id = row.Field<string>("Employee_Id"),
                    Employer_Id = row.Field<string>("Employer_Id"),
                    First_Name = row.Field<string>("First_Name"),
                    Surname = row.Field<string>("Surname"),
                    Other_Name = (row["Other_Name"]).ToString(),
                    Employee_Title = (row["Title"]).ToString(),
                    Gender = (row["Gender"]).ToString(),
                    Maiden_Name = (row["Maiden_Name"]).ToString(),
                    Date_Of_Birth = Convert.ToDateTime(row["Date_Of_Birth"]),
                    Resident_Country = (row["Resident_Country"]).ToString(),
                    Date_Of_Employment = Convert.ToDateTime(row["Date_Of_Employment"]),
                    Email_Address = (row["Email_Address"]).ToString(),
                    Employee_Type = (row["Employee_Type"]).ToString(),
                    //Identity_Expiry_Date = Convert.ToDateTime(row["Identity_Expiry_Date"]),
                    Identity_Expiry_Date = row.Field<DateTime?>("Identity_Expiry_Date"),
                    HomeTown = (row["HomeTown"]).ToString(),
                    //Identity_Issue_Date = Convert.ToDateTime(row["Identity_Issue_Date"]),
                    Identity_Issue_Date = row.Field<DateTime?>("Identity_Issue_Date"),
                    Identity_Number = (row["Identity_Number"]).ToString(),
                    Identity_Type = (row["Identity_Type"]).ToString(),
                    Marital_Status = (row["Marital_Status"]).ToString(),
                    Mobile_Number = (row["Mobile_Number"]).ToString(),
                    Other_Number = (row["Other_Number"]).ToString(),
                    Other_Email_Address = (row["Other_Email_Address"]).ToString(),
                    Position = (row["Position"]).ToString(),
                    Postal_Address = (row["Postal_Address"]).ToString(),
                    Residential_Address = (row["Residential_Address"]).ToString(),
                    SSNIT_NO = (row["SSNIT_NO"]).ToString(),
                    Town_Of_Birth = (row["Town_Of_Birth"]).ToString(),
                    Town_Of_Birth_City = (row["Town_Of_Birth_City"]).ToString(),
                    Town_Of_Birth_District = (row["Town_Of_Birth_District"]).ToString(),
                    Town_Of_Birth_Region = (row["Town_Of_Birth_Region"]).ToString(),
                    Father_First_Name = (row["Father_First_Name"]).ToString(),
                    Father_Last_Name = (row["Father_Last_Name"]).ToString(),
                    Father_Middle_Name = (row["Father_Middle_Name"]).ToString(),
                    Father_Birth_Date = Convert.ToDateTime(row["Father_Birth_Date"]),
                    Father_Phone_Number = (row["Father_Phone_Number"]).ToString(),
                    Mother_First_Name = (row["Mother_First_Name"]).ToString(),
                    Mother_Middle_Name = (row["Mother_Middle_Name"]).ToString(),
                    Mother_Maiden_Name = (row["Mother_Maiden_Name"]).ToString(),
                    Mother_Birth_Date = Convert.ToDateTime(row["Mother_Birth_Date"]),
                    Mother_Phone_Number = (row["Mother_Phone_Number"]).ToString(),
                  //  Employer_Name = (row["Employer_Name"]).ToString(),
                    Nationality = (row["Nationality"]).ToString(),
                    Resident_City = (row["Resident_City"]).ToString(),
                    Resident_District = (row["Resident_District"]).ToString(),
                    Resident_Region = (row["Resident_Region"]).ToString(),
                   // Scheme_Name = (row["Scheme_Name"]).ToString(),
                    //Scheme_Id = (row["Scheme_Id"]).ToString(),
                    
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
        public List<crm_EmployeeRepo> GetEmpList(string cust_no)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                string query = "Select * from CRM_EMPLOYEE WHERE Cust_No = '" + cust_no + "' ";
                return bn = con.Query<crm_EmployeeRepo>(query).ToList();

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

        public List<crm_EmployeeRepo> GetEmpTempList(string cust_no)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                string query = "Select * from CRM_EMPLOYEE_TEMP WHERE Cust_No = '" + cust_no + "' ";
                return bn = con.Query<crm_EmployeeRepo>(query).ToList();

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Employer_Id"></param>
        /// <returns></returns>


        public DataSet EmployeePendingData()
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

                cmd.CommandText = "SEL_CRM_PENDINGEMPLOYEE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("p_employee_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                //param.GetType<int>

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "employee");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IEnumerable<crm_EmployeeRepo> GetPendingEmployeeList()
        {
            try
            {
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_CRM_EMPLOYEE_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

               // OracleParameter paramemployer_Id = new OracleParameter("p_employer_Id", OracleDbType.Varchar2, Employer_Id, ParameterDirection.Input);
               // cmd.Parameters.Add(paramemployer_Id);

                param = cmd.Parameters.Add("p_employee_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;


                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "employee");
                var eList = ds.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
                {
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Id = row.Field<string>("Employee_Id"),
                    Employer_Id = row.Field<string>("Employer_Id"),
                    First_Name = row.Field<string>("First_Name"),
                    Surname = row.Field<string>("Surname"),
                    Other_Name = (row["Other_Name"]).ToString(),
                    Employee_Title = (row["Title"]).ToString(),
                    Gender = (row["Gender"]).ToString(),
                    Maiden_Name = (row["Maiden_Name"]).ToString(),
                    Date_Of_Birth = row.Field<DateTime>("Date_Of_Birth"),
                    Resident_Country = (row["Resident_Country"]).ToString(),
                    //Date_Of_Employment = Convert.ToDateTime(row["Date_Of_Employment"]),
                    Date_Of_Employment = row.Field<DateTime?>("Date_Of_Employment"),
                    Email_Address = (row["Email_Address"]).ToString(),
                    Employee_Type = (row["Employee_Type"]).ToString(),
                    //Identity_Expiry_Date = Convert.ToDateTime(row["Identity_Expiry_Date"]),
                    Identity_Expiry_Date = row.Field<DateTime?>("Identity_Expiry_Date"),
                    HomeTown = (row["HomeTown"]).ToString(),
                    //Identity_Issue_Date = Convert.ToDateTime(row["Identity_Issue_Date"]),
                    Identity_Issue_Date = row.Field<DateTime?>("Identity_Issue_Date"),
                    Identity_Number = (row["Identity_Number"]).ToString(),
                    Identity_Type = (row["Identity_Type"]).ToString(),
                    Marital_Status = (row["Marital_Status"]).ToString(),
                    Mobile_Number = (row["Mobile_Number"]).ToString(),
                    Other_Number = (row["Other_Number"]).ToString(),
                    Other_Email_Address = (row["Other_Email_Address"]).ToString(),
                    Position = (row["Position"]).ToString(),
                    Postal_Address = (row["Postal_Address"]).ToString(),
                    Residential_Address = (row["Residential_Address"]).ToString(),
                    SSNIT_NO = (row["SSNIT_NO"]).ToString(),
                    Town_Of_Birth = (row["Town_Of_Birth"]).ToString(),
                    Town_Of_Birth_City = (row["Town_Of_Birth_City"]).ToString(),
                    Town_Of_Birth_District = (row["Town_Of_Birth_District"]).ToString(),
                    Town_Of_Birth_Region = (row["Town_Of_Birth_Region"]).ToString(),
                    Father_First_Name = (row["Father_First_Name"]).ToString(),
                    Father_Last_Name = (row["Father_Last_Name"]).ToString(),
                    Father_Middle_Name = (row["Father_Middle_Name"]).ToString(),
                    //Father_Birth_Date = Convert.ToDateTime(row["Father_Birth_Date"]),
                    Father_Birth_Date = row.Field<DateTime?>("Father_Birth_Date"),
                    Father_Phone_Number = (row["Father_Phone_Number"]).ToString(),
                    Mother_First_Name = (row["Mother_First_Name"]).ToString(),
                    Mother_Last_Name = (row["Mother_Last_Name"]).ToString(),
                    Mother_Middle_Name = (row["Mother_Middle_Name"]).ToString(),
                    Mother_Maiden_Name = (row["Mother_Maiden_Name"]).ToString(),
                    // Mother_Birth_Date = Convert.ToDateTime(row["Mother_Birth_Date"]),
                    Mother_Birth_Date = row.Field<DateTime?>("Mother_Birth_Date"),
                    Mother_Phone_Number = (row["Mother_Phone_Number"]).ToString(),
                    Employer_Name = (row["Employer_Name"]).ToString(),
                    Nationality = (row["Nationality"]).ToString(),
                    Resident_City = (row["Resident_City"]).ToString(),
                    Resident_District = (row["Resident_District"]).ToString(),
                    Resident_Region = (row["Resident_Region"]).ToString(),
                    Scheme_Name = (row["Scheme_Name"]).ToString(),
                    SEND_SMS = (row["SEND_SMS"]).ToString(),
                    Scheme_Fund_Id = (row["Scheme_Fund_Id"]).ToString(),

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







        public DataSet EmployeeEditingData2()
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

                //cmd.CommandText = "PAC_EMPLOYEE.SEL_SETUP_EDITINGEMPLOYEE";
                cmd.CommandText = "SELECT * FROM VW_CRM_EMPLOYEETMP";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                param = cmd.Parameters.Add("p_employee_data", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                //param.GetType<int>

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "employee");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<crm_EmployeeRepo> GetEmployeesByEmployerId(string employer_Id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "sel_crm_EmployeeByEmployer_Id";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                OracleParameter paramemployer_Id = new OracleParameter("p_employer_Id", OracleDbType.Varchar2, employer_Id, ParameterDirection.Input);
                cmd.Parameters.Add(paramemployer_Id);
                OracleParameter param2 = new OracleParameter("p_employee_data", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(param2);

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "employee");

                var eList = ds.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
                {
                    Cust_No = row.Field<string>("CUST_NO"),
                    Employee_Id = row.Field<string>("Employee_Id"),
                    Employer_Id = row.Field<string>("Employer_Id"),
                    First_Name = row.Field<string>("First_Name"),
                    Surname = row.Field<string>("Surname"),
                    Other_Name = (row["Other_Name"]).ToString(),
                    Employee_Title = (row["Title"]).ToString(),
                    Gender = (row["Gender"]).ToString(),
                    Maiden_Name = (row["Maiden_Name"]).ToString(),
                    Date_Of_Birth = Convert.ToDateTime(row["Date_Of_Birth"]),
                    Resident_Country = (row["Resident_Country"]).ToString(),
                    Date_Of_Employment = Convert.ToDateTime(row["Date_Of_Employment"]),
                    Email_Address = (row["Email_Address"]).ToString(),
                    Employee_Type = (row["Employee_Type"]).ToString(),
                    //Identity_Expiry_Date = Convert.ToDateTime(row["Identity_Expiry_Date"]),
                    Identity_Expiry_Date = row.Field<DateTime?>("Identity_Expiry_Date"),
                    HomeTown = (row["HomeTown"]).ToString(),
                    //Identity_Issue_Date = Convert.ToDateTime(row["Identity_Issue_Date"]),
                    Identity_Issue_Date = row.Field<DateTime?>("Identity_Issue_Date"),
                    Identity_Number = (row["Identity_Number"]).ToString(),
                    Identity_Type = (row["Identity_Type"]).ToString(),
                    Marital_Status = (row["Marital_Status"]).ToString(),
                    Mobile_Number = (row["Mobile_Number"]).ToString(),
                    Other_Number = (row["Other_Number"]).ToString(),
                    Other_Email_Address = (row["Other_Email_Address"]).ToString(),
                    Position = (row["Position"]).ToString(),
                    Postal_Address = (row["Postal_Address"]).ToString(),
                    Residential_Address = (row["Residential_Address"]).ToString(),
                    SSNIT_NO = (row["SSNIT_NO"]).ToString(),
                    Town_Of_Birth = (row["Town_Of_Birth"]).ToString(),
                    Town_Of_Birth_City = (row["Town_Of_Birth_City"]).ToString(),
                    Town_Of_Birth_District = (row["Town_Of_Birth_District"]).ToString(),
                    Town_Of_Birth_Region = (row["Town_Of_Birth_Region"]).ToString(),
                    Father_First_Name = (row["Father_First_Name"]).ToString(),
                    Father_Last_Name = (row["Father_Last_Name"]).ToString(),
                    Father_Middle_Name = (row["Father_Middle_Name"]).ToString(),
                    Father_Birth_Date = Convert.ToDateTime(row["Father_Birth_Date"]),
                    Father_Phone_Number = (row["Father_Phone_Number"]).ToString(),
                    Mother_First_Name = (row["Mother_First_Name"]).ToString(),
                    Mother_Last_Name = (row["Mother_Last_Name"]).ToString(),
                    Mother_Middle_Name = (row["Mother_Middle_Name"]).ToString(),
                    Mother_Maiden_Name = (row["Mother_Maiden_Name"]).ToString(),
                    Mother_Birth_Date = Convert.ToDateTime(row["Mother_Birth_Date"]),
                    Mother_Phone_Number = (row["Mother_Phone_Number"]).ToString(),
                    Employer_Name = (row["Employer_Name"]).ToString(),
                    Nationality = (row["Nationality"]).ToString(),
                    Resident_City = (row["Resident_City"]).ToString(),
                    Resident_District = (row["Resident_District"]).ToString(),
                    Resident_Region = (row["Resident_Region"]).ToString(),
                    Scheme_Name = (row["Scheme_Name"]).ToString(),
                    Scheme_Id = (row["Scheme_Id"]).ToString(),

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


        public List<crm_EmployeeRepo> GetEditingEmployeeList(string Employer_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                string query = "Select * from CRM_EMPLOYEE_TEMP WHERE Employer_Id = '" + Employer_Id + "' ";
                return bn = con.Query<crm_EmployeeRepo>(query).ToList();

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

        public List<crm_EmployeeRepo> GetEditingEmployeeListAll()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<crm_EmployeeRepo> bn = new List<crm_EmployeeRepo>();

                string query = "Select * from VW_EMPTEM ";
                return bn = con.Query<crm_EmployeeRepo>(query).ToList();

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

        //public IEnumerable<crm_EmployeeRepo> GetEditingEmployeeList(string Employer_Id)
        //{
        //    try
        //    {
        //        var app = new AppSettings();
        //        con = app.GetConnection();

        //        DataSet ds = new DataSet();

        //        OracleDataAdapter da = new OracleDataAdapter();
        //        OracleCommand cmd = new OracleCommand();
        //        OracleParameter param = cmd.CreateParameter();

        //        cmd.CommandText = "sel_crm_Emp_TmpByEmployer_Id";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = (OracleConnection) con;

        //        OracleParameter paramemployer_Id = new OracleParameter("p_employer_Id", OracleDbType.Varchar2, Employer_Id, ParameterDirection.Input);
        //        cmd.Parameters.Add(paramemployer_Id);

        //        param = cmd.Parameters.Add("p_employee_data", OracleDbType.RefCursor);
        //        param.Direction = ParameterDirection.Output;

        //        //param.GetType<int>

        //        da = new OracleDataAdapter(cmd);
        //        da.Fill(ds, "employee");
        //        var eList = ds.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
        //        {
        //            Cust_No = row.Field<string>("CUST_NO"),
        //            Employee_Id = row.Field<string>("Employee_Id"),
        //            Employer_Id = row.Field<string>("Employer_Id"),
        //            First_Name = row.Field<string>("First_Name"),
        //            Surname = row.Field<string>("Surname"),
        //            Other_Name = (row["Other_Name"]).ToString(),
        //            Employee_Title = (row["Title"]).ToString(),
        //            Gender = (row["Gender"]).ToString(),
        //            Maiden_Name = (row["Maiden_Name"]).ToString(),
        //            Date_Of_Birth = Convert.ToDateTime(row["Date_Of_Birth"]),
        //            Resident_Country = (row["Resident_Country"]).ToString(),
        //            Date_Of_Employment = Convert.ToDateTime(row["Date_Of_Employment"]),
        //            Employee_Email_Address = (row["Email_Address"]).ToString(),
        //            Employee_Type = (row["Employee_Type"]).ToString(),
        //            Identity_Expiry_Date = Convert.ToDateTime(row["Identity_Expiry_Date"]),
        //            HomeTown = (row["HomeTown"]).ToString(),
        //            Identity_Issue_Date = Convert.ToDateTime(row["Identity_Issue_Date"]),
        //            Identity_Number = (row["Identity_Number"]).ToString(),
        //            Identity_Type = (row["Identity_Type"]).ToString(),
        //            Marital_Status = (row["Marital_Status"]).ToString(),
        //            Mobile_Number = (row["Mobile_Number"]).ToString(),
        //            Employee_Other_Number = (row["Other_Number"]).ToString(),
        //            Employee_Other_Email = (row["Other_Email_Address"]).ToString(),
        //            Position = (row["Position"]).ToString(),
        //            Employee_Postal_Address = (row["Postal_Address"]).ToString(),
        //            Residential_Address = (row["Residential_Address"]).ToString(),
        //            SSNIT_NO = (row["SSNIT_NO"]).ToString(),
        //            Town_Of_Birth = (row["Town_Of_Birth"]).ToString(),
        //            Town_Of_Birth_City = (row["Town_Of_Birth_City"]).ToString(),
        //            Town_Of_Birth_District = (row["Town_Of_Birth_District"]).ToString(),
        //            Town_Of_Birth_Region = (row["Town_Of_Birth_Region"]).ToString(),
        //            Father_First_Name = (row["Father_First_Name"]).ToString(),
        //            Father_Last_Name = (row["Father_Last_Name"]).ToString(),
        //            Father_Middle_Name = (row["Father_Middle_Name"]).ToString(),
        //            Father_Birth_Date = Convert.ToDateTime(row["Father_Birth_Date"]),
        //            Father_Phone_No = (row["Father_Phone_Number"]).ToString(),
        //            Mother_First_Name = (row["Mother_First_Name"]).ToString(),
        //            Mother_Last_Name = (row["Mother_Last_Name"]).ToString(),
        //            Mother_Middle_Name = (row["Mother_Middle_Name"]).ToString(),
        //            Mother_Maiden_Name = (row["Mother_Maiden_Name"]).ToString(),
        //            Mother_Birth_Date = Convert.ToDateTime(row["Mother_Birth_Date"]),
        //            Mother_Phone_No = (row["Mother_Phone_Number"]).ToString(),
        //            Employer_Name = (row["Employer_Name"]).ToString(),
        //            Nationality = (row["Nationality"]).ToString(),
        //            Resident_City = (row["Resident_City"]).ToString(),
        //            Resident_District = (row["Resident_District"]).ToString(),
        //            Resident_Region = (row["Resident_Region"]).ToString(),
        //            Scheme_Name = (row["Scheme_Name"]).ToString(),
        //            Scheme_Id = (row["Scheme_Id"]).ToString(),

        //        }).ToList();

        //        return eList;

        //    }
        //    catch (Exception)
        //    {

        //        throw;
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

                cmd.CommandText = "SEL_CRM_EMPLOYER_ES";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

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
        public IEnumerable<crm_EmployeeRepo> GetESList()
        {
            try
            {
                DataSet dt = ESData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
                {
                    //Scheme_Id = row.Field<string>("SCHEME_ID"),
                    //Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("Employer_Name")

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

        public bool ApprovePendingEmployee(string mEmployeeId, string mCust_No, string ESF_ID)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
                param.Add(name: "p_EmployeeId", value: mEmployeeId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Cust_No", value: mCust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_auth_id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ESF", value: ESF_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "upd_EmployeeAuthStatus", param: param, commandType: CommandType.StoredProcedure);
                if (result < 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }

        }


        public bool Approve_BatchEmployee_Pending(crm_EmployeeBatchLogRepo repo_emplog)
        {
            TransactionOptions tsOp = new TransactionOptions();
            tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
            tsOp.Timeout = TimeSpan.FromMinutes(20);
            try
            {
                if (string.IsNullOrEmpty(repo_emplog.Batch_No)) 
                    Batch_No = "0";

                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
              
                decimal emp_num = 0;
                var param = new DynamicParameters();
                param.Add(name: "p_batchNo", value: repo_emplog.Batch_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_auth_id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_auth_date", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                param.Add(name: "p_result", value: emp_num, dbType: DbType.Decimal, direction: ParameterDirection.Output);

                con.Execute(sql: "upd_crm_ApproveBatchEmployee", param: param, commandType: CommandType.StoredProcedure);
                decimal tot_emp = param.Get<decimal>("p_result");

                 
                ts.Complete();

                if (tot_emp > 0)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                ts.Dispose();
            }

        }

        public List<crm_EmployeeRepo> GetEmployee_BatchList_ByStatus(string batchno)
        {
            var batch_list = new List<crm_EmployeeRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //if (string.IsNullOrEmpty(cust_status))
                    //    cust_status = "0";
                    if (string.IsNullOrEmpty(batchno))
                        batchno = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_crm_EmployeesByBatchNo";
                        cmd.Parameters.Add("p_batchno", OracleDbType.Varchar2, ParameterDirection.Input).Value = batchno;
                        //cmd.Parameters.Add("p_cust_status", OracleDbType.Varchar2, ParameterDirection.Input).Value = cust_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new crm_EmployeeRepo
                            {
                                Surname = row.Field<string>("SURNAME"),
                                First_Name = row.Field<string>("FIRST_NAME"),
                                Other_Name = row.Field<string>("OTHER_NAME"),
                                Nationality = row.Field<string>("NATIONALITY"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Fund_Name = row.Field<string>("FUND_NAME")
                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        

        public bool Validate_Region_Or_Country_Name_Exist(string regionorcountry, string regionorcountryname)
        {
            try
            {
                regionorcountry = regionorcountry.ToLower();
                regionorcountryname = regionorcountryname.ToUpper();

                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "sel_crm_Check_RegionCountry";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                cmd.Parameters.Add("p_regionorcountry", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountry;
                cmd.Parameters.Add("p_regionorcountryname", OracleDbType.Varchar2, ParameterDirection.Input).Value = regionorcountryname;
                //Output param
                OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                cmd.Parameters.Add(count);

                cmd.ExecuteNonQuery();

                string mtotal = (count.Value).ToString();
                int total = Convert.ToInt32(mtotal);

                if (total > 0)
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
        public bool CompanyEmployeeID_Exist(string CompanyEmployeeID, string Employer_Id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "sel_crm_EmployeeIdExist";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                cmd.Parameters.Add("p_EmployeeId", OracleDbType.Varchar2, ParameterDirection.Input).Value = CompanyEmployeeID;
                cmd.Parameters.Add("p_EmployerId", OracleDbType.Varchar2, ParameterDirection.Input).Value = Employer_Id;
                //Output param
                OracleParameter count = new OracleParameter("p_result", OracleDbType.Decimal, 100, ParameterDirection.Output);
                cmd.Parameters.Add(count);

                cmd.ExecuteNonQuery();

                string mtotal = (count.Value).ToString();
                int total = Convert.ToInt32(mtotal);

                if (total > 0)
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
        public bool EmployeeIdentityIdNumberExist(string Idtypeid, string EmployeeIdNumber)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var cmd = new OracleCommand();

                cmd.CommandText = "sel_crm_ValEmpIdentityIdNo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                OracleParameter empIdentityId = new OracleParameter("p_EmpIdentityId", OracleDbType.Varchar2, Idtypeid, ParameterDirection.Input);
                cmd.Parameters.Add(empIdentityId);

                //Input param
                OracleParameter identityTypeNo = new OracleParameter("p_IdentityTypeNo", OracleDbType.Varchar2, EmployeeIdNumber, ParameterDirection.Input);
                cmd.Parameters.Add(identityTypeNo);

                //Output param
                OracleParameter count = new OracleParameter("p_count", OracleDbType.Decimal, 100, ParameterDirection.Output);
                cmd.Parameters.Add(count);

                cmd.ExecuteNonQuery();

                string mtotal = (count.Value).ToString();
                int result = Convert.ToInt32(mtotal);

                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (OracleException ex)
            {
                throw ex;
                //return false;
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
        public bool SocialSecurityNumberExist(string ssnitno)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "sel_crm_EmployeeSSNITNo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                OracleParameter empIdentityId = new OracleParameter("p_SSNITNo", OracleDbType.Varchar2, ssnitno, ParameterDirection.Input);
                cmd.Parameters.Add(empIdentityId);

                //Output param
                OracleParameter count = new OracleParameter("p_count", OracleDbType.Decimal, 100, ParameterDirection.Output);
                cmd.Parameters.Add(count);

                cmd.ExecuteNonQuery();

                string mtotal = (count.Value).ToString();
                int total = Convert.ToInt32(mtotal);

                if (total > 0)
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
        public bool ValidateEmployeeTIN(string ssnitno)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "sel_CRM_ValEMPLOYEETIN";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                OracleParameter empTin = new OracleParameter("p_TIN", OracleDbType.Varchar2, ssnitno, ParameterDirection.Input);
                cmd.Parameters.Add(empTin);

                //Output param
                OracleParameter count = new OracleParameter("p_count", OracleDbType.Decimal, 100, ParameterDirection.Output);
                cmd.Parameters.Add(count);

                cmd.ExecuteNonQuery();

                string mtotal = (count.Value).ToString();
                int total = Convert.ToInt32(mtotal);

                if (total > 0)
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
        public bool ApprovePendingEmployee(string mEmployeeId)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
                param.Add(name: "p_EmployeeId", value: mEmployeeId, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "PAC_EMPLOYEE.UPD_EMPLOYEEAUTHSTATUS", param: param, commandType: CommandType.StoredProcedure);
                if (result < 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }

        }
        public bool ApproveEditingEmployee(string mEmployeeId)
        {
            try
            {

                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
                param.Add(name: "p_EmployeeId", value: mEmployeeId, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "PAC_EMPLOYEE.UPD_EMPLOYEEAUTHSTATUS", param: param, commandType: CommandType.StoredProcedure);

                if (result < 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;

            }
        }
        public bool UpdateEmployeeWithEmpTmp(crm_EmployeeRepo crmEmployee)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //param.Add(name: "p_CustNo", value: crmEmployee.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmployeeId", value: crmEmployee.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmployerId", value: crmEmployee.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Title", value: crmEmployee.Title, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Surname", value: crmEmployee.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_FirstName", value: crmEmployee.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_OtherName", value: crmEmployee.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MaidenName", value: crmEmployee.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Gender", value: crmEmployee.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_DateofBirth", value: crmEmployee.Date_Of_Birth, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                //param.Add(name: "p_HomeTown", value: crmEmployee.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_TownOfBirth", value: crmEmployee.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_TownofBirthDistrict", value: crmEmployee.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_TownofBirthCity", value: crmEmployee.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_TownofBirthRegion", value: crmEmployee.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Nationality", value: crmEmployee.Nationality, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_PostalAddress", value: crmEmployee.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_ResAddress", value: crmEmployee.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MobileNumber", value: crmEmployee.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_OtherNumber", value: crmEmployee.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmailAddress", value: crmEmployee.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_OtherEmail", value: crmEmployee.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_IdentityType", value: crmEmployee.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_IdentityIssueDate", value: crmEmployee.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_IdentityExpiryDate", value: crmEmployee.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_IdentityNumber", value: crmEmployee.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MaritalStatus", value: crmEmployee.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Position", value: crmEmployee.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmployeeType", value: crmEmployee.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_DateOfEmployment", value: crmEmployee.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_FatherLastName", value: crmEmployee.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_FatherFirstName", value: crmEmployee.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_FatherMiddleName", value: crmEmployee.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_FatherBirthDate", value: crmEmployee.Father_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                //param.Add(name: "p_MotherFirstName", value: crmEmployee.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MotherLastName", value: crmEmployee.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MotherMiddleName", value: crmEmployee.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MotherMaidenName", value: crmEmployee.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MotherBirthDate", value: crmEmployee.Mother_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                //param.Add(name: "p_Profession", value: crmEmployee.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_FatherPhoneNumber", value: crmEmployee.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_MotherPhoneNo", value: crmEmployee.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_AuthId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_AuthDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_UpdateDate", value: crmEmployee.Update_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_UpdateId", value: crmEmployee.Update_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_RegistrationDate", value: crmEmployee.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_ResidentCity", value: crmEmployee.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_ResidentDistrict", value: crmEmployee.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_ResidentRegion", value: crmEmployee.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "P_EMPLOYEE_PHOTO", value: crmEmployee.Employee_Photo, dbType: DbType.Binary, direction: ParameterDirection.Input);
                //param.Add(name: "P_EMPLOYEE_PHOTO_ID", value: crmEmployee.Employee_Photo_Id, dbType: DbType.Binary, direction: ParameterDirection.Input);

                //param.Add(name: "p_SSNIT_NO", value: crmEmployee.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_TIN", value: crmEmployee.TIN, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "p_CustNo", value: crmEmployee.Cust_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeId", value: crmEmployee.Employee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployerId", value: crmEmployee.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Title", value: crmEmployee.Title, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Surname", value: crmEmployee.Surname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FirstName", value: crmEmployee.First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_OtherName", value: crmEmployee.Other_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MaidenName", value: crmEmployee.Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Gender", value: crmEmployee.Gender, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_DateofBirth", value: crmEmployee.Date_Of_Birth, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_HomeTown", value: crmEmployee.HomeTown, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownOfBirth", value: crmEmployee.Town_Of_Birth, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownofBirthDistrict", value: crmEmployee.Town_Of_Birth_District, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownofBirthCity", value: crmEmployee.Town_Of_Birth_City, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TownofBirthRegion", value: crmEmployee.Town_Of_Birth_Region, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Nationality", value: crmEmployee.Nationality, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_PostalAddress", value: crmEmployee.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResAddress", value: crmEmployee.Residential_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MobileNumber", value: crmEmployee.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_OtherNumber", value: crmEmployee.Other_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmailAddress", value: crmEmployee.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_OtherEmail", value: crmEmployee.Other_Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityType", value: crmEmployee.Identity_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityIssueDate", value: crmEmployee.Identity_Issue_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityExpiryDate", value: crmEmployee.Identity_Expiry_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_IdentityNumber", value: crmEmployee.Identity_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_SSNIT_NO", value: crmEmployee.SSNIT_NO, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_TIN", value: crmEmployee.TIN, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MaritalStatus", value: crmEmployee.Marital_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_Branch", value: crmEmployee.Branch_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentCountry", value: crmEmployee.Resident_Country, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Position", value: crmEmployee.Position, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_EmployeeType", value: crmEmployee.Employee_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_IndividualBatch", value: "Individual", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_BatchNo", value: crmEmployee.Batch_No, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmpIdSystemUser", value: crmEmployee.EmpId_SystemUser, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_DateOfEmployment", value: crmEmployee.Date_Of_Employment, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_EmploymentStatus", value: crmEmployee.Employment_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherLastName", value: crmEmployee.Father_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherFirstName", value: crmEmployee.Father_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherMiddleName", value: crmEmployee.Father_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherBirthDate", value: crmEmployee.Father_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherFirstName", value: crmEmployee.Mother_First_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherLastName", value: crmEmployee.Mother_Last_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherMiddleName", value: crmEmployee.Mother_Middle_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherMaidenName", value: crmEmployee.Mother_Maiden_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherBirthDate", value: crmEmployee.Mother_Birth_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "p_Profession", value: crmEmployee.Profession, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_FatherPhoneNumber", value: crmEmployee.Father_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MotherPhoneNo", value: crmEmployee.Mother_Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_RegistrationDate", value: crmEmployee.Employee_Registration_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentCity", value: crmEmployee.Resident_City, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentDistrict", value: crmEmployee.Resident_District, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_ResidentRegion", value: crmEmployee.Resident_Region, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "p_UserPassword", value: crmEmployee.user_Password, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "P_SCHEME_FUND_ID", value: crmEmployee.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_PHOTO", value: crmEmployee.Employee_Photo, dbType: DbType.Binary, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYEE_PHOTO_ID", value: crmEmployee.Employee_Photo_Id, dbType: DbType.Binary, direction: ParameterDirection.Input);


                int result = con.Execute(sql: "UPD_CRM_EMPLOYEEWITHEMPTMP", param: param, commandType: CommandType.StoredProcedure);

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


        public IEnumerable<crm_EmployeeRepo> GetBeneficiaryList(string Cust_No)
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

                cmd.CommandText = "SEL_CRM_BENEFICIARYLIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                OracleParameter paramemployer_Id = new OracleParameter("p_Cust_No", OracleDbType.Varchar2, Cust_No, ParameterDirection.Input);
                cmd.Parameters.Add(paramemployer_Id);

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ES");

                var eList = ds.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
                {
                    Beneficiary_NextOfKin = row.Field<string>("BENEFICIARY_NEXTOFKIN"),
                    Scheme_Name = row.Field<string>("BENEFICIARY_NEXTOFKIN_ID"),
                    bSurname = row.Field<string>("SURNAME"),
                    bFirst_Name = row.Field<string>("FIRST_NAME"),
                    bOther_Name = row.Field<string>("OTHER_NAME"),
                    bMaiden_Name = row.Field<string>("MAIDEN_NAME"),
                    bPhone_Number1 = row.Field<string>("PHONE_NUMBER1"),
                    bPhone_Number2 = row.Field<string>("PHONE_NUMBER2"),
                    bResidential_Address = row.Field<string>("RESIDENTIAL_ADDRESS"),
                    bEmail_Address = row.Field<string>("EMAIL_ADDRESS"),
                    bRelationship_Name = row.Field<string>("RELATIONSHIP_NAME"),
                    bName = row.Field<string>("B_NAME"),
                    bBeneficiary_Rate = row.Field<decimal>("BENEFICIARY_RATE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    bBeneficiary_NextOfKin_Status = row.Field<string>("BENEFICIARY_NEXTOFKIN_STATUS"),

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

        public IEnumerable<crm_EmployeeRepo> GetNextofKinList(string Cust_No)
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

                cmd.CommandText = "SEL_CRM_NEXTOFKINLIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                OracleParameter paramemployer_Id = new OracleParameter("p_Cust_No", OracleDbType.Varchar2, Cust_No, ParameterDirection.Input);
                cmd.Parameters.Add(paramemployer_Id);

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ES");

                var eList = ds.Tables[0].AsEnumerable().Select(row => new crm_EmployeeRepo
                {
                    Beneficiary_NextOfKin = row.Field<string>("BENEFICIARY_NEXTOFKIN"),
                    Scheme_Name = row.Field<string>("BENEFICIARY_NEXTOFKIN_ID"),
                    bSurname = row.Field<string>("SURNAME"),
                    bFirst_Name = row.Field<string>("FIRST_NAME"),
                    bOther_Name = row.Field<string>("OTHER_NAME"),
                    bMaiden_Name = row.Field<string>("MAIDEN_NAME"),
                    bPhone_Number1 = row.Field<string>("PHONE_NUMBER1"),
                    bPhone_Number2 = row.Field<string>("PHONE_NUMBER2"),
                    bResidential_Address = row.Field<string>("RESIDENTIAL_ADDRESS"),
                    bEmail_Address = row.Field<string>("EMAIL_ADDRESS"),
                    bRelationship_Name = row.Field<string>("RELATIONSHIP_NAME"),
                    bName = row.Field<string>("KIN_NAME"),
                    bBeneficiary_Rate = row.Field<decimal>("BENEFICIARY_RATE"),
                    Cust_No = row.Field<string>("CUST_NO"),
                    bBeneficiary_NextOfKin_Status = row.Field<string>("BENEFICIARY_NEXTOFKIN_STATUS"),

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

        //get employee list for employee scheme fund
        public List<crm_EmployeeRepo> GetESFEmployeeList()
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_CRM_EMPLOYEE WHERE CUST_STATUS = '" + "HIRED" + "' and employer_id = '" + "000204" + "'").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }


        //get  list for scheme 
        public List<crm_EmployeeRepo> GetESFEmployeeListscheme()
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from pfm_scheme").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }

        //get employee list 
        public List<crm_EmployeeRepo> GetESFEmployeeListAll()
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_CRM_EMPLOYEE").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }

        //get employee list for employer list
        public List<crm_EmployeeRepo> GetESFEmployerList()
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_EMPLOYER_ES WHERE ES_STATUS = '" + "ACTIVE" + "'").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }


        //get Scheme-Fund list for employer 
        public List<crm_EmployeeRepo> GetEmployerSFList(string Employer_Id)
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_ESF_SCHEME_FUND WHERE EMPLOYER_ID = '" + Employer_Id + "'").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }

        //get Scheme-Fund list for employee 
        public List<crm_EmployeeRepo> GetEmployeeSFList(string Scheme_Fund_Id, string Employer_Id, string Employer_Name)
        {
          
            try
            {

                //get emplyer name
                //Get connection
                var conn = new AppSettings();
                var param = new DynamicParameters();
                param.Add("p_employer_id", Employer_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                conn.GetConnection().Execute("GET_Employer_Name", param, commandType: CommandType.StoredProcedure);
                Employer_Name = param.Get<string>("VDATA");

                if (Employer_Name == "PERSONAL PENSIONS")
                {
                    

                    var db = new AppSettings();
                    con = db.GetConnection();
                    List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                    return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_ES_ESF WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' and PERSONAL_PENSIONS = 'YES' and ESF_STATUS = 'ACTIVE'").ToList();

                }
                else
                {
                    var db = new AppSettings();
                    con = db.GetConnection();
                    List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                    return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_ES_ESF WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' and Employer_id = '" + Employer_Id + "' and ESF_STATUS = 'ACTIVE'").ToList();


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }



        //get Scheme list 
        public List<crm_EmployeeRepo> GetEmployeeSFList2(string Scheme_Id)
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<crm_EmployeeRepo> ObjFund = new List<crm_EmployeeRepo>();

                return ObjFund = db.GetConnection().Query<crm_EmployeeRepo>("Select * from VW_ES_ESF WHERE scheme_id = '" + Scheme_Id + "'").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Dispose();
            }

        }


        private string GetRandomvalue()
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(10000, 90000);

                return randomNumber.ToString();
            }
            catch (Exception)
            {
                return "19074";
                // throw;
            }
        }


		public List<crm_EmployeeRepo> GetEmployee_BatchList_ByStatus_portout(crm_EmployeeRepo emp)
		{
			var batch_list = new List<crm_EmployeeRepo>();
			var app = new AppSettings();
			using (OracleConnection conp = new OracleConnection(app.conString()))
			{
				try
				{
					
					conp.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = conp;
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "SEL_CRM_EMPLOYEESBY_ES_POROUT";
						cmd.Parameters.Add("p_employerid", OracleDbType.Varchar2, ParameterDirection.Input).Value = emp.Employer_Id;
						cmd.Parameters.Add("p_schemeid", OracleDbType.Varchar2, ParameterDirection.Input).Value = emp.Scheme_Id;
						cmd.Parameters.Add("p_fundid", OracleDbType.Varchar2, ParameterDirection.Input).Value = emp.Fund_Id;
						cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

						using (OracleDataAdapter da = new OracleDataAdapter(cmd))
						{
							DataTable dt = new DataTable();
							da.Fill(dt);
							batch_list = dt.AsEnumerable().Select(row => new crm_EmployeeRepo
							{
								Surname = row.Field<string>("SURNAME"),
								First_Name = row.Field<string>("FIRST_NAME"),
								Other_Name = row.Field<string>("OTHER_NAME"),
								Nationality = row.Field<string>("NATIONALITY"),
								Scheme_Fund_Id = row.Field<string>("Scheme_Fund_Id")
								//Fund_Name = row.Field<string>("FUND_NAME")
							}).ToList();

						}
					}

					return batch_list;
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					if (conp.State == ConnectionState.Open)
					{
						conp.Close();
						conp.Dispose();
					}
				}
			}

		}



        public bool isValidUpdate(string Employee_Id)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_E_ID", Employee_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_ISVALID_UPD", param, commandType: CommandType.StoredProcedure);
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


    }//end class
}
