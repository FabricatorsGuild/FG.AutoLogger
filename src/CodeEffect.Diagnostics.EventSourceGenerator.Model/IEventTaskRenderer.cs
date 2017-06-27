namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IEventTaskRenderer : IRenderer
    {
        string Render(Project project, EventSourceModel eventSource, EventTaskModel model);
    }
}