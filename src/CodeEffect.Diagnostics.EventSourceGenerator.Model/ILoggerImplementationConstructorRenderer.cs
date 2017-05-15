namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerImplementationConstructorRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem);
    }
}