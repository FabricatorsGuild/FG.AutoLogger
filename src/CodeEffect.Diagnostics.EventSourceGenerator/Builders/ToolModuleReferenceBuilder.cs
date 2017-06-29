using System.Collections.Generic;
using System.IO;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ToolModuleReferenceBuilder : BaseCoreBuilder, IProjectBuilder
    {
        public void Build(Project model)
        {
            var cscToolPath = model.CscToolPath;

            if (cscToolPath.EndsWith("roslyn"))
            {
                cscToolPath = System.IO.Path.GetDirectoryName(cscToolPath);
            }
            var modulesPath = System.IO.Path.Combine(cscToolPath, @"modules");
            var moduleReferences = new List<string>();
            if (System.IO.Directory.Exists(modulesPath))
            {
                foreach (var modulePath in System.IO.Directory.GetDirectories(modulesPath))
                {
                    var dllFilesInModule = System.IO.Directory.GetFiles(modulePath, "*.dll", SearchOption.AllDirectories);
                    moduleReferences.AddRange(dllFilesInModule);
                }
            }
            model.ToolModuleReferences = moduleReferences;
        }
    }
}