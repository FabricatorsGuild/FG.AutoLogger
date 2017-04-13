using System.Runtime.Remoting.Messaging;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceLoggersBuilder : BaseWithLogging, IEventSourceBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            // TODO: Get all builders from project and allow all to build
            var loggerBuilders = new ILoggerBuilder[]
            {
                new LoggerTemplateBuilder(),
                new LoggerKeywordsBuilder(),
                new LoggerImplicitArgumentsBuilder(),
                new LoggerOverrideArgumentsBuilder(), 
                new LoggerEventsBuilder(),
                new LoggerImplementationBuilder(), 
                new LoggerEventSourcePartialBuilder(), 
            };
            var loggerStartId = 10000;
            foreach (var logger in eventSource.Loggers)
            {
                logger.StartId = logger.StartId ?? loggerStartId;

                foreach (var builder in loggerBuilders)
                {
                    builder.Build(project, model, logger);
                }

                loggerStartId += 1000;
            }
        }
    }
}