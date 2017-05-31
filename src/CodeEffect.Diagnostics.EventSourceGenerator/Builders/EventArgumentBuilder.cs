using System;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventArgumentBuilder : BaseWithLogging, IEventArgumentBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventArgumentModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if( eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            var type = model.Type;
            if (TypeExtensions.IsComplexType(model.Type))
            {
                model.TypeTemplate = this.GetTypeTemplate(project, eventSource.TypeTemplates, model);
                if (model.TypeTemplate != null)
                {
                    var parsedType = TypeExtensions.ParseType(model.TypeTemplate.CLRType);
                    var renderedType = TypeExtensions.RenderCLRType(parsedType);
                    if (renderedType == "string" && (parsedType != typeof(string)))
                    {
                        type = model.TypeTemplate.CLRType;
                    }
                    else
                    {
                        type = renderedType;
                    }

                }
                else
                {
                    model.AssignedCLRType = "string";
                    model.Assignment = "$this.ToString()";
                }  
            }
            model.CLRType = type;
        }
    }
}