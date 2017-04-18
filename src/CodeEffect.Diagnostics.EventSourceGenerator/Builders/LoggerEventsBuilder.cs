using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerEventsBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if( model ==  null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var eventBuilders = new ILoggerEventBuilder[]
            {
                new LoggerEventIdBuilder(),
                new LoggerEventImplicitArgumentsBuilder(),                 
                new LoggerEventOverrideArgumentsBuilder(), 
                new LoggerEventMessageFormatterBuilder(),
                new LoggerEventTemplatedArgumentsBuilder(), 
                new LoggerEventArgumentsComplexityCheckBuilder(),
            }.Union(project.GetExtensions<ILoggerEventBuilder>());

            foreach (var loggerEvent in model.Events)
            {
                foreach (var builder in eventBuilders)
                {
                    builder.Build(project, eventSourceProjectItem, model, loggerEvent);
                }
            }
        }
    }
}