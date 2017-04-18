using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerEventSourcePartialEventMethodRenderer : EventSourceEventMethodRenderer, ILoggerEventSourcePartialEventRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            return this.Render(model);
        }
    }    
}