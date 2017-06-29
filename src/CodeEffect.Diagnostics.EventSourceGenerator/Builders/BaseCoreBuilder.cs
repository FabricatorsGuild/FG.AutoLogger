using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public abstract class BaseCoreBuilder : BaseWithLogging, IExtension
    {
        public string Module => @"Core";
    }
}