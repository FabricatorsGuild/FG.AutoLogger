using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class ProjectLoggerRenderer : BaseWithLogging, IProjectRenderer
    {
        public void Render(Project model)
        {
            var files = model.ProjectItems.ToArray();
            LogMessage($"Rendering {files.Length} project file{(files.Length == 1 ? "" : "s")} for loggers");

            var loggerPartialProjectItems = files.OfType<LoggerModel>(ProjectItemType.EventSourceLoggerPartial);
            foreach (var loggerProjectItem in loggerPartialProjectItems)
            {
                LogMessage($"Rendering Partial Logger from file {loggerProjectItem.Name}");
                RenderLoggerEventSourcePartial(model, loggerProjectItem);
            }

            var loggerImplementationProjectItems = files.OfType<LoggerModel>(ProjectItemType.LoggerImplementation);
            foreach (var loggerProjectItem in loggerImplementationProjectItems)
            {
                LogMessage($"Rendering Logger Implementation from file {loggerProjectItem.Name}");
                RenderLoggerImplementation(model, loggerProjectItem);
            }

        }

        private void RenderLoggerImplementation(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            var loggerModel = loggerProjectItem.Content;
            if (loggerModel == null)
            {
                LogError(
                    $"{loggerProjectItem.Name} should have a content of type {typeof(LoggerModel).Name} set but found {loggerProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var loggerRenderers = new ILoggerImplementationRenderer[]
            {
                // TODO : add logger implementation renderers 
            };
            foreach (var renderer in loggerRenderers.Union(project.GetExtensions<ILoggerImplementationRenderer>()))
            {
                renderer.Render(project, loggerProjectItem);
            }
        }

        private void RenderLoggerEventSourcePartial(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            var loggerModel = loggerProjectItem.Content;
            if (loggerModel == null)
            {
                LogError(
                    $"{loggerProjectItem.Name} should have a content of type {typeof(LoggerModel).Name} set but found {loggerProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var loggerRenderers = new ILoggerEventSourcePartialRenderer[]
            {
                new EventSourceLoggerPartialRenderer(),
            };
            foreach (var renderer in loggerRenderers.Union(project.GetExtensions<ILoggerEventSourcePartialRenderer>()))
            {
                renderer.Render(project, loggerProjectItem);
            }
        }
    }
}