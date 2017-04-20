using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventTemplatedArgumentsBuilder : BaseWithLogging, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            Build(eventSource, model);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model)
        {
            Build(project, eventSourceProjectItem, model);
        }

        private void Build(EventSourceModel eventSource, EventModel model)
        {
            if (eventSource == null)
            {
                LogError($"{nameof(eventSource)} should not be null");
                return;
            }

            foreach (var argument in model.GetAllArguments())
            {
                var template = eventSource.TypeTemplates.GetTypeTemplate(argument.Type);
                if (template != null)
                {
                    argument.TypeTemplate = template;
                }
            }

        }
    }
}