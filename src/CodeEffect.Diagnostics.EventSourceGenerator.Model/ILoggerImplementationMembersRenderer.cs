namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerImplementationMembersRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem);
    }
}