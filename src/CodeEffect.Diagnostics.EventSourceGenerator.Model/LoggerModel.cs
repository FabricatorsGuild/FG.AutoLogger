using System.Collections.Generic;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Model
{
    public class LoggerModel
    {
        private EventArgumentModel[] _implicitArguments;
        private EventArgumentModel[] _overrideArguments;
        private EventModel[] _events;

        [JsonIgnore]
        public bool AutoDiscovered { get; set; }
        [JsonIgnore]
        public string SourceFileName { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string ClassName { get; set; }
        [JsonIgnore]
        public string Keyword { get; set; }
        [JsonIgnore]
        public string Include { get; set; }
        [JsonIgnore]
        public string LoggerNamespace { get; set; }
        public int? StartId { get; set; }

        public EventArgumentModel[] ImplicitArguments
        {
            get { return _implicitArguments ?? new EventArgumentModel[0]; }
            set { _implicitArguments = value; }
        }
        public EventArgumentModel[] OverrideArguments
        {
            get { return _overrideArguments ?? new EventArgumentModel[0]; }
            set { _overrideArguments = value; }
        }

        [JsonIgnore]
        public EventModel[] Events
        {
            get { return _events ?? new EventModel[0]; }
            set { _events = value; }
        }

        [JsonIgnore]
        public EventSourceModel EventSource { get; set; }

        public void AddTemplate(LoggerTemplateModel loggerTemplate)
        {
            var events = new List<EventModel>();
            this.LoggerNamespace = loggerTemplate.Namespace;
            this.Keyword = loggerTemplate.Name.Substring(1).Replace("Logger", "");
            foreach (var templateEvent in loggerTemplate.Events)
            {
                events.Add(templateEvent);
            }
            this.Events = events.ToArray();
            this.Include = loggerTemplate.Include;
        }
    }
}