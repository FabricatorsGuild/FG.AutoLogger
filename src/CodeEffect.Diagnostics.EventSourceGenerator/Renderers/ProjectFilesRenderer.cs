using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class ProjectFilesRenderer : BaseWithLogging, IProjectRenderer
    {
        public bool SaveChanges { get; set; }

        public void Render(Project model)
        {
            foreach (var output in model.ProjectItems.OfType(
                        ProjectItemType.EventSource,
                        ProjectItemType.DefaultGeneratedEventSourceDefinition,
                        ProjectItemType.EventSourceLoggerPartial,
                        ProjectItemType.LoggerImplementation,
                        ProjectItemType.ProjectSummary))
            {

                var exists = System.IO.File.Exists(output.Name);
                var newHash = output.Output.ToMD5();

                var writeContent = false;
                if (exists)
                {
                    var existingContent = System.IO.File.ReadAllText(output.Name);
                    var existingHash = existingContent.ToMD5();

                    if (existingHash != newHash)
                    {
                        LogMessage($"Writing file {output.Name} as the content has changed");
                        writeContent = true;
                    }
                }
                else
                {
                    LogMessage($"Writing new file {output.Name}");
                    writeContent = true;
                }

                if (writeContent && SaveChanges)
                {
                    System.IO.File.WriteAllText(output.Name, output.Output);
                }               
            }
        }
    }
}