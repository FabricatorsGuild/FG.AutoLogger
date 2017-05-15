using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;
using Microsoft.CSharp;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectExtensionsDiscoverBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            model.Extensions = DiscoverExtensions(model.ProjectItems, model.DynamicAssembly);
        }

        private IExtension[] DiscoverExtensions(IEnumerable<ProjectItem> projectFiles, Assembly dynamicAssembly)
        {
            var files = projectFiles.ToArray();

            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for extensions");
            var builderExtensions = new List<IExtension>();
            var extensionFiles = files.OfType(ProjectItemType.BuilderExtension);
            var currentAssemblyFileName = System.IO.Path.GetFileName(this.GetType().Assembly.CodeBase);
            var referenceFiles = files.OfType(ProjectItemType.Reference)
                .Where(r => ! System.IO.Path.GetFileName(r.Name).Equals(currentAssemblyFileName, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();
            /*foreach (var file in extensionFiles)
            {
                LogMessage($"Found Extension file {file.Name}");
                var extensions = CompileAndEvaluateExtensions(dynamicAssembly, file, referenceFiles);
                foreach (var foundExtensions in extensions)
                {
                    LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");

                }
                builderExtensions.AddRange(extensions);
            }*/
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

        /*
        private IExtension[] DiscoverExtensions(Assembly dynamicAssembly)
        {
            LogMessage($"Compiling possible logger builder extensions in referenced dlls");

            var extensions = new List<IExtension>();
            try
            {
                if (dynamicAssembly != null)
                {

                    AssemblyName.GetAssemblyName(dynamicAssembly)
                    var references = referenceItems.Where(r => System.IO.Path.IsPathRooted(r.Name)).Select(r => AssemblyName.GetAssemblyName(dynamicAssembly.Name)).ToArray();

                    var loaderType = dynamicAssembly.GetType("CodeEffect.Labs.ReferenceLoader.ExtensionLoader");
                    var loader = Activator.CreateInstance(loaderType);
                    var types = (loaderType?.GetMethod("GetExtensionTypes")?.Invoke(loader, new object[] { references }) as Type[]) ?? new Type[0];
                    foreach (var type in types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
                    {
                        var extension = (IExtension)Activator.CreateInstance(type);
                        extensions.Add(extension);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate references - {ex.Message}\r\n{ex.StackTrace}");
            }
            return extensions.ToArray();
        }
        */

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
                //                var complierHelper = new ComplierHelper();
                //                this.PassAlongLoggers(complierHelper);

                //                var loaderHelperDefinition = @"
                //using System;
                //using System.Collections.Generic;
                //using System.Diagnostics;
                //using System.Linq;
                //using System.Reflection;
                //using CodeEffect.Diagnostics.EventSourceGenerator.Model;

                //namespace CodeEffect.Labs.ReferenceLoader
                //{
                //    public class ExtensionLoader
                //    {
                //        public Type[] GetExtensionTypes(AssemblyName[] references)
                //        {
                //            var types = new List<Type>();
                //            foreach (var referencedAssembly in references)
                //            {
                //                var assembly = Assembly.Load(referencedAssembly);
                //                Debug.WriteLine($""Loaded assembly {assembly.GetName().Name} with {assembly.GetTypes().Length} types"");
                //                types.AddRange(assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExtension).IsAssignableFrom(t)));
                //            }
                //            return types.ToArray();
                //        }
                //    }
                //}";

                //var compiledAssembly = complierHelper.Compile(cscToolPath, loaderHelperDefinition, referenceItems);
                if (dynamicAssembly != null)
                {
                    var types = dynamicAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExtension).IsAssignableFrom(t));
                    foreach (var type in types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
                    {
                        var extension = (IExtension) Activator.CreateInstance(type);
                        extensions.Add(extension);
                    }
                    /*
                    var references = referenceItems.Where(r => System.IO.Path.IsPathRooted(r.Name)).Select(r => AssemblyName.GetAssemblyName(r.Name)).ToArray();

                    var loaderType = dynamicAssembly.GetType("CodeEffect.Labs.ReferenceLoader.ExtensionLoader");
                    var loader = Activator.CreateInstance(loaderType);
                    var types = (loaderType?.GetMethod("GetExtensionTypes")?.Invoke(loader, new object[] { references}) as Type[]) ?? new Type[0];
                    foreach (var type in types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
                    {
                        var extension = (IExtension)Activator.CreateInstance(type);
                        extensions.Add(extension);
                    }
                    */
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

                //var compiledAssembly = complierHelper.Compile(cscToolPath, projectItem, referenceItems);                
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