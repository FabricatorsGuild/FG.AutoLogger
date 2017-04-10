using System;
using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventRenderer : IEventRenderer
    {
        public string Render(EventModel model, int index, EventSourcePrototype eventSource)
        {
            var outputEventMethod = Template.Template_EVENT_METHOD;
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_NAME, model.Name);
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_ID, index.ToString());
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

            var next = 0;

            var messageFormatBuilder = new ListBuilder($"{model.Name.GetHumanReadable()}", " ", " ");
            var eventMethodArgumentsDeclarationBuilder = new EventArgumentsListBuilder((arg) => arg.RenderMethodArgument(), Template.Template_EVENT_METHOD_ARGUMENT_DELIMITER);
            var nonEventMethodArgumentsDeclarationBuilder = new EventArgumentsListBuilder((arg) => arg.RenderMethodArgument(), Template.Template_NONEVENT_METHOD_ARGUMENT_DELIMITER);
            var callArgumentsBuilder = new EventArgumentsListBuilder((arg) => arg.RenderWriteEventMethodCallArgument(), Template.Template_EVENT_METHOD_CALL_ARGUMENT_DELIMITER);
            var assignmentArgumentsBuilder = new EventArgumentsListBuilder((arg) => (arg as EventSourceEventCustomArgument)?.Assignment.Replace(@"$this", arg.Name) ?? arg.Name, Template.Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER);

            var hasComplexArguments = false;

            var allArguments = new List<EventArgumentModel>();
            var messageArgumentsStart = 0;
            var messageArgumentsCount = 0;
            if (model.ImplicitArguments != null && model.ImplicitArguments.Length > 0)
            {
                allArguments.AddRange(model.ImplicitArguments);
                messageArgumentsStart += model.ImplicitArguments.Length;
            }
            allArguments.AddRange(model.Arguments);

            var messageArgument = allArguments.FirstOrDefault(arg => model.Name.Equals("message", StringComparison.InvariantCultureIgnoreCase));
            var messageArgumentIndex = messageArgument != null ? allArguments.IndexOf(messageArgument) : -1;


            var flatIndex = 0;
            foreach (var argument in allArguments)
            {
                argument.SetCLRType(eventSource);

                var methodArgument = "";

                if (argument is EventSourceEventCustomArgument)
                {
                    methodArgument = argument.RenderMethodArgument();
                    if (flatIndex >= messageArgumentsStart) messageFormatBuilder.Append($"{{{next}}}");
                    var customArgument = argument as EventSourceEventCustomArgument;

                    assignmentArgumentsBuilder.Append(customArgument);
                    eventSource.AddKnownExtensions(customArgument, customArgument.CLRType);

                    var originalCLRType = customArgument.CLRType;
                    customArgument.CLRType = customArgument.AssignedCLRType ?? customArgument.CLRType;

                    eventMethodArgumentsDeclarationBuilder.Append(customArgument);
                    callArgumentsBuilder.Append(customArgument);
                    customArgument.CLRType = originalCLRType;

                    next += 1;

                }
                else if (!argument.IsComplexType())
                {
                    // Just render the argument as it is
                    methodArgument = argument.RenderMethodArgument();
                    if (flatIndex >= messageArgumentsStart) messageFormatBuilder.Append($"{{{next}}}");

                    assignmentArgumentsBuilder.Append(argument);
                    eventMethodArgumentsDeclarationBuilder.Append(argument);
                    callArgumentsBuilder.Append(argument);

                    next += 1;
                }
                else
                {
                    // Split the argument according to a template
                    hasComplexArguments = true;
                    var template = eventSource.TypeTemplates.GeTypeTemplate(argument.Type);
                    if (template != null)
                    {
                        methodArgument = argument.RenderMethodArgument();

                        foreach (var customArgument in template.Arguments)
                        {
                            if (flatIndex >= messageArgumentsStart) messageFormatBuilder.Append($"{{{next}}}");
                            customArgument.SetCLRType(eventSource);

                            assignmentArgumentsBuilder.Append(customArgument);
                            eventMethodArgumentsDeclarationBuilder.Append(customArgument);
                            callArgumentsBuilder.Append(customArgument);

                            next += 1;
                        }
                    }
                    else
                    {
                        // Render it as ToString()
                        methodArgument = argument.RenderMethodPureArgument();
                        if (flatIndex >= messageArgumentsStart) messageFormatBuilder.Append($"{{{next}}}");

                        assignmentArgumentsBuilder.Append($"{argument.Name}.ToString()");
                        eventMethodArgumentsDeclarationBuilder.Append(argument.RenderMethodArgument(useSimpleTypesOnly: true));
                        callArgumentsBuilder.Append(argument);

                        next += 1;
                    }
                }

                nonEventMethodArgumentsDeclarationBuilder.Append(methodArgument);

                flatIndex++;
            }
            outputNonEventMethod = outputNonEventMethod.Replace(Template.Variable_NON_EVENT_METHOD_ARGUMENTS, nonEventMethodArgumentsDeclarationBuilder.ToString());
            outputNonEventMethod = outputNonEventMethod.Replace(Template.Variable_NON_EVENT_ASSIGNMENT_ARGUMENTS, assignmentArgumentsBuilder.ToString());

            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_METHOD_ACCESS, hasComplexArguments ? "private" : "public");
            outputEventMethod = outputEventMethod.Replace(Template.Variable_EVENT_METHOD_ARGUMENTS, eventMethodArgumentsDeclarationBuilder.ToString());
            outputEventMethod = outputEventMethod.Replace(Template.Variable_WRITEEVENT_CALL_ARGUMENTS, callArgumentsBuilder.ToString());

            var nonEventMethodDeclaration = hasComplexArguments ? outputNonEventMethod : "";
            outputEventMethod = outputEventMethod.Replace(Template.Variable_NON_EVENT_METHOD_DECLARATION, nonEventMethodDeclaration);

            var formatter = $"{{{next - 1}}}";
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

            //var formatter = messageFormatBuilder.ToString();
            //output = output.Replace(Template_EVENT_METHOD_ARGUMENTS, MessageFormatter ?? "{7}");

            return outputEventMethod;
        }
    }
}