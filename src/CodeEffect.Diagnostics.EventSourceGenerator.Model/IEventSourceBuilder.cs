namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventSourceBuilder
    {
        void Build(Project project, EventSourceModel model);
    }
}