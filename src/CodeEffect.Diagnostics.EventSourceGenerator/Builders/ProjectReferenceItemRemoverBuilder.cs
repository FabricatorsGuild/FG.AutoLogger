using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectReferenceItemRemoverBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            var itemsWithoutReferences = model.GetItemsExcludeType(ProjectItemType.Reference);
            model.ProjectItems = itemsWithoutReferences.ToArray();
        }
    }
}