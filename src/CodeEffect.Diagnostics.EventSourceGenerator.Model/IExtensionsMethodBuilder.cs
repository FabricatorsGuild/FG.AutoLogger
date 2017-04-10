namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IExtensionsMethodBuilder
    {
        void Build(Project project, EventSourceModel eventSourceModel, ExtensionsMethodModel model);
    }
}