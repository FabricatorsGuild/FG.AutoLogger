namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IExtensionsMethodRenderer : IRenderer
    {
        string Render(Project project, EventSourceModel eventSource, ExtensionsMethodModel model);
    }
}