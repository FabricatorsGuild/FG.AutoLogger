namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationMethodRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel eventModel);
    }
}