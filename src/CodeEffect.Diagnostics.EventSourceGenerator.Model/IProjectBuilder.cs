namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IProjectBuilder : IBuilder
    {
        void Build(Project model);
    }
}