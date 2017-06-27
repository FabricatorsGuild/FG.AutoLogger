namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IEventSourceRenderer : IRenderer
    {
        void Render(Project project, ProjectItem<EventSourceModel> model);
    }
}