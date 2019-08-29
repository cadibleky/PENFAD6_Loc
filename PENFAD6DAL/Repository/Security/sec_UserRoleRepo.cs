
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;


namespace PENFAD6DAL.Repository.Security
{
  public  class sec_UserRoleRepo
    {
        //private sec_UserRole roles = new sec_UserRole();


        [Key]
        public string UserRoleId { get; set; }

        [Required(ErrorMessage = " Role Name is a required data item.")]
        [Display(Name = "User Role")]
        [StringLength(maximumLength: 1000)]
        public string UserRoleName { get; set; }

        [Display(Name = "User Role Description")]
        [StringLength(maximumLength: 999999999)]
        public string UserRoleDesc { get; set; }

        //[Required(ErrorMessage = " Status is a required data item.")]
        [Display(Name = "Status")]
        [StringLength(maximumLength: 100)]
        public string UserRoleStatus { get; set; }

        //[Required(ErrorMessage = " Added By User is a required data item.")]
        [StringLength(maximumLength: 100)]
        [ForeignKey("sec_User")]
        [Display(Name = "Created By")]
        public string MakerId { get; set; }
        //public sec_User sec_User { get; set; }

        [Required(ErrorMessage = " Date Added is a required data item.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Created")]
        public DateTime MakeDate { get; set; }

        //[Display(Name = "Last Updated By")]
        //public string LastUpdatedById { get; set; }
        //[DataType(DataType.DateTime)]
        //[Display(Name = "Date Last Updated")]
        //public DateTime? DateLastUpdated { get; set; }


        IDbConnection con;


        //methods

        public bool Add_Update_UserRole(sec_UserRoleRepo roles)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "p_UserRoleId", value: roles.UserRoleId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserRoleName", value: roles.UserRoleName.ToUpper(), dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserRoleDesc", value: roles.UserRoleDesc, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserRoleStatus", value: roles.UserRoleStatus, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakerId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "mix_sec_UserRole", param: param, commandType: CommandType.StoredProcedure);

                if (result != 0)
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

        public bool Add_Update_UserRole_NotInUse (sec_UserRoleRepo roles)
        {
            //int i = 0;
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    conp.Open();
                    using (OracleCommand cmdadd = new OracleCommand())
                    {
                        cmdadd.Connection = conp;
                        cmdadd.CommandType = CommandType.StoredProcedure;
                        cmdadd.CommandText = "mix_sec_UserRole";
                        cmdadd.Parameters.Add(":p_UserRoleId", OracleDbType.Varchar2, ParameterDirection.Input).Value = roles.UserRoleId;
                        cmdadd.Parameters.Add(":p_UserRoleName", OracleDbType.Varchar2, ParameterDirection.Input).Value = roles.UserRoleName;
                        cmdadd.Parameters.Add(":p_UserRoleDesc", OracleDbType.Varchar2, ParameterDirection.Input).Value = roles.UserRoleDesc; ;
                        cmdadd.Parameters.Add(":p_UserRoleStatus", OracleDbType.Varchar2, ParameterDirection.Input).Value = roles.UserRoleStatus;
                        cmdadd.Parameters.Add(":p_MakerId", OracleDbType.Varchar2, ParameterDirection.Input).Value = GlobalValue.User_ID;
                        cmdadd.Parameters.Add(":p_MakeDate", OracleDbType.Date, ParameterDirection.Input).Value = GlobalValue.Scheme_Today_Date;

                        cmdadd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception)
                {

                    throw;
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

    

        public List<sec_UserRoleRepo> GetUserRoleList()
        {
            var role_list = new List<sec_UserRoleRepo>();
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
                        cmd.CommandText = "sel_sec_UserRoleAll";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        //OracleDataReader odr = cmd.ExecuteReader();
                        //conp.Close();
                        //DataTable dt = new DataTable();
                        //dt.Load(odr);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            role_list = dt.AsEnumerable().Select(row => new sec_UserRoleRepo
                            {
                                UserRoleName = row.Field<string>("USER_ROLE_NAME"),
                                UserRoleStatus = row.Field<string>("USER_ROLE_STATUS"),
                                UserRoleId = row.Field<string>("USER_ROLE_ID"),
                                UserRoleDesc = row.Field<string>("USER_ROLE_DESC"),
                                MakerId = row.Field<string>("MAKER_ID"),
                                MakeDate = row.Field<DateTime>("MAKE_DATE")
                            }).ToList();

                        }
                    }

                    return role_list;
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

       

        public decimal Validate_UseId_GroupName_RoleName(string p_name_or_id, string p_user_group_role, string p_add_update, string p_obj_name, string p_obj_id)
        {
            decimal result = 0;
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
                        cmd.CommandText = "sel_sec_UserObjExistValidation";
                        cmd.Parameters.Add("p_name_or_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_name_or_id.ToLower();
                        cmd.Parameters.Add("p_user_group_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_user_group_role.ToLower();
                        cmd.Parameters.Add("p_add_update", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_add_update.ToLower();
                        cmd.Parameters.Add("p_obj_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_obj_name;
                        cmd.Parameters.Add("p_obj_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_obj_id;

                        OracleParameter p_result = new OracleParameter("p_result", OracleDbType.Decimal,10000);
                        p_result.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(p_result);
                        cmd.ExecuteNonQuery();
                        result = Convert.ToDecimal(p_result.Value.ToString());
                    }
                    return result;
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

        public int Delete_Role_NotInUse(string roleid)
        {
            int i = 0;
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    conp.Open();
                    using (OracleCommand cmdadd = new OracleCommand())
                    {
                        cmdadd.Connection = conp;
                        cmdadd.CommandType = CommandType.StoredProcedure;
                        cmdadd.CommandText = "del_sec_UserRole";
                        cmdadd.Parameters.Add(":p_roleid", OracleDbType.Varchar2, ParameterDirection.Input).Value = roleid;

                        i = cmdadd.ExecuteNonQuery();
                    }

                    return i;
                }
                catch (Exception)
                {

                    throw;
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


        public bool DeleteRecord(string roleid)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_roleid", value: roleid, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "del_sec_UserRole", param: param, commandType: CommandType.StoredProcedure);
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












        //class ends here

    }
}
