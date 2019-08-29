using Dapper;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace PENFAD6DAL.Repository.Setup.SystemSetup
{
    public class setup_SectorRepo
    {
        public string Sector_Id { get; set; }
        [Required(ErrorMessage = "Sector name is a required data item.")]
        public string Sector_Name { get; set; }
        public string Maker_Id { get; set; }
        public DateTime Make_Date { get; set; }
        public string Update_Id { get; set; }
        public DateTime? Update_Date { get; set; }


        AppSettings db = new AppSettings();
        public void SaveRecord(setup_SectorRepo setup_sectorrepo)
        {
            try
            {
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(setup_sectorrepo.Sector_Id))
                {
                    param.Add(name: "P_SECTOR_NAME", value: setup_sectorrepo.Sector_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_MAKE_DATE", value: DateTime.Today , dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.ADD_SETUP_SECTOR", param: param, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    param.Add(name: "P_SECTOR_ID", value: setup_sectorrepo.Sector_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_SECTOR_NAME", value: setup_sectorrepo.Sector_Name, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add(name: "P_UPDATE_DATE", value: DateTime.Today , dbType: DbType.Date, direction: ParameterDirection.Input);

                    db.GetConnection().Execute(sql: "SETUP_PROCEDURES.UPD_SETUP_SECTOR", param: param, commandType: CommandType.StoredProcedure);
                }

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

        public bool DeleteRecord(string sector_id)
        {
            try
            {
                var param = new DynamicParameters();
                //Input Param
                param.Add(name: "P_SECTOR_ID", value: sector_id, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = db.GetConnection().Execute(sql: "SETUP_PROCEDURES.DEL_SETUP_SECTOR", param: param, commandType: CommandType.StoredProcedure);
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
                db.Dispose();
            }

        }


        public List<setup_SectorRepo> GetSectorList()
        {
            try
            {
                List<setup_SectorRepo> ObjSector = new List<setup_SectorRepo>();

                return ObjSector = db.GetConnection().Query<setup_SectorRepo>("Select * from VW_SEL_SETUP_SECTOR").ToList();
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


        public bool isSectorUnique(string sector_name)
        {
            try
            {
                var param = new DynamicParameters();

                param.Add("VSECTOR_NAME", sector_name, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);

                db.GetConnection().Execute("SETUP_PROCEDURES.SEL_SETUP_SECTOR_EXIST", param, commandType: CommandType.StoredProcedure);

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
            finally
            {
                db.Dispose();
            }

        }



    }
}
