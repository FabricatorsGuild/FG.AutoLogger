namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IEventArgumentBuilder
    {
        void Build(Project project, EventSourceModel eventSource, EventArgumentModel model);
    }
}