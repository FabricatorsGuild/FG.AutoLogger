using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
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
        {
            return that.Extensions.Where(e => e is TExtension).Cast<TExtension>();
        }

        public static string GetIncludeName(this Project that, ProjectItem item)
        {
            return item.Name.StartsWith(that.ProjectBasePath) ? item.Name.Substring(that.ProjectBasePath.Length + 1) : item.Name;
        }
    }
}