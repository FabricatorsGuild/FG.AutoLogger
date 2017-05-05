namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class Project
    {
        public EventSourceModel[] EventSources { get; set; }
        public string ProjectFilePath { get; set; }
        public string ProjectBasePath { get; set; }
        public ProjectItem[] ProjectItems { get; set; }
        public LoggerTemplateModel[] Loggers { get; set; }
        public IExtension[] Extensions { get; set; }
        public string Platform { get; set; }
    }
}