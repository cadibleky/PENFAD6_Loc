using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Setup.InvestSetup
{
   public  class Invest_AssetClassRepo
    {
        public string Class_Id { get; set; }
        public string description { get; set; }

        IDbConnection con;

       

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

                cmd.CommandText = "SEL_INVEST_ASSET_CLASS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "clas");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IEnumerable<Invest_AssetClassRepo> GetAssetClassList()
        {
            try
            {
                DataSet dt = ProductData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new Invest_AssetClassRepo
                {
                    Class_Id = row.Field<string>("CLASS_ID"),
                    description  = row.Field<string>("DESCRIPTION"),
                   



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
        public List<Invest_AssetClassRepo> GetAssetList()
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
