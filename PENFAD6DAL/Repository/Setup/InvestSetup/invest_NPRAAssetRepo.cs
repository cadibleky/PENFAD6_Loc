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

namespace PENFAD6DAL.Repository.Setup.InvestSetup
{
    public class invest_NPRAAssetRepo
    {
        public string NPRA_Asset_Class_Id { get; set; }
        [Required]
        public string NPRA_Asset_Class_Name { get; set; }
        //[Required]
        //public string Class_Id { get; set; }
        public decimal Asset_Allocation_Limit { get; set; }
        public decimal Issuer_Limit { get; set; }
        public string Marker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public DateTime Update_Date { get; set; }
        public string Update_Id { get; set; }
        public string Record_Status { get; set; }
        public string Description { get; set; }

        IDbConnection con;
        public void SaveRecord(invest_NPRAAssetRepo NPRAAssetRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ASSET_CLASS_ID", value: NPRAAssetRepo.NPRA_Asset_Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ASSET_CLASS_NAME", value: NPRAAssetRepo.NPRA_Asset_Class_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                //param.Add(name: "P_CLASS_ID", value: NPRAAssetRepo.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ASSET_ALLOCATION_LIMIT", value: NPRAAssetRepo.Asset_Allocation_Limit, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_ISSUER_LIMIT", value: NPRAAssetRepo.Issuer_Limit, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);

                con.Execute("MIX_INVEST_NPRAASSET", param, commandType: CommandType.StoredProcedure);
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

        public bool DeleteRecord(string NPRA_Asset_Class_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_ASSET_CLASS_ID", value: NPRA_Asset_Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_INVEST_NPRAASSET", param: param, commandType: CommandType.StoredProcedure);

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
      
   

        public DataSet NPRAAssetData()
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

                cmd.CommandText = "SEL_INVEST_NPRAASSET";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "npraasset");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<invest_NPRAAssetRepo> GetNPRAAssetList()
        {
            try
            { 
                DataSet dt = NPRAAssetData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new invest_NPRAAssetRepo
                {
                    NPRA_Asset_Class_Id = row.Field<string>("NPRA_ASSET_CLASS_ID"),
                    NPRA_Asset_Class_Name = row.Field<string>("NPRA_ASSET_CLASS_NAME"),
          //          Class_Id = row.Field<string>("CLASS_ID"),
          //          Description = row.Field<string>("DESCRIPTION"),
                 Asset_Allocation_Limit = row.Field<decimal>("ASSET_ALLOCATION_LIMIT"),
                    Issuer_Limit = row.Field<decimal>("ISSUER_LIMIT"),
                    Record_Status = row.Field<string>("RECORD_STATUS"),
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

        public List<invest_NPRAAssetRepo> GetAssetList()
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<invest_NPRAAssetRepo> ObjFund = new List<invest_NPRAAssetRepo>();

                return ObjFund = db.GetConnection().Query<invest_NPRAAssetRepo>("Select * from INVEST_ASSET_CLASS").ToList();
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
        public List<Invest_AssetClassRepo> GetProductFixeIncAndT_BillList()
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Invest_AssetClassRepo> ObjFund = new List<Invest_AssetClassRepo>();

                return ObjFund = db.GetConnection().Query<Invest_AssetClassRepo>("Select * from INVEST_ASSET_CLASS where CLASS_ID = '01' OR CLASS_ID = '02' ").ToList();
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