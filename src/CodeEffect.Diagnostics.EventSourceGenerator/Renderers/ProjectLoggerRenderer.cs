using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class ProjectLoggerRenderer : BaseEtwRendererWithLogging, IProjectRenderer
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
                new LoggerImplementationRenderer(),
            }.Union(project.GetExtensions<ILoggerImplementationRenderer>(loggerModel.EventSource.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var renderer in loggerRenderers)
            {
                PassAlongLoggers(renderer as IWithLogging);
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
                new LoggerEventSourcePartialRenderer(),
            }.Union(project.GetExtensions<ILoggerEventSourcePartialRenderer>(loggerModel.EventSource.Settings?.Modules ?? new string[0])).ToArray();
            foreach (var renderer in loggerRenderers)
            {
                PassAlongLoggers(renderer as IWithLogging);
                renderer.Render(project, loggerProjectItem);
            }
        }
    }
}