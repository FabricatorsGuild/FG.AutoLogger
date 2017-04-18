using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
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