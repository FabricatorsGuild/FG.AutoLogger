using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectExtensionsDiscoverBuilder : BaseCoreBuilder, IProjectBuilder
    {
        public void Build(Project model)
        {
            var extensions = model.Extensions?.ToList() ?? new List<IExtension>();
            extensions.AddRange(DiscoverExtensions(model.CscToolPath, model.ProjectItems, model.DynamicAssembly));
            model.Extensions = extensions.ToArray();
        }

        private IExtension[] DiscoverExtensions(string cscToolPath, IEnumerable<ProjectItem> projectFiles, Assembly dynamicAssembly)
        {
            var files = projectFiles.ToArray();

            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for extensions");
            var builderExtensions = new List<IExtension>();
            var extensionFiles = files.OfType(ProjectItemType.BuilderExtension);
            var currentAssemblyFileName = System.IO.Path.GetFileName(this.GetType().Assembly.CodeBase);
            var referenceFiles = files.OfType(ProjectItemType.Reference).Union(files.OfType(ProjectItemType.ProjectReference))
                .Where(r => ! System.IO.Path.GetFileName(r.Name)?.Equals(currentAssemblyFileName, StringComparison.InvariantCultureIgnoreCase) ?? false)
                .ToArray();

            LogMessage($"Scanning for Extensions in code source only");
            var compiledExtensions = CompileAndEvaluateExtensions(dynamicAssembly, referenceFiles);
            foreach (var foundExtensions in compiledExtensions)
            {
                LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");
            }
            builderExtensions.AddRange(compiledExtensions);

            LogMessage($"Scanning for Extensions in references");
            var extensionsInReferences = FindExtensionsInReferences(referenceFiles.Select(r => r.Name).ToArray());
            foreach (var referencedExtensions in extensionsInReferences)
            {
                LogMessage($"Referenced Extension {referencedExtensions.GetType().FullName}");
            }
            builderExtensions.AddRange(extensionsInReferences);

            LogMessage($"Scanning for Extensions in tool modules folder");
            var extensionsInModules = FindExtensionsInToolModulesAssemblies(cscToolPath);
            builderExtensions.AddRange(extensionsInModules);

            foreach (var extensionGroup in builderExtensions.GroupBy(ext => ext.Module))
            {
                LogMessage($"Loaded extension group {extensionGroup.Key}");

                foreach (var extension in extensionGroup)
                {
                    LogMessage($"Loaded Extension {extension.GetType().FullName}");
                }
            }

            return builderExtensions.ToArray();
        }

        private IExtension[] FindExtensionsInToolModulesAssemblies(string cscToolPath)
        {
            if (cscToolPath.EndsWith("roslyn"))
            {
                cscToolPath = System.IO.Path.GetDirectoryName(cscToolPath);
            }
            var modulesPath = System.IO.Path.Combine(cscToolPath, @"modules");
            if (System.IO.Directory.Exists(modulesPath))
            {
                var extensions = new List<IExtension>();
                foreach (var modulePath in System.IO.Directory.GetDirectories(modulesPath))
                {
                    var dllFilesInModule = System.IO.Directory.GetFiles(modulePath, "*.dll", SearchOption.AllDirectories);
                    var extensionsInModule = FindExtensionsInReferences(dllFilesInModule);
                    extensions.AddRange(extensionsInModule);
                }
                return extensions.ToArray();
            }
            return new IExtension[0];
        }

        private bool IsTypeIExtensionFromOtherAssembly(Type type)
        {
            if (!type.IsClass) return false;
            if (type.IsAbstract) return false;

            return typeof(IExtension).IsAssignableFrom(type);
        }

        private bool IsInterfaceTypeSameButFromAnExternalAssembly<TTypeToCompareTo>(Type loadedType)
        {
            return ((loadedType.FullName == typeof(TTypeToCompareTo).FullName) &&
                    (loadedType.Assembly.FullName == typeof(TTypeToCompareTo).Assembly.FullName));
        }

        private IExtension[] FindExtensionsInReferences(IEnumerable<string> referenceFiles)
        {
            var referenceFilesArray = referenceFiles.ToArray();
            //var assemblyResolver = new AssemblyResolver(referenceFilesArray);
            //AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.CurrentDomainOnAssemblyResolve;
            
            var types = new List<Type>();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var loadedAssemblyNames = loadedAssemblies.Select(a => a.FullName.Split(',')[0]).ToArray();
            foreach (var referencedAssemblyFile in referenceFilesArray)
            {
                try
                {
                    if (System.IO.Path.IsPathRooted(referencedAssemblyFile))
                    {
                        var assemblyName = System.IO.Path.GetFileNameWithoutExtension(referencedAssemblyFile);
                        if (!loadedAssemblyNames.Contains(assemblyName))
                        {
                            try
                            {
                                var assembly = Assembly.LoadFile(referencedAssemblyFile);
                                Debug.WriteLine($"Loaded assembly {assembly.GetName().Name} with {assembly.GetTypes().Length} types");
                                types.AddRange(assembly
                                    .GetTypes()
                                    .Where(IsTypeIExtensionFromOtherAssembly)
                                    .ToArray());
                            }
                            catch (System.BadImageFormatException badImageFormatException)
                            {
                                LogError($"Failed to load {referencedAssemblyFile}, {badImageFormatException.Message}");
                            }

                        }
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

            //AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver.CurrentDomainOnAssemblyResolve;
            return extensions.ToArray();
        }        

        private IExtension[] CompileAndEvaluateExtensions(Assembly dynamicAssembly, IEnumerable<ProjectItem> references)
        {
            LogMessage($"Compiling possible logger builder extensions in referenced dlls");

            //var assemblyResolver = new AssemblyResolver(dynamicAssembly, references.Select(r => r.Name).ToArray());
            //AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.CurrentDomainOnAssemblyResolve;

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

            //AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver.CurrentDomainOnAssemblyResolve;
            return extensions.ToArray();
        }
    }
}