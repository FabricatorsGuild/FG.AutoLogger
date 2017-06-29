using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text.RegularExpressions;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class LoggerEventScopeExpanderBuilder : BaseCoreBuilder, ILoggerBuilder
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

                if (eventModel.ReturnType == "System.IDisposable")
                {
                    eventModel.IsScopedOperation = true;
                    eventModel.OperationName = GetEventOperationName(eventModel);
                }

                var stopEvent = new EventModel()
                {
                    Arguments = eventModel.Arguments.Select(a => (EventArgumentModel) a.Clone()).ToArray(),
                    Name = $"Stop{eventBaseName}",
                    OpCode = EventOpcode.Stop,
                    Task = eventTask,
                    IsScopedOperation = eventModel.IsScopedOperation,
                    OperationName = eventModel.OperationName,
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

        private readonly Regex _eventOperationNameRegex = new Regex("start|stop", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = $"{eventOperationName.Substring(0, 1).ToLowerInvariant()}{eventOperationName.Substring(1)}";
            return eventOperationName;
        }
    }
}