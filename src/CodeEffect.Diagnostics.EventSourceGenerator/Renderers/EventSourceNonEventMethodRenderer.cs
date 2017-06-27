using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class EventSourceNonEventMethodRenderer : EventSourceNonEventMethodBaseRenderer, IEventRenderer
    {        
        public string Render(Project project, EventSourceModel eventSource, EventModel model)
        {
            return Render(model);
        }
    }
}