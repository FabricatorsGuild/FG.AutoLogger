using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventIdBuilder : IEventBuilder
    {
        public void Build(Project project, EventSourceModel eventSource, EventModel model)
        {
            var next = model.Id ?? eventSource.Events.Max(e => e.Id) + 1;
            model.Id = next;
        }
    }
}