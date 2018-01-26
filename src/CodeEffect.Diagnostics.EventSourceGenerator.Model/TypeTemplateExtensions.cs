using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FG.Diagnostics.AutoLogger.Model
{

    public static class TypeTemplateExtensions
    {
        private static readonly IDictionary<string, TypeTemplateModel> TypeTemplates = new ConcurrentDictionary<string, TypeTemplateModel>();

        public static TypeTemplateModel GetTypeTemplate(this IEnumerable<TypeTemplateModel> templates, EventArgumentModel argument)
        {
            return templates.FirstOrDefault(t =>
                t.Name.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase) ||
                t.CLRType.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase));
        }

        public static TypeTemplateModel GetTypeTemplate(this BaseWithLogging caller, Project project, IEnumerable<TypeTemplateModel> templates, EventArgumentModel argument)
        {
            var cacheKey = GetCacheKey(argument);
            if (TypeTemplates.ContainsKey(cacheKey))
            {
                return TypeTemplates[cacheKey];
            }

            var directTypeTemplate = templates.FirstOrDefault(t =>
                t.Name.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase) ||
                t.CLRType.Equals(argument.Type, StringComparison.InvariantCultureIgnoreCase));
            if (directTypeTemplate != null)
            {
                return CacheAndReturn(argument, directTypeTemplate);
            }

            var typeTemplateExtensions = project.GetExtensions<ITypeTemplateDefinition>().ToArray();
            foreach (var typeTemplateExtension in typeTemplateExtensions)
            {
                caller.PassAlongLoggers(typeTemplateExtension as IWithLogging);
                if (typeTemplateExtension.IsTemplateFor(argument))
                {
                    return CacheAndReturn(argument, typeTemplateExtension.GetTypeTemplateModel());
                }
            }
            foreach (var typeTemplateExtension in typeTemplateExtensions)
            {
                caller.PassAlongLoggers(typeTemplateExtension as IWithLogging);
                if (typeTemplateExtension.IsInheritedTemplateFor(argument))
                {
                    return CacheAndReturn(argument, typeTemplateExtension.GetTypeTemplateModel());
                }
            }
            return null;
        }

        private static string GetCacheKey(EventArgumentModel argument)
        {
            var cacheKey = $"{argument.Type}/{argument.CLRType}";
            return cacheKey;
        }

        private static TypeTemplateModel CacheAndReturn(EventArgumentModel argument, TypeTemplateModel typeTemplateModel)
        {
            var cacheKey = GetCacheKey(argument);

            TypeTemplates[cacheKey] = typeTemplateModel;

            return typeTemplateModel;
        }
    }
}