using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public interface IEventRenderer
    {
        string Render(EventModel model, int index, EventSourcePrototype eventSource);
    }
}