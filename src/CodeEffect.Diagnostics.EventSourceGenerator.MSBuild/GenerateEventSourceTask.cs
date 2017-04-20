using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using Microsoft.Build.Evaluation;

namespace CodeEffect.Diagnostics.EventSourceGenerator.MSBuild
{
    public class GenerateEventSourceTask : Microsoft.Build.Utilities.Task
    {
        public string ProjectFilePath { get; set; }
        public override bool Execute()
        {
            Log.LogMessage($"Executing {nameof(GenerateEventSourceTask)} for project {ProjectFilePath}");

            var projectEventSourceGenerator = new ProjectEventSourceGenerator();
            projectEventSourceGenerator.SetLogMessage(m => Log.LogMessage(m));
            projectEventSourceGenerator.SetLogWarning(w => Log.LogWarning(w));
            projectEventSourceGenerator.SetLogMessage(e => Log.LogError(e));

            var project = projectEventSourceGenerator.Run(ProjectFilePath);

            //var projectBuilder = new ProjectBuilder(message => Log.LogMessage(message));            
            //projectBuilder.Build(project);

            //var builder = new EventSourceBuilder(message => Log.LogMessage(message));
            //var outputs = builder.Build(project);

            foreach (var output in project.ProjectItems)
            {
                Log.LogMessage($"Writing file {output.Name}");
                System.IO.File.WriteAllText(output.Name, output.Content.ToString());
            }

            Log.LogMessage($"Updating project file {ProjectFilePath}");
            //builder.AddGeneratedOutputsToProject(ProjectFilePath, outputs);

            Log.LogMessage($"Executed {nameof(GenerateEventSourceTask)} in {ProjectFilePath}");

            return true;
        }
    }
}
