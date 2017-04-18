using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerImplementationMethodCallEventSourceEventRenderer : BaseWithLogging, ILoggerImplementationMethodRenderer
    {
        private string RenderEventSourceEventMethodCallArgument(EventArgumentModel model)
        {
            var output = model.IsImplicit ? Template.Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT : Template.Template_METHOD_CALL_PASSTHROUGH_ARGUMENT;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
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

            var output = Template.Template_LOGGER_METHOD_CALL_EVENTSOURCE_EVENT;
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
            output = output.Replace(Template.Variable_LOGGER_METHOD_NAME, model.Name);

            var callArguments = new EventArgumentsListBuilder(
                RenderEventSourceEventMethodCallArgument, Template.Template_LOGGER_CALL_ARGUMENTS_DELIMITER);

            foreach (var argument in model.GetAllArguments())
            {
                callArguments.Append(argument);
            }
            output = output.Replace(Template.Variable_LOGGER_METHOD_IMPLEMENTATION_CALL_ARGUMENTS, callArguments.ToString());

            return output;
        }
    }
}