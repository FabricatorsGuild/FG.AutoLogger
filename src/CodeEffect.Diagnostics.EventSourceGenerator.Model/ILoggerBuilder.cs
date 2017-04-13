namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model);
    }
}