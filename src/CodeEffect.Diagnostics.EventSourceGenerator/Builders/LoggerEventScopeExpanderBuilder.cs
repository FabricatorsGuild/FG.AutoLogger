using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class LoggerEventScopeExpanderBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            ExpandScopedEvents(eventSource, model);
        }

        public void ExpandScopedEvents(EventSourceModel eventSource, LoggerModel model)
        {
            var scopeEvents = model.Events.Where(e => e.ReturnType == "System.IDisposable");

            var events = new List<EventModel>(model.Events);
            foreach (var eventModel in scopeEvents)
            {
                var eventBaseName = eventModel.Name;
                eventModel.Name = $"Start{eventBaseName}";
                eventModel.OpCode = EventOpcode.Start;

                var eventTask = eventSource.GetEventTask(eventBaseName);
                if (eventTask == null)
                {
                    eventTask = eventSource.AddEventTask(eventBaseName);
                }
                eventModel.Task = eventTask;

                var stopEvent = new EventModel()
                {
                    Arguments = eventModel.Arguments.Select(a => (EventArgumentModel) a.Clone()).ToArray(),
                    Name = $"Stop{eventBaseName}",
                    OpCode = EventOpcode.Stop,
                    Task = eventTask,
                    Keywords = eventModel.Keywords,
                    HasComplexArguments = eventModel.HasComplexArguments,
                    CorrelatesTo = eventModel,
                    ImplicitArguments = eventModel.ImplicitArguments,
                    Level = eventModel.Level,
                    MessageFormatter = eventModel.MessageFormatter,
                };

                eventModel.CorrelatesTo = stopEvent;

                var index = events.IndexOf(eventModel);
                if (index == events.Count - 1)
                {
                    events.Add(stopEvent);
                }
                else
                {
                    events.Insert(events.IndexOf(eventModel) + 1, stopEvent);
                }
            }
            model.Events = events.ToArray();
        }
    }
}