using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventSourceSettings
    {
        public bool AutogenerateLoggerInterfaces { get; set; }
    }

    public class EventSourcePrototype
    {
        [JsonIgnore]
        public EventSourceLoggerTemplate[] AvailableLoggers { get; set; }
        public EventSourceLogger[] Loggers { get; set; }
        [JsonIgnore]
        public string Namespace { get; set; }
        [JsonIgnore]
        public string ClassName { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string SourceFilePath { get; set; }
        [JsonIgnore]
        public string Include { get; set; }
        public EventSourceSettings Settings { get; set; }
        public string[] Keywords { get; set; }

        public EventSourceTypeTemplate[] TypeTemplates { get; set; }

        public List<EventSourceExtensionsMethod> Extensions { get; private set; }

        public EventSourceEvent[] Events { get; set; }

        public EventSourcePrototype()
        {
            Extensions = new List<EventSourceExtensionsMethod>();
            Loggers = new EventSourceLogger[] {};
        }

        public IEnumerable<ProjectItem> Render(string projectBasePath)
        {
            var outputs = new List<ProjectItem>();

            var output = ClassTemplate;
            output = output.Replace(Variable_SOURCE_FILE_NAME, this.SourceFilePath);
            output = output.Replace(Variable_EVENTSOURCE_NAME, this.Name);
            output = output.Replace(Variable_EVENTSOURCE_CLASS_NAME, ClassName);
            output = output.Replace(Variable_NAMESPACE_DECLARATION, Namespace);

            var next = 1;
            var loggerStartId = 10000;
            var allKeywords = new List<string>();
            if (this.Keywords != null)
            {
                allKeywords.AddRange(this.Keywords);
            }

            foreach (var logger in this.Loggers)
            {
                var loggerTemplate = this.AvailableLoggers.FirstOrDefault(l => l.Name.Equals(logger.Name, StringComparison.InvariantCultureIgnoreCase));
                if (loggerTemplate == null)
                {
                    throw new NotSupportedException($"Logger {logger.Name} was not found in the project. Declare an interface with a matching name in a file with the same name as the interface.");
                }
                logger.AddTemplate(loggerTemplate);

                next = logger.StartId ?? loggerStartId;
                var eventSourcePartialFileInclude = this.Include.Replace(this.ClassName, $"{ClassName}.{logger.Name}");
                var eventSourcePartialFileName = System.IO.Path.Combine(projectBasePath, eventSourcePartialFileInclude);
                var loggerFileInclude = this.Include.Replace(this.ClassName, logger.Name.Substring(1));
                var loggerFileName = System.IO.Path.Combine(projectBasePath, loggerFileInclude);
                var loggerKeyword = logger.GetKeyword();
                if (!allKeywords.Contains(loggerKeyword))
                {
                    allKeywords.Add(loggerKeyword);
                }
                outputs.Add(new ProjectItem(ProjectItemType.EventSourceLoggerPartial, eventSourcePartialFileName,
                    logger.RenderPartial(this, next, System.IO.Path.GetFileName(eventSourcePartialFileName)))
                {
                    Include = eventSourcePartialFileInclude

                });
                outputs.Add(new ProjectItem(ProjectItemType.LoggerImplementation, loggerFileName,
                    logger.RenderImplementation(this, next, System.IO.Path.GetFileName(loggerFileName)))
                {
                    Include = loggerFileInclude
                });
                loggerStartId += 1000;
            }

            next = 1;
            var events = new StringBuilder();
            foreach (var eventSourceEvent in this?.Events ?? new EventSourceEvent[0])
            {
                if (eventSourceEvent.Id != null)
                {
                    next = eventSourceEvent.Id.Value;
                }
                events.AppendLine(eventSourceEvent.Render(next, this));
                next += 1;
            }
            output = output.Replace(Variable_EVENTS_DECLARATION, events.ToString());

            next = 1;
            var keywords = new StringBuilder();
            foreach (var keyword in allKeywords.Select(k => new EventSourceKeyword() { Name = k }))
            {
                keywords.AppendLine(keyword.Render(next));
                next *= 2;
            }
            output = output.Replace(Variable_KEYWORDS_DECLARATION, keywords.ToString());

            if (this.Extensions != null && this.Extensions.Any())
            {
                var extensions = new StringBuilder();
                foreach (var extension in this.Extensions)
                {
                    extensions.AppendLine(extension.Render());
                }

                var extensionMethod = Template_EXTENSIONS_DECLARATION;
                extensionMethod = extensionMethod.Replace(Variable_EVENTSOURCE_CLASS_NAME, this.ClassName);
                extensionMethod = extensionMethod.Replace(Variable_EXTENSION_METHODS_DECLARATION, extensions.ToString());

                output = output.Replace(Variable_EXTENSIONS_DECLARATION, extensionMethod);
            }
            else
            {
                output = output.Replace(Variable_EXTENSIONS_DECLARATION, "");

            }

            outputs.Add(new ProjectItem(ProjectItemType.EventSource, this.Include, output) { Include = this.Include});
            return outputs;
        }

        public override string ToString()
        {
            return $"{nameof(EventSourcePrototype)} {this.Name}";
        }

        // ReSharper disable InconsistentNaming
        private const string Variable_SOURCE_FILE_NAME = @"@@SOURCE_FILE_NAME@@";
        private const string Variable_EVENTSOURCE_NAME = @"@@EVENTSOURCE_NAME@@";
        private const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";
        private const string Variable_KEYWORDS_DECLARATION = @"@@KEYWORDS_DECLARATION@@";
        private const string Variable_EVENTS_DECLARATION = @"@@EVENTS_DECLARATION@@";
        private const string Variable_NAMESPACE_DECLARATION = @"@@NAMESPACE_DECLARATION@@";
        private const string Variable_EXTENSIONS_DECLARATION = @"@@EXTENSIONS_DECLARATION@@";
        private const string Template_EXTENSIONS_DECLARATION = @"
	internal static class @@EVENTSOURCE_CLASS_NAME@@Helpers
	{
@@EXTENSION_METHODS_DECLARATION@@
	}";
        private const string Variable_EXTENSION_METHODS_DECLARATION = @"@@EXTENSION_METHODS_DECLARATION@@";


        private const string ClassTemplate = @"/*******************************************************************************************
*  This class is autogenerated from the class @@SOURCE_FILE_NAME@@
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace @@NAMESPACE_DECLARATION@@
{
	[EventSource(Name = ""@@EVENTSOURCE_NAME@@"")]
	internal sealed partial class @@EVENTSOURCE_CLASS_NAME@@ : EventSource
	{
		public static readonly @@EVENTSOURCE_CLASS_NAME@@ Current = new @@EVENTSOURCE_CLASS_NAME@@();

		static @@EVENTSOURCE_CLASS_NAME@@()
		{
			// A workaround for the problem where ETW activities do not 
			// get tracked until Tasks infrastructure is initialized.
			// This problem will be fixed in .NET Framework 4.6.2.
			Task.Run(() => { });
		}

		// Instance constructor is private to enforce singleton semantics
		private @@EVENTSOURCE_CLASS_NAME@@() : base() { }

		#region Keywords
		// Event keywords can be used to categorize events. 
		// Each keyword is a bit flag. A single event can be 
		// associated with multiple keywords (via EventAttribute.Keywords property).
		// Keywords must be defined as a public class named 'Keywords' 
		// inside EventSource that uses them.
		public static class Keywords
		{
@@KEYWORDS_DECLARATION@@
		}
		#endregion Keywords

		#region Events

@@EVENTS_DECLARATION@@

		#endregion Events
	}

@@EXTENSIONS_DECLARATION@@

}";
        // ReSharper restore InconsistentNaming
    }
}