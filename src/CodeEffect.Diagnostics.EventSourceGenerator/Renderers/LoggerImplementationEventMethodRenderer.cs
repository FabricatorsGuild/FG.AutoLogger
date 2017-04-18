using System;
using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerImplementationEventMethodRenderer : BaseWithLogging, ILoggerImplementationEventRenderer
    {
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

            var output = Template.Template_LOGGER_METHOD;
            output = output.Replace(Template.Variable_LOGGER_METHOD_NAME, model.Name);
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);

            var methodArguments = new StringBuilder();
            var eventArgumentRenderers = new ILoggerImplementationEventArgumentRenderer[]
            {
                new LoggerImplementationEventMethodArgumentRenderer(), 
            }.Union(project.GetExtensions<ILoggerImplementationEventArgumentRenderer>()).ToArray();
            foreach (var argument in model.GetAllNonImplicitArguments())
            {
                foreach (var renderer in eventArgumentRenderers)
                {
                    methodArguments.Append(renderer.Render(project, loggerProjectItem, model, argument));
                }                
            }
            output = output.Replace(Template.Variable_LOGGER_METHOD_ARGUMENTS, methodArguments.ToString());

            var methodImplementation = new StringBuilder();
            var renderers = new ILoggerImplementationMethodRenderer[]
            {
                new LoggerImplementationMethodCallEventSourceEventRenderer(),
            }.Union(project.GetExtensions<ILoggerImplementationMethodRenderer>()).ToArray();
            foreach (var renderer in renderers)
            {
                methodImplementation.Append(renderer.Render(project, loggerProjectItem, model));
            }
            output = output.Replace(Template.Variable_LOGGER_METHOD_IMPLEMENTATION, methodImplementation.ToString());

            return output;
        }
    }
}