using System.Collections.Generic;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class EventSourceModel
    {
        public EventSourceModel()
        {
            Extensions = new List<ExtensionsMethodModel>();
            Settings = new EventSourceSettings()
            {
                AutogenerateLoggerInterfaces = false,
            };
        }

        public LoggerModel[] Loggers { get; set; }

        [JsonIgnore]
        public string Namespace { get; set; }

        [JsonIgnore]
        public string ClassName { get; set; }

        public string Name { get; set; }
        public string ProviderName { get; set; }

        [JsonIgnore]
        public string SourceFilePath { get; set; }

        [JsonIgnore]
        public string Include { get; set; }

        public KeywordModel[] Keywords { get; set; }        

        public TypeTemplateModel[] TypeTemplates { get; set; }

        [JsonIgnore]
        public List<ExtensionsMethodModel> Extensions { get; private set; }

        public EventModel[] Events { get; set; }

        public EventSourceSettings Settings { get; set; }

        public override string ToString()
        {
            return $"{nameof(EventSourceModel)} {this.Name}";
        }

        public class EventSourceSettings
        {
            public bool AutogenerateLoggerInterfaces { get; set; }
        }

    }
}