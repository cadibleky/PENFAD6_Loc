using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace PENFAD6DAL.Repository.GL
{
    public class GLAccountRepo
    {
       
        public string Con_Code { get; set; }
        public string Con_Name { get; set; }
        //[Required]
        public string Memo_Code { get; set; }
        public string Memo_Name { get; set; }
       // [Required]
        public string GL_Account_No { get; set; }   
        public string GL_Account_Name { get; set; }
        [Required]
        public string Scheme_Fund_Id { get; set; }
        public string Scheme_Id { get; set; }
        public string Fund_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Fund_Name { get; set; }
        public string GL_Balance { get; set; }
        public string Rec_Status { get; set; }
        public string Bank_Account_Number { get; set; }
        public string Bank { get; set; }
        public string Bank_Branch { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Auth_Id { get; set; }
        public DateTime Auth_Date { get; set; }
        public String COL_ID { get; set; }



        // methods begin

        IDbConnection con;

        public void Add_GL_Account(GLAccountRepo GLARepo)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "P_GL_ACCOUNT_NO", value: GLARepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NAME", value: GLARepo.GL_Account_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MEMO_CODE", value: "108", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: GLARepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_BALANCE", value: "0.00", dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_REC_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "ADD_GL_ACCOUNT", param: param, commandType: CommandType.StoredProcedure);
         
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

        
    public void Add_GL_Account_Bank(GLAccountRepo GLARepo)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "P_GL_ACCOUNT_NO", value: GLARepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NAME", value: GLARepo.GL_Account_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MEMO_CODE", value: "101", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: GLARepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_BALANCE", value: "0.00", dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_REC_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                param.Add(name: "P_BANK_ACCOUNT_NO", value: GLARepo.Bank_Account_Number, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_BANK", value: GLARepo.Bank, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_BANK_BRANCH", value: GLARepo.Bank_Branch, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "ADD_GL_ACCOUNT_BANK", param: param, commandType: CommandType.StoredProcedure);

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


        public void Add_GL_Account_Expense(GLAccountRepo GLARepo)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "P_GL_ACCOUNT_NO", value: GLARepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NAME", value: GLARepo.GL_Account_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MEMO_CODE", value: "403", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: GLARepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_BALANCE", value: "0.00", dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_REC_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

             
                int result = con.Execute(sql: "ADD_GL_ACCOUNT_ESPENSE", param: param, commandType: CommandType.StoredProcedure);

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

        public void Rename_GL_Account_Bank(GLAccountRepo GLARepo)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();
                param.Add(name: "P_COL_ID", value: GLARepo.COL_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_GL_ACCOUNT_NO", value: GLARepo.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_FUND_ID", value: GLARepo.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);               
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "MIX_DEFAULT_ACCOUNT_BANK", param: param, commandType: CommandType.StoredProcedure);

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


        //FILTERING LIST FOR GL ACCOUNT FOR SF
        public List<GLAccountRepo> GetASFList(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from VW_GL_ACCOUNT_SF WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "'  and gl_account_name != 'BANK ACCOUNTS' ";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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

        //FILTERING LIST FOR BANK ACCOUNT FOR SF
        public List<GLAccountRepo> GetASFList2(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from VW_GL_ACCOUNT WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' and MEMO_CODE = '101' and gl_default = 'NO'";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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

        //FILTERING LIST FOR EXPENSE ACCOUNT FOR SF
        public List<GLAccountRepo> GetASFList3(string Scheme_Fund_Id)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from VW_GL_ACC_ALL WHERE SCHEME_FUND_ID = '" + Scheme_Fund_Id + "' and MEMO_CODE = '403'";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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


        //GET BANKS
        public List<GLAccountRepo> GetBank()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from setup_Bank";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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

        //FILTERING LIST FOR  DEFAULT BANK ACCOUNT
        public List<GLAccountRepo> GetASFListDefault()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from VW_GL_ACCOUNT WHERE MEMO_CODE = '101' AND FUND_ID != '01' AND GL_DEFAULT = 'YES'";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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

        //FILTERING LIST FOR GL MEMO
        public List<GLAccountRepo> GetMEMOList(string Con_Code)
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from GL_CLASS WHERE CON_CODE = '" + Con_Code + "'";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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


        public bool DeleteGLRecord(string GL_Account_No)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_GL_ACCOUNT_NO", value: GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_GL_ACCOUNT", param: param, commandType: CommandType.StoredProcedure);

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

        public bool DeleteGLERecord(string GL_Account_No)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_GL_ACCOUNT_NO", value: GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_GL_ACCOUNT_ESPENSE", param: param, commandType: CommandType.StoredProcedure);

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
        


        //LIST FOR GL CONTROL
        public List<GLAccountRepo> GetGLControlList()
        {
            try
            {
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                List<GLAccountRepo> bn = new List<GLAccountRepo>();

                string query = "Select * from GL_CONTROL";
                return bn = con.Query<GLAccountRepo>(query).ToList();

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

        public List<GLChartRepo> GetGLList()
        {
            var db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<GLChartRepo> ObjFund = new List<GLChartRepo>();

                return ObjFund = db.GetConnection().Query<GLChartRepo>("Select * from VW_GL_CHART").ToList();
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


        //Get GL ACCOUNT
        public List<GLAccountRepo> GetGLAccountList()
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<GLAccountRepo> ObjFund = new List<GLAccountRepo>();

                return ObjFund = db.GetConnection().Query<GLAccountRepo>("Select * from VW_GL_ACCOUNT where Memo_Code = '102' OR Memo_Code = '103' ").ToList();
                
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
        public DataSet BankAccountData()
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

                cmd.CommandText = ("SEL_GL_ACCOUNT_BY_BANK_ACCOUNT");

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("p_result", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "bankact");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
      
        public IEnumerable<GLAccountRepo> GetBankAccountList()
        {
            try
            {
                DataSet dt = BankAccountData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new GLAccountRepo
                {
                    GL_Account_No  = row.Field<string>("GL_Account_No"),
                    GL_Account_Name = row.Field<string>("GL_Account_Name"),
                  
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
    } //end class gl repo
}
