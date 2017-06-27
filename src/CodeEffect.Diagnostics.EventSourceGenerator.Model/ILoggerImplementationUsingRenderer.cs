namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationUsingRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem);
    }
}