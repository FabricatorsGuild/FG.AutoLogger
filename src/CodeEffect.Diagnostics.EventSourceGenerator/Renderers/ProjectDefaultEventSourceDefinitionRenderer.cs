using System.Collections.Generic;
using FG.Diagnostics.AutoLogger.Model;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class ProjectDefaultEventSourceDefinitionRenderer : BaseEtwRendererWithLogging, IProjectRenderer
    {
        public void Render(Project model)
        {
            var defaultEventSourceDefinitionProjectItems = model.ProjectItems.OfType<EventSourceModel>(ProjectItemType.DefaultGeneratedEventSourceDefinition);
            foreach (var defaultEventSourceDefinitionProjectItem in defaultEventSourceDefinitionProjectItems)
            {
                Render(model, defaultEventSourceDefinitionProjectItem);
            }
        }


        private void Render(Project project, ProjectItem<EventSourceModel> defaultEventSourceDefinitionProjectItem)
        {
            var defaultEventSource = defaultEventSourceDefinitionProjectItem.Content as EventSourceModel;
            if (defaultEventSource == null)
            {
                LogError(
                    $"{defaultEventSourceDefinitionProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {defaultEventSourceDefinitionProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var converters = new List<JsonConverter> {new KeywordModelJsonConverter()};            
            var jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(defaultEventSource, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore, Converters = converters
            });
            defaultEventSourceDefinitionProjectItem.Output = jsonFile;
        }
    }
}