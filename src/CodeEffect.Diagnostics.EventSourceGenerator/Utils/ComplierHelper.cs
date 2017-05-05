using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Utils
{
    public class ComplierHelper : BaseWithLogging
    {
        private Assembly Compile(Func<Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider, CompilerParameters, CompilerResults> compile,
            IEnumerable<ProjectItem> referenceItems, string platform)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var roslynDirectory = System.IO.Path.Combine(baseDirectory, "roslyn");
            var roslynBinDirectory = System.IO.Path.Combine(baseDirectory, "bin/roslyn");
            if (!System.IO.Directory.Exists(roslynBinDirectory))
            {
                System.IO.Directory.CreateDirectory(roslynBinDirectory);
                foreach (var filePath in System.IO.Directory.GetFiles(roslynDirectory))
                {
                    var fileName = System.IO.Path.GetFileName(filePath);
                    var newFilePath = System.IO.Path.Combine(roslynBinDirectory, fileName);
                    if (!System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Copy(filePath, newFilePath);
                    }
                }
            }

            var parameters = new CompilerParameters();

            foreach (var referenceItem in referenceItems)
            {
                parameters.ReferencedAssemblies.Add(referenceItem.Name);
            }

            //parameters.ReferencedAssemblies.Add("System.dll");
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            if (platform == "x64")
            {
                parameters.CompilerOptions = "/platform:x64";
            }

            parameters.IncludeDebugInformation = false;
            //var cSharpCodeProvider = new CSharpCodeProvider();
            var cSharpCodeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            var compilerResults = compile(cSharpCodeProvider, parameters);
            foreach (CompilerError compilerResultsError in compilerResults.Errors)
            {
                LogWarning(compilerResultsError.ToString());
            }

            return compilerResults.CompiledAssembly;
        }

        public Assembly Compile(string code, IEnumerable<ProjectItem> referenceItems, string platform)
        {
            try
            {
                return Compile((cSharpCodeProvider, parameters) => cSharpCodeProvider.CompileAssemblyFromSource(parameters, code), referenceItems, platform);
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate code source - {ex.Message}\r\n{ex.StackTrace}");
            }
            return null;
        }

        public Assembly Compile(ProjectItem projectItem, IEnumerable<ProjectItem> referenceItems, string platform)
        {
            try
            {
                return Compile((cSharpCodeProvider, parameters) => cSharpCodeProvider.CompileAssemblyFromFile(parameters, projectItem.Name), referenceItems, platform);
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate {projectItem.Include} - {ex.Message}\r\n{ex.StackTrace}");
            }
            return null;
        }
    }
}