namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IExtensionsMethodBuilder : IBuilder
    {
        void Build(Project project, ProjectItem<EventSourceModel> eventSourceModelProjectItem, ExtensionsMethodModel model);
    }
}