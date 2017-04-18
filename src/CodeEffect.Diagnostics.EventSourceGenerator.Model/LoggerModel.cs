using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class LoggerModel
    {
        public string GetImplementationName()
        {
            return this.Name.Substring(1);
        }
        public string GetKeyword()
        {
            return this.Name.Substring(1).Replace("Logger", "");
        }
        public string SourceFileName { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Include { get; set; }
        public string LoggerNamespace { get; set; }
        public int? StartId { get; set; }
        public EventArgumentModel[] ImplicitArguments { get; set; }
        public EventArgumentModel[] OverrideArguments { get; set; }
        [JsonIgnore]
        public EventModel[] Events { get; set; }
        [JsonIgnore]
        public EventSourceModel EventSource { get; set; }

        public void AddTemplate(LoggerTemplateModel loggerTemplate)
        {
            var events = new List<EventModel>();
            this.LoggerNamespace = loggerTemplate.Namespace;
            foreach (var templateEvent in loggerTemplate.Events)
            {
                events.Add(templateEvent);
            }
            this.Events = events.ToArray();
            this.Include = loggerTemplate.Include;
        }
    }
}