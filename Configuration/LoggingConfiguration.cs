using Microsoft.Extensions.Logging;

namespace SchoolManagementApp.MVC.Configuration
{
    public static class LoggingConfiguration
    {
        public static ILoggingBuilder ConfigureLogging(this ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.AddEventSourceLogger();
            
            return logging;
        }
    }
}