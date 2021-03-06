using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class ProjectLoggerDiscoverBuilder : BaseCoreBuilder, IProjectBuilder
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

            var foundLoggerTemplates =  CompileAndEvaluateInterface(model.DynamicAssembly, loggerProjectItems);

            foreach (var foundLoggerTemplate in foundLoggerTemplates)
            {
                LogMessage($"Compiled Logger Template {foundLoggerTemplate.Name}");                
                loggerTemplates.Add(foundLoggerTemplate);
            }

            model.Loggers = loggerTemplates.ToArray();            
        }

        private bool IsLoggerInterface(Type type)
        {
            return type.IsInterface && type.Name.Matches(@"^I[^\\]*Logger", StringComparison.InvariantCultureIgnoreCase, useWildcards: false);
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
                    foreach (var type in types.Where(IsLoggerInterface).ToArray())
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
                                eventSourceEvent.ReturnType = methodInfo.ReturnType.FullName != "System.Void" ? methodInfo.ReturnType?.GetFriendlyName() : null;
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