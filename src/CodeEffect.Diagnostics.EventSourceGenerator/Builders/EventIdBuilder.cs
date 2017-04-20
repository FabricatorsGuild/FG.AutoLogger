using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventIdBuilder : BaseWithLogging, ILoggerEventBuilder, IEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel loggerModel, EventModel model)
        {
            var startId = loggerModel.StartId ?? 0;
            var maxAssignedId = loggerModel.Events?.Count(e => e.Id != null) ?? 0;

            Build(loggerModel.Events, model, loggerModel.StartId);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            Build(eventSource.Events, model);
        }

        private void Build(IEnumerable<EventModel> events, EventModel model, int? startId = 0)
        {
            var maxAssignedId = (events?.Where(e => e.Id != null).Max(e => e.Id) ?? 0);

            var next = model.Id ?? (startId + maxAssignedId + 1);
            model.Id = next;
        }
    }
}