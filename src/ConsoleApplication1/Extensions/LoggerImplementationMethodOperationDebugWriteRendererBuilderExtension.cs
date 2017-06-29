using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using FG.Diagnostics.AutoLogger.Model;

namespace ConsoleApplication1.Extensions
{
    public class LoggerImplementationMethodOperationDebugWriteRendererBuilderExtension : ILoggerImplementationMethodRenderer
    {
        private readonly Regex _eventOperationNameRegex = new Regex("start|stop", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = eventOperationName.Substring(0, 1).ToLowerInvariant() + eventOperationName.Substring(1);
            return eventOperationName;
        }

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {

            if (model.OpCode == EventOpcode.Stop)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"			_@@LOGGER_EVENT_OPERATION_NAME@@Stopwatch.Stop();
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;
            }
            else if( model.OpCode == EventOpcode.Start)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"			_@@LOGGER_EVENT_OPERATION_NAME@@Stopwatch.Restart();
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;

            }
            return "";
        }

        public string Module => @"Demo";
    }
}