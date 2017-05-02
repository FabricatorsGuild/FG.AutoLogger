using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceNonEventMethodBaseRenderer : EventMethodBaseRenderer
    {
        private static string RenderAssignment(EventArgumentModel model)
        {
            var output = model.Assignment?.Replace(@"$this", model.Name) ?? model.Name;
            return output;
        }

        private static string RenderNonEventMethodArgument(EventArgumentModel model)
        {
            var output = EventSourceNonEventMethodTemplate.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(EventSourceNonEventMethodTemplate.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(EventSourceNonEventMethodTemplate.Template_ARGUMENT_CLR_TYPE, RenderEventSourceType(model) ?? model.CLRType ?? model.Type);
            return output;
        }

        protected string Render(EventModel model)
        {
            if (!model.HasComplexArguments)
            {
                return "";
            }

            var outputNonEventMethod = EventSourceNonEventMethodTemplate.Template_NON_EVENT_METHOD;
            outputNonEventMethod = outputNonEventMethod.Replace(EventSourceNonEventMethodTemplate.Variable_EVENT_NAME, model.Name);
            var methodArgumentsDeclarationBuilder = new EventArgumentsListBuilder(
                RenderNonEventMethodArgument, EventSourceNonEventMethodTemplate.Template_NONEVENT_METHOD_ARGUMENT_DELIMITER);
            var assignmentArgumentsBuilder = new EventArgumentsListBuilder(
                RenderAssignment, EventSourceNonEventMethodTemplate.Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER);

            foreach (var argument in model.GetAllArguments())
            {
                methodArgumentsDeclarationBuilder.Append(argument);
            }
            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                assignmentArgumentsBuilder.Append(argument);
            }
            outputNonEventMethod = outputNonEventMethod.Replace(EventSourceNonEventMethodTemplate.Variable_NON_EVENT_METHOD_ARGUMENTS, methodArgumentsDeclarationBuilder.ToString());
            outputNonEventMethod = outputNonEventMethod.Replace(EventSourceNonEventMethodTemplate.Variable_NON_EVENT_ASSIGNMENT_ARGUMENTS, assignmentArgumentsBuilder.ToString());

            return outputNonEventMethod;
        }
    }
}