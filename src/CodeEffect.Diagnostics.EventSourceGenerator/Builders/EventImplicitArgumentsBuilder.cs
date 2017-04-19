using System;
using System.Diagnostics.Eventing.Reader;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventImplicitArgumentsBuilder : IEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
           //TODO: Do we need this?
        }
    }
}