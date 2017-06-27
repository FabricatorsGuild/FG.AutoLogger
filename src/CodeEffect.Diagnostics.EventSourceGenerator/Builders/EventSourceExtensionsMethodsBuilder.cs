using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
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
            }.Union(project.GetExtensions<IExtensionsMethodBuilder>()).ToArray();

            foreach (var extension in (IEnumerable<ExtensionsMethodModel>) eventSource.Extensions ?? new ExtensionsMethodModel[0])
            {
                foreach (var builder in extensionMethodBuilders)
                {
                    PassAlongLoggers(builder as IWithLogging);
                    builder.Build(project, model, extension);
                }
            }
        }
    }
}