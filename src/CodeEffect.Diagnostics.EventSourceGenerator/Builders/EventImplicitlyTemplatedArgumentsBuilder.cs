using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
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
                    if (template == null && eventSource.Settings.ImplicitTypeTemplates)
                    {
                        foreach (var implicitTypeTemplateNamespace in eventSource.Settings.ImplicitTypeTemplateNamespaces ?? new string[0])
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
                var propertyType = property.PropertyType;
                var nullableType = Nullable.GetUnderlyingType(propertyType);
                var defaultValue = "";
                if (nullableType != null)
                {
                    propertyType = nullableType;
                    if (!nullableType.IsClass)
                    {
                        defaultValue = EventArgumentModel.GetDefaultValue(nullableType) ?? "";
                    }
                }

                var parsedType = EventArgumentModel.ParseType(propertyType.FullName);

                var assignment = $"$this.{property.Name}";
                var propertyTypeName = propertyType.FullName;
                if (parsedType == typeof(object))
                {
                    if (nullableType != null)
                    {
                        assignment = $"($this.{property.Name}??{defaultValue})";
                        propertyTypeName = nullableType.FullName;
                    }
                    else
                    {
                        assignment = $"$this.{property.Name}.AsJson()";
                        propertyTypeName = typeof(string).FullName;
                    }
                }                

                yield return new EventArgumentModel(property.Name, propertyTypeName, assignment);
            }
        }
    }
}