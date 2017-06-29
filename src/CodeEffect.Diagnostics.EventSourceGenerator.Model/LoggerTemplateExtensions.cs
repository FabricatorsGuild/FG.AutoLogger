using System;
using System.Collections.Generic;
using System.Linq;

namespace FG.Diagnostics.AutoLogger.Model
{
    public static class LoggerTemplateExtensions
    {
        public static LoggerTemplateModel GetMatchingLoggerTemplate(this IEnumerable<LoggerTemplateModel> loggers, LoggerModel logger)
        {
            return loggers.FirstOrDefault(l => l.Name.Equals(logger.Name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}