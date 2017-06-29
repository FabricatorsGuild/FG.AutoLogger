namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationConstructorRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem);
    }
}