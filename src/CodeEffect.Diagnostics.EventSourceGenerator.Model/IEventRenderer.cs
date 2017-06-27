namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IEventRenderer : IRenderer
    {
        string Render(Project project, EventSourceModel eventSource, EventModel model);
    }
}