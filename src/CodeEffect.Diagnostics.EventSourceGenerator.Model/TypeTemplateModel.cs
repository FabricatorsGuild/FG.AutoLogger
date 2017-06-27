namespace FG.Diagnostics.AutoLogger.Model
{
    public class TypeTemplateModel
    {
        public string Name { get; set; }
        public string CLRType { get; set; }

        public EventArgumentModel[] Arguments { get; set; }

        public override string ToString()
        {
            return $"{nameof(TypeTemplateModel)} {this.Name}";
        }
    }
}