using System;
using System.Collections.Generic;
using System.Linq;

namespace FG.Diagnostics.AutoLogger.Model
{
    public static class ProjectExtensions
    {
        public static ProjectItem GetItemForContent(this Project that, EventSourceModel eventSource)
        {
            return that.ProjectItems.FirstOrDefault(item => (item.Content as EventSourceModel)?.Include == eventSource.Include);
        }


        public static IEnumerable<ProjectItem> GetItemsExcludeType(this IEnumerable<ProjectItem> that, ProjectItemType excludedItemType)
        {
            return that.Where(i => i.ItemType != excludedItemType);
        }

        public static void AddProjectItem(this Project that, ProjectItem item)
        {
            var projectItems = that.ProjectItems.ToList();
            projectItems.Add(item);
            that.ProjectItems = projectItems.ToArray();
        }

        public static IEnumerable<TExtension> GetExtensions<TExtension>(this Project that)
            where TExtension : IExtension
        {
            return that.Extensions.Where(e => e is TExtension).Cast<TExtension>().ToArray();
        }

        public static IEnumerable<TExtension> GetExtensions<TExtension>(this Project that, string[] modules)
            where TExtension : IExtension
        {
            modules = modules ?? new string[0];
            return that.Extensions
                .Where(e => e is TExtension)
                .Cast<TExtension>()
                .Where(e => modules.Any(m => m.Equals(e.Module, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();
        }

        public static IEnumerable<string> GetExtensionModules(this Project that)
        {
            return that.Extensions
                .Select(e => e.Module)
                .Where(e => e != @"Core")
                .Distinct()
                .ToArray();
        }

        public static string GetIncludeName(this Project that, ProjectItem item)
        {
            return item.Name.StartsWith(that.ProjectBasePath) ? item.Name.Substring(that.ProjectBasePath.Length + 1) : item.Name;
        }
    }
}