using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Build.Evaluation;

namespace CodeEffect.Diagnostics.EventSourceGenerator.MSBuild
{
    public class GenerateEventSourceTask : Microsoft.Build.Utilities.Task
    {
        public string ProjectFilePath { get; set; }
        public override bool Execute()
        {
            Log.LogMessage($"Executing {nameof(GenerateEventSourceTask)} for project {ProjectFilePath}");

            var builder = new EventSourceBuilder(message => Log.LogMessage(message));
            var projectItems = builder.GetProjectItems(ProjectFilePath);
            var projectFileBasePath = System.IO.Path.GetDirectoryName(ProjectFilePath);
            var outputs = builder.Execute(projectFileBasePath, projectItems);

            foreach (var output in outputs)
            {
                Log.LogMessage($"Writing file {output.Name}");
                System.IO.File.WriteAllText(output.Name, output.Content);
            }

            Log.LogMessage($"Updating project file {ProjectFilePath}");
            builder.AddGeneratedOutputsToProject(ProjectFilePath, outputs);

            Log.LogMessage($"Executed {nameof(GenerateEventSourceTask)} in {ProjectFilePath}");

            return true;
        }
    }
}
