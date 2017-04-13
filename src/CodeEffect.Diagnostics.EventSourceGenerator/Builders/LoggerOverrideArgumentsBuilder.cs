using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerOverrideArgumentsBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            // TODO: Get ALL builders from project and allow them to 'build'
            var eventArgumentBuilders = new[]
            {
                new EventArgumentBuilder()
            };
            foreach (var argument in model?.OverrideArguments?? new EventArgumentModel[0])
            {
                argument.IsOverriden = true;
                foreach (var eventArgumentBuilder in eventArgumentBuilders)
                {
                    eventArgumentBuilder.Build(project, eventSourceProjectItem, argument);
                }
            }
        }
    }
}