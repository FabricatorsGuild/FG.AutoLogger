using System;
using System.Linq;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectRefenceCopyBuilder : BaseCoreBuilder, IProjectBuilder
    {
        public void Build(Project model)
        {
            var references = model.ProjectItems.OfType(ProjectItemType.Reference).Union(model.ProjectItems.OfType(ProjectItemType.ProjectReference));
            var tempOutput = System.IO.Path.Combine(System.IO.Path.GetTempPath(), model.ProjectFilePath.ToMD5().ToHex());
            var tempOutputPrimary = System.IO.Path.Combine(tempOutput, "a");
            var tempOutputSecondary = System.IO.Path.Combine(tempOutput, "b");
            System.IO.Directory.CreateDirectory(tempOutputPrimary);
            System.IO.Directory.CreateDirectory(tempOutputSecondary);
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
                    LogWarning($"Failed to copy reference {reference.Name} to temp directory - {ex.Message}");

                    if (secondaryDestinationFolder != null)
                    {
                        CopyFileToDestination(reference, secondaryDestinationFolder, null);
                    }
                }
            }
        }
    }
}