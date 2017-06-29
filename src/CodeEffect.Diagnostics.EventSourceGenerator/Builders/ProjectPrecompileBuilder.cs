using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectPrecompileBuilder : BaseCoreBuilder, IProjectBuilder
    {
        public void Build(Project model)
        {
            var dynamicAssembly = DiscoverDynamicAssemblies(model.ProjectItems, model.CscToolPath);
            model.DynamicAssembly = dynamicAssembly;
        }

        private Assembly DiscoverDynamicAssemblies(IEnumerable<ProjectItem> projectFiles, string cscToolPath)
        {
            var files = projectFiles.ToArray();

            var extensionFiles = files.OfType(ProjectItemType.BuilderExtension);
            var referenceFiles = files.OfType(ProjectItemType.Reference).Union(files.OfType(ProjectItemType.ProjectReference)).ToArray();
            var loggerProjectItems = files.OfType<LoggerTemplateModel>(ProjectItemType.LoggerInterface);

            var compileFiles = new List<ProjectItem>();
            compileFiles.AddRange(extensionFiles);
            compileFiles.AddRange(loggerProjectItems);

            //var loaderHelperFile = CreateLoaderHelperClass();
            //compileFiles.Add(loaderHelperFile);

            var complierHelper = new ComplierHelper();
            this.PassAlongLoggers(complierHelper);
            var compiledAssembly = complierHelper.Compile(cscToolPath, compileFiles, referenceFiles);

            return compiledAssembly;
        }

        private ProjectItem CreateLoaderHelperClass()
        {
            var loaderHelperDefinition = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Model;

namespace CodeEffect.Labs.ReferenceLoader
{
    public class ExtensionLoader
    {
        public Type[] GetExtensionTypes(string[] referenceFiles)
        {
            var types = new List<Type>();
            foreach (var referencedAssemblyFile in referenceFiles)
            {
                var assembly = Assembly.LoadFile(referencedAssemblyFile);
                Debug.WriteLine($""Loaded assembly {assembly.GetName().Name} with {assembly.GetTypes().Length} types"");
                types.AddRange(assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExtension).IsAssignableFrom(t)));
            }
            return types.ToArray();
        }


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
            var tempCodeFile = $"{System.IO.Path.GetTempFileName()}.cs";
            System.IO.File.WriteAllText(tempCodeFile, loaderHelperDefinition);
            var loaderHelperProjectItem = new ProjectItem(ProjectItemType.Unknown, tempCodeFile, loaderHelperDefinition);
            return loaderHelperProjectItem;
        }
    }
}