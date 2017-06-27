namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerEventBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model);
    }
}