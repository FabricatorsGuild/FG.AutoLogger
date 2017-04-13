namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventSourceRenderer : IRenderer
    {
        void Render(Project project, ProjectItem<EventSourceModel> model);
    }
}