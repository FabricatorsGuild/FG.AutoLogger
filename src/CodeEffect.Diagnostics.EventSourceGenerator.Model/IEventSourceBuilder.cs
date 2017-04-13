namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventSourceBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> model);
    }
}