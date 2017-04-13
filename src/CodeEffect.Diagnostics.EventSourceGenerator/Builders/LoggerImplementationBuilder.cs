using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerImplementationBuilder : BaseWithLogging, ILoggerBuilder
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

            var loggerFileInclude = eventSource.Include.Replace(eventSource.ClassName, model.Name.Substring(1));
            var loggerFileName = System.IO.Path.Combine(project.ProjectBasePath, loggerFileInclude);
            model.SourceFileName = model.Name.Substring(1);

            var newProjectItem = new ProjectItem<LoggerModel>(
                type: ProjectItemType.LoggerImplementation,
                name: loggerFileName,
                content: model,
                include: loggerFileInclude)
            {
                DependentUpon = eventSourceProjectItem
            };

            project.AddProjectItem(newProjectItem);
        }
    }
}