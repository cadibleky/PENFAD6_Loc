using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Security
{
    public class LoginModelRepo
    {

        AppSettings db = new AppSettings();

        public bool GetLoginUser(sec_UserRepo userrepo)
        {
            try
            {
                var param = new DynamicParameters();
                string security_Code = "teksolencrypt$@teksol.com987908123";
                userrepo.Password = GlobalValue.AES_Encrypt(userrepo.Password, security_Code);

                param.Add("P_USER_ID", userrepo.User_Id, DbType.String, ParameterDirection.Input);
                param.Add("P_PASSWORD", userrepo.Password, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                db.GetConnection().Execute("LOGIN_PROCEDURES.SEL_LOGIN_USER", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");
                if (paramoption <= 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<sec_UserRepo> GetUser(sec_UserRepo loginmodel)
        {
            try
            {
                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "LOGIN_PROCEDURES.SEL_USER_DATA";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)db.GetConnection();

                OracleParameter User_name = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, loginmodel.User_Id, ParameterDirection.Input);
                cmd.Parameters.Add(User_name);

                OracleParameter param2 = new OracleParameter("P_USER_DATA", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(param2);

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "user");
                var eList = ds.Tables[0].AsEnumerable().Select(row => new sec_UserRepo
                {
                    User_Id = row.Field<string>("User_Id"),
                    //Employee_Id = row.Field<string>("Employee_Id"),
                    //User_Group_Id = row.Field<string>("User_Group_Id"),
                    //User_Status = row.Field<string>("User_Status"),
                    //Password_Expiry_Date = row.Field<DateTime?>("Password_Expiry_Date"),
                    //Password_Failure_Count = row.Field<int>("Password_Failure_Count"),
                    //Token_Number = row.Field<string>("Token_Number"),
                }).ToList();

                return eList;

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


        public List<sec_UserRepo> GetUserData(sec_UserRepo loginmodel)
        {
            try
            {
                var param = new DynamicParameters();

                List<sec_UserRepo> Objuserdata = new List<sec_UserRepo>();
                string context = "SELECT DISTINCT * FROM VW_SEL_USER_DATA WHERE USER_ID = '" + loginmodel.User_Id + "'";
                return Objuserdata = db.GetConnection().Query<sec_UserRepo>(context).ToList();

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


        public void AddLoginFailureCount(sec_UserRepo userrepo)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("P_USER_ID", userrepo.User_Id, DbType.String, ParameterDirection.Input);

                db.GetConnection().Execute("LOGIN_PROCEDURES.ADD_USER_FAILURE_COUNT", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void CleanLoginFailureCount(sec_UserRepo userrepo)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("P_USER_ID", userrepo.User_Id, DbType.String, ParameterDirection.Input);

                db.GetConnection().Execute("LOGIN_PROCEDURES.REMOVE_USER_FAILURE_COUNT", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void AddUserLoginSession(sec_UserRepo userrepo)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("P_USER_ID", userrepo.User_Id, DbType.String, ParameterDirection.Input);

                db.GetConnection().Execute("LOGIN_PROCEDURES.ADD_USER_TO_LOGIN_SESSION", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveUserLoginSession(string user_id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("P_USER_ID", user_id, DbType.String, ParameterDirection.Input);

                db.GetConnection().Execute("LOGIN_PROCEDURES.DEL_USER_FROM_LOGIN_SESSION", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public bool IsLoggedIn(sec_UserRepo userrepo)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("P_USER_ID", userrepo.User_Id, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                db.GetConnection().Execute("LOGIN_PROCEDURES.SEL_USER_IS_LOGIN", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");
                if (paramoption <= 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public bool AuthenticateUserPass(string user_id, string password)
        {
            try
            {
                var param = new DynamicParameters();
                password = cSecurityRepo.AES_Encrypt(password);

                param.Add("P_USER_ID", user_id, DbType.String, ParameterDirection.Input);
                param.Add("P_PASSWORD", password, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                db.GetConnection().Execute("LOGIN_PROCEDURES.SEL_LOGIN_USER", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");
                if (paramoption <= 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







    }
}
