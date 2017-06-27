namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model);
    }
}