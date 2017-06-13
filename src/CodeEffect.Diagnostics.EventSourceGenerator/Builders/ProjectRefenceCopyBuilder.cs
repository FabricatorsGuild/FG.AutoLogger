using System;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectRefenceCopyBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            var references = model.ProjectItems.OfType(ProjectItemType.Reference).Union(model.ProjectItems.OfType(ProjectItemType.ProjectReference));
            var tempOutput = System.IO.Path.Combine(System.IO.Path.GetTempPath(), model.ProjectFilePath.ToMD5().ToHex());
            var tempOutputPrimary = System.IO.Path.Combine(tempOutput, "a");
            var tempOutputSecondary = System.IO.Path.Combine(tempOutput, "b");
            System.IO.Directory.CreateDirectory(tempOutput);
            foreach (var reference in references)
            {
                CopyFileToDestination(reference, tempOutputPrimary, tempOutputSecondary);
            }
        }

        private void CopyFileToDestination(ProjectItem reference, string destinationFolder, string secondaryDestinationFolder)
        {
            if (System.IO.Path.IsPathRooted(reference.Name))
            {
                var fileName = System.IO.Path.GetFileName(reference.Name);
                var destination = System.IO.Path.Combine(destinationFolder, fileName);
                try
                {
                    if (System.IO.File.Exists(destination))
                    {
                        System.IO.File.Copy(reference.Name, destination, overwrite: true);
                        LogMessage($"Overwriting reference {destination} as it already exists");
                    }
                    else
                    {
                        System.IO.File.Copy(reference.Name, destination);
                        LogMessage($"Copied reference to {destination}");
                    }
                    reference.Name = destination;
                }
                catch (Exception ex)
                {
                    LogWarning($"Failed to copy reference {reference.Name} to temp directory");

                    if (secondaryDestinationFolder != null)
                    {
                        CopyFileToDestination(reference, secondaryDestinationFolder, null);
                    }
                }
            }
        }
    }
}