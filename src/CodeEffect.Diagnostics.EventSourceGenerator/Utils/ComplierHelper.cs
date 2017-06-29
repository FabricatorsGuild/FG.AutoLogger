using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Utils
{
    public class ComplierHelper : BaseWithLogging
    {
        private Assembly CompileInternal(
            string cscToolPath,
            IEnumerable<ProjectItem> sourceItems,
            IEnumerable<ProjectItem> referenceItems)
        {
            var x64 = Environment.Is64BitProcess;

            var errorLogFile = System.IO.Path.GetTempFileName();
            var tempOutput = System.IO.Path.GetTempFileName();
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
            var roslynDirectory = System.IO.Path.Combine(cscToolPath, "roslyn");
            if (System.IO.Directory.Exists(roslynDirectory))
            {
                var roslynCscExePath = System.IO.Path.Combine(roslynDirectory, @"csc.exe");
                if (System.IO.File.Exists(roslynCscExePath))
                {
                    LogMessage($"Found roslyn compiler at {roslynCscExePath}");
                    cscExePath = roslynCscExePath;
                }
            }
            
            LogMessage($"Compiling sources {cscExePath} {commandLineForProject}");
            Assembly compiledAssembly = null;

            try
            {
                var process = Process.Start(new ProcessStartInfo(cscExePath, commandLineForProject)
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                });
                while (!process.StandardOutput.EndOfStream)
                {
                    LogMessage(process.StandardOutput.ReadLine());
                }
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
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(errorLogFile))
                {
                    var errorLogFileContent = System.IO.File.ReadAllText(errorLogFile);
                    LogError(errorLogFileContent);
                }
            }

            return compiledAssembly;
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