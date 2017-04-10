using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerOverrideArgumentsBuilder : ILoggerBuilder
    {
        public void Build(Project project, EventSourceModel eventSource, LoggerModel model)
        {
            if (model == null) return;

            // TODO: Get ALL builders from project and allow them to 'build'
            var eventArgumentBuilders = new[]
            {
                new EventArgumentBuilder()
            };
            foreach (var argument in model?.OverrideArguments?? new EventArgumentModel[0])
            {
                argument.IsOverriden = true;
                foreach (var eventArgumentBuilder in eventArgumentBuilders)
                {
                    eventArgumentBuilder.Build(project, eventSource, argument);
                }
            }
        }
    }
}