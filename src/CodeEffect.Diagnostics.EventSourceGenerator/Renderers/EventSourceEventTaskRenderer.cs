using FG.Diagnostics.AutoLogger.Generator.Templates;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class EventSourceEventTaskRenderer : BaseEtwRendererWithLogging, IEventTaskRenderer
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