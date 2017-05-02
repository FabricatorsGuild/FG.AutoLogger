using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceKeywordRenderer : BaseWithLogging, IKeywordRenderer
    {
        public string Render(Project project, EventSourceModel eventSource, KeywordModel model)
        {
            var output = EventSourceKeywordTemplate.Template_KEYWORD;
            output = output.Replace(EventSourceKeywordTemplate.Template_KEYWORD_NAME, model.Name);
            output = output.Replace(EventSourceKeywordTemplate.Template_KEYWORD_INDEX, model.Value.ToString());

            return output;
        }
    }
}