using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class LoggerOverrideArgumentsBuilder : BaseCoreBuilder, ILoggerBuilder
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

            var eventArgumentBuilders = new IEventArgumentBuilder[]
            {
                new EventArgumentBuilder(),
                new EventArgumentExtensionMethodBuilder(),
            }.Union(project.GetExtensions<IEventArgumentBuilder>(eventSource.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var argument in model?.OverrideArguments?? new EventArgumentModel[0])
            {
                argument.IsOverriden = true;
                foreach (var builder in eventArgumentBuilders)
                {
                    PassAlongLoggers(builder as IWithLogging);
                    builder.Build(project, eventSourceProjectItem, argument);
                }
            }
        }
    }
}