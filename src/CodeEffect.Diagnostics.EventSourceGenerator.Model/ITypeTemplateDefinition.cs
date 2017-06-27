namespace FG.Diagnostics.AutoLogger.Model
{
    public interface ITypeTemplateDefinition : IExtension
    {
        bool IsTemplateFor(EventArgumentModel argument);
        bool IsInheritedTemplateFor(EventArgumentModel argument);

        TypeTemplateModel GetTypeTemplateModel();
    }
}