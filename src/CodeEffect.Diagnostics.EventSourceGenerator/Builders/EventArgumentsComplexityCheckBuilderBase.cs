using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public abstract class EventArgumentsComplexityCheckBuilderBase : BaseWithLogging
    {
        protected void Build(EventModel model)
        {
            var hasComplexArguments = false;
            foreach (var argument in model.GetAllArguments())
            {                
                hasComplexArguments = hasComplexArguments || argument.IsComplexType();
            }

            model.HasComplexArguments = hasComplexArguments;
        }        
    }
}