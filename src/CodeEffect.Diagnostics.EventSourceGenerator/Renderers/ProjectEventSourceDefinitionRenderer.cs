using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class ProjectEventSourceDefinitionRenderer : BaseEtwRendererWithLogging, IProjectRenderer
    {
        public void Render(Project model)
        {
            var files = model.ProjectItems.ToArray();
            LogMessage($"Rendering {files.Length} project file{(files.Length == 1 ? "" : "s")} for eventsources");

            var eventSourceDefinitionProjectItems = files.OfType<EventSourceModel>(ProjectItemType.EventSourceDefinition);
            foreach (var eventSourceDefinitionProjectItem in eventSourceDefinitionProjectItems)
            {
                LogMessage($"Rendering EventSource definition from file {eventSourceDefinitionProjectItem.Name}");
                RenderEventSourceDefinition(model, eventSourceDefinitionProjectItem);
            }
        }

        private void RenderEventSourceDefinition(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem)
        {
            var eventSourceModel = eventSourceProjectItem.Content as EventSourceModel;
            if (eventSourceModel == null)
            {
                LogError(
                    $"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var shouldRender = eventSourceModel.Loggers.Any(l => l.AutoDiscovered);

            var converters = new List<JsonConverter> { new KeywordModelJsonConverter() };
            var jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(eventSourceModel, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = converters,
                ContractResolver = new EventSourceDefinitionContractResolver()
            });

            if (shouldRender)
            {
                eventSourceProjectItem.Output = jsonFile;
            }
        }


    }

    public class EventSourceDefinitionContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType.IsArray)
            {
                var propertyName = property.PropertyName; 
                property.ShouldSerialize = 
                    instance =>
                    {
                        var propertyInfo = instance.GetType().GetProperty(propertyName);
                        var value = propertyInfo.GetValue(instance);
                        var enumerable = value as IEnumerable;
                        if (enumerable != null)
                        {
                            foreach (var item in enumerable)
                            {
                                return true;
                            }
                            return false;
                        }
                        return true;
                    };
            }

            return property;
        }
    }
}