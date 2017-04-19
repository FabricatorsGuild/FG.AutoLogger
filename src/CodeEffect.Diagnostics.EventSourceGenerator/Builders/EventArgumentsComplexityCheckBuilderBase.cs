using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public abstract class EventArgumentsComplexityCheckBuilderBase : BaseWithLogging
    {
        private static bool IsComplexType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                case ("int"):
                case ("system.int32"):
                case ("long"):
                case ("system.int64"):
                case ("bool"):
                case ("system.boolean"):
                case ("datetime"):
                case ("system.dateTime"):
                case ("guid"):
                case ("system.guid"):
                    return false;
                default:
                    return true;
            }
        }

        protected void Build(EventModel model)
        {
            var hasComplexArguments = false;
            foreach (var argument in model.GetAllArguments())
            {
                hasComplexArguments = hasComplexArguments || IsComplexType(argument.CLRType ?? argument.Type);
            }

            model.HasComplexArguments = hasComplexArguments;
        }        
    }
}