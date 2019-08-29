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

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
    public class pfm_SchemeFeeRepo
    {   
        public string Scheme_Fee_Id { get; set; }
        public string Scheme_fm_Fee_Id { get; set; }
        [Required(ErrorMessage = "Please select Scheme")]
        public string Scheme_Id { get; set; }       
        public string Scheme_Name { get; set; }
        [Required(ErrorMessage = "Please select Fee")]
        public string Fee_Id { get; set; }
        public string Fee_Description { get; set; }
        [Required(ErrorMessage = "Fee Type is required")]
        public string Flat_Or_Rate { get; set; }
        [Required]
        public decimal Flat_Or_Rate_Value { get; set; }
        [Required(ErrorMessage = "First Accrual Date is required")]
        public DateTime First_Accrual_Date { get; set; }
       //[Required(ErrorMessage = "Next Accrual Date is required")]
        public DateTime Next_Accrual_Date { get; set; }
        [Required(ErrorMessage = "Accrual Frequency is required")]
        public string Accrual_Frequency { get; set; }       
        //public int Accrual_Frequency_Value { get; set; }
        [Required(ErrorMessage = "First Apply Date is required")]
        public DateTime First_Apply_Date { get; set; }
       // [Required(ErrorMessage = "Next Apply Date is required")]
        public DateTime Next_Apply_Date { get; set; }
        
        //public int Apply_Frequency_Value { get; set; }
        [Required(ErrorMessage = "Apply Frequency is required")]
        public string Apply_Frequency { get; set; }
        public string Auth_Status { get; set; }
        public string Scheme_Fee_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Maker_Id { get; set; }
        public string Update_Id { get; set; }
        public decimal Fee { get; set; }
        public String Fund_Manager_ID { get; set; }
        public String Fund_Manager { get; set; }
        IDbConnection con;
        public void SaveRecord(pfm_SchemeFeeRepo schemeFeeRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_SCHEME_FEE_ID", value:schemeFeeRepo.Scheme_Id + schemeFeeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: schemeFeeRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FEE_ID", value: schemeFeeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FLAT_OR_RATE", value: schemeFeeRepo.Flat_Or_Rate, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FLAT_OR_RATE_VALUE", value: schemeFeeRepo.Flat_Or_Rate_Value, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_FIRST_ACCRUAL_DATE", value: schemeFeeRepo.First_Accrual_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_NEXT_ACCRUAL_DATE", value: schemeFeeRepo.First_Accrual_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_ACCRUAL_FREQUENCY", value: schemeFeeRepo.Accrual_Frequency, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FIRST_APPLY_DATE", value: schemeFeeRepo.First_Apply_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_NEXT_APPLY_DATE", value: schemeFeeRepo.First_Apply_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_APPLY_FREQUENCY", value: schemeFeeRepo.Apply_Frequency, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "P_LAST_ACCRUAL_DATE", value: schemeFeeRepo.First_Accrual_Date.AddDays(-1), dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_LAST_APPLY_DATE", value: schemeFeeRepo.First_Accrual_Date.AddDays(-1), dbType: DbType.Date, direction: ParameterDirection.Input);

                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_SCHEME_FEE", param, commandType: CommandType.StoredProcedure);
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


        //SAVE FUND MANAGER SCHEME FEE RECORD
        public void SaveRecordFM(pfm_SchemeFeeRepo schemeFeeRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters(); 
                param.Add(name: "P_FUND_MANAGER_ID", value: schemeFeeRepo.Fund_Manager_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FEE_ID", value: schemeFeeRepo.Fund_Manager_ID + schemeFeeRepo.Scheme_Id + schemeFeeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: schemeFeeRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FEE_ID", value: schemeFeeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FLAT_OR_RATE", value: schemeFeeRepo.Flat_Or_Rate, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FLAT_OR_RATE_VALUE", value: schemeFeeRepo.Flat_Or_Rate_Value, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_FIRST_ACCRUAL_DATE", value: schemeFeeRepo.First_Accrual_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_NEXT_ACCRUAL_DATE", value: schemeFeeRepo.First_Accrual_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_ACCRUAL_FREQUENCY", value: schemeFeeRepo.Accrual_Frequency, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FIRST_APPLY_DATE", value: schemeFeeRepo.First_Apply_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_NEXT_APPLY_DATE", value: schemeFeeRepo.First_Apply_Date, dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_APPLY_FREQUENCY", value: schemeFeeRepo.Apply_Frequency, dbType: DbType.String, direction: ParameterDirection.Input);

                param.Add(name: "P_LAST_ACCRUAL_DATE", value: schemeFeeRepo.First_Accrual_Date.AddDays(-1), dbType: DbType.Date, direction: ParameterDirection.Input);
                param.Add(name: "P_LAST_APPLY_DATE", value: schemeFeeRepo.First_Accrual_Date.AddDays(-1), dbType: DbType.Date, direction: ParameterDirection.Input);

                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_SCHEME_FEE_FM", param, commandType: CommandType.StoredProcedure);
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


        // Approve Scheme Fee
        public void ApproveSchemeFeeRecord(string SCHEME_FEE_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_SCHEME_FEE_ID", value: SCHEME_FEE_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FEE_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("APP_PFM_SCHEME_FEE", param, commandType: CommandType.StoredProcedure);
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

        public bool DeleteRecord(pfm_SchemeFeeRepo SchemeFeeRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SCHEME_FEE_ID", value: SchemeFeeRepo.Scheme_Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: SchemeFeeRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FEE_ID", value: SchemeFeeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FM_ID", value: "NA", dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_PFM_SCHEME_FEE", param: param, commandType: CommandType.StoredProcedure);

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

        public bool DeleteRecordFM(pfm_SchemeFeeRepo SchemeFeeRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SCHEME_FEE_ID", value: SchemeFeeRepo.Scheme_Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: SchemeFeeRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FEE_ID", value: SchemeFeeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_FM_ID", value: SchemeFeeRepo.Fund_Manager_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_PFM_SCHEME_FEE", param: param, commandType: CommandType.StoredProcedure);

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

        public bool isSchemeFeeUnique(string MSCHEMEFEE)
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_SCHEME_FEE_ID", MSCHEMEFEE, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_PFM_SCHEME_FEE_EXIST", param, commandType: CommandType.StoredProcedure);
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

        public DataSet SchemeFeeData()
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

                cmd.CommandText = "SEL_PFM_SCHEME_FEE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "schemeFee");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_SchemeFeeRepo> GetSchemeFeeList()
        {
            try
            {
                DataSet dt = SchemeFeeData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_SchemeFeeRepo
                {
                    Scheme_Fee_Id = row.Field<string>("SCHEME_FEE_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fee_Id = row.Field<string>("FEE_ID"),
                    Fee_Description = row.Field<string>("FEE_DESCRIPTION"),
                    Flat_Or_Rate = row.Field<string>("FLAT_OR_RATE"),
                    Flat_Or_Rate_Value = row.Field<Decimal>("FLAT_OR_RATE_VALUE"),
                    First_Accrual_Date = row.Field<DateTime>("FIRST_ACCRUAL_DATE"),
                    Next_Accrual_Date = row.Field<DateTime>("NEXT_ACCRUAL_DATE"),
                    Accrual_Frequency = row.Field<string>("ACCRUAL_FREQUENCY"),
                    First_Apply_Date = row.Field<DateTime>("FIRST_APPLY_DATE"),
                    Next_Apply_Date = row.Field<DateTime>("NEXT_APPLY_DATE"),
                    Apply_Frequency = row.Field<string>("APPLY_FREQUENCY"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Scheme_Fee_Status = row.Field<string>("SCHEME_FEE_STATUS"),
                    Fund_Manager_ID = row.Field<string>("SCHEME_FEE_STATUS")
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


        // GET FUND MANAGER FEES
        public DataSet SchemeFMFeeData()
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

                cmd.CommandText = "SEL_PFM_SCHEME_FM_FEE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "schemeFMFee");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_SchemeFeeRepo> GetSchemeFMFeeList()
        {
            try
            {
                DataSet dt = SchemeFMFeeData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_SchemeFeeRepo
                {
                    Scheme_fm_Fee_Id = row.Field<string>("SCHEME_FM_FEE_ID"),
                    Fund_Manager_ID = row.Field<string>("SCHEME_FUND_MANAGER_ID"),
                    Fund_Manager = row.Field<string>("FUND_MANAGER"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fee_Id = row.Field<string>("FEE_ID"),
                    Fee_Description = row.Field<string>("FEE_DESCRIPTION"),
                    Flat_Or_Rate = row.Field<string>("FLAT_OR_RATE"),
                    Flat_Or_Rate_Value = row.Field<Decimal>("FLAT_OR_RATE_VALUE"),
                    First_Accrual_Date = row.Field<DateTime>("FIRST_ACCRUAL_DATE"),
                    Next_Accrual_Date = row.Field<DateTime>("NEXT_ACCRUAL_DATE"),
                    Accrual_Frequency = row.Field<string>("ACCRUAL_FREQUENCY"),
                    First_Apply_Date = row.Field<DateTime>("FIRST_APPLY_DATE"),
                    Next_Apply_Date = row.Field<DateTime>("NEXT_APPLY_DATE"),
                    Apply_Frequency = row.Field<string>("APPLY_FREQUENCY"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Scheme_Fee_Status = row.Field<string>("SCHEME_FEE_STATUS")
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


        // FOR PENDING SCHEME FEES
        public DataSet SchemeFeePendingData()
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

                cmd.CommandText = "SEL_PFM_SCHEME_FEE_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "schemeFee");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_SchemeFeeRepo> GetSchemeFeePendingList()
        {
            try
            {
                DataSet dt = SchemeFeePendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_SchemeFeeRepo
                {
                    Scheme_Fee_Id = row.Field<string>("SCHEME_FEE_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Fee_Id = row.Field<string>("FEE_ID"),
                    Fee_Description = row.Field<string>("FEE_DESCRIPTION"),
                    Flat_Or_Rate = row.Field<string>("FLAT_OR_RATE"),
                    Flat_Or_Rate_Value = row.Field<Decimal>("FLAT_OR_RATE_VALUE"),
                    First_Accrual_Date = row.Field<DateTime>("FIRST_ACCRUAL_DATE"),
                    Next_Accrual_Date = row.Field<DateTime>("NEXT_ACCRUAL_DATE"),
                    Accrual_Frequency = row.Field<string>("ACCRUAL_FREQUENCY"),
                    First_Apply_Date = row.Field<DateTime>("FIRST_APPLY_DATE"),
                    Next_Apply_Date = row.Field<DateTime>("NEXT_APPLY_DATE"),
                    Apply_Frequency = row.Field<string>("APPLY_FREQUENCY"),
                    Auth_Status = row.Field<string>("AUTH_STATUS"),
                    Scheme_Fee_Status = row.Field<string>("SCHEME_FEE_STATUS")
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

        public void checkmaxfee(pfm_SchemeFeeRepo schemeFeeRepo)
        {
            try
            {
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_FEEID", schemeFeeRepo.Fee_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Decimal, ParameterDirection.Output);
                con.GetConnection().Execute("CHECK_FEE", param, commandType: CommandType.StoredProcedure);
                schemeFeeRepo.Fee = param.Get<decimal>("VDATA");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
