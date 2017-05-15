using System.Text.RegularExpressions;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public abstract class AITelemetryRendererExtensionBase : BaseWithLogging
    {
        private readonly Regex _eventOperationNameRegex = new Regex("start|stop", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = eventOperationName.Substring(0, 1).ToLowerInvariant() + eventOperationName.Substring(1);
            return eventOperationName;
        }
    }
}