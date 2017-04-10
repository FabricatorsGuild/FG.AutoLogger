using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerEventsBuilder : ILoggerBuilder
    {
        public void Build(Project project, EventSourceModel eventSource, LoggerModel model)
        {
            if( model ==  null) return;

            
            var nextEventId = model.StartId;
            
            foreach (var loggerEvent in model.Events)
            {
                if (model.ImplicitArguments != null && model.ImplicitArguments.Length > 0)
                {
                    loggerEvent.InsertImplicitArguments(model.ImplicitArguments);
                }
                if (model.OverrideArguments != null && model.OverrideArguments.Length > 0)
                {
                    loggerEvent.OverrideArguments(model.OverrideArguments);
                }
                loggerEvent.Keywords = new[] { eventSource.Keywords.Find(model.GetKeyword()) };
                loggerEvent.Id = nextEventId;

                //TODO: Create events builder

                nextEventId += 1;
            }
        }
    }
}