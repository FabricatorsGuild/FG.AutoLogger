using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public abstract class EventMessageFormatterBuilderBase : BaseWithLogging
    {
        public void Build(EventSourceModel eventSourceModel, EventModel model)
        {
            if (model == null) return;
            if (model.MessageFormatter != null) return;

            var messageFormatBuilder = new ListBuilder($"{model.Name.GetHumanReadable()}", " ", " ");

            var nextArgumentIndex = 1;
            var messageArgumentIndex = -1;
            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                if (!argument.IsImplicit) messageFormatBuilder.Append($"{{{nextArgumentIndex}}}");
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