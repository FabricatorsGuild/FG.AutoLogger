namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IKeywordRenderer : IRenderer
    {
        string Render(Project project, EventSourceModel eventSource, KeywordModel model);
    }
}