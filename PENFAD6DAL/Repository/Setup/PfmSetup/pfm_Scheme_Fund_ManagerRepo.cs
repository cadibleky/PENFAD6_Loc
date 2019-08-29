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

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_Scheme_Fund_ManagerRepo
    {
        public string Scheme_Fund_Manager_Id { get; set; }
        [Required(ErrorMessage = "Scheme is a required data item.")]
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        [Required(ErrorMessage = "Fund manager is a required data item.")]
        public string Fund_Manager_Id { get; set; }
        public string Fund_Manager { get; set; }
        public string Scheme_Fund_Manager_Status { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Auth_Status { get; set; }
        public string Auth_Id { get; set; }
        public DateTime? Auth_Date { get; set; }


        AppSettings db = new AppSettings();
        public void SaveRecord(pfm_Scheme_Fund_ManagerRepo pfm_schemefundmanagerrepo)
        {
            try
            {
                var param = new DynamicParameters();


                pfm_schemefundmanagerrepo.Auth_Status = "AUTHORIZED";
                pfm_schemefundmanagerrepo.Scheme_Fund_Manager_Status = "ACTIVE";
                param.Add(name: "P_SCHEME_ID", value: pfm_schemefundmanagerrepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FUND_MANAGER_ID", value: pfm_schemefundmanagerrepo.Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_MANAGER_STATUS", value: pfm_schemefundmanagerrepo.Scheme_Fund_Manager_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: pfm_schemefundmanagerrepo.Make_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: pfm_schemefundmanagerrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_PFM_SCHEME_FUND_MANAGER", param: param, commandType: CommandType.StoredProcedure);


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

        public void ApproveRecord(pfm_Scheme_Fund_ManagerRepo pfm_schemefundmanagerrepo)
        {
            try
            {
                var param = new DynamicParameters();

               
                    pfm_schemefundmanagerrepo.Auth_Status = "AUTHORIZED";

                    param.Add(name: "P_SCHEME_FUND_MANAGER_ID", value: pfm_schemefundmanagerrepo.Scheme_Fund_Manager_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: pfm_schemefundmanagerrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: pfm_schemefundmanagerrepo.Auth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.APP_PFM_SCHEME_FUND_MANAGER", param: param, commandType: CommandType.StoredProcedure);
                

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


        public bool DeleteRecord(string scheme_fund_manager_id)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SCHEME_FUND_MANAGER_ID", value: scheme_fund_manager_id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "SETUP_PROCEDURES.DEL_PFM_SCHEME_FUND_MANAGER", param: param, commandType: CommandType.StoredProcedure);
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


        public List<pfm_Scheme_Fund_ManagerRepo> GetSchemeFundManagerList()
        {
            try
            {
                List<pfm_Scheme_Fund_ManagerRepo> ObjSchemeFundManager = new List<pfm_Scheme_Fund_ManagerRepo>();

                return ObjSchemeFundManager = db.GetConnection().Query<pfm_Scheme_Fund_ManagerRepo>("Select * from VW_SEL_PFM_SCHEME_FUND_MANAGER").ToList();
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

        public List<pfm_Scheme_Fund_ManagerRepo> GetUnApproveSchemeFundManagerList()
        {
            try
            {
                List<pfm_Scheme_Fund_ManagerRepo> ObjSchemeFundManager = new List<pfm_Scheme_Fund_ManagerRepo>();

                return ObjSchemeFundManager = db.GetConnection().Query<pfm_Scheme_Fund_ManagerRepo>("Select * from VW_SEL_PFM_UNAPP_SCH_FUND_MAN").ToList();
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


        public bool isSchemeFundManagerUnique(string scheme_id, string fund_manager_Id)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("VSCHEME_ID", scheme_id, DbType.String, ParameterDirection.Input);
                param.Add("VFUND_MANAGER_ID", fund_manager_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);

                db.GetConnection().Execute("SETUP_PROCEDURES.SEL_PFM_SCHEME_FUND_MAN_EXIST", param, commandType: CommandType.StoredProcedure);

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