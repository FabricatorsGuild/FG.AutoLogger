using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public abstract class EventTemplatedArgumentsBuilderBase : BaseWithLogging
    {
        public void Build(EventSourceModel eventSource, EventModel model)
        {
            if (eventSource == null)
            {
                LogError($"{nameof(eventSource)} should not be null");
                return;
            }

            foreach (var argument in model.GetAllArguments())
            {
                var template = eventSource.TypeTemplates.GetTypeTemplate(argument.Type);
                if (template != null)
                {
                    argument.TypeTemplate = template;
                }
            }

        }
    }
}