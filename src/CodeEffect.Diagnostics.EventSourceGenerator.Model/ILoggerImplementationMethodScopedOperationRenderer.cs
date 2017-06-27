namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationMethodScopedOperationRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel eventModel);
    }
}