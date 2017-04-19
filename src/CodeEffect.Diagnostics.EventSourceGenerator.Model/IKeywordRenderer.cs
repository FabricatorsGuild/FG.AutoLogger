namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IKeywordRenderer : IRenderer
    {
        string Render(Project project, EventSourceModel eventSource, KeywordModel model);
    }
}