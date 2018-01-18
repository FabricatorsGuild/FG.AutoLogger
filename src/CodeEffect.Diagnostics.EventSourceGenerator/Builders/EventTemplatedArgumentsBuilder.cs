using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventTemplatedArgumentsBuilder : BaseCoreBuilder, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError(
                    $"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            Build(project, eventSource, model);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model)
        {
            Build(project, eventSourceProjectItem, model);
        }

        private void Build(Project project, EventSourceModel eventSource, EventModel model)
        {
            if (eventSource == null)
            {
                LogError($"{nameof(eventSource)} should not be null");
                return;
            }

            foreach (var argument in model.GetAllArguments())
            {
                if (TypeExtensions.IsComplexType(argument.Type) && (argument.TypeTemplate == null))
                {
                    var template = this.GetTypeTemplate(project, eventSource.TypeTemplates, argument);
                    if (template != null)
                    {
                        argument.TypeTemplate = template;
                    }                   
                }
            }
        }
    }
    public class EventImplicitlyTemplatedArgumentsBuilder : BaseCoreBuilder, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError(
                    $"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            Build(project, eventSource, model);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model)
        {
            Build(project, eventSourceProjectItem, model);
        }

        private void Build(Project project, EventSourceModel eventSource, EventModel model)
        {
            if (eventSource == null)
            {
                LogError($"{nameof(eventSource)} should not be null");
                return;
            }

            foreach (var argument in model.GetAllArguments())
            {
                if (TypeExtensions.IsComplexType(argument.Type) && (argument.TypeTemplate == null))
                {
                    var template = this.GetTypeTemplate(project, eventSource.TypeTemplates, argument);
                    if (template == null && eventSource.ImplicitTypeTemplates)
                    {
                        foreach (var implicitTypeTemplateNamespace in eventSource.ImplicitTypeTemplateNamespaces ?? new string[0])
                        {
                            if (argument.Type.StartsWith(implicitTypeTemplateNamespace))
                            {
                                var typeTemplate = new TypeTemplateModel()
                                {
                                    Arguments = GetTypeArguments(project, argument.Type).ToArray(),
                                    CLRType = argument.Type,
                                    Name = argument.Type
                                };
                                argument.TypeTemplate = typeTemplate;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<EventArgumentModel> GetTypeArguments(Project project, string type)
        {
            var argumentType = project.DynamicAssembly.GetType(type);
            if (argumentType == null)
            {
                foreach (var referencedAssembly in project.DynamicAssembly.GetReferencedAssemblies())
                {
                    argumentType = Assembly.Load(referencedAssembly).GetType(type);
                    if (argumentType != null)
                    {
                        break;
                    }
                }
            }

            var properties = argumentType?.GetProperties();
            foreach (var property in properties ?? new PropertyInfo[0])
            {
                yield return new EventArgumentModel(property.Name, property.PropertyType.FullName, $"$this.{property.Name}");
            }
        }
    }
}