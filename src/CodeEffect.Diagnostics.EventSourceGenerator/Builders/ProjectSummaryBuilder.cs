using System;
using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectSummaryBuilder : BaseCoreBuilder, IProjectBuilder
    {
        public void Build(Project model)
        {
            var extensions = model.ProjectItems.OfType(ProjectItemType.BuilderExtension);
            var loggers = model.ProjectItems.OfType(ProjectItemType.LoggerInterface);
            var references = model.ProjectItems.OfType(ProjectItemType.Reference);
            var projectReferences = model.ProjectItems.OfType(ProjectItemType.ProjectReference);
            var eventSourceDefinitions = model.ProjectItems.OfType(ProjectItemType.EventSourceDefinition);

            var hasProjectChanges = false;
            foreach (var eventSourceDefinition in eventSourceDefinitions)
            {
                hasProjectChanges = hasProjectChanges || BuildEventSourceSummary(model, eventSourceDefinition, extensions, loggers, references, projectReferences);
            }
            
            model.HasProjectChanges = hasProjectChanges;
        }

        private string GetProjectSummaryInclude(ProjectItem projectSummaryProjectItem)
        {
            return projectSummaryProjectItem.Include.RemoveFromEnd(".eventsource.output.json");
        }

        private string GetEventSourceInclude(ProjectItem eventSourceProjectItem)
        {
            return eventSourceProjectItem.Include.RemoveFromEnd(".eventsource.json");
        }

        private bool BuildEventSourceSummary(Project project, ProjectItem eventSourceProjectItem,
            IEnumerable<ProjectItem> extensions,
            IEnumerable<ProjectItem> loggers,
            IEnumerable<ProjectItem> references,
            IEnumerable<ProjectItem> projectReferences)
        {
            var eventSourceInclude = GetEventSourceInclude(eventSourceProjectItem);

            var summaryProjectItem = project.ProjectItems.OfType(ProjectItemType.ProjectSummary).FirstOrDefault(pi => GetProjectSummaryInclude(pi) == eventSourceInclude);
            var hasMultipleSummaries = project.ProjectItems.OfType(ProjectItemType.ProjectSummary).Count() > 1;
            if (hasMultipleSummaries)
            {
                LogWarning($"Multiple project summary files for {eventSourceInclude} found, should only have one. Additional summaries will be disregarded.");
            }

            ProjectSummary existingSummary = null;
            if (summaryProjectItem != null)
            {
                try
                {
                    var existingSummaryContent = System.IO.File.ReadAllText(summaryProjectItem.Name);
                    existingSummary = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectSummary>(existingSummaryContent);
                }
                catch (Exception ex)
                {
                    LogWarning($"Could not read/deserialize summary file {summaryProjectItem.Name}, ignoring it and treating it as not existing");
                }
            }
            else
            {
                var summaryInclude = $"{eventSourceInclude}.eventsource.output.json";
                var projectItemFilePath = System.IO.Path.Combine(project.ProjectBasePath, summaryInclude);
                summaryProjectItem = new ProjectItem<ProjectSummary>(ProjectItemType.ProjectSummary, projectItemFilePath) { Include = summaryInclude, DependentUpon = eventSourceProjectItem};
                project.AddProjectItem(summaryProjectItem);
            }

            var newSummary = new ProjectSummary()
            {
                CompileTargets = extensions.Select(pi => GetExtensionSummary(project, pi)).Union(loggers.Select(pi => GetLoggerSummary(project, pi))).ToArray(),
                References = references.Select(pi => GetReferenceSummary(project, pi)).ToArray(),
                ProjectReferences = projectReferences.Select(pi => GetProjectReferenceSummary(project, pi)).ToArray(),
                EventSources = new ProjectSummary.ProjectSummaryEventSource[] { GetEventSourceDefinitionSummary(project, eventSourceProjectItem)},
            };
            var jsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            var newSummaryContent = Newtonsoft.Json.JsonConvert.SerializeObject(newSummary, Formatting.Indented, jsonSerializerSettings);
            newSummary.Hash = newSummaryContent.ToMD5().ToHex();

            summaryProjectItem.Content = newSummary;

            var hasProjectChanges = existingSummary?.Hash != newSummary.Hash;
            return hasProjectChanges;
        }


        private static string GetBaseName(Project project, ProjectItem projectItem)
        {
            return PathExtensions.MakeRelativePath(project.ProjectBasePath, projectItem.Name);
        }

        private ProjectSummary.ProjectSummaryProjectReferenceSummary GetProjectReferenceSummary(Project project, ProjectItem referenceProjectItem)
        {
            var contentWriteTime = System.IO.File.GetLastWriteTimeUtc(referenceProjectItem.Name);
            var contentWriteTimeString = contentWriteTime.ToLongTimeString();
            return new ProjectSummary.ProjectSummaryProjectReferenceSummary() {ReferenceName = GetBaseName(project, referenceProjectItem), Hash = contentWriteTimeString };
        }

        private string GetReferenceSummary(Project project, ProjectItem referenceProjectItem)
        {
            if (System.IO.Path.IsPathRooted(referenceProjectItem.Name))
            {
                return GetBaseName(project, referenceProjectItem);
            }
            else
            {
                return referenceProjectItem.Name;
            }
        }

        private ProjectSummary.ProjectSummaryEventSource GetEventSourceDefinitionSummary(Project project, ProjectItem eventSourceDefinitionProjectItem)
        {
            var content = System.IO.File.ReadAllText(eventSourceDefinitionProjectItem.Name);
            var hash = content.ToMD5().ToHex();
            return new ProjectSummary.ProjectSummaryEventSource() { Name = GetBaseName(project, eventSourceDefinitionProjectItem), Hash = hash };
        }

        private ProjectSummary.ProjectSummaryCompileTarget GetExtensionSummary(Project project, ProjectItem extensionProjectItem)
        {
            var content = System.IO.File.ReadAllText(extensionProjectItem.Name);
            var hash = content.ToMD5().ToHex();
            return new ProjectSummary.ProjectSummaryCompileTarget() {Name = GetBaseName(project, extensionProjectItem), Hash = hash};
        }

        private ProjectSummary.ProjectSummaryCompileTarget GetLoggerSummary(Project project, ProjectItem loggerProjectItem)
        {
            string hash = null;
            var content = System.IO.File.ReadAllBytes(loggerProjectItem.Name);
            hash = content.ToMD5().ToHex();
            return new ProjectSummary.ProjectSummaryCompileTarget() { Name = GetBaseName(project, loggerProjectItem), Hash = hash };
        }        
    }
}