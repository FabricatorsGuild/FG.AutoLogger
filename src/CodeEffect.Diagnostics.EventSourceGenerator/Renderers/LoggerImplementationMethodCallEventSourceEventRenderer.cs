using FG.Diagnostics.AutoLogger.Generator.Templates;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class LoggerImplementationMethodCallEventSourceEventRenderer : BaseEtwRendererWithLogging, ILoggerImplementationMethodRenderer
    {
        private string RenderEventSourceEventMethodCallArgument(EventArgumentModel model)
        {
            var output = model.IsImplicit ? LoggerImplementationMethodCallEventSourceEventTemplate.Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT : LoggerImplementationMethodCallEventSourceEventTemplate.Template_METHOD_CALL_PASSTHROUGH_ARGUMENT;
            output = output.Replace(LoggerImplementationMethodCallEventSourceEventTemplate.Template_ARGUMENT_NAME, model.Name);
            return output;
        }

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            if (loggerProjectItem.ItemType != ProjectItemType.LoggerImplementation)
            {
                LogError($"{loggerProjectItem.Name} should be of ItemType {nameof(ProjectItemType.LoggerImplementation)} set but found {loggerProjectItem.ItemType}");
                return "";
            }

            var loggerModel = loggerProjectItem.Content;
            if (loggerModel == null)
            {
                LogError($"{loggerProjectItem?.Name ?? nameof(loggerProjectItem)} should have content of type {typeof(LoggerModel).Name}, but found {loggerProjectItem?.GetType().Name ?? "null"}");
                return "";
            }

            var eventSourceModel = loggerModel.EventSource;
            if (eventSourceModel == null)
            {
                LogError($"{loggerProjectItem?.Name ?? nameof(loggerProjectItem)} should have content of type {typeof(LoggerModel).Name} with property EventSource set, but found {loggerModel.EventSource?.Name ?? "null"}");
                return "";
            }

            var output = LoggerImplementationMethodCallEventSourceEventTemplate.Template_LOGGER_METHOD_CALL_EVENTSOURCE_EVENT;
            output = output.Replace(LoggerImplementationMethodCallEventSourceEventTemplate.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
            output = output.Replace(LoggerImplementationMethodCallEventSourceEventTemplate.Variable_LOGGER_METHOD_NAME, model.Name);

            var callArguments = new EventArgumentsListBuilder(
                RenderEventSourceEventMethodCallArgument, LoggerImplementationMethodCallEventSourceEventTemplate.Template_LOGGER_CALL_ARGUMENTS_DELIMITER);

            foreach (var argument in model.GetAllArguments())
            {
                callArguments.Append(argument);
            }
            output = output.Replace(LoggerImplementationMethodCallEventSourceEventTemplate.Variable_LOGGER_METHOD_IMPLEMENTATION_CALL_ARGUMENTS, callArguments.ToString());

            return output;
        }
    }
}