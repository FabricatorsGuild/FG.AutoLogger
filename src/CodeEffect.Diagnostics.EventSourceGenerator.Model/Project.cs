using System.Collections.Generic;
using System.Reflection;

namespace FG.Diagnostics.AutoLogger.Model
{
    public class Project
    {
        public string ProjectFilePath { get; set; }
        public string ProjectBasePath { get; set; }
        public ProjectItem[] ProjectItems { get; set; }
        public LoggerTemplateModel[] Loggers { get; set; }
        public IExtension[] Extensions { get; set; }
        public string Platform { get; set; }
        public string CscToolPath { get; set; }
        public Assembly DynamicAssembly { get; set; }
        public bool HasProjectChanges { get; set; }
        public bool ForceUpdate { get; set; }
        public List<string> ToolModuleReferences { get; set; }
        public string RootNamespace { get; set; }
        public string AssemblyName { get; set; }
    }
}