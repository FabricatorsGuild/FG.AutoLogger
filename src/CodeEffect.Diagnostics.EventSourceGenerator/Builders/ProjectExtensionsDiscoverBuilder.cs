using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;
using Microsoft.CSharp;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectExtensionsDiscoverBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            model.Extensions = DiscoverExtensions(model.ProjectItems);
        }

        private IExtension[] DiscoverExtensions(IEnumerable<ProjectItem> projectFiles)
        {
            var files = projectFiles.ToArray();

            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for extensions");
            var builderExtensions = new List<IExtension>();
            //var loggerFiles = files.Where(projectFile => projectFile.Matches(@"(^|\\)I[^\\]*Logger.cs", StringComparison.InvariantCultureIgnoreCase, useWildcards: false));
            var extensionFiles = files.OfType(ProjectItemType.BuilderExtension);
            var referenceFiles = files.OfType(ProjectItemType.Reference).ToArray();
            foreach (var file in extensionFiles)
            {
                LogMessage($"Found Extension file {file.Name}");
                var extensions = CompileAndEvaluateExtensions(file, referenceFiles);
                foreach (var foundExtensions in extensions)
                {
                    LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");

                }
                builderExtensions.AddRange(extensions);
            }
            if (!extensionFiles.Any())
            {
                LogMessage($"Scanning for Extensions in references only");
                var extensions = CompileAndEvaluateExtensions(null, referenceFiles);
                foreach (var foundExtensions in extensions)
                {
                    LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");

                }
                builderExtensions.AddRange(extensions);
            }
            return builderExtensions.ToArray();
        }

        private IExtension[] CompileAndEvaluateExtensions(ProjectItem projectItem, IEnumerable<ProjectItem> referenceItems)
        {
            LogMessage($"Compiling possible logger builder extension file {projectItem?.Include ?? "in referenced dlls"}");

            var extensions = new List<IExtension>();
            try
            {
                var parameters = new CompilerParameters();

                foreach (var referenceItem in referenceItems)
                {
                    parameters.ReferencedAssemblies.Add(referenceItem.Name);
                }

                //parameters.ReferencedAssemblies.Add("System.dll");
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;

                parameters.IncludeDebugInformation = false;
                var cSharpCodeProvider = new CSharpCodeProvider();
                //var cSharpCodeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

                var items = projectItem != null ? new string[] { projectItem.Name } : new string[0];
                CompilerResults compilerResults;
                compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(parameters, items);
                foreach (CompilerError compilerResultsError in compilerResults.Errors)
                {
                    LogWarning(compilerResultsError.ToString());
                }

                var types = compilerResults.CompiledAssembly.GetTypes();
                foreach (
                    var type in
                    types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
                {
                    var extension = (IExtension)Activator.CreateInstance(type);
                    extensions.Add(extension);
                }
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate {projectItem.Include} - {ex.Message}\r\n{ex.StackTrace}");
            }
            return extensions.ToArray();
        }

    }
}