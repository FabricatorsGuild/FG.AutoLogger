using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public abstract class BaseEtwRendererWithLogging : BaseWithLogging, IExtension
    {
        public string Module => @"ETW";
    }
}