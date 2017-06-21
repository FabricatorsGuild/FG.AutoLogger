using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Templates;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceEventTaskRenderer : BaseWithLogging, IEventTaskRenderer
    {
        public string Render(Project project, EventSourceModel eventSource, EventTaskModel model)
        {
            var output = EventSourceEventTaskTemplate.Template_EVENTTASK;
            output = output.Replace(EventSourceEventTaskTemplate.Template_EVENTTASK_NAME, model.Name);
            output = output.Replace(EventSourceEventTaskTemplate.Template_EVENTTASK_INDEX, model.Value.ToString());

            return output;
        }
    }
}