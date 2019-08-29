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
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Setup.PfmSetup
{
        public class pfm_FeesRepo
        {
            public string Fee_Id { get; set; }
            [Required]
            public string Fee_Description { get; set; }
            [Required]
            public decimal Fee { get; set; }
            public string Maker_Id { get; set; }
            public string Update_Id { get; set; }
            [Required]
            public string Before_Nav { get; set; }

            IDbConnection con;
            public void SaveRecord(pfm_FeesRepo feeRepo)
            {
                try
                {
                    //Get Connection
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    DynamicParameters param = new DynamicParameters();
                    param.Add(name: "P_FEE_ID", value: feeRepo.Fee_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FEE_DESCRIPTION", value: feeRepo.Fee_Description, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_FEE", value: feeRepo.Fee, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_BEFORE_NAV", value: feeRepo.Before_Nav, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_PFM_FEES", param, commandType: CommandType.StoredProcedure);
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


            public bool DeleteRecord(string FEE_ID)
            {
                try
                {
                    //Get Connection
                    AppSettings app = new AppSettings();
                    con = app.GetConnection();
                    DynamicParameters param = new DynamicParameters();
                    //Input Param
                    param.Add(name: "P_FEE_ID", value: FEE_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    int result = con.Execute(sql: "DEL_PFM_FEES", param: param, commandType: CommandType.StoredProcedure);

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


            public DataSet GetTIDataset()
            {
                try
                {
                    //Get connection
                    var app = new AppSettings();
                    con = app.GetConnection();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = (SqlConnection)con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_PFM_FEES";

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "PFM_FEES");
                            return ds;
                        }
                    }

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
            public bool isFeesUnique(string MFEE_DESCRIPTION)
            {
                try
                {
                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_FEE_DESCRIPTION", MFEE_DESCRIPTION, DbType.String, ParameterDirection.Input);
                    param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_PFM_FEES_EXIST", param, commandType: CommandType.StoredProcedure);
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

       

        public DataSet FeesData()
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

                    cmd.CommandText = "SEL_PFM_FEES";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = (OracleConnection)con;

                    param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                    param.Direction = ParameterDirection.Output;

                    da = new OracleDataAdapter(cmd);
                    da.Fill(ds, "fees");
                    return ds;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            public IEnumerable<pfm_FeesRepo> GetFeesList()
            {
                try
                {
                    DataSet dt = FeesData();
                    var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_FeesRepo
                    {
                        Fee_Id = row.Field<string>("FEE_ID"),
                        Fee_Description = row.Field<string>("FEE_DESCRIPTION"),
                        Fee = row.Field<decimal>("FEE"),
                        Before_Nav = row.Field<string>("Before_Nav")
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

        /// FOR FUND MANAGER FEE
        /// 
        public DataSet FeesDataFM()
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

                cmd.CommandText = "SEL_PFM_FEES_FM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "feesFM");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<pfm_FeesRepo> GetFeesListFM()
        {
            try
            {
                DataSet dt = FeesDataFM();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_FeesRepo
                {
                    Fee_Id = row.Field<string>("FEE_ID"),
                    Fee_Description = row.Field<string>("FEE_DESCRIPTION"),
                    Fee = row.Field<decimal>("FEE"),
                    Before_Nav = row.Field<string>("Before_Nav")
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

		public DataSet FeesData_r()
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

				cmd.CommandText = "SEL_PFM_FEES_R";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Connection = (OracleConnection)con;

				param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
				param.Direction = ParameterDirection.Output;

				da = new OracleDataAdapter(cmd);
				da.Fill(ds, "fees");
				return ds;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public IEnumerable<pfm_FeesRepo> GetFeesList_r()
		{
			try
			{
				DataSet dt = FeesData_r();
				var eList = dt.Tables[0].AsEnumerable().Select(row => new pfm_FeesRepo
				{
					Fee_Id = row.Field<string>("FEE_ID"),
					Fee_Description = row.Field<string>("FEE_DESCRIPTION"),
					Fee = row.Field<decimal>("FEE"),
					Before_Nav = row.Field<string>("Before_Nav")
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


	}
    }



