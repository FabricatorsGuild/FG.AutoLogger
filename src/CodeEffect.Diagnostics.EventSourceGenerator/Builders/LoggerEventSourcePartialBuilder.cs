using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class LoggerEventSourcePartialBuilder : BaseWithLogging, ILoggerBuilder
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

            var eventSourcePartialFileInclude = eventSource.Include.Replace(eventSource.Name, $"{eventSource.ClassName}.{model.Name}");
            var eventSourcePartialFileName = System.IO.Path.Combine(project.ProjectBasePath, eventSourcePartialFileInclude);
            model.SourceFileName = model.Name.Substring(1);            

            var newProjectItem = new ProjectItem<LoggerModel>(
                type: ProjectItemType.EventSourceLoggerPartial,
                name: eventSourcePartialFileName,
                content: model,
                include: eventSourcePartialFileInclude)
            {
                DependentUpon = eventSourceProjectItem
            };

            project.AddProjectItem(newProjectItem);            
        }
    }
}