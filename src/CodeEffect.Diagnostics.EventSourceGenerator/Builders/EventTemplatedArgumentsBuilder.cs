using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventTemplatedArgumentsBuilder : BaseCoreBuilder, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError(
                    $"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            Build(project, eventSource, model);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model)
        {
            Build(project, eventSourceProjectItem, model);
        }

        private void Build(Project project, EventSourceModel eventSource, EventModel model)
        {
            if (eventSource == null)
            {
                LogError($"{nameof(eventSource)} should not be null");
                return;
            }

            foreach (var argument in model.GetAllArguments())
            {
                if (TypeExtensions.IsComplexType(argument.Type) && (argument.TypeTemplate == null))
                {
                    var template = this.GetTypeTemplate(project, eventSource.TypeTemplates, argument);
                    if (template != null)
                    {
                        argument.TypeTemplate = template;
                    }
                }
            }
        }
    }
}