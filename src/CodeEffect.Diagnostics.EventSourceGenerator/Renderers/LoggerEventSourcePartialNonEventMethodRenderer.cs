using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class LoggerEventSourcePartialNonEventMethodRenderer : EventSourceNonEventMethodBaseRenderer, ILoggerEventSourcePartialEventRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            var loggerModel = loggerProjectItem.Content;
            if (loggerModel == null)
            {
                LogError($"{loggerProjectItem?.Name ?? nameof(loggerProjectItem)} should have content of type {typeof(LoggerModel).Name}, but found {loggerProjectItem?.GetType().Name ?? "null"}");
                return "";
            }

            var eventSourceModel = loggerModel.EventSource;
            if (eventSourceModel == null)
            {
                LogError($"{loggerProjectItem?.Name ?? nameof(loggerProjectItem)} should have content of type {typeof(LoggerModel).Name} with property EventSource set, but found {loggerModel.EventSource?.Name ?? "null"}");
                return "";
            }

            return Render(model);            
        }
    }
}