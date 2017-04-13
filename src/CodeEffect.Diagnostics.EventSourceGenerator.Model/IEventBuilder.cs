using System;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model);
    }
}