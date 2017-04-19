using System;
using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class LoggerEventRenderer : IEventRenderer
    {
        public string Render(EventModel model, int index, EventSourcePrototype eventSource)
        {
            var output = Template.Template_LOGGER_METHOD;
            output = output.Replace(Template.Variable_LOGGER_METHOD_NAME, model.Name);
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSource.ClassName);

            var next = 0;

            var methodArguments = new StringBuilder();
            var methodArgumentDelimiter = "";

            var callArguments = new StringBuilder();
            var callArgumentDelimiter = "";

            foreach (var argument in model?.ImplicitArguments ?? new EventArgumentModel[0])
            {
                callArguments.Append($"{callArgumentDelimiter}{argument.RenderWriteEventMethodCallArgument(isPrivateMember: true)}");
                callArgumentDelimiter = Template.Template_LOGGER_CALL_ARGUMENTS_DELIMITER;

                next += 1;
            }

            foreach (var argument in model.Arguments)
            {
                var type = argument.Type;
                if (argument.IsComplexType())
                {
                    var typeTemplate = eventSource.TypeTemplates.FirstOrDefault(t =>
                        t.Name.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase) ||
                        t.CLRType.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase));
                    if (typeTemplate != null)
                    {
                        type = typeTemplate.CLRType;
                    }
                }

                methodArguments.Append($"{methodArgumentDelimiter}{argument.RenderMethodArgument()}");
                methodArgumentDelimiter = Template.Template_LOGGER_METHOD_ARGUMENTS_DELIMITER;

                callArguments.Append($"{callArgumentDelimiter}{argument.RenderWriteEventMethodCallArgument(isPrivateMember: false)}");
                callArgumentDelimiter = Template.Template_LOGGER_CALL_ARGUMENTS_DELIMITER;

                next += 1;
            }
            output = output.Replace(Template.Variable_LOGGER_METHOD_ARGUMENTS, methodArguments.ToString());
            output = output.Replace(Template.Variable_LOGGER_CALL_ARGUMENTS, callArguments.ToString());

            foreach (var builderExtension in eventSource.BuilderExtensions)
            {
                //builderExtension.OnEventRendered()
            }

            return output;
        }
    }
}