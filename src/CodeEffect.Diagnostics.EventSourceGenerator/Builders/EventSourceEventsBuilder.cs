using System;
using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceEventsBuilder : BaseWithLogging, IEventSourceBuilder, ILoggerBuilder
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
            var eventBuilders = new IBuilder[]
            {
                new EventIdBuilder(),
                new EventImplicitKeywordBuilder(),
                new EventImplicitArgumentsBuilder(),
                new LoggerEventOverrideArgumentsBuilder(),
                new EventArgumentsBuilder(),
                new EventMessageFormatterBuilder(),
                new EventTemplatedArgumentsBuilder(),
                new EventArgumentsComplexityCheckBuilder(),
            }.Cast<TBuilder>().Union(project.GetExtensions<TBuilder>()).ToArray();

            foreach (var evt in events)
            {
                foreach (var builder in eventBuilders)
                {
                    build(builder, evt);
                }
            }
        }
    }
}