namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventArgumentBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSource, EventArgumentModel model);
    }
}