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
            var itemsWithoutReferences = model.ProjectItems.GetItemsExcludeType(ProjectItemType.Reference).GetItemsExcludeType(ProjectItemType.ProjectReference);
            model.ProjectItems = itemsWithoutReferences.ToArray();

            
        }
    }
}