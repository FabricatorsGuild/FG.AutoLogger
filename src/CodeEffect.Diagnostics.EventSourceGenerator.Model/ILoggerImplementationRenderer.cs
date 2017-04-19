namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerImplementationRenderer : IRenderer
    {
        void Render(Project project, ProjectItem<LoggerModel> model);
    }
}