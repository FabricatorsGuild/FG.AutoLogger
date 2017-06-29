using System;
using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventSourceAutoGenerateLoggersBuilder : BaseCoreBuilder, IEventSourceBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            if (!(eventSource.Settings?.AutogenerateLoggerInterfaces ?? false)) return;

            var existingLoggers = eventSource.Loggers.Select(l => l.Name).ToArray();
            var loggers = new List<LoggerModel>(eventSource.Loggers);
            var maxStartId = eventSource.Loggers.Max(l => l.StartId ?? 0);
            var startId = (int)Math.Floor((maxStartId + 1000) / 1000.0) * 1000;
            foreach (var loggerTemplateModel in project.Loggers)
            {
                if (!existingLoggers.Any(l => l.Equals(loggerTemplateModel.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    loggers.Add(new LoggerModel()
                    {
                        Name = loggerTemplateModel.Name,
                        StartId = startId,
                    });
                }
                startId += 1000;
            }
            eventSource.Loggers = loggers.ToArray();
        }
    }
}