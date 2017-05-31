using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            var foundLoggerTemplates =  CompileAndEvaluateInterface(model.DynamicAssembly, loggerProjectItems);

            foreach (var foundLoggerTemplate in foundLoggerTemplates)
            {
                LogMessage($"Compiled Logger Template {foundLoggerTemplate.Name}");                
                loggerTemplates.Add(foundLoggerTemplate);
            }

            model.Loggers = loggerTemplates.ToArray();            
        }

        private LoggerTemplateModel[] CompileAndEvaluateInterface(Assembly dynamicAssembly, IEnumerable<ProjectItem> projectItems)
        {
            var loggerFiles = projectItems.GetCSVList(p => p.Include);
            LogMessage($"Compiling possible logger files {loggerFiles}");

            var loggers = new List<LoggerTemplateModel>();
            try
            {
                var complierHelper = new ComplierHelper();
                this.PassAlongLoggers(complierHelper);

                
                //var compiledAssembly = complierHelper.Compile(cscToolPath, projectItem, referenceItems);
                if (dynamicAssembly != null)
                {
                    var types = dynamicAssembly.GetTypes();
                    foreach (
                        var type in
                        types.Where(t => t.IsInterface && t.Name.Matches(@"^I[^\\]*Logger", StringComparison.InvariantCultureIgnoreCase, useWildcards: false)))
                    {
                        var projectItem = projectItems.FirstOrDefault(l => l.Name.Matches($"*{type.Name}.cs", StringComparison.InvariantCultureIgnoreCase, true));
                        if (projectItem == null)
                        {
                            LogWarning($"Could not find matching project item for type {type.Name}");
                        }
                        else
                        {
                            var include = projectItem.Include.Replace(projectItem.Name, type.Name);
                            var eventSourceLogger = new LoggerTemplateModel()
                            {
                                Name = type.Name,
                                Namespace = type.Namespace,
                                Include = include
                            };
                            var eventSourceEvents = new List<EventModel>();
                            foreach (var methodInfo in type.GetAllInterfaceMethods())
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

                            projectItem.Content = eventSourceLogger;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to compile/evaluate {loggerFiles} - {ex.Message}\r\n{ex.StackTrace}");
            }
            return loggers.ToArray();
        }
    }
}