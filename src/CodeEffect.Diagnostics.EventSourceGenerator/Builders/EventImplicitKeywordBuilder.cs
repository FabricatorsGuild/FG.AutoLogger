using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventImplicitKeywordBuilder : BaseWithLogging, ILoggerEventBuilder, IEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            if ((model.Name.Matches(@"*Error*", StringComparison.InvariantCultureIgnoreCase))
                || (model.Name.Matches(@"*Exception*", StringComparison.InvariantCultureIgnoreCase)))
            {
                var errorKeyword = eventSource.GetKeyword("Error") ?? eventSource.AddKeyword("Error");

                model.Keywords = model.Keywords.Add(errorKeyword);
            }            
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model)
        {
            Build(project, eventSourceProjectItem, model);
        }
    }
}