using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class ProjectEventSourceRenderer : BaseEtwRendererWithLogging, IProjectRenderer
    {
        public void Render(Project model)
        {
            var files = model.ProjectItems.ToArray();
            LogMessage($"Rendering {files.Length} project file{(files.Length == 1 ? "" : "s")} for eventsources");

            var eventSourceProjectItems = files.OfType<EventSourceModel>(ProjectItemType.EventSource);
            foreach (var eventSourceProjectItem in eventSourceProjectItems)
            {
                LogMessage($"Rendering EventSource from file {eventSourceProjectItem.Name}");
                RenderEventSource(model, eventSourceProjectItem);
            }
        }

        private void RenderEventSource(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem)
        {
            var eventSourceModel = eventSourceProjectItem.Content as EventSourceModel;
            if (eventSourceModel == null)
            {
                LogError(
                    $"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var eventSourceRenderers = new IEventSourceRenderer[]
            {
                new EventSourceRenderer(), 
            };
            foreach (var renderer in eventSourceRenderers.Union(project.GetExtensions<IEventSourceRenderer>()))
            {
                PassAlongLoggers(renderer as IWithLogging);
                renderer.Render(project, eventSourceProjectItem);
            }
        }
    }
}