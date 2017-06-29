using FG.Diagnostics.AutoLogger.Generator.Templates;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class EventSourceKeywordRenderer : BaseEtwRendererWithLogging, IKeywordRenderer
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