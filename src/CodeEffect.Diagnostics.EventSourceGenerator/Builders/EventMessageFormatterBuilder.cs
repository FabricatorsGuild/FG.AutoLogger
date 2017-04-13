using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventMessageFormatterBuilder : BaseWithLogging, IEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            if( model == null ) return;
            if(model.MessageFormatter != null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var messageFormatBuilder = new ListBuilder($"{model.Name.GetHumanReadable()}", " ", " ");

            var nextArgumentIndex = 1;
            var messageArgumentIndex = -1;
            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                if( !argument.IsImplicit) messageFormatBuilder.Append($"{{{nextArgumentIndex}}}");
                if (argument.Name.Equals("message", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageArgumentIndex = nextArgumentIndex;
                }
                nextArgumentIndex++;
            }

            model.MessageFormatter = messageArgumentIndex >= 0 ? $"{{{messageArgumentIndex}}}" : messageFormatBuilder.ToString();
        }
    }
}