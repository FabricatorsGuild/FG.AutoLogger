using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
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