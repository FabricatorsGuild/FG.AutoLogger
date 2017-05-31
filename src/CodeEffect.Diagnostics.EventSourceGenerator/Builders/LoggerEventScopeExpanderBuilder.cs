using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerEventScopeExpanderBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            ExpandScopedEvents(model);
        }

        public void ExpandScopedEvents(LoggerModel model)
        {
            var scopeEvents = model.Events.Where(e => e.ReturnType == "System.IDisposable");

            var events = new List<EventModel>(model.Events);
            foreach (var eventModel in scopeEvents)
            {
                    var eventBaseName = eventModel.Name;
                    eventModel.Name = $"Start{eventBaseName}";
                    eventModel.OpCode = EventOpcode.Start;

                    var stopEvent = new EventModel()
                    {
                        Arguments = eventModel.Arguments.Select(a => (EventArgumentModel)a.Clone()).ToArray(),
                        Name = $"Stop{eventBaseName}",
                        OpCode = EventOpcode.Stop,
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