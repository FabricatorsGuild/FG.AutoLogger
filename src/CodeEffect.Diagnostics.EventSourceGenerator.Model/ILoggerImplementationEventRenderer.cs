namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationEventRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model);
    }
}