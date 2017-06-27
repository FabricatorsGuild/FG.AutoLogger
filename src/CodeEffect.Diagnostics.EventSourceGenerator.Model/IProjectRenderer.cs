namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IProjectRenderer  : IRenderer
    {
        void Render(Project model);
    }
}