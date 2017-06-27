using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventArgumentsComplexityCheckBuilder : BaseWithLogging, IEventBuilder, ILoggerEventBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventModel model)
        {
            Build(model);
        }

        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel logger, EventModel model)
        {
            Build(model);
        }

        private static bool IsComplexType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                case ("int"):
                case ("system.int32"):
                case ("long"):
                case ("system.int64"):
                case ("bool"):
                case ("system.boolean"):
                case ("datetime"):
                case ("system.dateTime"):
                case ("guid"):
                case ("system.guid"):
                    return false;
                default:
                    return true;
            }
        }

        private void Build(EventModel model)
        {
            var hasComplexArguments = false;
            foreach (var argument in model.GetAllArguments())
            {
                hasComplexArguments = hasComplexArguments || IsComplexType(argument.CLRType ?? argument.Type);
            }

            model.HasComplexArguments = hasComplexArguments;
        }
    }
}