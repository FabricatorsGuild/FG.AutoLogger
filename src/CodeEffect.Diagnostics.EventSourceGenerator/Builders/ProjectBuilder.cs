using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            LogMessage($"Scanning project {model.ProjectFilePath} for eventsource definitions");

            model.ProjectBasePath = System.IO.Path.GetDirectoryName(model.ProjectFilePath);

            var projectItems = new List<ProjectItem>();

            if (model.ProjectFilePath == null)
            {
                LogMessage($"Could not find basePath of {model.ProjectFilePath}");
            }
            if (model.ProjectBasePath != null)
            {
                var projectName = System.IO.Path.GetFileNameWithoutExtension(model.ProjectFilePath);
                Microsoft.Build.Evaluation.Project project = null;
                using (var projectFileReader = XmlReader.Create(model.ProjectFilePath))
                {
                    project = new Microsoft.Build.Evaluation.Project(projectFileReader);
                    LogMessage($"Loaded project {model.ProjectFilePath} from XML with {project.Items.Count} items");
                }

                var hasEventSource = false;
                foreach (
                    var projectItem in project.Items.Where(item => item.EvaluatedInclude.EndsWith(@".eventsource.json", StringComparison.InvariantCultureIgnoreCase))
                )
                {
                    var rootNamespace = project.Properties.FirstOrDefault(property => property.Name.Equals("RootNamespace"))?.EvaluatedValue ?? projectName;

                    var projectItemFilePath = System.IO.Path.Combine(model.ProjectBasePath, projectItem.EvaluatedInclude);
                    projectItems.Add(new ProjectItem<EventSourceModel>(ProjectItemType.EventSourceDefinition, projectItemFilePath)
                    {
                        Include = projectItem.EvaluatedInclude,
                        RootNamespace = rootNamespace
                    });
                    hasEventSource = true;
                }

                var platformProperty = project.Properties.FirstOrDefault(p => p.Name == "PlatformTarget")?.EvaluatedValue ?? "AnyCPU";
                model.Platform = platformProperty;

                foreach (var projectItem in project.Items.Where(item =>
                    item.EvaluatedInclude.Matches(@"(^|\\)I[^\\]*Logger.cs", StringComparison.InvariantCultureIgnoreCase, useWildcards: false)
                    && item.ItemType == "Compile"))
                {
                    var projectItemFilePath = System.IO.Path.Combine(model.ProjectBasePath, projectItem.EvaluatedInclude);
                    projectItems.Add(new ProjectItem<LoggerTemplateModel>(ProjectItemType.LoggerInterface, projectItemFilePath) {Include = projectItem.EvaluatedInclude});
                }

                foreach (var projectItem in project.Items.Where(item =>
                    item.EvaluatedInclude.Matches(@"(^|\\)[^\\]*BuilderExtension.cs", StringComparison.InvariantCultureIgnoreCase, useWildcards: false)
                    && item.ItemType == "Compile"))
                {
                    var projectItemFilePath = System.IO.Path.Combine(model.ProjectBasePath, projectItem.EvaluatedInclude);
                    projectItems.Add(new ProjectItem(ProjectItemType.BuilderExtension, projectItemFilePath) {Include = projectItem.EvaluatedInclude});
                }
                var anyHintPath = "";
                foreach (var projectItem in project.Items.Where(item => item.ItemType == "Reference"))
                {
                    var hintPath = projectItem.HasMetadata("HintPath") ? projectItem.GetMetadataValue("HintPath") : null;
                    hintPath = hintPath != null ? PathExtensions.GetAbsolutePath(model.ProjectBasePath, hintPath) : null;

                    anyHintPath = hintPath ?? anyHintPath;

                    var projectItemFilePath = hintPath == null ? $"{projectItem.EvaluatedInclude}.dll" : System.IO.Path.Combine(model.ProjectBasePath, hintPath);

                    projectItems.Add(new ProjectItem(ProjectItemType.Reference, projectItemFilePath) {Include = projectItem.EvaluatedInclude});
                }

                var outputPath =
                    project.Items.FirstOrDefault(item => item.ItemType.Equals("_OutputPathItem", StringComparison.InvariantCultureIgnoreCase))?.EvaluatedInclude;
                foreach (var projectItem in project.Items.Where(item => item.ItemType == "ProjectReference"))
                {
                    var referencedProjectPath = PathExtensions.GetAbsolutePath(model.ProjectBasePath, projectItem.EvaluatedInclude);
                    var referencedProjectName = System.IO.Path.GetFileNameWithoutExtension(projectItem.EvaluatedInclude);
                    var expectedDllName = $"{referencedProjectName}.dll";
                    var referencedProjectOutputPath = PathExtensions.GetAbsolutePath(System.IO.Path.GetDirectoryName(referencedProjectPath), outputPath);
                    var projectItemFilePath = System.IO.Path.Combine(referencedProjectOutputPath, expectedDllName);
                    if (System.IO.File.Exists(projectItemFilePath))
                    {
                        projectItems.Add(new ProjectItem(ProjectItemType.Reference, projectItemFilePath) {Include = projectItem.EvaluatedInclude});
                    }
                }

                if (!hasEventSource)
                {
                    var rootNamespace = project.Properties.FirstOrDefault(property => property.Name.Equals("RootNamespace"))?.EvaluatedValue ?? projectName;

                    var include = $"DefaultEventSource.eventsource.json";
                    var projectItemFilePath = System.IO.Path.Combine(model.ProjectBasePath, include);
                    projectItems.Add(new ProjectItem<EventSourceModel>(ProjectItemType.DefaultGeneratedEventSourceDefinition, projectItemFilePath)
                    {
                        Include = include,
                        RootNamespace = rootNamespace
                    });
                }
            }

            model.ProjectItems = projectItems.ToArray();
        }
    }
}