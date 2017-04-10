using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public static class TypeTemplateExtensions
    {
        public static TypeTemplateModel GetTypeTemplate(this IEnumerable<TypeTemplateModel> templates, string type)
        {
            return templates.FirstOrDefault(t =>
                t.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase) ||
                t.CLRType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}