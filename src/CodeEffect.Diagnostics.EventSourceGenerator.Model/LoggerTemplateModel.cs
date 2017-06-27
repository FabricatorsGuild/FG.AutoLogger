namespace FG.Diagnostics.AutoLogger.Model
{
    public class LoggerTemplateModel
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Include { get; set; }
        public EventModel[] Events { get; set; }

        public override string ToString()
        {
            return $"{nameof(LoggerTemplateModel)} {this.Name}";
        }
    }
}