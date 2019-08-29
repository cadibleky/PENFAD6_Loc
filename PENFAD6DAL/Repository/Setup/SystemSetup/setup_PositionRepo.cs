using PENFAD6DAL.Repository.Setup.SystemSetup;
using Dapper;
using Oracle.ManagedDataAccess.Client;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.DbContext;

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public class setup_PositionRepo
    {
        //  Richard.............................

        public string Position_Id { get; set; }

        [Required(ErrorMessage = "Position Name is a required data item.")]
        public string Position_Name { get; set; }

        public string Maker_Id { get; set; }
       
         public string Update_Id { get; set; }
       


        IDbConnection con;

       


     
        public void SaveRecord(setup_PositionRepo Positions)
        {
            try
            {
                //Get Connection
                //AppSettings app = new AppSettings();
                //con = app.GetConnection();

                DynamicParameters param = new DynamicParameters();

              

                param.Add("p_Position_Id", Positions.Position_Id, DbType.String, ParameterDirection.Input);
                param.Add("p_Position_Name", Positions.Position_Name, DbType.String, ParameterDirection.Input);
                param.Add(name: "p_Maker_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_Update_Id", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_SETUP_POSITION", param, commandType: CommandType.StoredProcedure);
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
     
        public bool DeleteRecord(string positionId)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_Position_Id", value: positionId, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "del_setup_Position", param: param, commandType: CommandType.StoredProcedure);
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
        public DataSet PositionData()
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

                cmd.CommandText = "SEL_SETUP_POSITION";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "position");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<setup_PositionRepo> GetPositionList()
        {
            try
            {
                DataSet dt = PositionData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_PositionRepo
                {
                    Position_Id= row.Field<string>("Position_Id"),
                    Position_Name = row.Field<string>("Position_Name"),
               

                }).ToList();

                return eList;

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
      
    
        public bool PositionExist(string positionnname, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "p_Position_Name", value: positionnname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_SETUP_POSITIONNAMEEXIT ", param: param, commandType: CommandType.StoredProcedure);

                tott = param.Get<Int32>("p_count");
                if (tott > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // throw ex;
                error = ex.ToString();
                return false;
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

