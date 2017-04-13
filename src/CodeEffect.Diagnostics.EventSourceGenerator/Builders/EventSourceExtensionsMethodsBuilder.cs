using System.Collections.Generic;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceExtensionsMethodsBuilder : BaseWithLogging, IEventSourceBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            var extensionMethodBuilders = new IExtensionsMethodBuilder[]
            {

            };
            foreach (var extension in eventSource.Extensions ?? new List<ExtensionsMethodModel>())
            {
                foreach (var extensionMethodBuilder in extensionMethodBuilders)
                {
                    extensionMethodBuilder.Build(project, model, extension);
                }
            }
        }
    }
}