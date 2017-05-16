using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectSummaryBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            var extensions = model.ProjectItems.OfType(ProjectItemType.BuilderExtension);
            var loggers = model.ProjectItems.OfType(ProjectItemType.LoggerInterface);
            var references = model.ProjectItems.OfType(ProjectItemType.Reference);
            var eventSourceDefinitions = model.ProjectItems.OfType(ProjectItemType.EventSourceDefinition);

            var summary = new ProjectSummary()
            {
                CompileTargets = extensions.Select(GetExtensionSummary).Union(loggers.Select(GetLoggerSummary)),
                References = references.Select(r => new ProjectSummary.ProjectSummaryReferenceSummary() {ReferenceName = r.Name}),
                EventSources = eventSourceDefinitions.Select(GetEventSourceDefinitionSummary),
            };
            var summaryContent = Newtonsoft.Json.JsonConvert.SerializeObject(summary);
            var summaryHash = summaryContent.ToMD5().ToHex();

            model.HasProjectChanges = model.SummaryHash != summaryHash;
            model.SummaryHash = summaryHash;
        }

        private ProjectSummary.ProjectSummaryEventSource GetEventSourceDefinitionSummary(ProjectItem eventSourceDefinitionProjectItem)
        {
            var content = System.IO.File.ReadAllText(eventSourceDefinitionProjectItem.Name);
            var hash = content.ToMD5().ToHex();
            return new ProjectSummary.ProjectSummaryEventSource() { Name = eventSourceDefinitionProjectItem.Name, Hash = hash };
        }

        private ProjectSummary.ProjectSummaryCompileTarget GetExtensionSummary(ProjectItem extensionProjectItem)
        {
            var content = System.IO.File.ReadAllText(extensionProjectItem.Name);
            var hash = content.ToMD5().ToHex();
            return new ProjectSummary.ProjectSummaryCompileTarget() {Name = extensionProjectItem.Name, Hash = hash};
        }

        private ProjectSummary.ProjectSummaryCompileTarget GetLoggerSummary(ProjectItem loggerProjectItem)
        {
            var content = System.IO.File.ReadAllText(loggerProjectItem.Name);
            var hash = content.ToMD5().ToHex();
            return new ProjectSummary.ProjectSummaryCompileTarget() { Name = loggerProjectItem.Name, Hash = hash };
        }

        public class ProjectSummary
        {
            public IEnumerable<ProjectSummaryEventSource> EventSources { get; set; }

            public IEnumerable<ProjectSummaryReferenceSummary> References { get; set; }

            public IEnumerable<ProjectSummaryCompileTarget> CompileTargets { get; set; }


            public class ProjectSummaryEventSource
            {
                public string Name { get; set; }
                public string Hash { get; set; }
            }

            public class ProjectSummaryCompileTarget
            {
                public string Name { get; set; }
                public string Hash { get; set; }
            }

            public class ProjectSummaryReferenceSummary
            {
                public string ReferenceName { get; set; }
            }
        }
    }
}