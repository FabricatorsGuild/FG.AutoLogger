namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IEventSourceBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> model);
    }
}