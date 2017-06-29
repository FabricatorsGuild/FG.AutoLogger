using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventOverrideArgumentsBuilder : BaseCoreBuilder, IEventBuilder, ILoggerEventBuilder
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