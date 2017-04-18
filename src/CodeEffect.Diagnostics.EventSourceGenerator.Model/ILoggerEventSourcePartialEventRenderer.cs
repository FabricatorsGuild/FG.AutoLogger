namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerEventSourcePartialEventRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model);
    }
}