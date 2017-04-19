namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerEventSourcePartialRenderer : IRenderer
    {
        void Render(Project project, ProjectItem<LoggerModel> model);
    }
}