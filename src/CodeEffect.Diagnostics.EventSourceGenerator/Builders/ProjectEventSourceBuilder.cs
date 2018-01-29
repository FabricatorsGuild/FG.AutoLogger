using System;
using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectEventSourceBuilder : BaseCoreBuilder, IProjectBuilder
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
            if( eventSourceModel.Settings == null) eventSourceModel.Settings = new EventSourceModel.EventSourceSettings();

            if (eventSourceModel.Settings.UseReferencedHelpers)
            {
                var referenceAssemblyName = typeof(EventArgumentModel).Assembly.GetName().Name;
                var referenceToAutoLoggerModel = project.DynamicAssembly.GetReferencedAssemblies().FirstOrDefault(r =>
                    r.Name == referenceAssemblyName);

                if (referenceToAutoLoggerModel == null)
                {
                    throw new NotSupportedException($"The 'UseReferencedHelpers' setting for EventSource '{eventSourceModel.Name}' can only be specified if the project references '{referenceAssemblyName}'");
                }
            }

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

        private void BuildEventSource(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var eventSourceBuilders = new IEventSourceBuilder[]
            {
                new EventSourceKeywordBuilder(),
                new EventSourceEventTaskBuilder(),
                new EventSourceAutoGenerateLoggersBuilder(), 
                new EventSourceCorrelatingEventsBuilder(),
                new EventSourceLoggersBuilder(),
                //new EventSourceAutoGeneratedLoggersBuilder(), 
                new EventSourceEventsBuilder(),
                new EventSourceExtensionsMethodsBuilder(),
            }.Union(project.GetExtensions<IEventSourceBuilder>(eventSource.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var builder in eventSourceBuilders)
            {
                PassAlongLoggers(builder as IWithLogging);
                builder.Build(project, eventSourceProjectItem);
            }
        }
    }
}