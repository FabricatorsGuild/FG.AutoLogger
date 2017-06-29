using System;
using System.Text.RegularExpressions;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.AI
{
    public abstract class AITelemetryRendererExtensionBase : BaseWithLogging, IExtension
    {
        private readonly Regex _eventOperationNameRegex = new Regex("start|stop", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = $"{eventOperationName.Substring(0, 1).ToLowerInvariant()}{eventOperationName.Substring(1)}";            
            return eventOperationName;
        }

        public EventArgumentModel GetRequestNameArgument(EventModel model)
        {
            var hasRequestNameArgument = false;
            EventArgumentModel requestNameArgument = null;
            foreach (var eventArgumentModel in model.GetAllArgumentsExpanded())
            {
                if (eventArgumentModel.Name.Matches("*request*", StringComparison.InvariantCultureIgnoreCase, useWildcards: true))
                {
                    if (hasRequestNameArgument)
                    {
                        LogWarning($"Event {model.Name} has multiple potential request name arguments");
                    }
                    requestNameArgument = eventArgumentModel;
                    hasRequestNameArgument = true;
                }
            }

            return requestNameArgument;
        }

        public string Module => @"AI";
    }
}