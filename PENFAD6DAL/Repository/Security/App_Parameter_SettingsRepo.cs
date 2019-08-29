using Dapper;
using PENFAD6DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Security
{
    public class App_Parameter_SettingsRepo
    {
        public string App_Code { get; set; }
        public string App_Desc { get; set; }
        public string App_Value { get; set; }

        AppSettings db = new AppSettings();

        public List<App_Parameter_SettingsRepo> GetBankParameterSettings(string code)
        {
            try
            {
                var param = new DynamicParameters();
                List<App_Parameter_SettingsRepo> Objbanksettings = new List<App_Parameter_SettingsRepo>();
                string context = "SELECT DISTINCT * FROM SEL_APP_PARAMETER_SETTINGS WHERE APP_CODE = '" + code + "'";
                return Objbanksettings = db.GetConnection().Query<App_Parameter_SettingsRepo>(context).ToList();
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
