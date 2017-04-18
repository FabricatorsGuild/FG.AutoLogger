using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceNonEventMethodBaseRenderer : EventMethodBaseRenderer
    {
        protected string Render(EventModel model)
        {
            if (!model.HasComplexArguments)
            {
                return "";
            }

            var outputNonEventMethod = Template.Template_NON_EVENT_METHOD;
            outputNonEventMethod = outputNonEventMethod.Replace(Template.Variable_EVENT_NAME, model.Name);
            var methodArgumentsDeclarationBuilder = new EventArgumentsListBuilder(
                RenderMethodArgument, Template.Template_NONEVENT_METHOD_ARGUMENT_DELIMITER);
            var assignmentArgumentsBuilder = new EventArgumentsListBuilder(
                RenderAssignment, Template.Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER);

            foreach (var argument in model.GetAllArguments())
            {
                methodArgumentsDeclarationBuilder.Append(argument);
            }
            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                assignmentArgumentsBuilder.Append(argument);
            }
            outputNonEventMethod = outputNonEventMethod.Replace(Template.Variable_NON_EVENT_METHOD_ARGUMENTS, methodArgumentsDeclarationBuilder.ToString());
            outputNonEventMethod = outputNonEventMethod.Replace(Template.Variable_NON_EVENT_ASSIGNMENT_ARGUMENTS, assignmentArgumentsBuilder.ToString());

            return outputNonEventMethod;
        }
    }
}