using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public class LoggerImplementationMethodAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationMethodRenderer
    {
        // ReSharper disable InconsistentNaming
        private const string Variable_LOGGER_METHOD_TRACKEVENT_NAME = @"@@LOGGER_METHOD_TRACKEVENT_NAME@@";
        private const string Variable_LOGGER_METHOD_TRACKEVENT_PROPERTIES_DECLARATION = @"@@LOGGER_METHOD_TRACKEVENT_PROPERTIES_DECLARATION@@";
        private const string Variable_LOGGER_METHOD_TRACKEVENT_PROPERTY_NAME = @"@@LOGGER_METHOD_TRACKOPERATION_PROPERTY_NAME@@";
        private const string Variable_LOGGER_METHOD_TRACKEVENT_PROPERTY_ASSIGNMENT = @"@@LOGGER_METHOD_TRACKOPERATION_PROPERTY_ASSIGNMENT@@";
        private const string Template_LOGGER_METHOD_TRACKEVENT_PROPERTY_DECLARATION = @"{""@@LOGGER_METHOD_TRACKOPERATION_PROPERTY_NAME@@"", @@LOGGER_METHOD_TRACKOPERATION_PROPERTY_ASSIGNMENT@@}";
        private const string Template_LOGGER_METHOD_TRACKEVENT_DECLARATION = @"			_telemetryClient.TrackEvent(
	            nameof(@@LOGGER_METHOD_TRACKEVENT_NAME@@),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                @@LOGGER_METHOD_TRACKEVENT_PROPERTIES_DECLARATION@@
	            });
";
        private const string Variable_LOGGER_METHOD_TRACKOPERATION_NAME = @"@@LOGGER_METHOD_TRACKOPERATION_NAME@@";
        private const string Variable_LOGGER_METHOD_TRACKOPERATION_PROPERTIES_DECLARATION = @"@@LOGGER_METHOD_TRACKOPERATION_PROPERTIES_DECLARATION@@";
        private const string Variable_LOGGER_METHOD_TRACKOPERATION_PROPERTY_NAME = @"@@LOGGER_METHOD_TRACKOPERATION_PROPERTY_NAME@@";
        private const string Variable_LOGGER_METHOD_TRACKOPERATION_PROPERTY_ASSIGNMENT = @"@@LOGGER_METHOD_TRACKOPERATION_PROPERTY_ASSIGNMENT@@";
        private const string Template_LOGGER_METHOD_TRACKOPERATION_PROPERTY_DECLARATION = @"_@@LOGGER_METHOD_TRACKOPERATION_NAME@@OperationHolder.Telemetry.Properties.Add(""@@LOGGER_METHOD_TRACKOPERATION_PROPERTY_NAME@@"", @@LOGGER_METHOD_TRACKOPERATION_PROPERTY_ASSIGNMENT@@);";
        private const string Template_LOGGER_METHOD_TRACKOPERATIONSTART_DECLARATION = @"            _@@LOGGER_METHOD_TRACKOPERATION_NAME@@OperationHolder = _telemetryClient.StartOperation<RequestTelemetry>(""@@LOGGER_METHOD_TRACKOPERATION_NAME@@"");
	       @@LOGGER_METHOD_TRACKOPERATION_PROPERTIES_DECLARATION@@
";
        private const string Template_LOGGER_METHOD_TRACKOPERATIONSTOP_DECLARATION = @"            _telemetryClient.StopOperation(_@@LOGGER_METHOD_TRACKOPERATION_NAME@@OperationHolder);
	            _@@LOGGER_METHOD_TRACKOPERATION_NAME@@OperationHolder.Dispose();
";

        private const string Variable_LOGGER_METHOD_TRACKEXCEPTION_EVENTNAME = @"@@LOGGER_METHOD_TRACKEXCEPTION_EVENTNAME@@";
        private const string Variable_LOGGER_METHOD_TRACKEXCEPTION_NAME = @"@@LOGGER_METHOD_TRACKEXCEPTION_NAME@@";
        private const string Variable_LOGGER_METHOD_TRACKEXCEPTION_EXCEPTION_NAME = @"@@LOGGER_METHOD_TRACKEXCEPTION_EXCEPTION_NAME@@";
        private const string Template_LOGGER_METHOD_TRACKEXCEPTION_DECLARATION = @"			_telemetryClient.TrackException(
	            @@LOGGER_METHOD_TRACKEXCEPTION_EXCEPTION_NAME@@,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { ""@@LOGGER_METHOD_TRACKEXCEPTION_EVENTNAME@@"", ""@@LOGGER_METHOD_TRACKEXCEPTION_NAME@@"" },
	                @@LOGGER_METHOD_TRACKEVENT_PROPERTIES_DECLARATION@@
	            });
";


        // ReSharper restore InconsistentNaming        

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            if (model.OpCode == EventOpcode.Start)
            {
                return RenderStartOperation(model);
            }
            else if (model.OpCode == EventOpcode.Stop)
            {
                return RenderStopOperation(model);
            }

            // TODO: This is a litle iffy. Fix the last part (fixable? I don't want to have this code know all types... How do we truly figure out if a CLR type name string is an exception?
            if (GetExceptionArgumentName(model, false) != null)
            {
                return RenderTrackException(model);
            }
            return RenderTrackEvent(model);
        }

        private string RenderStartOperation(EventModel model)
        {
            var operationName = GetEventOperationName(model);

            var output = Template_LOGGER_METHOD_TRACKOPERATIONSTART_DECLARATION;
            output = output.Replace(Variable_LOGGER_METHOD_TRACKOPERATION_NAME, operationName);

            var arguments = new EventArgumentsListBuilder("", arg => RenderDictionaryKeyValueAdd(arg, operationName), "\r\n			");
            foreach (var argumentModel in model.GetAllArgumentsExpanded(directArgumentAssignments: false))
            {
                arguments.Append(argumentModel);
            }
            output = output.Replace(Variable_LOGGER_METHOD_TRACKOPERATION_PROPERTIES_DECLARATION, arguments.ToString());

            return output;
        }

        private string RenderStopOperation(EventModel model)
        {
            var operationName = GetEventOperationName(model);

            var output = Template_LOGGER_METHOD_TRACKOPERATIONSTOP_DECLARATION;
            output = output.Replace(Variable_LOGGER_METHOD_TRACKOPERATION_NAME, operationName);
            
            return output;
        }

        private string RenderTrackEvent(EventModel model)
        {
            var output = Template_LOGGER_METHOD_TRACKEVENT_DECLARATION;
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEVENT_NAME, model.Name);

            var arguments = new EventArgumentsListBuilder("", RenderDictionaryKeyValue, ",\r\n                    ");
            foreach (var argumentModel in model.GetAllArgumentsExpanded(directArgumentAssignments: false))
            {
                arguments.Append(argumentModel);
            }
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEVENT_PROPERTIES_DECLARATION, arguments.ToString());

            return output;
        }

        private string RenderTrackException(EventModel model)
        {
            var exceptionName = GetExceptionArgumentName(model)?.Name;

            var propertyEventName = "Name";

            var output = Template_LOGGER_METHOD_TRACKEXCEPTION_DECLARATION;
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEXCEPTION_NAME, model.Name);
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEXCEPTION_EXCEPTION_NAME, exceptionName);

            var arguments = new EventArgumentsListBuilder("", RenderDictionaryKeyValue, ",\r\n                    ");
            var eventArgumentModels = model.GetAllArgumentsExpanded(directArgumentAssignments:false).ToArray();
            foreach (var argumentModel in eventArgumentModels)
            {
                if (argumentModel.Name == propertyEventName)
                {
                    propertyEventName = null;
                }
                arguments.Append(argumentModel);
            }
            var nameIndex = 1;
            while (eventArgumentModels.Any(a => a.Name == propertyEventName))
            {
                propertyEventName = $"Name{nameIndex}";
                nameIndex++;
            }

            output = output.Replace(Variable_LOGGER_METHOD_TRACKEXCEPTION_EVENTNAME, propertyEventName);
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEVENT_PROPERTIES_DECLARATION, arguments.ToString());

            return output;
        }

        private KeyValuePair<string, string> CreateDictionaryKeyValue(EventArgumentModel model)
        {
            var variable = model.Name;
            if (model.IsImplicit)
            {
                variable = $"_{model.Name}";
            }

            var assignment = variable;
            var assignmentType = model.Type;
            if (model.Assignment != null)
            {
                assignment = model.Assignment.Replace("$this", variable);

                assignmentType = model.AssignedCLRType;
            }
            if ((assignmentType != null) && (EventArgumentModel.ParseType(assignmentType) != typeof(string)))
            {
                assignment = $"{assignment}.ToString()";
            }

            var keyOutput = $"{model.Name.Substring(0, 1).ToUpperInvariant()}{model.Name.Substring(1)}";

            return new KeyValuePair<string, string>(keyOutput, assignment);
        }

        private string RenderDictionaryKeyValueAdd(EventArgumentModel model, string operationName)
        {
            var value = CreateDictionaryKeyValue(model);

            var output = Template_LOGGER_METHOD_TRACKOPERATION_PROPERTY_DECLARATION;
            output = output.Replace(Variable_LOGGER_METHOD_TRACKOPERATION_NAME, operationName);
            output = output.Replace(Variable_LOGGER_METHOD_TRACKOPERATION_PROPERTY_NAME, value.Key);
            output = output.Replace(Variable_LOGGER_METHOD_TRACKOPERATION_PROPERTY_ASSIGNMENT, value.Value);

            return output;
        }

        private string RenderDictionaryKeyValue(EventArgumentModel model)
        {
            var value = CreateDictionaryKeyValue(model);

            var output = Template_LOGGER_METHOD_TRACKEVENT_PROPERTY_DECLARATION;
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEVENT_PROPERTY_NAME, value.Key);
            output = output.Replace(Variable_LOGGER_METHOD_TRACKEVENT_PROPERTY_ASSIGNMENT, value.Value);

            return output;
        }

        private EventArgumentModel GetExceptionArgumentName(EventModel model, bool warn = true)
        {
            var exceptionArgument = (EventArgumentModel) null;
            foreach (var argument in model.Arguments)
            {
                if (argument.CLRType.Contains("Exception"))
                {
                    if (exceptionArgument != null)
                    {
                        if( warn)
                            LogWarning($"Found multiple possible Exception arguments in event {model?.Name}");
                    }
                    exceptionArgument = argument;
                }
            }

            if (exceptionArgument == null)
            {
                if (warn)
                    LogWarning($"Could not find Exception argument in event {model?.Name}");
            }
            return exceptionArgument;
        }
    }
}