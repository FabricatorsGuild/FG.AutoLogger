using System.Linq;
using FG.Diagnostics.AutoLogger.Model;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class ProjectSummaryRenderer : BaseEtwRendererWithLogging, IProjectRenderer
    {
        public void Render(Project model)
        {
            var files = model.ProjectItems.ToArray();
            LogMessage($"Rendering {files.Length} project summary ");

            var summaryProjectItems = files.OfType<ProjectSummary>(ProjectItemType.ProjectSummary);
            foreach (var summaryProjectItem in summaryProjectItems)
            {
                LogMessage($"Rendering summary from file {summaryProjectItem.Name}");
                RenderSummary(model, summaryProjectItem);
            }
        }

        private void RenderSummary(Project project, ProjectItem<ProjectSummary> summaryProjectItem)
        {
            var summary = summaryProjectItem.Content as ProjectSummary;
            if (summary == null)
            {
                LogError(
                    $"{summaryProjectItem.Name} should have a content of type {typeof(ProjectSummary).Name} set but found {summaryProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(summary, Formatting.Indented);
            summaryProjectItem.Output = output;
        }
    }
}