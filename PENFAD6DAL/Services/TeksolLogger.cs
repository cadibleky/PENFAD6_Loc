using Serilog;

namespace PENFAD6DAL.Services
{
    public static class TeksolLogger
    {
        public static ILogger MyLogger() => new LoggerConfiguration()
            .WriteTo.Seq($"{MessageFactory.ClientUrl.Value}:{MessageFactory.ClientPort.Value}")
            .WriteTo.Seq($"{MessageFactory.TeksolUrl.Value}:{MessageFactory.TeksolPort.Value}")
            .CreateLogger();
    }
}