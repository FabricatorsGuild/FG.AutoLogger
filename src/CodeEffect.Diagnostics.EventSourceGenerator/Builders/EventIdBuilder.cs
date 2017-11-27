using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventIdBuilder : BaseCoreBuilder, ILoggerEventBuilder, IEventBuilder
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
            var maxAssignedId = (events?.Where(e => e.Id != null).Max(e => e.Id) ?? startId);

            var next = model.Id ?? (maxAssignedId + 1);
            model.Id = next;
        }
    }
}