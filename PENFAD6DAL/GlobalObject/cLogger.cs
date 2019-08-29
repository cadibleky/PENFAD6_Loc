using Serilog;
using System;
using System.Configuration;

namespace PENFAD6DAL.GlobalObject
{
    public class cLogger
    {
        public void WriteLog(string message)
        {
            try
            {
                string loggerUrl = ConfigurationManager.AppSettings["LoggerUrl"];
                var log = new LoggerConfiguration().WriteTo.Seq(loggerUrl).CreateLogger();

                log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: message + " " + DateTime.Now);

            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }
           
        }
    }
}
