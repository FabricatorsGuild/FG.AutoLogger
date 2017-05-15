using System.Diagnostics.Tracing;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public class LoggerImplementationAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationEventRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            if (model.OpCode == EventOpcode.Start)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"		private IOperationHolder<RequestTelemetry> _@@LOGGER_EVENT_OPERATION_NAME@@OperationHolder;
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;
            }
            return "";
        }
    }
}