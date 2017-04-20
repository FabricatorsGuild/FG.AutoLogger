namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IExtensionsMethodRenderer : IRenderer
    {
        string Render(Project project, EventSourceModel eventSource, ExtensionsMethodModel model);
    }
}