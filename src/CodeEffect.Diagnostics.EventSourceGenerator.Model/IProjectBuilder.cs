namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IProjectBuilder : IBuilder
    {
        void Build(Project model);
    }
}