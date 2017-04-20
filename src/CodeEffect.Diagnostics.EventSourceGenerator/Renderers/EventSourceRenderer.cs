using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceRenderer : BaseWithLogging, IEventSourceRenderer
    {
        public void Render(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSourceModel = model.Content;
            if (eventSourceModel == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            var output = Template.Template_EVENTSOURCE_CLASS_DECLARATION;
            output = output.Replace(Template.Variable_SOURCE_FILE_NAME, eventSourceModel.SourceFilePath);
            output = output.Replace(Template.Variable_EVENTSOURCE_NAME, eventSourceModel.ProviderName);
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
            output = output.Replace(Template.Variable_NAMESPACE_DECLARATION, eventSourceModel.Namespace);

            // Render all events
            var events = new StringBuilder();
            var eventRenderers = new IEventRenderer[]
            {
                new EventSourceEventMethodRenderer(),
                new EventSourceNonEventMethodRenderer(),
                new LoggerEventSourcePartialEventMethodRenderer(), 
            }.Union(project.GetExtensions<IEventRenderer>()).ToArray();
            foreach (var eventSourceEvent in eventSourceModel?.Events ?? new EventModel[0])
            {
                foreach (var renderer in eventRenderers.Union(project.GetExtensions<IEventRenderer>()))
                {
                    events.AppendLine(renderer.Render(project, eventSourceModel, eventSourceEvent));
                }
            }
            output = output.Replace(Template.Variable_EVENTS_DECLARATION, events.ToString());


            // Render all keywords
            var keywords = new StringBuilder();
            var keywordsRenderers = new IKeywordRenderer[]
            {
                new EventSourceKeywordRenderer(),
            }.Union(project.GetExtensions<IKeywordRenderer>()).ToArray();
            foreach (var keyword in eventSourceModel.Keywords ?? new KeywordModel[0])
            {
                foreach (var renderer in keywordsRenderers.Union(project.GetExtensions<IKeywordRenderer>()))
                {
                    keywords.AppendLine(renderer.Render(project, eventSourceModel, keyword));
                }
            }
            output = output.Replace(Template.Variable_KEYWORDS_DECLARATION, keywords.ToString());


            // Render extensions
            if (eventSourceModel.Extensions != null && eventSourceModel.Extensions.Any())
            {
                var extensions = new StringBuilder();
                var extensionRenderers = new IExtensionsMethodRenderer[]
                {
                    new EventSourceExtensionMethodRenderer(), 
                }.Union(project.GetExtensions<IExtensionsMethodRenderer>()).ToArray();
                foreach (var extension in eventSourceModel.Extensions)
                {
                    foreach (var renderer in extensionRenderers.Union(project.GetExtensions<IExtensionsMethodRenderer>()))
                    {
                        extensions.AppendLine(renderer.Render(project, eventSourceModel, extension));
                    }
                }

                var extensionMethod = Template.Template_EXTENSIONS_DECLARATION;
                extensionMethod = extensionMethod.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
                extensionMethod = extensionMethod.Replace(Template.Variable_EXTENSION_METHODS_DECLARATION, extensions.ToString());

                output = output.Replace(Template.Variable_EXTENSIONS_DECLARATION, extensionMethod);
            }
            else
            {
                output = output.Replace(Template.Variable_EXTENSIONS_DECLARATION, "");

            }

            model.Output = output;
        }
    }
}