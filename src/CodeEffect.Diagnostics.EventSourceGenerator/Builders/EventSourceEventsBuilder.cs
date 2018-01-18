using System;
using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventSourceEventsBuilder : BaseCoreBuilder, IEventSourceBuilder, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }
            Build<IEventBuilder>(project, model, eventSource?.Events ?? new EventModel[0], (builder, evt) => builder.Build(project, model, evt));
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            Build<ILoggerEventBuilder>(project, eventSourceProjectItem, model.Events ?? new EventModel[0], (builder, evt) => builder.Build(project, eventSourceProjectItem, model, evt));
        }

        private void Build<TBuilder>(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, IEnumerable<EventModel> events, Action<TBuilder, EventModel> build)
            where TBuilder : IBuilder
        {
            var eventSourceModel = eventSourceProjectItem.Content;
            if (eventSourceModel == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var eventBuilders = new IBuilder[]
            {
                new EventIdBuilder(),
                new EventImplicitKeywordBuilder(),
                new EventImplicitArgumentsBuilder(),
                new EventOverrideArgumentsBuilder(),
                new EventArgumentsBuilder(),
                new EventMessageFormatterBuilder(),
                new EventTemplatedArgumentsBuilder(),
                new EventImplicitlyTemplatedArgumentsBuilder(), 
                new EventArgumentsComplexityCheckBuilder(),
            }.Cast<TBuilder>().Union(project.GetExtensions<TBuilder>(eventSourceModel.Settings?.Modules ?? new string[0])).ToArray();

            foreach (var evt in events)
            {
                foreach (var builder in eventBuilders)
                {
                    PassAlongLoggers(builder as IWithLogging);
                    build(builder, evt);
                }
            }
        }
    }
}