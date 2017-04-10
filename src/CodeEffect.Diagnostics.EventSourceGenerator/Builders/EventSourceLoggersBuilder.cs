using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceLoggersBuilder : IEventSourceBuilder
    {
        public void Build(Project project, EventSourceModel model)
        {
            // TODO: Get all builders from project and allow all to build
            var loggerBuilders = new ILoggerBuilder[]
            {
                new LoggerTemplateBuilder(),
                new LoggerKeywordsBuilder(),
                new LoggerImplicitArgumentsBuilder(),
                new LoggerOverrideArgumentsBuilder(), 
                new LoggerEventsBuilder(),
            };
            var loggerStartId = 10000;
            foreach (var logger in model.Loggers)
            {
                logger.StartId = logger.StartId ?? loggerStartId;

                foreach (var builder in loggerBuilders)
                {
                    builder.Build(project, model, logger);
                }

                loggerStartId += 1000;
            }
        }
    }
}