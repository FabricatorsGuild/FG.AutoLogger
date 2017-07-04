using System.Linq;
using System.Text;
using FG.Diagnostics.AutoLogger.Generator.Templates;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class EventSourceRenderer : BaseEtwRendererWithLogging, IEventSourceRenderer
    {
        public void Render(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSourceModel = model.Content;
            if (eventSourceModel == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            var output = EventSourceTemplate.Template_EVENTSOURCE_CLASS_DECLARATION;
            output = output.Replace(EventSourceTemplate.Variable_SOURCE_FILE_NAME, eventSourceModel.SourceFilePath);
            output = output.Replace(EventSourceTemplate.Variable_EVENTSOURCE_NAME, eventSourceModel.ProviderName);
            output = output.Replace(EventSourceTemplate.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
            output = output.Replace(EventSourceTemplate.Variable_NAMESPACE_DECLARATION, eventSourceModel.Namespace);

            // Render all events
            var events = new StringBuilder();
            var eventRenderers = new IEventRenderer[]
            {
                new EventSourceEventMethodRenderer(),
                new EventSourceNonEventMethodRenderer(),
                new LoggerEventSourcePartialEventMethodRenderer(), 
            }.Union(project.GetExtensions<IEventRenderer>(eventSourceModel.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var eventSourceEvent in eventSourceModel?.Events ?? new EventModel[0])
            {
                foreach (var renderer in eventRenderers)
                {
                    PassAlongLoggers(renderer as IWithLogging);
                    events.AppendLine(renderer.Render(project, eventSourceModel, eventSourceEvent));
                }
            }
            output = output.Replace(EventSourceTemplate.Variable_EVENTS_DECLARATION, events.ToString());


            // Render all keywords
            var keywords = new StringBuilder();
            var keywordsRenderers = new IKeywordRenderer[]
            {
                new EventSourceKeywordRenderer(),
            }.Union(project.GetExtensions<IKeywordRenderer>(eventSourceModel.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var keyword in eventSourceModel.Keywords ?? new KeywordModel[0])
            {
                foreach (var renderer in keywordsRenderers)
                {
                    PassAlongLoggers(renderer as IWithLogging);
                    keywords.AppendLine(renderer.Render(project, eventSourceModel, keyword));
                }
            }
            output = output.Replace(EventSourceTemplate.Variable_KEYWORDS_DECLARATION, keywords.ToString());

            // Render all tasks
            var eventTasks = new StringBuilder();
            var eventTaskRenderers = new IEventTaskRenderer[]
            {
                new EventSourceEventTaskRenderer(), 
            }.Union(project.GetExtensions<IEventTaskRenderer>(eventSourceModel.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var eventTask in eventSourceModel.Tasks?? new EventTaskModel[0])
            {
                foreach (var renderer in eventTaskRenderers)
                {
                    PassAlongLoggers(renderer as IWithLogging);
                    eventTasks.AppendLine(renderer.Render(project, eventSourceModel, eventTask));
                }
            }
            output = output.Replace(EventSourceTemplate.Variable_EVENTTASKS_DECLARATION, eventTasks.ToString());

            // Render extensions
            if (eventSourceModel.Extensions != null && eventSourceModel.Extensions.Any())
            {
                var extensions = new StringBuilder();
                var extensionRenderers = new IExtensionsMethodRenderer[]
                {
                    new EventSourceExtensionMethodRenderer(), 
                }.Union(project.GetExtensions<IExtensionsMethodRenderer>(eventSourceModel.Settings?.Modules ?? new string[0])).ToArray();
                foreach (var extension in eventSourceModel.Extensions)
                {
                    foreach (var renderer in extensionRenderers)
                    {
                        PassAlongLoggers(renderer as IWithLogging);
                        extensions.AppendLine(renderer.Render(project, eventSourceModel, extension));
                    }
                }

                var extensionMethod = EventSourceTemplate.Template_EXTENSIONS_DECLARATION;
                extensionMethod = extensionMethod.Replace(EventSourceTemplate.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
                extensionMethod = extensionMethod.Replace(EventSourceTemplate.Variable_EXTENSION_METHODS_DECLARATION, extensions.ToString());

                output = output.Replace(EventSourceTemplate.Variable_EXTENSIONS_DECLARATION, extensionMethod);
            }
            else
            {
                output = output.Replace(EventSourceTemplate.Variable_EXTENSIONS_DECLARATION, "");

            }

            model.Output = output;
        }
    }
}