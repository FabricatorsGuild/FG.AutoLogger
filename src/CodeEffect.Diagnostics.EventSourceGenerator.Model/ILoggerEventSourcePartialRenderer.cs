namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerEventSourcePartialRenderer : IRenderer
    {
        void Render(Project project, ProjectItem<LoggerModel> model);
    }
}