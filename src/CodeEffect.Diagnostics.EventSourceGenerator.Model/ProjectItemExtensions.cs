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

        public static ProjectItem GetDefaultEventSourceProjectItem(this IEnumerable<ProjectItem> that)
        {
            return that.FirstOrDefault(item => item.ItemType == ProjectItemType.DefaultGeneratedEventSource);
        }

        public static string Name(this ProjectItemType that)
        {
            return Enum.GetName(typeof(ProjectItemType), that);
        }
    }
}