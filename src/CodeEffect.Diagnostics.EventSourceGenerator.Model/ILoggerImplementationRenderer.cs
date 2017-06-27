namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ILoggerImplementationRenderer : IRenderer
    {
        void Render(Project project, ProjectItem<LoggerModel> model);
    }
}