using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerEventSourcePartialNonEventMethodRenderer : EventSourceNonEventMethodBaseRenderer, ILoggerEventSourcePartialEventRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
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

            return Render(model);
            /*
            if (!model.HasComplexArguments)
            {
                return "";
            }

            var output = Template.Template_NON_EVENT_METHOD;
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
            output = output.Replace(Template.Variable_EVENT_NAME, model.Name);

            var nonEventMethodArgumentsDeclarationBuilder = new EventArgumentsListBuilder(
                RenderMethodArgument, Template.Template_NONEVENT_METHOD_ARGUMENT_DELIMITER);
            foreach (var argument in model.GetAllArguments())
            {
                nonEventMethodArgumentsDeclarationBuilder.Append(argument);
            }

            var assignmentArgumentsBuilder = new EventArgumentsListBuilder((arg) => RenderAssignment(arg), Template.Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER);


            output = output.Replace(Template.Variable_NON_EVENT_METHOD_ARGUMENTS, nonEventMethodArgumentsDeclarationBuilder.ToString());

            return output;*/
        }
    }
}