namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IEventBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model);
    }
}