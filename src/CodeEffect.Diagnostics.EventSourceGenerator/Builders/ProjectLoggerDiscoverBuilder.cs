using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;
using Microsoft.CSharp;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectLoggerDiscoverBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            DiscoverLoggers(model);
        }
        
        private void DiscoverLoggers(Project model)
        {
            var files = model.ProjectItems.ToArray();

            LogMessage($"Scanning {files.Length} project file{(files.Length == 1 ? "" : "s")} for loggers");
            var loggerTemplates = new List<LoggerTemplateModel>();
            var loggerProjectItems = files.OfType<LoggerTemplateModel>(ProjectItemType.LoggerInterface);
            var referenceProjectItems = files.OfType(ProjectItemType.Reference).ToArray();
            foreach (var loggerProjectItem in loggerProjectItems)
            {
                LogMessage($"Found Logger file {loggerProjectItem.Name}");
                var foundLoggerTemplates = CompileAndEvaluateInterface(loggerProjectItem, referenceProjectItems);
                var updateExisting = true;
                foreach (var foundLoggerTemplate in foundLoggerTemplates)
                {
                    LogMessage($"Compiled Logger Template {foundLoggerTemplate.Name}");

                    if (updateExisting)
                    {
                        loggerProjectItem.Content = foundLoggerTemplate;
                        updateExisting = false;
                    }
                    else
                    {
                        var newProjectItem = new ProjectItem<LoggerTemplateModel>(
                            type: loggerProjectItem.ItemType,
                            name: loggerProjectItem.Name,
                            content: foundLoggerTemplate,
                            include: loggerProjectItem.Include);

                        model.AddProjectItem(newProjectItem);
                    }
                    loggerTemplates.Add(foundLoggerTemplate);
                }
            }

            model.Loggers = loggerTemplates.ToArray();            
        }

        private LoggerTemplateModel[] CompileAndEvaluateInterface(ProjectItem<LoggerTemplateModel> projectItem, IEnumerable<ProjectItem> referenceItems)
        {
            LogMessage($"Compiling possible logger file {projectItem.Include}");

            var loggers = new List<LoggerTemplateModel>();
            try
            {
                var parameters = new CompilerParameters();

                foreach (var referenceItem in referenceItems)
                {
                    parameters.ReferencedAssemblies.Add(referenceItem.Name);
                }

                //parameters.ReferencedAssemblies.Add("System.dll");
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;

                parameters.IncludeDebugInformation = false;
                var cSharpCodeProvider = new CSharpCodeProvider();
                //var cSharpCodeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
                var compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(parameters, projectItem.Name);
                foreach (CompilerError compilerResultsError in compilerResults.Errors)
                {
                    LogWarning(compilerResultsError.ToString());
                }

                var types = compilerResults.CompiledAssembly.GetTypes();
                foreach (
                    var type in
                    types.Where(t => t.IsInterface && t.Name.Matches(@"^I[^\\]*Logger", StringComparison.InvariantCultureIgnoreCase, useWildcards: false)))
                {
                    var include = projectItem.Include.Replace(projectItem.Name, type.Name);
                    var eventSourceLogger = new LoggerTemplateModel()
                    {
                        Name = type.Name,
                        Namespace = type.Namespace,
                        Include = include
                    };
                    var eventSourceEvents = new List<EventModel>();
                    foreach (var methodInfo in type.GetMethods())
                    {
                        var eventSourceEventArguments = new List<EventArgumentModel>();
                        var eventSourceEvent = new EventModel()
                        {
                            Name = methodInfo.Name,
                        };
                        foreach (var parameterInfo in methodInfo.GetParameters())
                        {
                            var typeString = parameterInfo.ParameterType.GetFriendlyName();
                            eventSourceEventArguments.Add(new EventArgumentModel()
                            {
                                Name = parameterInfo.Name,
                                Type = typeString,
                            });
                        }
                        eventSourceEvent.Arguments = eventSourceEventArguments.ToArray();
                        eventSourceEvents.Add(eventSourceEvent);
                    }

                    eventSourceLogger.Events = eventSourceEvents.ToArray();
                    loggers.Add(eventSourceLogger);
                }
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate {projectItem.Include} - {ex.Message}\r\n{ex.StackTrace}");
            }
            return loggers.ToArray();
        }
    }
}