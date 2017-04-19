using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public static class ProjectItemExtensions
    {
        public static IEnumerable<ProjectItem> OfType(this IEnumerable<ProjectItem> that, ProjectItemType type)
        {
            return that.Where(item => item.ItemType == type);
        }

        public static IEnumerable<ProjectItem<TModel>> OfType<TModel>(this IEnumerable<ProjectItem> that, ProjectItemType type) 
            where TModel : class
        {
            return that.Where(item => item.ItemType == type).Cast<ProjectItem<TModel>>();
        }
        public static IEnumerable<ProjectItem> OfType(this IEnumerable<ProjectItem> that, params ProjectItemType[] types)
        {
            return that.Where(item => types.Any(t => item.ItemType == t));
        }

        public static ProjectItem GetDefaultEventSourceProjectItem(this IEnumerable<ProjectItem> that)
        {
            return that.FirstOrDefault(item => item.ItemType == ProjectItemType.DefaultGeneratedEventSourceDefinition);
        }

        public static string Name(this ProjectItemType that)
        {
            return Enum.GetName(typeof(ProjectItemType), that);
        }
    }
}