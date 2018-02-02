namespace FG.Diagnostics.AutoLogger.Model
{    
    public enum ProjectItemType
    {
        EventSourceDefinition = 1,
        LoggerInterface = 2,
        BuilderExtension = 3,
        Reference = 4,
        ProjectReference = 5,
        Unknown = 7,

        EventSource = 100,
        EventSourceLoggerPartial = 101,
        DefaultGeneratedEventSourceDefinition = 102,
        LoggerImplementation = 103,
        ProjectSummary = 104,
    }
}