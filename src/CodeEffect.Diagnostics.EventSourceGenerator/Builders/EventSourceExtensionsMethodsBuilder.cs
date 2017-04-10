using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceExtensionsMethodsBuilder : IEventSourceBuilder
    {
        public void Build(Project project, EventSourceModel model)
        {
            var extensionMethodBuilders = new IExtensionsMethodBuilder[]
            {

            };
            foreach (var extension in model.Extensions)
            {
                foreach (var extensionMethodBuilder in extensionMethodBuilders)
                {
                    extensionMethodBuilder.Build(project, model, extension);
                }
            }
        }
    }
}