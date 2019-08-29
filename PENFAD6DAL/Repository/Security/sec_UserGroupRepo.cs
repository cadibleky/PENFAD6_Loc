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
using System.Collections;

namespace PENFAD6DAL.Repository.Security
{
  public  class sec_UserGroupRepo
    {
        [Key]
        public string UserGroupId { get; set; }
        
        [Required(ErrorMessage = " Group Name is a required data item.")]
        [Display(Name = "User Group")]
        [StringLength(maximumLength: 1000)]
        public string UserGroupName { get; set; }

        [Display(Name = "User Group Description")]
        [StringLength(maximumLength: 999999999)]
        public string UserGroupDesc { get; set; }

        //[Required(ErrorMessage = " Status is a required data item.")]
        [StringLength(maximumLength: 100)]
        public string UserGroupStatus { get; set; }

        //[Required(ErrorMessage = " Added By User is a required data item.")]
        [StringLength(maximumLength: 100)]
        public string MakerId { get; set; }
        //public sec_User sec_User { get; set; }

        [Required(ErrorMessage = " Date Added is a required data item.")]
        [DataType(DataType.DateTime)]
        public DateTime MakeDate { get; set; }

        [Required(ErrorMessage = " User Role is a required data item.")]
        public string UserRoleId { get; set; }
        public string UserRoleName { get; set; }

        //-------------------------------------------------------------------------------
        IDbConnection con;
        //public sec_UserRoleRepo user_role { get; set; }

        //methods begin


        public bool Add_Update_UserGroup(sec_UserGroupRepo user_group)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                param.Add(name: "p_UserGroupId", value: user_group.UserGroupId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserRoleId", value: user_group.UserRoleId, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserGroupName", value: user_group.UserGroupName, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserGroupDesc", value: user_group.UserGroupDesc, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_UserGroupStatus", value: user_group.UserGroupStatus, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakerId", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_MakeDate", value: GlobalValue.Scheme_Today_Date, dbType: DbType.Date, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "mix_sec_UserRoleGroup", param: param, commandType: CommandType.StoredProcedure);

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

        public List<sec_UserGroupRepo> GetUserGroupList()
        {
            var role_list = new List<sec_UserGroupRepo>();
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
                        cmd.CommandText = "sel_sec_UserGroupAll";
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            role_list = dt.AsEnumerable().Select(row => new sec_UserGroupRepo
                            {
                                UserGroupName = row.Field<string>("USER_GROUP_NAME"),
                                UserGroupStatus = row.Field<string>("USER_GROUP_STATUS"),
                                UserGroupId = row.Field<string>("USER_GROUP_ID"),
                                UserGroupDesc = row.Field<string>("USER_GROUP_DESC"),
                                MakerId = row.Field<string>("MAKER_ID"),
                                MakeDate = row.Field<DateTime>("MAKE_DATE"),
                                UserRoleId = row.Field<string>("USER_ROLE_ID"),
                                UserRoleName = row.Field<string>("USER_ROLE_NAME")

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

        public IEnumerable GetUserGroupForRoleIdList(string userroleid)
        {
            //var role_list = new List<sec_UserGroupRepo>();
            List<object> data = new List<object>();
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
                        cmd.CommandText = "sel_sec_UserGroupForRoleId";
                        cmd.Parameters.Add("", OracleDbType.Varchar2, ParameterDirection.Input).Value = userroleid;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach(DataRow roww in dt.Rows)
                            {
                                string id = roww["USER_GROUP_ID"].ToString(); // cityNode.SelectSingleNode("id").InnerText;
                                string name = roww["USER_GROUP_NAME"].ToString();  //cityNode.SelectSingleNode("name").InnerText;
                                data.Add(new { Id = id, Name = name });
                            }

                        }
                    }

                    return data;
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

        public IEnumerable GetUserGroupForRoleIdList_Dataset(string userroleid)
        {
            var role_list = new List<sec_UserGroupRepo>();
            //List<object> data = new List<object>();
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
                        cmd.CommandText = "sel_sec_UserGroupForRoleId";
                        cmd.Parameters.Add("", OracleDbType.Varchar2, ParameterDirection.Input).Value = userroleid;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            //foreach (DataRow roww in dt.Rows)
                            //{
                            //    string id = roww["USER_GROUP_ID"].ToString(); // cityNode.SelectSingleNode("id").InnerText;
                            //    string name = roww["USER_GROUP_NAME"].ToString();  //cityNode.SelectSingleNode("name").InnerText;
                            //    data.Add(new { Id = id, Name = name });
                            //}
 
                                role_list = dt.AsEnumerable().Select(row => new sec_UserGroupRepo
                                {
                                    UserGroupName = row.Field<string>("USER_GROUP_NAME"),
                                    UserGroupId = row.Field<string>("USER_GROUP_ID"),
                                    UserRoleId = row.Field<string>("USER_ROLE_ID"),
                                    UserRoleName = row.Field<string>("USER_ROLE_NAME")

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

        public bool DeleteUserGroup(string groupId)
        {
            try
            {
                //Get connectoin
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_groupid", value: groupId, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "del_sec_UserGroup", param: param, commandType: CommandType.StoredProcedure);
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

















    }//end class

}
