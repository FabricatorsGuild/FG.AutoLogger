using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            
            model.Extensions = DiscoverExtensions(model.ProjectItems, model.Platform);
        }

        private IExtension[] DiscoverExtensions(IEnumerable<ProjectItem> projectFiles, string platform)
        {
            var files = projectFiles.ToArray();

            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for extensions");
            var builderExtensions = new List<IExtension>();
            var extensionFiles = files.OfType(ProjectItemType.BuilderExtension);
            var referenceFiles = files.OfType(ProjectItemType.Reference).ToArray();
            foreach (var file in extensionFiles)
            {
                LogMessage($"Found Extension file {file.Name}");
                var extensions = CompileAndEvaluateExtensions(file, referenceFiles, platform);
                foreach (var foundExtensions in extensions)
                {
                    LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");

                }
                builderExtensions.AddRange(extensions);
            }
            LogMessage($"Scanning for Extensions in references only");
            var referencedExtensions = CompileAndEvaluateExtensions(referenceFiles, platform);
            foreach (var foundExtensions in referencedExtensions)
            {
                LogMessage($"Compiled Extension {foundExtensions.GetType().FullName}");

            }
            builderExtensions.AddRange(referencedExtensions);
            return builderExtensions.ToArray();
        }        

        private IExtension[] CompileAndEvaluateExtensions(IEnumerable<ProjectItem> referenceItems, string platform)
        {
            LogMessage($"Compiling possible logger builder extensions in referenced dlls");

            var extensions = new List<IExtension>();
            try
            {
                var complierHelper = new ComplierHelper();
                this.PassAlongLoggers(complierHelper);

                var loaderHelperDefinition = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Labs.ReferenceLoader
{
    public class ExtensionLoader
    {
        public Type[] GetExtensionTypes(AssemblyName[] references)
        {
            var types = new List<Type>();
            foreach (var referencedAssembly in references)
            {
                var assembly = Assembly.Load(referencedAssembly);
                Debug.WriteLine($""Loaded assembly {assembly.GetName().Name} with {assembly.GetTypes().Length} types"");
                types.AddRange(assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExtension).IsAssignableFrom(t)));
            }
            return types.ToArray();
        }
    }
}";

                var compiledAssembly = complierHelper.Compile(loaderHelperDefinition, referenceItems, platform);
                if (compiledAssembly != null)
                {
                    var references = referenceItems.Where(r => System.IO.Path.IsPathRooted(r.Name)).Select(r => AssemblyName.GetAssemblyName(r.Name)).ToArray();

                    var loaderType = compiledAssembly.GetType("CodeEffect.Labs.ReferenceLoader.ExtensionLoader");
                    var loader = Activator.CreateInstance(loaderType);
                    var types = (loaderType?.GetMethod("GetExtensionTypes")?.Invoke(loader, new object[] { references}) as Type[]) ?? new Type[0];
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


        private IExtension[] CompileAndEvaluateExtensions(ProjectItem projectItem, IEnumerable<ProjectItem> referenceItems, string platform)
        {
            LogMessage($"Compiling possible logger builder extension file {projectItem?.Include ?? "in referenced dlls"}");

            var extensions = new List<IExtension>();
            try
            {
                var complierHelper = new ComplierHelper();
                this.PassAlongLoggers(complierHelper);

                var compiledAssembly = complierHelper.Compile(projectItem, referenceItems, platform);                
                var types = compiledAssembly.GetTypes();                
                foreach (var type in types.Where(t => typeof(IExtension).IsAssignableFrom(t)))
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