using Dapper;
using PENFAD6DAL.DbContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_Scheme_FundRepo
    {
        public string Scheme_Fund_Id { get; set; }
        [Required(ErrorMessage = "Scheme is a required data item.")]
        public string Scheme_Id { get; set; }
        public string Scheme_Name { get; set; }
        [Required(ErrorMessage = "Fund is a required data item.")]
        public string Fund_Id { get; set; }
        public string Fund_Name { get; set; }
        [Required(ErrorMessage = "Unit price is a required data item.")]
        public decimal? Unit_Price { get; set; }
        public decimal? Total_Unit_Balance { get; set; }
        public decimal? NAV { get; set; }
        public decimal? AUM { get; set; }
        public DateTime? Last_Value_Date { get; set; }
        //[Required(ErrorMessage = "Status is a required data item.")]
        public string Scheme_Fund_Status { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Auth_Status { get; set; }
        public string Auth_Id { get; set; }
        public string COL_ID { get; set; }
        public string GL_Account_Name { get; set; }
        public string GL_Account_Id { get; set; }
        public string GL_Account_No { get; set; }
        public string Fund_Manager_Id { get; set; }
        public string Fund_Manager { get; set; }
        public string Account_Name { get; set; }
        public DateTime? Auth_Date { get; set; }
        public string TID { get; set; }
        IDbConnection con;

        AppSettings db = new AppSettings();
    
        public void SaveRecord(pfm_Scheme_FundRepo pfm_schemefundrepo)
        {
            try
            {
                var param = new DynamicParameters();

                pfm_schemefundrepo.Auth_Status = "AUTHORIZED";
                pfm_schemefundrepo.Scheme_Fund_Status = "ACTIVE";
                param.Add(name: "P_SCHEME_ID", value: pfm_schemefundrepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FUND_ID", value: pfm_schemefundrepo.Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UNIT_PRICE", value: pfm_schemefundrepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_STATUS", value: pfm_schemefundrepo.Scheme_Fund_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: pfm_schemefundrepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: pfm_schemefundrepo.Make_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: pfm_schemefundrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_PFM_SCHEME_FUND", param: param, commandType: CommandType.StoredProcedure);
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

        public void SaveRecordWithNAV(pfm_Scheme_FundRepo pfm_schemefundrepo)
        {
            try
            {
                var param = new DynamicParameters();


               // pfm_schemefundrepo.Auth_Status = "EDITING";
                param.Add(name: "P_SCHEME_FUND_ID", value: pfm_schemefundrepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UNIT_PRICE", value: pfm_schemefundrepo.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_NAV", value: pfm_schemefundrepo.NAV, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_AUM", value: pfm_schemefundrepo.AUM, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: pfm_schemefundrepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: pfm_schemefundrepo.Make_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                //param.Add(name: "P_AUTH_STATUS", value: pfm_schemefundrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "SETUP_PROCEDURES.UPD_PFM_SCHEME_FUND_NAV", param: param, commandType: CommandType.StoredProcedure);


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

        public void ApproveRecord(pfm_Scheme_FundRepo pfm_schemefundrepo)
        {
            try
            {
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(pfm_schemefundrepo.Scheme_Fund_Id))
                {
                    pfm_schemefundrepo.Auth_Status = "AUTHORIZED";

                    param.Add(name: "P_SCHEME_FUND_ID", value: pfm_schemefundrepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: pfm_schemefundrepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_ID", value: pfm_schemefundrepo.Auth_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_DATE", value: pfm_schemefundrepo.Auth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

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


        public bool DeleteRecord(string scheme_fund_id)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SCHEME_FUND_ID", value: scheme_fund_id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "SETUP_PROCEDURES.DEL_PFM_SCHEME_FUND", param: param, commandType: CommandType.StoredProcedure);
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


        public List<pfm_Scheme_FundRepo> GetSchemeFundList()
        {
            try
            {
                List<pfm_Scheme_FundRepo> ObjFund = new List<pfm_Scheme_FundRepo>();

                return ObjFund = db.GetConnection().Query<pfm_Scheme_FundRepo>("Select * from VW_PFM_SCHEME_FUND ").ToList();
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

        public List<pfm_Scheme_FundRepo> GetSchemeFundListscheme()
        {
            try
            {
                List<pfm_Scheme_FundRepo> ObjFund = new List<pfm_Scheme_FundRepo>();

                return ObjFund = db.GetConnection().Query<pfm_Scheme_FundRepo>("Select * from PFM_SCHEME ").ToList();
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


        public List<pfm_Scheme_FundRepo> GetSchemeFundListschemeFM()
        {
            try
            {
                List<pfm_Scheme_FundRepo> ObjFund = new List<pfm_Scheme_FundRepo>();

                return ObjFund = db.GetConnection().Query<pfm_Scheme_FundRepo>("Select * from VW_FM_PVR ").ToList();
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

        public List<pfm_Scheme_FundRepo> GetSchemeFundList_coll()
        {
            try
            {
                List<pfm_Scheme_FundRepo> ObjFund = new List<pfm_Scheme_FundRepo>();

                return ObjFund = db.GetConnection().Query<pfm_Scheme_FundRepo>("Select * from VW_COLL_ACCOUNT").ToList();
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

        public List<pfm_Scheme_FundRepo> GetSchemeFundListBySchemeId(string Scheme_Id)
        {
            try
            {
                List<pfm_Scheme_FundRepo> ObjFund1 = new List<pfm_Scheme_FundRepo>();

                return ObjFund1 = db.GetConnection().Query<pfm_Scheme_FundRepo>("Select * from VW_PFM_SCHEME_FUND Where SCHEME_ID = '" + Scheme_Id + "' ").ToList();
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

        //FILTERING LIST OF SCHEME-FUND
        public List<pfm_Scheme_FundRepo> GetESCHEMEList()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<pfm_Scheme_FundRepo> bn = new List<pfm_Scheme_FundRepo>();

                string query = "Select * from VW_SF_LIST ";
                return bn = con.Query<pfm_Scheme_FundRepo>(query).ToList();

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

        public List<pfm_Scheme_FundRepo> GetSchemeFundGLList()
        {
            try
            {
                List<pfm_Scheme_FundRepo> ObjFund = new List<pfm_Scheme_FundRepo>();

                return ObjFund = db.GetConnection().Query<pfm_Scheme_FundRepo>("Select * from VW_MERGE_GL_SCHEME ").ToList();
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


        public void SaveRecordGL(pfm_Scheme_FundRepo pfm_schemefundrepo)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add(name: "P_SCHEME_ID", value: pfm_schemefundrepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_ID", value: pfm_schemefundrepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ACCOUNT_NAME", value: pfm_schemefundrepo.Account_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                 param.Add(name: "P_MAKER_ID", value: pfm_schemefundrepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: pfm_schemefundrepo.Make_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
               db.GetConnection().Execute(sql: "ADD_MERGE_GL_SCHEME", param: param, commandType: CommandType.StoredProcedure);
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


        public bool DeleteRecordGL(string TID)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_TID", value: TID, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "DEL_MERGE_SCHEME_GL", param: param, commandType: CommandType.StoredProcedure);
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


    }
}
