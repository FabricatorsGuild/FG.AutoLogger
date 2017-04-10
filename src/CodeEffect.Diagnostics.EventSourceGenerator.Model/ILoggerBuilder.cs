namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerBuilder
    {
        void Build(Project project, EventSourceModel eventSource, LoggerModel model);
    }
}