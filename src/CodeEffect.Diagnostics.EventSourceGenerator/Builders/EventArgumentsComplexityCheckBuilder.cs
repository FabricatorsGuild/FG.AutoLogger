using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventArgumentsComplexityCheckBuilder : EventArgumentsComplexityCheckBuilderBase, IEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            Build(model);
        }
    }
}