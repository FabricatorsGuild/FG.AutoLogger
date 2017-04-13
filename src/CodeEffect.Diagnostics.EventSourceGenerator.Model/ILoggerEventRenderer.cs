namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerEventRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model);
    }
}