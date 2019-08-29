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
    public class Invest_ProductRepo
    {
        public string Product_Id { get; set; }
        [Required]
        public string Product_Name { get; set; }
       [Required]
        public string NPRA_Asset_Class_Id { get; set; }
        public string NPRA_Asset_Class_Name { get; set; }
        public decimal Equity_CIS_Unit_Price { get; set; }
        public string Listed { get; set; }
        public string Marker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public DateTime Update_Date { get; set; }
        public string Update_Id { get; set; }
        public string Record_Status { get; set; }
        public string Class_Id { get; set; }
        public string Bond_Class { get; set; }
        public string Description { get; set; }
        public string Fix_Floating { get; set; }
        public DateTime? E_Unit_Date { get; set; }

        IDbConnection con;
        public void SaveRecord(Invest_ProductRepo productRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_PRODUCT_ID", value: productRepo.Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_PRODUCT_NAME", value: productRepo.Product_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_NPRA_CLASS_ID", value: productRepo.NPRA_Asset_Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_CLASS_ID", value: productRepo.Class_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EQUITY_CIS_UNIT_PRICE", value: productRepo.Equity_CIS_Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                param.Add(name: "P_LISTED", value: productRepo.Listed, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_BOND_TYPE", value: productRepo.Bond_Class, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_DATE", value: GlobalValue.Scheme_Today_Date, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                param.Add(name: "P_FIX_FLOATING", value: productRepo.Fix_Floating, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("MIX_INVEST_PRODUCT", param, commandType: CommandType.StoredProcedure);
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
      
        public bool DeleteRecord(string Product_Id)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_PRODUCT_ID", value: Product_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                int result = con.Execute(sql: "DEL_INVEST_PRODUCT", param: param, commandType: CommandType.StoredProcedure);

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

        public void getclasscode(Invest_ProductRepo ProductRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(ProductRepo.Class_Id))
                {
                   
                }
                else
                {

                    //Get connection
                    var con = new AppSettings();
                    var param = new DynamicParameters();
                    param.Add("P_CLASS_ID", ProductRepo.Class_Id, DbType.String, ParameterDirection.Input);
                    param.Add("VDATA", "", DbType.String, ParameterDirection.Output);
                    con.GetConnection().Execute("SEL_INVEST_CLASS_ID", param, commandType: CommandType.StoredProcedure);
                    ProductRepo.Class_Id = param.Get<String>("VDATA");
                }
                }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataSet ProductData()
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

                cmd.CommandText = "SEL_INVEST_PRODUCTS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "product");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Invest_ProductRepo> GetProductList()
        {
            try
            {
                DataSet dt = ProductData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_ProductRepo
                {
                    Product_Id = row.Field<string>("PRODUCT_ID"),
                    Product_Name = row.Field<string>("PRODUCT_NAME"),
                    NPRA_Asset_Class_Id = row.Field<string>("NPRA_Asset_Class_Id"),
                    NPRA_Asset_Class_Name = row.Field<string>("NPRA_Asset_Class_Name"),
                    Equity_CIS_Unit_Price = row.Field<decimal>("EQUITY_CIS_UNIT_PRICE"),
                    Listed = row.Field<string>("LISTED"),
                    Record_Status = row.Field<string>("RECORD_STATUS"),
                    Class_Id = row.Field<string>("CLASS_ID"),
                    Description = row.Field<string>("DESCRIPTION"),
                    Bond_Class = row.Field<String>("BOND_CLASS"),
                    Fix_Floating = row.Field<String>("FIX_FLOATING")
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

        public List<Invest_ProductRepo> GetAssetList()
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Invest_ProductRepo> ObjFund = new List<Invest_ProductRepo>();

                return ObjFund = db.GetConnection().Query<Invest_ProductRepo>("Select * from INVEST_NPRA_ASSET_CLASS").ToList();
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

        public List<Invest_ProductRepo> GetAssetTypeList()
        {
            AppSettings db = new AppSettings();
            con = db.GetConnection();
            try
            {
                List<Invest_ProductRepo> ObjFund = new List<Invest_ProductRepo>();

                return ObjFund = db.GetConnection().Query<Invest_ProductRepo>("Select * from INVEST_ASSET_CLASS").ToList();
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