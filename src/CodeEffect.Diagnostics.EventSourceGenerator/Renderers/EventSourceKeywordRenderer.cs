using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceKeywordRenderer : BaseWithLogging, IKeywordRenderer
    {
        public string Render(Project project, EventSourceModel eventSource, KeywordModel model)
        {
            var output = Template.Template_KEYWORD;
            output = output.Replace(Template.Template_KEYWORD_NAME, model.Name);
            output = output.Replace(Template.Template_KEYWORD_INDEX, model.Value.ToString());

            return output;
        }
    }
}