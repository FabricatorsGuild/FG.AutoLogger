using System;
using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Templates;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerImplementationEventMethodRenderer : BaseWithLogging, ILoggerImplementationEventRenderer
    {
        public string RenderMethodArgument(EventArgumentModel model)
        {
            var output = LoggerImplementationEventMethodTemplate.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(LoggerImplementationEventMethodTemplate.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(LoggerImplementationEventMethodTemplate.Template_ARGUMENT_CLR_TYPE, model.CLRType);

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

            var output = LoggerImplementationEventMethodTemplate.Template_LOGGER_METHOD;
            output = output.Replace(LoggerImplementationEventMethodTemplate.Variable_LOGGER_METHOD_NAME, model.Name);
            output = output.Replace(LoggerImplementationEventMethodTemplate.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);

            var methodArguments = new EventArgumentsListBuilder(
                RenderMethodArgument, LoggerImplementationEventMethodTemplate.Template_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION_DELIMITER);
            foreach (var argument in model.GetAllNonImplicitArguments())
            {
                methodArguments.Append(argument);
            }
            output = output.Replace(LoggerImplementationEventMethodTemplate.Variable_LOGGER_METHOD_ARGUMENTS, methodArguments.ToString());

            var methodImplementation = new StringBuilder();
            var renderers = new ILoggerImplementationMethodRenderer[]
            {
                new LoggerImplementationMethodCallEventSourceEventRenderer(),
            }.Union(project.GetExtensions<ILoggerImplementationMethodRenderer>()).ToArray();
            foreach (var renderer in renderers)
            {
                methodImplementation.Append(renderer.Render(project, loggerProjectItem, model));
            }
            output = output.Replace(LoggerImplementationEventMethodTemplate.Variable_LOGGER_METHOD_IMPLEMENTATION, methodImplementation.ToString());

            return output;
        }
    }
}