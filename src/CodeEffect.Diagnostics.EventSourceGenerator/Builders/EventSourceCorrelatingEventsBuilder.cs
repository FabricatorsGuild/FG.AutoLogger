using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventSourceCorrelatingEventsBuilder : BaseWithLogging, IEventSourceBuilder, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            Build(eventSource.Events ?? new EventModel[0]);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            
            Build(model.Events ?? new EventModel[0]);
        }

        private void Build(IEnumerable<EventModel> events)
        {
            var startEvents = events.Where(e => StringMatchExtensions.Matches(e.Name, "*start*", StringComparison.InvariantCultureIgnoreCase, useWildcards: true)).ToArray();
            var stopEvents = events.Where(e => e.Name.Matches("*stop*", StringComparison.InvariantCultureIgnoreCase, useWildcards: true)).ToArray();

            foreach (var startEvent in startEvents)
            {
                var stopEventName = startEvent.Name.ToLowerInvariant().Replace("start", "stop");
                var stopEvent = stopEvents.FirstOrDefault(e => e.Name.Equals(stopEventName, StringComparison.InvariantCultureIgnoreCase));
                if (stopEvent != null)
                {
                    startEvent.OpCode = EventOpcode.Start;
                    stopEvent.OpCode = EventOpcode.Stop;
                    startEvent.CorrelatesTo = stopEvent;
                    stopEvent.CorrelatesTo = startEvent;
                }
            }
        }

    }
}