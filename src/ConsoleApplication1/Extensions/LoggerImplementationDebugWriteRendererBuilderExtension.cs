using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace ConsoleApplication1.Extensions
{
    public class LoggerImplementationDebugWriteRendererBuilderExtension : ILoggerImplementationEventRenderer
    {
        private readonly Regex _eventOperationNameRegex = new Regex("start", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            if (model.OpCode == EventOpcode.Start)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"		private System.Diagnostics.Stopwatch _@@LOGGER_EVENT_OPERATION_NAME@@Stopwatch = new System.Diagnostics.Stopwatch();
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;
            }
            return "";
        }

        private string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = eventOperationName.Substring(0, 1).ToLowerInvariant() + eventOperationName.Substring(1);
            return eventOperationName;
        }
    }
}