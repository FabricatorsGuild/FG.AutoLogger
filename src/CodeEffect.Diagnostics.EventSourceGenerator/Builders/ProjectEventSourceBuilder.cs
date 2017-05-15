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
            var sourceFileName = System.IO.Path.GetFileName(eventSourceDefinitionProjectItem.Name);
            var name = sourceFileName.Replace(".eventsource.json", "");
            var implementationFileName = $"{name}.cs";

            var fileRelativePath = eventSourceDefinitionProjectItem.Name
                .RemoveFromStart(project.ProjectBasePath + System.IO.Path.DirectorySeparatorChar)
                .Replace(sourceFileName, implementationFileName);

            var fileRelateiveFolderPath = System.IO.Path.GetDirectoryName(fileRelativePath);

            var eventSourceNamespace = fileRelateiveFolderPath.Length > 0
                   ? $"{eventSourceDefinitionProjectItem.RootNamespace}.{fileRelateiveFolderPath.Replace(System.IO.Path.DirectorySeparatorChar, '.')}"
                   : eventSourceDefinitionProjectItem.RootNamespace;

            if (eventSourceModel == null)
            {

                var content = System.IO.File.ReadAllText(eventSourceDefinitionProjectItem.Name);

                var converters = new List<JsonConverter> { new KeywordModelJsonConverter() };
                eventSourceModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSourceModel>(content, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = converters
                });

                eventSourceModel.Include = fileRelativePath;
            }

            eventSourceModel.Name = name;
            eventSourceModel.Namespace = eventSourceModel.Namespace ?? eventSourceNamespace;
            eventSourceModel.ProviderName = eventSourceModel.ProviderName ?? $"{eventSourceModel.Namespace.Replace('.', '-')}-{name}";
            eventSourceModel.ClassName = name.GetUpperCasedInitial();
            eventSourceModel.SourceFilePath = eventSourceDefinitionProjectItem.Include;

            var filePath = PathExtensions.GetAbsolutePath(System.IO.Path.GetDirectoryName(eventSourceDefinitionProjectItem.Name), implementationFileName);

            eventSourceDefinitionProjectItem.Content = eventSourceModel;
            BuildEventSource(project, eventSourceDefinitionProjectItem);

            var newProjectItem = new ProjectItem<EventSourceModel>(
                type: ProjectItemType.EventSource,
                name: filePath,
                content: eventSourceModel,
                include: eventSourceModel.Include)
            {
                DependentUpon = eventSourceDefinitionProjectItem,
            };

            project.AddProjectItem(newProjectItem);
        }

        private void BuildEventSource(Project project, ProjectItem<EventSourceModel> eventSourceDefinitionProjectItem)
        {
            var eventSourceBuilders = new IEventSourceBuilder[]
            {
                new EventSourceKeywordBuilder(),
                new EventSourceAutoGenerateLoggersBuilder(), 
                new EventSourceCorrelatingEventsBuilder(),
                new EventSourceLoggersBuilder(),
                new EventSourceEventsBuilder(),
                new EventSourceExtensionsMethodsBuilder(),
            }.Union(project.GetExtensions<IEventSourceBuilder>()).ToArray();
            foreach (var builder in eventSourceBuilders)
            {
                PassAlongLoggers(builder as IWithLogging);
                builder.Build(project, eventSourceDefinitionProjectItem);
            }
        }
    }
}