namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerImplementationUsingRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem);
    }
}