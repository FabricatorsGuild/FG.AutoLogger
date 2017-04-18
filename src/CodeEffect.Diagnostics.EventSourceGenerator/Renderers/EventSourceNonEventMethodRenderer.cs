using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceNonEventMethodRenderer : EventSourceNonEventMethodBaseRenderer, IEventRenderer
    {        
        public string Render(Project project, EventSourceModel eventSource, EventModel model)
        {
            return Render(model);
        }
    }
}