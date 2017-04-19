using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerImplicitArgumentsBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            if (model == null) return;

            var eventArgumentBuilders = new IEventArgumentBuilder[]
            {
                new EventArgumentBuilder(),
                new EventArgumentExtensionMethodBuilder(), 
            }.Union(project.GetExtensions<IEventArgumentBuilder>()).ToArray();
            foreach (var argument in model?.ImplicitArguments)
            {
                foreach (var eventArgumentBuilder in eventArgumentBuilders)
                {
                    eventArgumentBuilder.Build(project, eventSourceProjectItem, argument);
                }
            }
        }
    }
}