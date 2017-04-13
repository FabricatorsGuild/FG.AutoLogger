using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerEventsBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if( model ==  null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var nextEventId = model.StartId;
            
            foreach (var loggerEvent in model.Events)
            {
                if (model.ImplicitArguments != null && model.ImplicitArguments.Length > 0)
                {
                    loggerEvent.InsertImplicitArguments(model.ImplicitArguments);
                }
                if (model.OverrideArguments != null && model.OverrideArguments.Length > 0)
                {
                    loggerEvent.OverrideArguments(model.OverrideArguments);
                }
                loggerEvent.Keywords = new[] { eventSource.Keywords.Find(model.GetKeyword()) };
                loggerEvent.Id = nextEventId;

                //TODO: Create events builder

                nextEventId += 1;
            }
        }
    }
}