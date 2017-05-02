using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace ConsoleApplication1.Extensions
{
    public class LoggerImplementationDebugWriteRendererBuilderExtensionL : ILoggerImplementationEventRenderer
    {
        private readonly Regex _eventOperationNameRegex = new Regex("start", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            if (model.OpCode == EventOpcode.Start)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"		private System.Diagnostics.Stopwatch _@@LOGGER_EVENT_OPERATION_NAME@@Stopwatch = new System.Diagnostics.Stopwatch();
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;
            }
            return "";
        }

        private string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = eventOperationName.Substring(0, 1).ToLowerInvariant() + eventOperationName.Substring(1);
            return eventOperationName;
        }
    }

    public class LoggerImplementationMethodOperationDebugWriteRendererBuilderExtension : ILoggerImplementationMethodRenderer
    {
        private readonly Regex _eventOperationNameRegex = new Regex("start|stop", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = eventOperationName.Substring(0, 1).ToLowerInvariant() + eventOperationName.Substring(1);
            return eventOperationName;
        }

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {

            if (model.OpCode == EventOpcode.Stop)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"			_@@LOGGER_EVENT_OPERATION_NAME@@Stopwatch.Stop();
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;
            }
            else if( model.OpCode == EventOpcode.Start)
            {
                var eventOperationName = GetEventOperationName(model);
                var output = @"			_@@LOGGER_EVENT_OPERATION_NAME@@Stopwatch.Restart();
";
                output = output.Replace("@@LOGGER_EVENT_OPERATION_NAME@@", eventOperationName);

                return output;

            }
            return "";
        }
    }

    public class LoggerImplementationMethodDebugWriteRendererBuilderExtension : ILoggerImplementationMethodRenderer
    {
        // ReSharper disable InconsistentNaming
        private const string Variable_LOGGER_METHOD_IMPLEMENTATION_KEYWORDS = @"@@LOGGER_METHOD_IMPLEMENTATION_KEYWORDS@@";
        private const string Variable_LOGGER_METHOD_IMPLEMENTATION_LEVEL = @"@@LOGGER_METHOD_IMPLEMENTATION_LEVEL@@";
        private const string Variable_LOGGER_METHOD_IMPLEMENTATION_NAME = @"@@LOGGER_METHOD_IMPLEMENTATION_NAME@@";
        private const string Variable_LOGGER_METHOD_IMPLEMENTATION_ARGUMENTS = @"@@LOGGER_METHOD_IMPLEMENTATION_ARGUMENTS@@";
        private const string Template_LOGGER_METHOD_IMPLEMENTATION = @"
			System.Diagnostics.Debug.WriteLine($""[@@LOGGER_METHOD_IMPLEMENTATION_KEYWORDS@@] @@LOGGER_METHOD_IMPLEMENTATION_LEVEL@@: @@LOGGER_METHOD_IMPLEMENTATION_NAME@@"");
           @@LOGGER_METHOD_IMPLEMENTATION_ARGUMENTS@@
";
        private const string Variable_LOGGER_METHOD_IMPLEMENTATION_ARGUMENT_NAME = @"@@LOGGER_METHOD_IMPLEMENTATION_ARGUMENT_NAME@@";
        private const string Template_LOGGER_METHOD_IMPLEMENTATION_ARGUMENT = @"
			System.Diagnostics.Debug.WriteLine($""\t@@LOGGER_METHOD_IMPLEMENTATION_ARGUMENT_NAME@@:\t{@@LOGGER_METHOD_IMPLEMENTATION_ARGUMENT_NAME@@}"");";
        // ReSharper restore InconsistentNaming

        private readonly Regex _eventOperationNameRegex = new Regex("stop", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string GetEventOperationName(EventModel model)
        {
            var eventOperationName = _eventOperationNameRegex.Replace(model.Name, "");
            eventOperationName = eventOperationName.Substring(0, 1).ToLowerInvariant() + eventOperationName.Substring(1);
            return eventOperationName;
        }

        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            var output = Template_LOGGER_METHOD_IMPLEMENTATION;
            output = output.Replace(Variable_LOGGER_METHOD_IMPLEMENTATION_NAME, model.Name);

            // ReSharper disable UseStringInterpolation - don't compile C# 6 in extensions
            var keywords = model.Keywords == null ? "" : model.Keywords.Select(k => k.Name).Aggregate("", (s, s1) => string.Format("{0}{1}", (s == "" ? "" : string.Format("{0}, ", s)), s1));
            // ReSharper restore UseStringInterpolation
            output = output.Replace(Variable_LOGGER_METHOD_IMPLEMENTATION_KEYWORDS, keywords);

            var level = model.Level <= EventLevel.Error ? "ERR" : model.Level == EventLevel.Warning ? "WARN" : "INFO";
            output = output.Replace(Variable_LOGGER_METHOD_IMPLEMENTATION_LEVEL, level);

            var arguments = new StringBuilder();
            foreach (var argumentModel in model.GetAllArgumentsExpanded())
            {
                // ReSharper disable UseStringInterpolation - don't compile C# 6 in extensions
                var argumentName = string.Format("{0}{1}", (argumentModel.IsImplicit ? "_" : ""), argumentModel.Name);
                var argumentAssignmentOutput = argumentModel.Assignment != null ?  argumentModel.Assignment.Replace(@"$this", argumentName) : argumentName;
                // ReSharper restore UseStringInterpolation
                var argumentOutput = Template_LOGGER_METHOD_IMPLEMENTATION_ARGUMENT;
                argumentOutput = argumentOutput.Replace(Variable_LOGGER_METHOD_IMPLEMENTATION_ARGUMENT_NAME, argumentAssignmentOutput);
                arguments.Append(argumentOutput);
            }
            output = output.Replace(Variable_LOGGER_METHOD_IMPLEMENTATION_ARGUMENTS, arguments.ToString());

            return output;
        }
    }
}