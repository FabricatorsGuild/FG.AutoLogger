using System.Collections.Generic;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class ProjectItem
    {
        public string Name { get; set; }
        public string Include { get; set; }
        public string Content { get; set; }
        public string RootNamespace { get; set; }
        public ProjectItem DependentUpon { get; set; }
        public ProjectItemType ItemType { get; set; }

        public ProjectItem()
        {
            ItemType = ProjectItemType.Unknown;
        }

        public ProjectItem(ProjectItemType type, string name)
        {
            Name = name;
            ItemType = type;
        }

        public ProjectItem(ProjectItemType type, string name, string content, string include = null)
            : this(type, name)
        {
            Content = content;
            Include = include;
        }

        public ProjectItem(ProjectItemType type, KeyValuePair<string, string> item)
            : this(type, item.Key, item.Value)
        {
        }

        public override string ToString()
        {
            return $"{ItemType.Name()}: {Include}";
        }
    }
}