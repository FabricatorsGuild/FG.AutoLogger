using System;
using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventMethodRenderer : BaseWithLogging, IEventRenderer
    {
        private static string RenderCLRType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                    return @"string";
                case ("int"):
                case ("system.int32"):
                    return @"int";
                case ("long"):
                case ("system.int64"):
                    return @"long";
                case ("bool"):
                case ("system.boolean"):
                    return @"bool";
                case ("datetime"):
                case ("system.dateTime"):
                    return @"DateTime";
                case ("guid"):
                case ("system.guid"):
                    return @"Guid";
                default:
                    return @"string";
            }
        }

        private string RenderWriteEventMethodCallArgument(EventArgumentModel model, bool isPrivateMember = false)
        {
            var output = isPrivateMember ? Template.Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT : Template.Template_METHOD_CALL_PASSTHROUGH_ARGUMENT;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            return output;
        }

        private string RenderMethodArgument(EventArgumentModel model)
        {
            var output = Template.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(Template.Template_ARGUMENT_CLR_TYPE, RenderCLRType(model.CLRType));
            return output;
        }


        public string Render(Project project, EventSourceModel eventSource, EventModel model)
        {
            var outputEventMethod = Template.Template_EVENT_METHOD;
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_NAME, model.Name);
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_ID, model.Id.ToString());
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_LEVEL, model.Level.ToString());

            var outputNonEventMethod = Template.Template_NON_EVENT_METHOD;
            outputNonEventMethod = outputNonEventMethod.Replace(Template.Variable_EVENT_NAME, model.Name);

            var keywords = new ListBuilder("", " | ", "");
            foreach (var keyword in model.Keywords ?? new KeywordModel[] { })
            {
                keywords.Append($"Keywords.{keyword.Name}");
            }
            var keywordsDeclaration = (keywords.Length > 0) ? $", Keywords = {keywords}" : "";
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_KEYWORDS_DECLARATION, keywordsDeclaration);


            var messageFormatBuilder = new ListBuilder($"{model.Name.GetHumanReadable()}", " ", " ");
            var messageArgumentIndex = -1;

            var eventMethodArgumentsDeclarationBuilder = new EventArgumentsListBuilder(
                (arg) => RenderMethodArgument(arg), Template.Template_EVENT_METHOD_ARGUMENT_DELIMITER);
            var writeEventMethodCallArgument = new EventArgumentsListBuilder(
                (arg) => RenderWriteEventMethodCallArgument(arg), Template.Template_EVENT_METHOD_CALL_ARGUMENT_DELIMITER);

            var next = 0;
            var hasComplexArguments = false;
            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                hasComplexArguments = hasComplexArguments || argument.IsTemplated;
                if (argument.Name.Equals("message", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageArgumentIndex = next;
                }
                if (!argument.IsImplicit)
                {
                    messageFormatBuilder.Append($"{{{next}}}");
                }

                eventMethodArgumentsDeclarationBuilder.Append(argument);
                writeEventMethodCallArgument.Append(argument);
                next++;
            }
            
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_METHOD_ACCESS, hasComplexArguments ? "private" : "public");
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_METHOD_ARGUMENTS, eventMethodArgumentsDeclarationBuilder.ToString());
            outputEventMethod = outputEventMethod.Replace(Template.Variable_WRITEEVENT_CALL_ARGUMENTS, writeEventMethodCallArgument.ToString());

            string formatter;
            if (model.MessageFormatter != null)
            {
                formatter = model.MessageFormatter;
            }
            else if (messageArgumentIndex >= 0)
            {
                formatter = $"{{{messageArgumentIndex}}}";
            }
            else
            {
                formatter = messageFormatBuilder.ToString();
            }
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_MESSAGE_FORMATTER, formatter);
            
            return outputEventMethod;
        }
    }
}