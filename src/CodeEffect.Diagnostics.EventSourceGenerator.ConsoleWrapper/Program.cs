using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.MSBuild;
using CommandLine;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateDummy();
            //var projectFilePath = @"C:\Code\labs\ServiceFabric.Labs.LoadAndHealth\WebScraperService\WebScraperService.csproj";
            //var projectFilePath = @"C:\Code\labs\ServiceFabric.Labs.LoadAndHealth\ArticleService\ArticleService.csproj";
            //var projectFilePath = @"C:\Code\labs\ServiceFabric.Labs.LoadAndHealth\ArticleActor\ArticleActor.csproj";
            var projectFilePath = @"C:\Code\labs\Monitoring.Labs.EventSourceAndAI\Monitoring.Labs.EventSourceAndAI.Console\Monitoring.Labs.EventSourceAndAI.Console\Monitoring.Labs.EventSourceAndAI.Console.csproj";
            //var projectFilePath = @"C:\Code\labs\CodeEffect.Diagnostics.EventSourceGenerator\ConsoleApplication1\ConsoleApplication1.csproj";

            Console.ForegroundColor = ConsoleColor.Magenta;
            var builder = new EventSourceBuilder(message =>
            {
                Console.WriteLine(message);
                Debug.WriteLine(message);
            });

            var projectItems = builder.GetProjectItems(projectFilePath);
            var projectFileBasePath = System.IO.Path.GetDirectoryName(projectFilePath);
            var outputs = builder.Execute(projectFileBasePath, projectItems);

            foreach (var output in outputs)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{"".PadRight(40, '_')}");
                Console.WriteLine($"{"".PadRight(40, '=')}");
                Console.WriteLine($"File: {output.Name}");
                Console.WriteLine($"{"".PadRight(40, '_')}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(output.Content);


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
            builder.AddGeneratedOutputsToProject(projectFilePath, outputs, false);


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{"".PadRight(40, '_')}");
            Console.WriteLine($"{"".PadRight(40, '=')}");


            Console.ReadKey();
        }

        //private IEnumerable<string> GetFiles()
        //{
        //    var scanPath = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).Parent.FullName;
        //    var path2 = System.IO.Path.GetDirectoryName(scanPath);
        //    Console.WriteLine(path2);
        //    var files2 = System.IO.Directory.GetFiles(path2, "I*Logger.cs");
        //    var loggers = new List<EventSourceLoggerTemplate>();
        //    foreach (var file in files2)
        //    {
        //        Console.WriteLine($"\t{file}");
        //        loggers.AddRange(CompileAndEvaluateInterface(file));
        //    }
        //}

        //private static EventSourceLoggerTemplate[] CompileAndEvaluateInterface(string filePath)
        //{
        //    var parameters = new CompilerParameters();

        //    parameters.ReferencedAssemblies.Add("System.dll");
        //    parameters.GenerateExecutable = false;
        //    parameters.GenerateInMemory = true;

        //    parameters.IncludeDebugInformation = false;

        //    var cSharpCodeProvider = new CSharpCodeProvider();
        //    var compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(parameters, filePath);
        //    foreach (CompilerError compilerResultsError in compilerResults.Errors)
        //    {
        //        Console.WriteLine(compilerResultsError.ToString());
        //    }

        //    var loggers = new List<EventSourceLoggerTemplate>();
        //    var types = compilerResults.CompiledAssembly.GetExportedTypes();
        //    foreach (var type in types.Where(t => t.IsInterface && t.Name.EndsWith("Logger")))
        //    {
        //        var eventSourceLogger = new EventSourceLoggerTemplate() {Name = type.Name};
        //        //var eventSourceEventArguments = new List<EventSourceEventArgument>();
        //        var eventSourceEvents = new List<EventSourceEvent>();
        //        Console.WriteLine($"\t\t{type.FullName}");
        //        foreach (var methodInfo in type.GetMethods())
        //        {
        //            var eventSourceEventArguments = new List<EventSourceEventArgument>();
        //            var eventSourceEvent = new EventSourceEvent()
        //            {
        //                Name = methodInfo.Name
        //            };
        //            Console.WriteLine($"\t\t\t{methodInfo.Name}");
        //            foreach (var parameterInfo in methodInfo.GetParameters())
        //            {
        //                var typeString = EventSourceEventArgument.RenderCLRType(parameterInfo.ParameterType);
        //                if (typeString == "string" && parameterInfo.ParameterType != typeof(string))
        //                {
        //                    typeString = parameterInfo.ParameterType.Name;
        //                }
        //                eventSourceEventArguments.Add(new EventSourceEventArgument()
        //                {
        //                    Name = parameterInfo.Name,
        //                    Type = typeString,
        //                });
        //                Console.WriteLine($"\t\t\t\t{parameterInfo.ParameterType.Name} {parameterInfo.Name}");

        //            }
        //            eventSourceEvent.Arguments = eventSourceEventArguments.ToArray();
        //            eventSourceEvents.Add(eventSourceEvent);
        //        }

        //        eventSourceLogger.Events = eventSourceEvents.ToArray();
        //        loggers.Add(eventSourceLogger);
        //    }
        //    return loggers.ToArray();
        //}

        //private static void GenerateEventSourceCode(string filePath, params EventSourceLoggerTemplate[] loggers)
        //{
        //    var content = System.IO.File.ReadAllText(filePath);
        //    var eventSourcePrototype = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSourcePrototype>(content);

        //    var path = System.IO.Path.GetDirectoryName(filePath);
        //    var fileName = System.IO.Path.GetFileName(filePath);
        //    var className = System.IO.Path.GetFileNameWithoutExtension(fileName);
        //    var newFileName = $"{fileName}.cs";
        //    Console.WriteLine($"\t{newFileName}");

        //    eventSourcePrototype.ClassName = className;
        //    eventSourcePrototype.AvailableLoggers = loggers;
        //    var outputs = eventSourcePrototype.Render(name => System.IO.Path.Combine(path, $"{name}.cs"));

        //    foreach (var output in outputs)
        //    {
        //        Console.WriteLine("**************************************************************************************");
        //        Console.WriteLine(output.Key);
        //        Console.WriteLine(output.Value);
        //        System.IO.File.WriteAllText(output.Key, output.Value);
        //        Console.WriteLine("**************************************************************************************");
        //    }
        //}

        private static void GenerateDummy()
        {
            var eventSourcePrototype = new EventSourcePrototype();

            eventSourcePrototype.Namespace = typeof(Program).Namespace;
            eventSourcePrototype.Name = "CE-CodeEffect.Diagnostics.EventSourceGenerator";
            eventSourcePrototype.Keywords = new string[] { "HostInitialization", "Domain", "Infrastructure" };
            eventSourcePrototype.Loggers = new[]
            {
                new EventSourceLogger()
                {
                    Name = "IDomainLogger",
                    StartId = 20000,
                    ImplicitArguments = new EventSourceEventArgument[]
                    {
                        new EventSourceEventArgument() {Name = "actor", Type = "Actor"},
                        new EventSourceEventArgument() {Name = "correlationId", Type = "Guid"},
                        new EventSourceEventArgument() {Name = "message", Type = "string"},
                    }
                },
                new EventSourceLogger()
                {
                    Name = "IInfrastructureLogger",
                    ImplicitArguments = new EventSourceEventArgument[]
                    {
                        new EventSourceEventArgument() {Name = "actor", Type = "Actor"},
                        new EventSourceEventArgument() {Name = "correlationId", Type = "Guid"},
                    }
                }
            };

            eventSourcePrototype.TypeTemplates = new EventSourceTypeTemplate[]
            {
                new EventSourceTypeTemplate()
                {
                    Name = "Actor",
                    CLRType = "Microsoft.ServiceFabric.Actors.Runtime.Actor",
                    Arguments = new EventSourceEventCustomArgument[]
                    {
                        new EventSourceEventCustomArgument("actorType", "string", "$this.GetType().ToString()"),
                        new EventSourceEventCustomArgument("actorId", "string","$this.Id.ToString()"),
                        new EventSourceEventCustomArgument("applicationTypeName", "string","$this.ActorService.Context.CodePackageActivationContext.ApplicationTypeName"),
                        new EventSourceEventCustomArgument("applicationName", "string","$this.ActorService.Context.CodePackageActivationContext.ApplicationName"),
                        new EventSourceEventCustomArgument("serviceTypeName", "string","$this.ActorService.Context.ServiceTypeName"),
                        new EventSourceEventCustomArgument("serviceName", "string","$this.ActorService.Context.ServiceName.ToString()"),
                        new EventSourceEventCustomArgument("partitionId", "Guid","$this.ActorService.Context.PartitionId"),
                        new EventSourceEventCustomArgument("replicaOrInstanceId", "long","$this.ActorService.Context.ReplicaId"),
                        new EventSourceEventCustomArgument("nodeName", "string","$this.ActorService.Context.NodeContext.NodeName"),
                    }
                },
                new EventSourceTypeTemplate()
                {
                    Name = "Exception",
                    CLRType = "System.Exception" +
                              "",
                    Arguments = new EventSourceEventCustomArgument[]
                    {
                        new EventSourceEventCustomArgument("message", "string", "$this.Message"),
                        new EventSourceEventCustomArgument("source", "string","$this.Source"),
                        new EventSourceEventCustomArgument("exceptionTypeName", "string","$this.GetType().FullName"),
                        new EventSourceEventCustomArgument("exception", "string","$this.AsJson()"),
                    }
                }
            };

            eventSourcePrototype.Events = new EventSourceEvent[]
            {
                new EventSourceEvent()
                {
                    Id = 100,
                    Level = EventLevel.Informational,
                    Name = "Message",
                    Arguments = new[]
                    {
                        new EventSourceEventArgument() {Name = "message", Type = "string"}
                    }
                },
                new EventSourceEvent()
                {
                    Level = EventLevel.Informational,
                    Name = "SpecialMessage",
                    Arguments = new[]
                    {
                        new EventSourceEventArgument() {Name = "message", Type = "string"},
                        new EventSourceEventArgument() {Name = "specialValue", Type = "int"},
                    }
                },
                new EventSourceEvent()
                {
                    Id = 300,
                    Level = EventLevel.Error,
                    Name = "Error",
                    Keywords = new [] {"Domain", "Infrastructure"},
                    Arguments = new[]
                    {
                        new EventSourceEventArgument() {Name = "exception", Type = "string"}
                    }
                },
                new EventSourceEvent()
                {
                    Id = 400,
                    Level = EventLevel.Informational,
                    Name = "ActorMessage",
                    Arguments = new[]
                    {
                        new EventSourceEventArgument() {Name = "actor", Type = "Actor"},
                        new EventSourceEventArgument() {Name = "message", Type = "string"},
                    }
                },
                new EventSourceEvent()
                {
                    Level = EventLevel.Warning,
                    Name = "ActorError",
                    Arguments = new[]
                    {
                        new EventSourceEventArgument() {Name = "actor", Type = "Actor"},
                        new EventSourceEventArgument() {Name = "exception", Type = "Exception"},
                    }
                },
            };


            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(eventSourcePrototype, Formatting.Indented, jsonSerializerSettings);
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = $"test.eventsource";
            var filePath = System.IO.Path.Combine(path, fileName);
            System.IO.File.WriteAllText(filePath, jsonFile);
        }
    }

    class Options
    {
        [Option('t', "templateFile", Required = true,
          HelpText = "Template file to be processed.")]
        public string TemplateFile { get; set; }

        [Option('v', "verbose", DefaultValue = false,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }    
}
