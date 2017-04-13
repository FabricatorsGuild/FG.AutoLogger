namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IExtensionsMethodBuilder : IRenderer
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceModelProjectItem, ExtensionsMethodModel model);
    }
}