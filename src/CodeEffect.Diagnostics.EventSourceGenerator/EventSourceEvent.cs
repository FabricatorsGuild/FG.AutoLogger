using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventSourceEvent
    {
        private HashSet<string> _implicitArgumentsAdded = new HashSet<string>();

        private EventSourceEventArgument[] ImplicitArguments { get; set; }

        public int? Id { get; set; }
        public string Name { get; set; }
        public EventSourceEventArgument[] Arguments { get; set; }
        public System.Diagnostics.Tracing.EventLevel Level { get; set; }
        public string[] Keywords { get; set; }
        public string MessageFormatter { get; set; }

        // ReSharper disable InconsistentNaming
        public const string Variable_EVENT_NAME = @"@@EVENT_NAME@@";
        public const string Variable_EVENT_ID = @"@@EVENT_ID@@";
        public const string Variable_EVENT_LEVEL = @"@@EVENT_LEVEL@@";
        public const string Variable_EVENT_KEYWORDS_DECLARATION = @"@@EVENT_KEYWORDS_DECLARATION@@";
        public const string Variable_EVENT_MESSAGE_FORMATTER = @"@@EVENT_MESSAGE_FORMATTER@@";
        public const string Variable_EVENT_METHOD_ACCESS = @"@@EVENT_METHOD_ACCESS@@";
        public const string Variable_EVENT_METHOD_ARGUMENTS = @"@@EVENT_METHOD_ARGUMENTS@@";
        public const string Template_EVENT_METHOD_ARGUMENT_DELIMITER = @", 
			";
        public const string Template_EVENT_METHOD_CALL_ARGUMENT_DELIMITER = @", 
				";
        public const string Variable_WRITEEVENT_CALL_ARGUMENTS = @"@@WRITEEVENT_CALL_ARGUMENTS@@";

        public const string Template_EVENT_METHOD = @"
        @@NON_EVENT_METHOD_DECLARATION@@
		private const int @@EVENT_NAME@@EventId = @@EVENT_ID@@;

		[Event(@@EVENT_NAME@@EventId, Level = EventLevel.@@EVENT_LEVEL@@, Message = ""@@EVENT_MESSAGE_FORMATTER@@""@@EVENT_KEYWORDS_DECLARATION@@)]
		@@EVENT_METHOD_ACCESS@@ void @@EVENT_NAME@@(
			@@EVENT_METHOD_ARGUMENTS@@)
		{
			WriteEvent(
				@@EVENT_NAME@@EventId,
				@@WRITEEVENT_CALL_ARGUMENTS@@);
		}";

        public const string Variable_NON_EVENT_METHOD_DECLARATION = @"@@NON_EVENT_METHOD_DECLARATION@@";
        public const string Variable_NON_EVENT_METHOD_ARGUMENTS = @"@@NON_EVENT_METHOD_ARGUMENTS@@";
        public const string Template_NONEVENT_METHOD_ARGUMENT_DELIMITER = @", 
			";
        public const string Variable_NON_EVENT_ASSIGNMENT_ARGUMENTS = @"@@NON_EVENT_ASSIGNMENT_ARGUMENTS@@";
        public const string Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER = @", 
					";

        public const string Template_NON_EVENT_METHOD = @"
		[NonEvent]
		public void @@EVENT_NAME@@(
			@@NON_EVENT_METHOD_ARGUMENTS@@)
		{
			if (this.IsEnabled())
			{
				@@EVENT_NAME@@(
					@@NON_EVENT_ASSIGNMENT_ARGUMENTS@@);
			}
		}
";

        public const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";

        public const string Variable_LOGGER_METHOD_NAME = @"@@LOGGER_METHOD_NAME@@";
        public const string Variable_LOGGER_METHOD_ARGUMENTS = @"@@LOGGER_METHOD_ARGUMENTS@@";
        public const string Template_LOGGER_METHOD_ARGUMENTS_DELIMITER = @", 
			";
        public const string Variable_LOGGER_CALL_ARGUMENTS = @"@@LOGGER_EVENTSOURCE_NONEVENT_METHOD_ARGUMENTS@@";
        public const string Template_LOGGER_CALL_ARGUMENTS_DELIMITER = @", 
				";

        public const string Template_LOGGER_METHOD = @"
		public void @@LOGGER_METHOD_NAME@@(
			@@LOGGER_METHOD_ARGUMENTS@@)
		{

			@@EVENTSOURCE_CLASS_NAME@@.Current.@@LOGGER_METHOD_NAME@@(
				@@LOGGER_EVENTSOURCE_NONEVENT_METHOD_ARGUMENTS@@
			);
		}
";
        // ReSharper restore InconsistentNaming


        public string RenderLogger(int index, EventSourcePrototype eventSource)
        {
            var output = Template_LOGGER_METHOD;
            output = output.Replace(Variable_LOGGER_METHOD_NAME, this.Name);
            output = output.Replace(Variable_EVENTSOURCE_CLASS_NAME, eventSource.ClassName);

            var next = 0;

            var methodArguments = new StringBuilder();
            var methodArgumentDelimiter = "";

            var callArguments = new StringBuilder();
            var callArgumentDelimiter = "";

            foreach (var argument in this?.ImplicitArguments ?? new EventSourceEventArgument[0])
            {
                callArguments.Append($"{callArgumentDelimiter}{argument.RenderWriteEventMethodCallArgument(isPrivateMember:true)}");
                callArgumentDelimiter = Template_LOGGER_CALL_ARGUMENTS_DELIMITER;

                next += 1;
            }

            foreach (var argument in this.Arguments)
            {
                var type = argument.Type;
                if (argument.IsComplexType())
                {
                    var template = eventSource.TypeTemplates.FirstOrDefault(t =>
                        t.Name.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase) ||
                        t.CLRType.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase));
                    if (template != null)
                    {
                        type = template.CLRType;
                    }
                }

                methodArguments.Append($"{methodArgumentDelimiter}{argument.RenderMethodArgument()}");
                methodArgumentDelimiter = Template_LOGGER_METHOD_ARGUMENTS_DELIMITER;

                callArguments.Append($"{callArgumentDelimiter}{argument.RenderWriteEventMethodCallArgument(isPrivateMember: false)}");
                callArgumentDelimiter = Template_LOGGER_CALL_ARGUMENTS_DELIMITER;

                next += 1;
            }
            output = output.Replace(Variable_LOGGER_METHOD_ARGUMENTS, methodArguments.ToString());
            output = output.Replace(Variable_LOGGER_CALL_ARGUMENTS, callArguments.ToString());

            return output;
        }

        public string Render(int index, EventSourcePrototype eventSource)
        {
            var outputEventMethod = Template_EVENT_METHOD;
            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_NAME, this.Name);
            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_ID, index.ToString());
            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_LEVEL, Level.ToString());

            var outputNonEventMethod = Template_NON_EVENT_METHOD;
            outputNonEventMethod = outputNonEventMethod.Replace(Variable_EVENT_NAME, this.Name);

            var keywords = new StringBuilder();
            var keywordDelimiter = "";
            foreach (var keyword in Keywords ?? new string[] { })
            {
                keywords.Append($"{keywordDelimiter}Keywords.{keyword}");
                keywordDelimiter = " | ";
            }
            var keywordsDeclaration = (keywords.Length > 0) ? $", Keywords = {keywords}" : "";
            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_KEYWORDS_DECLARATION, keywordsDeclaration);

            var next = 0;

            var messageFormatBuilder = new ArgumentBuilder($"{this.Name.GetHumanReadable()}", " ", " ");
            var eventMethodArgumentsDeclarationBuilder = new ArgumentBuilder((arg) => arg.RenderMethodArgument(), Template_EVENT_METHOD_ARGUMENT_DELIMITER);
            var nonEventMethodArgumentsDeclarationBuilder = new ArgumentBuilder((arg) => arg.RenderMethodArgument(), Template_NONEVENT_METHOD_ARGUMENT_DELIMITER);
            var callArgumentsBuilder = new ArgumentBuilder((arg) => arg.RenderWriteEventMethodCallArgument(), Template_EVENT_METHOD_CALL_ARGUMENT_DELIMITER);            
            var assignmentArgumentsBuilder = new ArgumentBuilder((arg) => (arg as EventSourceEventCustomArgument)?.Assignment.Replace(@"$this", arg.Name) ?? arg.Name, Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER);

            var hasComplexArguments = false;

            var allArguments = new List<EventSourceEventArgument>();
            var messageArgumentsStart = 0;
            var messageArgumentsCount = 0;
            if (this.ImplicitArguments != null && this.ImplicitArguments.Length > 0)
            {
                allArguments.AddRange(this.ImplicitArguments);
                messageArgumentsStart += this.ImplicitArguments.Length;
            }
            allArguments.AddRange(this.Arguments);

            var messageArgument = allArguments.FirstOrDefault(arg => Name.Equals("message", StringComparison.InvariantCultureIgnoreCase));
            var messageArgumentIndex = messageArgument != null ? allArguments.IndexOf(messageArgument) : -1;


            var flatIndex = 0;
            foreach (var argument in allArguments)
            {
                argument.SetCLRType(eventSource);

                var methodArgument = "";

                if (argument is EventSourceEventCustomArgument)
                {
                    methodArgument = argument.RenderMethodArgument();
                    if(flatIndex >= messageArgumentsStart) messageFormatBuilder.Append($"{{{next}}}");
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
            outputNonEventMethod = outputNonEventMethod.Replace(Variable_NON_EVENT_METHOD_ARGUMENTS, nonEventMethodArgumentsDeclarationBuilder.ToString());
            outputNonEventMethod = outputNonEventMethod.Replace(Variable_NON_EVENT_ASSIGNMENT_ARGUMENTS, assignmentArgumentsBuilder.ToString());

            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_METHOD_ACCESS, hasComplexArguments ? "private" : "public");
            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_METHOD_ARGUMENTS, eventMethodArgumentsDeclarationBuilder.ToString());
            outputEventMethod = outputEventMethod.Replace(Variable_WRITEEVENT_CALL_ARGUMENTS, callArgumentsBuilder.ToString());

            var nonEventMethodDeclaration = hasComplexArguments ? outputNonEventMethod : "";
            outputEventMethod = outputEventMethod.Replace(Variable_NON_EVENT_METHOD_DECLARATION, nonEventMethodDeclaration);

            var formatter = $"{{{next - 1}}}";
            if (MessageFormatter != null)
            {
                formatter = MessageFormatter;
            }
            else if (messageArgumentIndex >= 0)
            {
                formatter = $"{{{messageArgumentIndex}}}";
            }
            else
            { 
                formatter = messageFormatBuilder.ToString();
            }
            outputEventMethod = outputEventMethod.Replace(Variable_EVENT_MESSAGE_FORMATTER, formatter);

            //var formatter = messageFormatBuilder.ToString();
            //output = output.Replace(Template_EVENT_METHOD_ARGUMENTS, MessageFormatter ?? "{7}");

            return outputEventMethod;
        }

        public void InsertImplicitArguments(EventSourceEventArgument[] implicitArguments)
        {
            var hash = implicitArguments.Aggregate("", (a, i) => $"{a}{i.Name}");
            if(_implicitArgumentsAdded.Contains(hash)) return;            

            this.ImplicitArguments = implicitArguments.ToArray();

            _implicitArgumentsAdded.Add(hash);
        }

        public void OverrideArguments(EventSourceEventCustomArgument[] overrideArguments)
        {
            var index = 0;
            foreach (var argument in this.Arguments)
            {
                var signature = $"{this.Name}.{argument.Name}";

                var overrideArgument = overrideArguments.FirstOrDefault(a => a.Name.Equals(signature, StringComparison.InvariantCulture));
                if (overrideArgument != null)
                {
                    this.Arguments[index] = new EventSourceEventCustomArgument(argument.Name, argument.Type, overrideArgument.Assignment)
                    {
                        CLRType = overrideArgument.CLRType ?? argument.CLRType
                    };
                }
                index++;
            }
        }



        public override string ToString()
        {
            return $"{nameof(EventSourceEvent)} {this.Name}";
        }
    }

    internal class ArgumentBuilder
    {
        private readonly Func<EventSourceEventArgument, string> _renderer;
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly string _delimiter;
        private string _currentDelimiter;

        public ArgumentBuilder(string initialContent = "", string delimiter = "", string initialDelimiter = "")
            : this(initialContent, (arg) => arg.Name, delimiter, initialDelimiter)
        {
        }
        public ArgumentBuilder(Func<EventSourceEventArgument, string> renderer, string delimiter, string initialDelimiter = "")
            : this("", renderer, delimiter, initialDelimiter)
        {
        }
        public ArgumentBuilder(string initialContent, Func<EventSourceEventArgument, string> renderer, string delimiter, string initialDelimiter = "")
        {
            _renderer = renderer;
            _delimiter = delimiter;
            _currentDelimiter = initialDelimiter;

            _builder.Append(initialContent);
        }
        public void Append(EventSourceEventArgument argument)
        {
            var renderedArgument = _renderer(argument);
            _builder.Append($"{_currentDelimiter}{renderedArgument}");
            _currentDelimiter = _delimiter;
        }

        public void Append(string argument)
        {
            _builder.Append($"{_currentDelimiter}{argument}");
            _currentDelimiter = _delimiter;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}