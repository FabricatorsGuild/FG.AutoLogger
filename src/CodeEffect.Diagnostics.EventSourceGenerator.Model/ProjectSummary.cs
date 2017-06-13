using System.Collections.Generic;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class ProjectSummary
    {
        public ProjectSummaryEventSource[] EventSources { get; set; }

        public string[] References { get; set; }
        public ProjectSummaryProjectReferenceSummary[] ProjectReferences { get; set; }

        public ProjectSummaryCompileTarget[] CompileTargets { get; set; }


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

        public class ProjectSummaryProjectReferenceSummary
        {
            public string ReferenceName { get; set; }
            public string Hash { get; set; }
        }

        public string Hash { get; set; }
    }
}