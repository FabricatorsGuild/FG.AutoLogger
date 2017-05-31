using System.Security.Cryptography.X509Certificates;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ITypeTemplateDefinition : IExtension
    {
        bool IsTemplateFor(EventArgumentModel argument);
        bool IsInheritedTemplateFor(EventArgumentModel argument);

        TypeTemplateModel GetTypeTemplateModel();
    }
}