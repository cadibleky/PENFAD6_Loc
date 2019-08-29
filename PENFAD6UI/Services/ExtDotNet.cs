using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using PENFAD6DAL.Services;

namespace PENFAD6UI.Services
{
    public class ExtDotNet
    {
        public static void ClearControls(string formName)
        {
            try
            {
                var x = X.GetCmp<FormPanel>(formName);
                x.Reset();
            }
            catch (Exception ex)
            {
                TeksolLogger.MyLogger().Error($"Exception Class: ExtDotNet; Method: ClearControl; Exception Details :{ex}");
                throw;
            }

        }

        public static void ReloadStore(string storeName)
        {
            try
            {
                var store = X.GetCmp<Store>(storeName);
                store.Reload();
            }
            catch (Exception ex)
            {
                //TeksolLogger.GetLogger().Error($"Exception Class: ExtDotNet; Method: Reload()Store; Exception Details :{ex}");
                throw;
            }
        }
    }
}