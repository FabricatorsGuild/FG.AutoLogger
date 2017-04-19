﻿using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceEventsBuilder : BaseWithLogging, IEventSourceBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            var eventBuilders = new IEventBuilder[]
            {
                new EventIdBuilder(),
                new EventImplicitArgumentsBuilder(),
                new EventTemplatedArgumentsBuilder(),
                new EventArgumentsBuilder(),
                new EventMessageFormatterBuilder(),
                new EventArgumentsComplexityCheckBuilder(),
            }.Union(project.GetExtensions<IEventBuilder>()).ToArray();

            foreach (var evt in eventSource?.Events ?? new EventModel[0])
            {
                foreach (var eventBuilder in eventBuilders)
                {
                    eventBuilder.Build(project, model, evt);
                }
            }
        }
    }
}