using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_SchemeRepo
    {
        public string Scheme_Id { get; set; }
        [Required(ErrorMessage = "Scheme name is a required.")]
        public string Scheme_Name { get; set; }
        public string NPRA_Number { get; set; }
        [Required(ErrorMessage = "Please select a tier type.")]
        public string Tier_Type { get; set; }
        public string Tier { get; set; }
        [Required(ErrorMessage = "Please select a custodian.")]
        public string Custodian_Id { get; set; }
        public string Custodian_Name { get; set; }
        [Required(ErrorMessage = "Provide salary rate.")]
        [Range(0, 40, ErrorMessage = "Salary rate must fall between zero (0) and 40.")]
        public decimal Salary_Rate { get; set; }
        [Required(ErrorMessage = "Select final day of submission. e.g select 14 for 14th of the month.")]
        public DateTime First_Deadline_Date { get; set; }
        public DateTime? Next_Deadline_Date { get; set; }
        [Required(ErrorMessage = "Provide a grace period or zero (0) for none.")]
        public decimal Surcharge_Grace_Period { get; set; }
        public DateTime? Next_Surcharge_Date { get; set; }
        [Required(ErrorMessage = "Provide monthly surcharge rate.")]
        [Range(0, 40, ErrorMessage = "Surcharge rate must fall between zero (0) and 40.")]
        public decimal Monthly_Penal_Rate { get; set; }
        //[Required(ErrorMessage = "Status is a required data item.")]
        public string Scheme_Status { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Auth_Status { get; set; }
        public string Auth_Id { get; set; }
        public DateTime? Auth_Date { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Today_Date { get; set; }
        [Required]
        public string Pricing_Type { get; set; }
        public decimal DeadLine_Day { get; set; }














        AppSettings db = new AppSettings();
        public bool AddSaveRecord(pfm_SchemeRepo setup_schemerepo)
        {
            try
            {
                bool success = false;
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(setup_schemerepo.Scheme_Id))
                {
                    setup_schemerepo.Scheme_Status = "PENDING";
                    setup_schemerepo.Auth_Status = "PENDING";
                    setup_schemerepo.Tier = "N/A";
                    param.Add(name: "P_SCHEME_NAME", value: setup_schemerepo.Scheme_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_NPRA_NUMBER", value: setup_schemerepo.NPRA_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TIER_TYPE", value: setup_schemerepo.Tier_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TIER", value: setup_schemerepo.Tier, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_CUSTODIAN_ID", value: setup_schemerepo.Custodian_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_SALARY_RATE", value: setup_schemerepo.Salary_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_FIRST_DEADLINE_DATE", value: setup_schemerepo.First_Deadline_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_NEXT_DEADLINE_DATE", value: setup_schemerepo.First_Deadline_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_SURCHARGE_GRACE_PERIOD", value: setup_schemerepo.Surcharge_Grace_Period, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_MONTHLY_PENAL_RATE", value: setup_schemerepo.Monthly_Penal_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_SCHEME_STATUS", value: setup_schemerepo.Scheme_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_START_DATE", value: setup_schemerepo.Start_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_TODAY_DATE", value: setup_schemerepo.Start_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_PRICING_TYPE", value: setup_schemerepo.Pricing_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: setup_schemerepo.Maker_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKE_DATE", value: setup_schemerepo.Make_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_AUTH_STATUS", value: setup_schemerepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_PFM_SCHEME", param: param, commandType: CommandType.StoredProcedure);
                    success = true;
                }
                return success;

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


        public void EditSaveRecord(pfm_SchemeRepo setup_schemerepo)
        {
            try
            {
                var param = new DynamicParameters();

                
                    //setup_schemerepo.Auth_Status = "EDITING";
                    setup_schemerepo.Tier = "N/A";
                    param.Add(name: "P_SCHEME_ID", value: setup_schemerepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_SCHEME_NAME", value: setup_schemerepo.Scheme_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_NPRA_NUMBER", value: setup_schemerepo.NPRA_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TIER_TYPE", value: setup_schemerepo.Tier_Type, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_TIER", value: setup_schemerepo.Tier, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_CUSTODIAN_ID", value: setup_schemerepo.Custodian_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_SALARY_RATE", value: setup_schemerepo.Salary_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_FIRST_DEADLINE_DATE", value: setup_schemerepo.First_Deadline_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_NEXT_DEADLINE_DATE", value: setup_schemerepo.First_Deadline_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                     param.Add(name: "P_SURCHARGE_GRACE_PERIOD", value: setup_schemerepo.Surcharge_Grace_Period, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_MONTHLY_PENAL_RATE", value: setup_schemerepo.Monthly_Penal_Rate, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_DATE", value: setup_schemerepo.Update_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_ID", value: setup_schemerepo.Update_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.UPD_PFM_SCHEME", param: param, commandType: CommandType.StoredProcedure);
                

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

        public void ReassignSaveRecord(pfm_SchemeRepo setup_schemerepo)
        {
            try
            {
                var param = new DynamicParameters();


                //setup_schemerepo.Auth_Status = "EDITING";
                setup_schemerepo.Tier = "N/A";
                param.Add(name: "P_SCHEME_ID", value: setup_schemerepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CUSTODIAN_ID", value: setup_schemerepo.Custodian_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: setup_schemerepo.Update_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: setup_schemerepo.Update_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "SETUP_PROCEDURES.REASSIGN_CUST_PFM_SCHEME", param: param, commandType: CommandType.StoredProcedure);


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


        public void ApproveRecord(pfm_SchemeRepo pfm_schemerepo)
        {
            try
            {
                var param = new DynamicParameters();


                pfm_schemerepo.Auth_Status = "AUTHORIZED";

                pfm_schemerepo.Scheme_Status = "ACTIVE";


                param.Add(name: "P_SCHEME_ID", value: pfm_schemerepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_STATUS", value: pfm_schemerepo.Scheme_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: pfm_schemerepo.Auth_Status, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_DATE", value: pfm_schemerepo.Auth_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                db.GetConnection().Execute(sql: "SETUP_PROCEDURES.APP_PFM_SCHEME", param: param, commandType: CommandType.StoredProcedure);


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

        public bool DeleteRecord(string scheme_id)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SCHEME_ID", value: scheme_id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "SETUP_PROCEDURES.DEL_PFM_SCHEME", param: param, commandType: CommandType.StoredProcedure);
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


        public List<pfm_SchemeRepo> GetSchemeList()
        {
            try
            {
                List<pfm_SchemeRepo> ObjScheme = new List<pfm_SchemeRepo>();

                return ObjScheme = db.GetConnection().Query<pfm_SchemeRepo>("Select * from VW_SEL_PFM_SCHEME WHERE SCHEME_STATUS ='ACTIVE'").ToList();
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

        public List<pfm_SchemeRepo> GetSchemeActiveList()
        {
            try
            {
                List<pfm_SchemeRepo> ObjScheme = new List<pfm_SchemeRepo>();

                return ObjScheme = db.GetConnection().Query<pfm_SchemeRepo>("Select * from VW_SEL_PFM_SCHEME WHERE SCHEME_STATUS = 'ACTIVE' ").ToList();
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

        public List<pfm_SchemeRepo> GetUnApproveSchemeList()
        {
            try
            {
                List<pfm_SchemeRepo> ObjScheme = new List<pfm_SchemeRepo>();

                return ObjScheme = db.GetConnection().Query<pfm_SchemeRepo>("Select * from VW_SEL_PFM_UNAPPROVE_SCHEME").ToList();
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


        public string GetSchemeStatus(string scheme_id)
        {
            string scheme_statuss = "";
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_pfm_SchemeStatus";
                        cmd.Parameters.Add("p_scheme_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = scheme_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach (DataRow r in dt.Rows)
                            {
                                scheme_statuss = r["SCHEME_STATUS"].ToString();
                            }
                        }
                    }

                    return scheme_statuss;
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

        public List<pfm_CustodianRepo> GetCustodianList()
        {
            try
            {
                List<pfm_CustodianRepo> ObjCustodian = new List<pfm_CustodianRepo>();

                return ObjCustodian = db.GetConnection().Query<pfm_CustodianRepo>("Select * from VW_PFM_CUSTODIAN").ToList();
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


        public bool isSchemeUnique(string scheme_name)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("VSCHEME_NAME", scheme_name, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);

                db.GetConnection().Execute("SETUP_PROCEDURES.SEL_PFM_SCHEME_EXIST", param, commandType: CommandType.StoredProcedure);

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
