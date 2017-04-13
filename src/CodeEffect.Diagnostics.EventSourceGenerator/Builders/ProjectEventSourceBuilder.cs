using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectEventSourceBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            var files = model.ProjectItems.ToArray();
            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for eventsources");

            var eventSourceDefinitionProjectItems = files.OfType(ProjectItemType.EventSourceDefinition, ProjectItemType.DefaultGeneratedEventSourceDefinition);
            foreach (var eventSourceDefinitionProjectItem in eventSourceDefinitionProjectItems)
            {
                LogMessage($"Found EventSource from file {eventSourceDefinitionProjectItem.Name}");
                LoadAndBuildEventSource(model, eventSourceDefinitionProjectItem as ProjectItem<EventSourceModel>);
            }
        }

        private void LoadAndBuildEventSource(Project project, ProjectItem<EventSourceModel> eventSourceDefinitionProjectItem)
        {
            var eventSourceModel = eventSourceDefinitionProjectItem.Content;            
            if (eventSourceModel == null)
            {
                var sourceFileName = System.IO.Path.GetFileName(eventSourceDefinitionProjectItem.Name);
                var implementationFileName = $"{System.IO.Path.GetFileNameWithoutExtension(eventSourceDefinitionProjectItem.Name)}.cs";
                var fileRelativePath = eventSourceDefinitionProjectItem.Name
                    .RemoveFromStart(project.ProjectBasePath + System.IO.Path.DirectorySeparatorChar)
                    .Replace(sourceFileName, implementationFileName);

                var fileRelateiveFolderPath = System.IO.Path.GetDirectoryName(fileRelativePath);
                var eventSourceNamespace =
                    $"{eventSourceDefinitionProjectItem.RootNamespace}.{fileRelateiveFolderPath.Replace(System.IO.Path.DirectorySeparatorChar, '.')}";

                var content = System.IO.File.ReadAllText(eventSourceDefinitionProjectItem.Name);

                var converters = new List<JsonConverter> { new KeywordModelJsonConverter() };
                eventSourceModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSourceModel>(content, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = converters
                });

                var fileName = System.IO.Path.GetFileName(eventSourceDefinitionProjectItem.Name);
                var className = System.IO.Path.GetFileNameWithoutExtension(fileName);

                eventSourceModel.ClassName = className;
                eventSourceModel.Include = fileRelativePath;
                eventSourceModel.SourceFilePath = eventSourceDefinitionProjectItem.Include;
                eventSourceModel.Namespace = eventSourceNamespace;
            }
            eventSourceDefinitionProjectItem.Content = eventSourceModel;
            BuildEventSource(project, eventSourceDefinitionProjectItem);

            var newProjectItem = new ProjectItem<EventSourceModel>(
                type: ProjectItemType.EventSource,
                name: eventSourceModel.Include,
                content: eventSourceModel,
                include: eventSourceModel.Include)
            {
                DependentUpon = eventSourceDefinitionProjectItem,
            };

            project.AddProjectItem(newProjectItem);
        }

        private void BuildEventSource(Project project, ProjectItem<EventSourceModel> eventSourceDefinitionProjectItem)
        {
            // TODO: Get all builders from project and allow all to build
            var eventSourceBuilders = new IEventSourceBuilder[]
            {
                new EventSourceKeywordBuilder(),
                new EventSourceLoggersBuilder(),
                new EventSourceEventsBuilder(),
                new EventSourceExtensionsMethodsBuilder(),
            };
            foreach (var builder in eventSourceBuilders)
            {
                builder.Build(project, eventSourceDefinitionProjectItem);
            }
        }
    }
}