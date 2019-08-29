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

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public class setup_EmployeeTypeRepo
    {
        public string Employee_Type_Id { get; set; }

        [Required(ErrorMessage = " Employee Type is a required data item.")]
        public string Employee_Type { get; set; }

        IDbConnection con;

        public bool SaveRecord(setup_EmployeeTypeRepo employeeType)
        {
            try
            {
                //Get connectoin
                AppSettings app = new AppSettings();
                con = app.GetConnection();

                DynamicParameters param = new DynamicParameters();

               

                param.Add("p_EmployeeTypeId", employeeType.Employee_Type_Id, DbType.String, ParameterDirection.Input);
                param.Add("p_EmployeeType", employeeType.Employee_Type.ToUpper(), DbType.String, ParameterDirection.Input);
                param.Add("p_MakeId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add("p_UpdateId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);


                int result = con.Execute("MIX_SETUP_EMPLOYEETYPE", param, commandType: CommandType.StoredProcedure);

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
        public bool DeleteRecord(string empTypeId)
        {
            try
            {
                //Get connectoin
                AppSettings app = new AppSettings();
                con = app.GetConnection();

                DynamicParameters param = new DynamicParameters();

                //Input Param
                param.Add("p_EmployeeTypeId", empTypeId, DbType.String, ParameterDirection.Input);

                int result = con.Execute("DEL_SETUP_EMPLOYEETYPE", param, commandType: CommandType.StoredProcedure);
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
        public IEnumerable<setup_EmployeeTypeRepo> GetEmployeeTypeList()
        {
            try
            {
                //Get connectoin
                AppSettings app = new AppSettings();
                con = app.GetConnection();

                var result = con.Query<setup_EmployeeTypeRepo>("select * from vw_Setup_Employee_Type");
                return result;
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
        public bool EmployeeTypeExist(string empType)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                OracleCommand cmd = new OracleCommand();

                cmd.CommandText = "SEL_SETUP_EMPLOYEETYPE_EXIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection) con;

                //Input param
                OracleParameter depName = new OracleParameter("p_EmployeeType", OracleDbType.Varchar2, empType, ParameterDirection.Input);
                cmd.Parameters.Add(depName);

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


    }
}
