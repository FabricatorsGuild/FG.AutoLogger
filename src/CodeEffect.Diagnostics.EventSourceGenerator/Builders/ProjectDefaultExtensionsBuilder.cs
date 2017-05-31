using System.Collections.Generic;
using CodeEffect.Diagnostics.EventSourceGenerator.Extensions;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class ProjectDefaultExtensionsBuilder : BaseWithLogging, IProjectBuilder
    {
        public void Build(Project model)
        {
            var extensions = new List<IExtension>(model.Extensions);
            extensions.AddRange(CreateDefaultExtensions());
            model.Extensions = extensions.ToArray();
        }

        private IEnumerable<IExtension> CreateDefaultExtensions()
        {
            yield return new ExceptionTypeTemplateExtension();
        }
    }
}