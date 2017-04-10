using System;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventBuilder
    {
        void Build(Project project, EventSourceModel eventSource, EventModel model);
    }
}