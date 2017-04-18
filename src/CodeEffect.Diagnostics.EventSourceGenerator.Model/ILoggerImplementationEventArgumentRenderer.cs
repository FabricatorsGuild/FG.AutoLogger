namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerImplementationEventArgumentRenderer : IRenderer
    {
        string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel eventModel, EventArgumentModel model);
    }
}