using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectExtensionsDiscoverBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            var extensions = model.Extensions?.ToList() ?? new List<IExtension>();
            extensions.AddRange(DiscoverExtensions(model.ProjectItems, model.DynamicAssembly));
            model.Extensions = extensions.ToArray();
        }

        private IExtension[] DiscoverExtensions(IEnumerable<ProjectItem> projectFiles, Assembly dynamicAssembly)
        {
            var files = projectFiles.ToArray();

            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for extensions");
            var builderExtensions = new List<IExtension>();
            var extensionFiles = files.OfType(ProjectItemType.BuilderExtension);
            var currentAssemblyFileName = System.IO.Path.GetFileName(this.GetType().Assembly.CodeBase);
            var referenceFiles = files.OfType(ProjectItemType.Reference).Union(files.OfType(ProjectItemType.ProjectReference))
                .Where(r => ! System.IO.Path.GetFileName(r.Name).Equals(currentAssemblyFileName, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

            LogMessage($"Scanning for Extensions in code source only");
            var compiledExtensions = CompileAndEvaluateExtensions(dynamicAssembly);
            foreach (var foundExtensions in compiledExtensions)
            {
                LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");
            }
            builderExtensions.AddRange(compiledExtensions);

            LogMessage($"Scanning for Extensions in references");
            var extensionsInReferences = FindExtensionsInReferences(referenceFiles.Select(r => r.Name));
            foreach (var referencedExtensions in extensionsInReferences)
            {
                LogMessage($"Referenced Extension {referencedExtensions.GetType().FullName}");
            }
            builderExtensions.AddRange(extensionsInReferences);
            return builderExtensions.ToArray();
        }        

        private IExtension[] FindExtensionsInReferences(IEnumerable<string> referenceFiles)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                LogMessage($"Trying to load assembly {args.Name}");

                var assemblyName = args.Name.Split(',')[0];
                var reference = referenceFiles.FirstOrDefault(r => r.Matches($"*{assemblyName}.dll", StringComparison.InvariantCultureIgnoreCase, true));
                if (reference != null)
                {
                    
                    var assembly = Assembly.LoadFile(reference);
                    LogMessage($"Loaded assembly {assembly.FullName}");
                    return assembly;
                }

                return null;
            };

            var types = new List<Type>();
            foreach (var referencedAssemblyFile in referenceFiles)
            {
                try
                {
                    if (System.IO.Path.IsPathRooted(referencedAssemblyFile))
                    {
                        var assembly = Assembly.LoadFrom(referencedAssemblyFile);
                        Debug.WriteLine($"Loaded assembly {assembly.GetName().Name} with {assembly.GetTypes().Length} types");
                        types.AddRange(assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExtension).IsAssignableFrom(t)));
                    }
                }
                catch (Exception ex)
                {
                    if (ex is System.Reflection.ReflectionTypeLoadException)
                    {
                        var typeLoadException = ex as ReflectionTypeLoadException;
                        var loaderExceptions = typeLoadException.LoaderExceptions;
                        LogWarning($"Failed to load references {referencedAssemblyFile} - {ex.Message}");
                        foreach (var loaderException in loaderExceptions)
                        {
                            LogWarning($"\t{loaderException.Message}");
                        }

                    }
                    else
                    {
                        LogWarning($"Failed to load references {referencedAssemblyFile} - {ex.Message}");
                    }
                }

            }

            var extensions = new List<IExtension>();
            foreach (var type in types)
            {
                var extension = Activator.CreateInstance(type) as IExtension;
                if (extension != null)
                {
                    extensions.Add(extension);
                }
            }
            return extensions.ToArray();
        }        

        private IExtension[] CompileAndEvaluateExtensions(Assembly dynamicAssembly)
        {
            LogMessage($"Compiling possible logger builder extensions in referenced dlls");

            var extensions = new List<IExtension>();
            try
            {
                if (dynamicAssembly != null)
                {
                    var types = dynamicAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExtension).IsAssignableFrom(t));
                    foreach (var type in types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
                    {
                        try
                        {
                            var extension = (IExtension) Activator.CreateInstance(type);
                            extensions.Add(extension);
                        }
                        catch (Exception ex)
                        {
                            LogWarning($"Failed to create instance of type {type?.Name} - {ex.Message}\r\n{ex.StackTrace}");
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var exLoaderException in ex.LoaderExceptions)
                {
                    LogWarning($"Failed to compile/evaluate references - {exLoaderException.Message}\r\n{exLoaderException.StackTrace}");
                }                
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate references - {ex.Message}\r\n{ex.StackTrace}");
            }
            return extensions.ToArray();
        }

        private IExtension[] CompileAndEvaluateExtensions(Assembly dynamicAssembly, ProjectItem projectItem, IEnumerable<ProjectItem> referenceItems)
        {
            LogMessage($"Compiling possible logger builder extension file {projectItem?.Include ?? "in referenced dlls"}");

            var extensions = new List<IExtension>();
            try
            {
                var complierHelper = new ComplierHelper();
                this.PassAlongLoggers(complierHelper);

                var types = dynamicAssembly.GetTypes();
                foreach (var type in types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
                {
                    var extension = (IExtension) Activator.CreateInstance(type);
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