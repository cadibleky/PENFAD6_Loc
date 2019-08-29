using Dapper;
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
    public class setup_IssuerRepo
    {

        public string Issuer_Id { get; set; }
        [Required(ErrorMessage = "Issuer name is a required data item.")]
        public string Issuer_Name { get; set; }
        public string Postal_Address { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email_Address { get; set; }
        //[DataType(dataType: DataType.PhoneNumber) RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered a valid phone number.")]
       // [DataType(dataType: DataType.PhoneNumber) RegularExpression(@"^\(?([+]{1})?[-. ]?([0-9]{2})?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered a valid phone number.")]

        
        public string Mobile_Number { get; set; }
        public string Phone_Number { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Auth_Status { get; set; }
        public string Auth_Id { get; set; }
        public DateTime? Auth_Date { get; set; }


        AppSettings db = new AppSettings();
        public void SaveRecord(setup_IssuerRepo setup_issuerrepo)
        {
            try
            {
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(setup_issuerrepo.Issuer_Id))
                {
                    setup_issuerrepo.Auth_Status = "AUTHORIZED";

                    param.Add(name: "P_ISSUER_NAME", value: setup_issuerrepo.Issuer_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_POSTAL_ADDRESS", value: setup_issuerrepo.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_EMAIL_ADDRESS", value: setup_issuerrepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MOBILE_NUMBER", value: setup_issuerrepo.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PHONE_NUMBER", value: setup_issuerrepo.Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: setup_issuerrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKE_DATE", value: DateTime.Today , dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_SETUP_ISSUER", param: param, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    setup_issuerrepo.Auth_Status = "AUTHORIZED";

                    param.Add(name: "P_ISSUER_ID", value: setup_issuerrepo.Issuer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_ISSUER_NAME", value: setup_issuerrepo.Issuer_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_POSTAL_ADDRESS", value: setup_issuerrepo.Postal_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_EMAIL_ADDRESS", value: setup_issuerrepo.Email_Address, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MOBILE_NUMBER", value: setup_issuerrepo.Mobile_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_PHONE_NUMBER", value: setup_issuerrepo.Phone_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: setup_issuerrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_DATE", value:DateTime.Today , dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.UPD_SETUP_ISSUER", param: param, commandType: CommandType.StoredProcedure);
                }

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

        public void ApproveRecord(setup_IssuerRepo setup_issuerrepo)
        {
            try
            {
                var param = new DynamicParameters();

                
                    setup_issuerrepo.Auth_Status = "AUTHORIZED";

                    param.Add(name: "P_ISSUER_ID", value: setup_issuerrepo.Issuer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: setup_issuerrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: setup_issuerrepo.Auth_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: setup_issuerrepo.Auth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.APP_SETUP_ISSUER", param: param, commandType: CommandType.StoredProcedure);

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

        public bool DeleteRecord(string sector_id)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_ISSUER_ID", value: sector_id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "SETUP_PROCEDURES.DEL_SETUP_ISSUER", param: param, commandType: CommandType.StoredProcedure);
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
                db.Dispose();
            }

        }


        public List<setup_IssuerRepo> GetIssuerList()
        {
            try
            {
                List<setup_IssuerRepo> ObjIssuer = new List<setup_IssuerRepo>();

                return ObjIssuer = db.GetConnection().Query<setup_IssuerRepo>("Select * from VW_SEL_SETUP_ISSUER").ToList();
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


        
        public List<setup_IssuerRepo> GetIssuerUnApproveList()
        {
            try
            {
                List<setup_IssuerRepo> ObjIssuer = new List<setup_IssuerRepo>();

                return ObjIssuer = db.GetConnection().Query<setup_IssuerRepo>("Select * from VW_SEL_SETUP_UNAPPROVE_ISSUER").ToList();
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
    }
}
