using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceEventsBuilder : IEventSourceBuilder
    {
        public void Build(Project project, EventSourceModel model)
        {
            // TODO: Get all builders from project and allow all to build
            var eventBuilders = new IEventBuilder[]
            {
                new EventIdBuilder(),
                new EventImplicitArgumentsBuilder(), 
                new EventMessageFormatterBuilder(), 
            };

            foreach (var evt in model?.Events ?? new EventModel[0])
            {
                foreach (var eventBuilder in eventBuilders)
                {
                    eventBuilder.Build(project, model, evt);
                }
            }
        }
    }
}