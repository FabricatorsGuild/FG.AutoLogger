using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventImplicitArgumentsBuilder : BaseCoreBuilder, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
           //TODO: Do we need this?
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel loggerModel, EventModel model)
        {
            var eventSourceModel = eventSourceProjectItem.Content;
            if (eventSourceModel == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            if (loggerModel.ImplicitArguments != null && loggerModel.ImplicitArguments.Length > 0)
            {
                model.InsertImplicitArguments(loggerModel.ImplicitArguments);
            }
        }
    }
}