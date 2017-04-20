namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IExtensionsMethodBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceModelProjectItem, ExtensionsMethodModel model);
    }
}