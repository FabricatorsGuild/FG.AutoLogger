using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Utils
{
    public class ComplierHelper : BaseWithLogging
    {



        private Assembly CompileInternal(
            string cscToolPath,
            IEnumerable<ProjectItem> sourceItems,
            IEnumerable<ProjectItem> referenceItems)
        {
            var x64 = Environment.Is64BitProcess;
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var errorLogFile = System.IO.Path.GetTempFileName();
            var tempOutput = System.IO.Path.GetTempFileName();
            var x64Swith = x64 ? " /platform:x64" : "";
            var references = referenceItems.Aggregate("", (s, i) => $"{s},\"{i.Name}\"").Substring(1);
            var sources = sourceItems.Aggregate("", (s, i) => $"{s} \"{i.Name}\"");
            var commandLineArgumentsBuilder = new StringBuilder();
            commandLineArgumentsBuilder.Append($"/reference:{references}");
            commandLineArgumentsBuilder.Append(sources);
            commandLineArgumentsBuilder.Append(" /target:library");
            commandLineArgumentsBuilder.Append($" /out:\"{tempOutput}\"");
            commandLineArgumentsBuilder.Append($" /errorlog:{errorLogFile}");
            if (x64)
            {
                commandLineArgumentsBuilder.Append(" /platform:x64");
            }
            var commandLineForProject = commandLineArgumentsBuilder.ToString();

            var cscExePath = System.IO.Path.Combine(cscToolPath, @"csc.exe");
            LogMessage($"Compiling sources {cscExePath} {commandLineForProject}");

            var process = Process.Start(new ProcessStartInfo(cscExePath, commandLineForProject) { CreateNoWindow = true });
            Assembly compiledAssembly = null;
            process.Exited += (sender, args) =>
            {
                compiledAssembly = Assembly.LoadFile(tempOutput);
            };
            

            var timeoutchecker = TimeSpan.Zero;            
            while ((!process.HasExited) && (timeoutchecker.TotalMilliseconds < 30000))
            {
                System.Threading.Tasks.Task.Delay(100).GetAwaiter().GetResult();
                timeoutchecker += TimeSpan.FromMilliseconds(100);
            }

            return compiledAssembly;
            /*

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
            */
        }

        public Assembly Compile(string cscToolPath, string code, IEnumerable<ProjectItem> referenceItems)
        {
            var tempCodeFile = $"{System.IO.Path.GetTempFileName()}.cs";
            System.IO.File.WriteAllText(tempCodeFile, code);
            var sourceProjectItem = new ProjectItem(ProjectItemType.Unknown, "temp source code", code);
            try
            {
                return CompileInternal(cscToolPath, new ProjectItem[] { sourceProjectItem}, referenceItems);
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate code source - {ex.Message}\r\n{ex.StackTrace}");
            }
            return null;
        }

        public Assembly Compile(string cscToolPath, IEnumerable<ProjectItem> projectItems, IEnumerable<ProjectItem> referenceItems)
        {
            try
            {
                return CompileInternal(cscToolPath, projectItems, referenceItems);

            }
            catch (Exception ex)
            {
                var files = projectItems?.Aggregate("", (s, i) => $"{s}, {i.Name}");
                LogWarning($"Failed to compile/evaluate {files} - {ex.Message}\r\n{ex.StackTrace}");
            }
            return null;
        }
    }
}