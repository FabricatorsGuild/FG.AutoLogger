using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class LoggerTemplateBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var loggerTemplate = project.Loggers.GetMatchingLoggerTemplate(model);
            if (loggerTemplate == null)
            {
                LogError($"Logger {model.Name} was not found in the project. Declare an interface with a matching name in a file with the same name as the interface.");
                return;
            }
            model.AddTemplate(loggerTemplate);
        }
    }
}