using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class LoggerTemplateModel
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Include { get; set; }
        public EventModel[] Events { get; set; }

        public override string ToString()
        {
            return $"{nameof(LoggerTemplateModel)} {this.Name}";
        }
    }

    public static class LoggerTemplateExtensions
    {
        public static LoggerTemplateModel GetMatchingLoggerTemplate(this IEnumerable<LoggerTemplateModel> loggers, LoggerModel logger)
        {
            return loggers.FirstOrDefault(l => l.Name.Equals(logger.Name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}