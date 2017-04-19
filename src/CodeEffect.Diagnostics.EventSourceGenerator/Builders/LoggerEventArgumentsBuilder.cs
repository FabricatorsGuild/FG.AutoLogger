using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerEventArgumentsBuilder : BaseWithLogging, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel loggerModel, EventModel model)
        {
            var eventSourceModel = eventSourceProjectItem.Content;
            if (eventSourceModel == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var eventArgumentBuilders = new IEventArgumentBuilder[]
            {
                new EventArgumentBuilder(),
                new EventArgumentExtensionMethodBuilder(),
            }.Union(project.GetExtensions<IEventArgumentBuilder>()).ToArray();
            foreach (var argument in model.Arguments ?? new EventArgumentModel[0])
            {
                foreach (var eventArgumentBuilder in eventArgumentBuilders)
                {
                    eventArgumentBuilder.Build(project, eventSourceProjectItem, argument);
                }
            }
        }
    }
}