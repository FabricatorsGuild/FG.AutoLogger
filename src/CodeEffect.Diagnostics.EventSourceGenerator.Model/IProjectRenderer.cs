namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IProjectRenderer  : IRenderer
    {
        void Render(Project model);
    }
}