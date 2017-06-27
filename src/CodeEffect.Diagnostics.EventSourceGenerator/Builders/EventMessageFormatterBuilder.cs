using System;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventMessageFormatterBuilder : BaseWithLogging, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel loggerModel, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} with property EventSource set but found {eventSourceProjectItem.Content?.Name ?? "null"}");
            }

            Build(eventSource, model);
        }

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

        private void Build(EventSourceModel eventSourceModel, EventModel model)
        {
            if (model == null) return;
            if (model.MessageFormatter != null) return;

            var messageFormatBuilder = new ListBuilder($"{model.Name.GetHumanReadable()}", " ", " ");

            var nextArgumentIndex = 1;
            var messageArgumentIndex = -1;
            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                if (!argument.IsImplicit) messageFormatBuilder.Append($"{{{nextArgumentIndex - 1}}}");
                if (argument.Name.Equals("message", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageArgumentIndex = nextArgumentIndex;
                }
                nextArgumentIndex++;
            }

            model.MessageFormatter = messageArgumentIndex >= 0 ? $"{{{messageArgumentIndex - 1}}}" : messageFormatBuilder.ToString();
        }

    }
}