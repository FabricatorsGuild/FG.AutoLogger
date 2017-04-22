using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Schema;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;
using CommandLine;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                LogMessage("Enter arguments for program:");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) line = "-o -s -g";
                args = line.Split(' ');
            }

            var options = Parser.Default.ParseArguments<ConsoleOptions>(args);
            options.WithParsed(t =>
            {
                if (t.Verbose) LogMessage($"Filename: {t.ProjectFile}");

                if (t.ProjectFile == null)
                {
                    var possibleProjectFiles = System.IO.Directory.GetFiles("*.csproj");
                    if (possibleProjectFiles.Any())
                    {
                        t.ProjectFile = possibleProjectFiles.First();
                    }
                }

                if (!System.IO.File.Exists(t.ProjectFile))
                {
                    LogMessage($"The project file {t.ProjectFile} could not be found", EventLevel.Critical);
                    return;
                }

                if (!System.IO.Path.IsPathRooted(t.ProjectFile))
                {
                    t.ProjectFile = PathExtensions.GetAbsolutePath(t.ProjectFile);
                }
                var projectFilePath = t.ProjectFile;


                var projectEventSourceGenerator = new ProjectEventSourceGenerator();
                projectEventSourceGenerator.SetLogMessage(m => LogMessage(m, EventLevel.Informational));
                projectEventSourceGenerator.SetLogWarning(w => LogMessage(w, EventLevel.Warning));
                projectEventSourceGenerator.SetLogError(e => LogMessage(e, EventLevel.Error));

                var project = projectEventSourceGenerator.Run(projectFilePath, t.SaveChanges);

                if (t.DisplayOutput)
                {
                    foreach (var output in project.ProjectItems.OfType(
                        ProjectItemType.EventSource, 
                        ProjectItemType.DefaultGeneratedEventSourceDefinition,
                        ProjectItemType.EventSourceLoggerPartial,
                        ProjectItemType.LoggerImplementation))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{"".PadRight(40, '_')}");
                        Console.WriteLine($"{"".PadRight(40, '=')}");
                        Console.WriteLine($"File: {output.Name}");
                        Console.WriteLine($"{"".PadRight(40, '_')}");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(output.Output);


                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{"".PadRight(40, '_')}");
                        Console.WriteLine($"{"".PadRight(40, '=')}");
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"".PadRight(40, '_')}");
                    Console.WriteLine($"{"".PadRight(40, '=')}");
                    Console.WriteLine($"File: {projectFilePath}");
                    Console.WriteLine($"{"".PadRight(40, '_')}");

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"".PadRight(40, '_')}");
                    Console.WriteLine($"{"".PadRight(40, '=')}");

                }


                if (t.GenerateSchema)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Generating JSON Schema");

                    var output = SchemaWriter.GenerateSchema(t.SaveChanges).GetAwaiter().GetResult();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"".PadRight(40, '_')}");
                    Console.WriteLine($"{"".PadRight(40, '=')}");
                    Console.WriteLine($"File: {output.Name}");
                    Console.WriteLine($"{"".PadRight(40, '_')}");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(output.Output);


                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"".PadRight(40, '_')}");
                    Console.WriteLine($"{"".PadRight(40, '=')}");

                    Console.WriteLine($"Done generating JSON Schema");
                }
            });
            Console.ReadKey();
        }

        private static void LogMessage(string message, EventLevel eventLevel)
        {
            var previousConsoleColor = Console.ForegroundColor;
            var consoleColor = Console.ForegroundColor;

            switch (eventLevel)
            {
                case (EventLevel.Critical):
                case (EventLevel.Error):
                    consoleColor = ConsoleColor.Red;
                    break;
                case (EventLevel.Warning):
                    consoleColor = ConsoleColor.DarkYellow;
                    break;
                case (EventLevel.Informational):
                    consoleColor = ConsoleColor.White;
                    break;
                case (EventLevel.Verbose):
                    consoleColor = ConsoleColor.Gray;
                    break;
            }

            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
            Debug.WriteLine($"{eventLevel}: {message}");
            Console.ForegroundColor = previousConsoleColor;
        }

        private static void LogMessage(string message)
        {
            LogMessage(message, EventLevel.Informational);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
}
