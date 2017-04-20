using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventOverrideArgumentsBuilder : BaseWithLogging, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            //TODO: Do we need this?
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel loggerModel, EventModel model)
        {
            if (loggerModel.OverrideArguments != null && loggerModel.OverrideArguments.Length > 0)
            {
                model.OverrideArguments(loggerModel.OverrideArguments);
            }
        }
    }
}