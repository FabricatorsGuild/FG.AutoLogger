using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventSourceTypeTemplate
    {
        public string Name { get; set; }
        public string CLRType { get; set; }
        public EventSourceEventCustomArgument[] Arguments { get; set; }

        public override string ToString()
        {
            return $"{nameof(EventSourceTypeTemplate)} {this.Name}";
        }
    }

    public static class EventSourceTypeTemplateExtensions
    {
        public static EventSourceTypeTemplate GeTypeTemplate(this IEnumerable<EventSourceTypeTemplate> templates, string type)
        {
            return templates.FirstOrDefault(t =>
                t.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase) ||
                t.CLRType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}