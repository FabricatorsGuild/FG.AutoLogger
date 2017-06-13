using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Templates;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerEventSourcePartialRenderer : BaseWithLogging, ILoggerEventSourcePartialRenderer
    {
        public void Render(Project project, ProjectItem<LoggerModel> model)
        {
            if (model.ItemType != ProjectItemType.EventSourceLoggerPartial)
            {
                LogError($"{model.Name} should be of ItemType {nameof(ProjectItemType.EventSourceLoggerPartial)} set but found {model.ItemType}");
                return;
            }

            var loggerModel = model.Content;
            if (loggerModel == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }
            var eventSourceProjectItem = model.DependentUpon as ProjectItem<EventSourceModel>;
            var eventSourceModel = eventSourceProjectItem?.Content;
            if (eventSourceModel == null)
            {
                LogError($"{model.Name} should be DependentUpon a ProjectItem<EventSourceModel>, but found {model.DependentUpon?.GetType().Name ?? "null"}");
                return;
            }

            var output = LoggerEventSourcePartialTemplate.Template_LOGGER_PARTIAL_CLASS_DECLARATION;
            output = output.Replace(LoggerEventSourcePartialTemplate.Variable_EVENTSOURCE_CLASS_NAME, eventSourceModel.ClassName);
            output = output.Replace(LoggerEventSourcePartialTemplate.Variable_NAMESPACE_DECLARATION, eventSourceModel.Namespace);
            output = output.Replace(LoggerEventSourcePartialTemplate.Variable_EVENTSOURCE_PARTIAL_FILE_NAME, loggerModel.Name);
            output = output.Replace(LoggerEventSourcePartialTemplate.Variable_LOGGER_SOURCE_FILE_NAME, loggerModel.SourceFileName);


            var logger = new StringBuilder();
            var loggerEventRenderers = new ILoggerEventSourcePartialEventRenderer[]
            {
                new LoggerEventSourcePartialEventMethodRenderer(), 
                new LoggerEventSourcePartialNonEventMethodRenderer(), 
            };

            foreach (var loggerEvent in loggerModel.Events)
            {
                foreach (var renderer in loggerEventRenderers.Union(project.GetExtensions<ILoggerEventSourcePartialEventRenderer>()))
                {
                    PassAlongLoggers(renderer as IWithLogging);
                    var eventRender = renderer.Render(project, model, loggerEvent);
                    logger.AppendLine(eventRender);
                }
            }

            output = output.Replace(LoggerEventSourcePartialTemplate.Variable_LOGGER_EVENTS_DECLARATION, logger.ToString());

            model.Output = output;
        }
    }
}