namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventSourceLoggerTemplate
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Include { get; set; }
        public EventSourceEvent[] Events { get; set; }

        public override string ToString()
        {
            return $"{nameof(EventSourceLoggerTemplate)} {this.Name}";
        }
    }
}