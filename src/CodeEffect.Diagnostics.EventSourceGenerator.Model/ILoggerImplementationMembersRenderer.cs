namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationMembersRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem);
    }
}