using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerTemplateBuilder : ILoggerBuilder
    {
        public void Build(Project project, EventSourceModel eventSource, LoggerModel model)
        {
            var loggerTemplate = project.Loggers.GetMatchingLoggerTemplate(model);
            if (loggerTemplate == null)
            {
                throw new NotSupportedException($"Logger {model.Name} was not found in the project. Declare an interface with a matching name in a file with the same name as the interface.");
            }
            model.AddTemplate(loggerTemplate);
        }
    }
}