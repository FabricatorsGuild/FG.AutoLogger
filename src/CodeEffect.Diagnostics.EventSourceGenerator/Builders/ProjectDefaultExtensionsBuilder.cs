using System.Collections.Generic;
using FG.Diagnostics.AutoLogger.Generator.Extensions;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
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