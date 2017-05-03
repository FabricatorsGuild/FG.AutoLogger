using System.Linq;
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

            var loggerBuilders = new ILoggerBuilder[]
            {
                new LoggerTemplateBuilder(),
                new LoggerKeywordsBuilder(),
                new EventSourceCorrelatingEventsBuilder(), 
                new LoggerImplicitArgumentsBuilder(),
                new LoggerOverrideArgumentsBuilder(), 
                new EventSourceEventsBuilder(),
                new LoggerImplementationBuilder(), 
                new LoggerEventSourcePartialBuilder(), 
            }.Union(project.GetExtensions<ILoggerBuilder>()).ToArray();
            var loggerStartId = 10000;
            foreach (var logger in eventSource.Loggers)
            {
                logger.StartId = logger.StartId ?? loggerStartId;
                
                logger.EventSource = eventSource;

                foreach (var builder in loggerBuilders)
                {
                    builder.Build(project, model, logger);
                }

                loggerStartId += 1000;
            }
        }
    }
}