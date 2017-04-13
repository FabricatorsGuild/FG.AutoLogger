using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceLoggerPartialRenderer : BaseWithLogging, ILoggerEventSourcePartialRenderer
    {
        public void Render(Project project, ProjectItem<LoggerModel> model)
        {
            var loggerModel = model.Content;
            if (loggerModel == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }
            var eventSoureModel = (model.DependentUpon as ProjectItem<EventSourceModel>)?.Content;
            if (eventSoureModel == null)
            {
                LogError($"{model.Name} should be DependentUpon a ProjectItem<EventSourceModel>, but found {model.DependentUpon?.GetType().Name ?? "null"}");
                return;
            }

            var output = Template.Template_LOGGER_PARTIAL_CLASS_DELCARATION;
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSoureModel.ClassName);
            output = output.Replace(Template.Variable_NAMESPACE_DECLARATION, eventSoureModel.Namespace);
            output = output.Replace(Template.Variable_EVENTSOURCE_PARTIAL_FILE_NAME, loggerModel.Name);
            output = output.Replace(Template.Variable_LOGGER_SOURCE_FILE_NAME, loggerModel.SourceFileName);


            var logger = new StringBuilder();
            var loggerEventRenderers = new ILoggerEventRenderer[]
            {
                
            };

            foreach (var loggerEvent in loggerModel.Events)
            {
                foreach (var renderer in loggerEventRenderers.Union(project.GetExtensions<ILoggerEventRenderer>()))
                {
                    var eventRender = renderer.Render(project, model, loggerEvent);
                    logger.AppendLine(eventRender);
                }
            }

            output = output.Replace(Template.Variable_LOGGER_EVENTS_DECLARATION, logger.ToString());

            model.Output = output;
        }
    }
}