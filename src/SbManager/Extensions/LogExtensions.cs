using System;
using System.Configuration;
using System.IO;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.RollingFile;

namespace SbManager.Extensions
{
    public static class LogExtensions
    {
        public static LoggerConfiguration FileSinkDefinedFromConfig(this LoggerSinkConfiguration loggerConfiguration)
        {
            var logDir = ConfigurationManager.AppSettings["LoggingDirectory"];
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

            var filePath = Path.Combine(logDir, Constants.AppName + "-{Date}.log.json");
            var maxSize = int.Parse(ConfigurationManager.AppSettings["LoggingFileSizeLimitMBytes"])*1048576;
            var retainLimit = int.Parse(ConfigurationManager.AppSettings["LoggingRetainedFileCountLimit"]);
            var minLevel = ConfigurationManager.AppSettings["LoggingMinLevel"];
            var sink = new RollingFileSink(filePath, new CustomLogJsonFormatter(), maxSize, retainLimit);
            return loggerConfiguration.Sink(sink, (LogEventLevel)Enum.Parse(typeof(LogEventLevel), minLevel));
        }
    }
}
