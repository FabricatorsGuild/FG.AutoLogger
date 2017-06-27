using System.Collections.Generic;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventSourceEventTaskBuilder : BaseWithLogging, IEventSourceBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            if (model == null) return;
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            var allEventTasks = new List<EventTaskModel>();
            var nextEventTask = 1;
            foreach (var eventTask in eventSource.Tasks ?? new EventTaskModel[0])
            {
                eventTask.Value = nextEventTask;
                allEventTasks.Add(eventTask);
                nextEventTask *= 2;
            }
            eventSource.Tasks = allEventTasks.ToArray();
        }
    }
}