namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerEventSourcePartialEventRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model);
    }
}