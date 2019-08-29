using Dapper;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_FundRepo
    {
        public string Fund_Id { get; set; }
        [Required(ErrorMessage = "Fund name is a required data item.")]
        public string Fund_Name { get; set; }
        public string Fund_Description { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Auth_Status { get; set; }
        public string Auth_Id { get; set; }
        public DateTime? Auth_Date { get; set; }


        AppSettings db = new AppSettings();
        public void SaveRecord(pfm_FundRepo pfm_fundrepo)
        {
            try
            {
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(pfm_fundrepo.Fund_Id))
                {
                    pfm_fundrepo.Auth_Status = "AUTHORIZED";
                    param.Add(name: "P_FUND_NAME", value: pfm_fundrepo.Fund_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FUND_DESCRIPTION", value: pfm_fundrepo.Fund_Description, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKE_DATE", value: pfm_fundrepo.Make_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: pfm_fundrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_PFM_FUND", param: param, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    pfm_fundrepo.Auth_Status = "AUTHORIZED";
                    param.Add(name: "P_FUND_ID", value: pfm_fundrepo.Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FUND_NAME", value: pfm_fundrepo.Fund_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FUND_DESCRIPTION", value: pfm_fundrepo.Fund_Description, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_ID", value: pfm_fundrepo.Update_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_DATE", value: pfm_fundrepo.Update_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: pfm_fundrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.UPD_PFM_FUND", param: param, commandType: CommandType.StoredProcedure);
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

        public void ApproveRecord(pfm_FundRepo pfm_fundrepo)
        {
            try
            {
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(pfm_fundrepo.Fund_Id))
                {
                    pfm_fundrepo.Auth_Status = "AUTHORIZED";

                    param.Add(name: "P_FUND_ID", value: pfm_fundrepo.Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: pfm_fundrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: pfm_fundrepo.Auth_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: pfm_fundrepo.Auth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.APP_PFM_FUND", param: param, commandType: CommandType.StoredProcedure);
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


        public bool DeleteRecord(string fund_id)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_FUND_ID", value: fund_id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "SETUP_PROCEDURES.DEL_PFM_FUND", param: param, commandType: CommandType.StoredProcedure);
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


        public List<pfm_FundRepo> GetFundList()
        {
            try
            {
                List<pfm_FundRepo> ObjFund = new List<pfm_FundRepo>();

                return ObjFund = db.GetConnection().Query<pfm_FundRepo>("Select * from VW_SEL_PFM_FUND").ToList();
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


        public bool isFundUnique(string fund_name)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("VFUND_NAME", fund_name, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);

                db.GetConnection().Execute("SETUP_PROCEDURES.SEL_PFM_FUND_EXIST", param, commandType: CommandType.StoredProcedure);

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
            finally
            {
                db.Dispose();
            }

        }



    }
}
