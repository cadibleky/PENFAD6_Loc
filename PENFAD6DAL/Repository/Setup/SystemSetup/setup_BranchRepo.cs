using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;


namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public  class setup_BranchRepo
    {
        public string Branch_Id { get; set; }

        [Required(ErrorMessage = "Branch is a required data item.")]
        public string Branch_Name { get; set; }
        public string Branch_Code { get; set; }
        public string Postal_Address { get; set; }
        public string Region_Id { get; set; }
        public string Region_Name { get; set; }
        public string Telephone_No { get; set; }
        public string Mobile_No { get; set; }

         [DataType(DataType.EmailAddress, ErrorMessage = "Please, enter a valid email address.")]
        public string Email_Address { get; set; }
        public string Maker_Id { get; set; }
        public string Update_Id { get; set; }
  
       
      
        IDbConnection con;
       

        public double SaveRecord(string p_BRANCH_ID, string p_BRANCH_NAME,string p_BRANCH_CODE,string p_POSTAL_ADDRESS,string p_REGION_ID,string p_TELEPHONE_NO,string p_MOBILE_NO,string  p_EMAIL_ADDRESS, string p_MAKER_ID,string  p_UPDATE_ID)
        {
            var app = new AppSettings();
            //using (OracleConnection conp = new OracleConnection(app.GetConnection()))
            //    var app = new AppSettings();
            con = app.GetConnection();

            {

                OracleCommand oraCmd = new OracleCommand("MIX_SETUP_BRANCH");
                oraCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oraCmd.Parameters.Add(new OracleParameter("p_BRANCH_ID", OracleDbType.Varchar2)).Value = p_BRANCH_ID;
                oraCmd.Parameters.Add(new OracleParameter("p_BRANCH_NAME", OracleDbType.Varchar2)).Value = p_BRANCH_NAME;
                oraCmd.Parameters.Add(new OracleParameter("p_BRANCH_CODE", OracleDbType.Varchar2)).Value = p_BRANCH_CODE;
                oraCmd.Parameters.Add(new OracleParameter("p_POSTALA_DDRESS", OracleDbType.Varchar2)).Value = p_POSTAL_ADDRESS;
                oraCmd.Parameters.Add(new OracleParameter("p_REGION_ID", OracleDbType.Varchar2)).Value = p_REGION_ID;
                oraCmd.Parameters.Add(new OracleParameter("p_TELEPHONE_NO", OracleDbType.Varchar2)).Value = p_TELEPHONE_NO;
                oraCmd.Parameters.Add(new OracleParameter("p_MOBILE_NO", OracleDbType.Varchar2)).Value = p_MOBILE_NO;
                oraCmd.Parameters.Add(new OracleParameter("p_EMAIL_ADDRESS", OracleDbType.Varchar2)).Value = p_EMAIL_ADDRESS;
                oraCmd.Parameters.Add(new OracleParameter("p_MAKER_ID", OracleDbType.Varchar2)).Value = p_MAKER_ID;
                oraCmd.Parameters.Add(new OracleParameter("p_UPDATE_ID", OracleDbType.Varchar2)).Value = p_UPDATE_ID;
               
                OracleConnection oConn = new OracleConnection(app.conString());
                try
                {
                    oConn.Open();
                    oraCmd.Connection = oConn;
                    oraCmd.ExecuteNonQuery();
                    return 9;

                }
                finally
                {
                    oraCmd.Dispose();
                    oConn.Close();
                }
            }
        }
       
        public bool DeleteRecord(string branch_Id)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_BRANCH_ID", value: branch_Id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_SET_BRANCH", param: param, commandType: CommandType.StoredProcedure);
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
     


        //Richard
        public DataSet BranchData()
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

                cmd.CommandText = "SEL_SETUP_BRANCH";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "Branch");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<setup_BranchRepo> GetBranchList()
        {
            try
            {
                DataSet dt = BranchData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new setup_BranchRepo
                {
                    Branch_Id = row.Field<string>("Branch_Id"),
                    Branch_Name = row.Field<string>("Branch_Name"),
                    Branch_Code = row.Field<string>("Branch_Code"),
                    Postal_Address = row.Field<string>("Postal_Address"),
                    Region_Id = row.Field<string>("Region_Id"),
                    Telephone_No = row.Field<string>("Telephone_No"),
                    Mobile_No = row.Field<string>("Mobile_No"),
                    

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
        public OracleDataReader Sel_Getsetup_Branch()
        {
            //Get connection
            //setup_PositionRepo data = new setup_PositionRepo();
            AppSettings app = new AppSettings();
            con = app.GetConnection();
            OracleDataReader functionReturnValue = default(OracleDataReader);

            OracleCommand oraCmd = new OracleCommand("SEL_SETUP_BRANCH");
            oraCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oraCmd.Parameters.Add(new OracleParameter("cur", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

            //OracleConnection con = new OracleConnection();
            OracleConnection oConn = new OracleConnection(app.conString());
            try
            {
                //con.Open();
                //oraCmd.Connection = (OracleConnection)con;
                oConn.Open();
                oraCmd.Connection = oConn;
                functionReturnValue = oraCmd.ExecuteReader();


            }
            finally
            {
            }
            return functionReturnValue;
        }
        public bool branchExist(string branch_nname, out string error)
        {
            try
            {
                error = "";
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();
                var param = new DynamicParameters();
                Int32 tott = 0;
                param.Add(name: "BRANCH_NAME", value: branch_nname, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "p_count", value: tott, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute(sql: "SEL_SETUP_BRANCHNAMEEXIT ", param: param, commandType: CommandType.StoredProcedure);

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
