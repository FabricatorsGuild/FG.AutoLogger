namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IEventArgumentBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSource, EventArgumentModel model);
    }
}