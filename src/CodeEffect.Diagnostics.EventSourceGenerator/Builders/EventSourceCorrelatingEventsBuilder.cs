using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
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

            Build(eventSource.Events);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            
            Build(model.Events);
        }

        private void Build(IEnumerable<EventModel> events)
        {
            var startEvents = events.Where(e => StringMatchExtensions.Matches(e.Name, "*start*", StringComparison.InvariantCultureIgnoreCase, useWildcards: true)).ToArray();
            var stopEvents = events.Where(e => e.Name.Matches("*stop*", StringComparison.InvariantCulture, useWildcards: true)).ToArray();

            foreach (var startEvent in startEvents)
            {
                var stopEventName = startEvent.Name.Replace("start", "stop");
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