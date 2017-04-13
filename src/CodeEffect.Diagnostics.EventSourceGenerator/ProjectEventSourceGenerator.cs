using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Renderers;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class ProjectEventSourceGenerator : BaseWithLogging
    {
        public Project Run(string projectBasePath)
        {

            var project = new Project() { ProjectFilePath = projectBasePath };

            var builders = new IProjectBuilder[]
            {
                new ProjectBuilder(),
                new ProjectExtensionsDiscoverBuilder(),
                new ProjectLoggerDiscoverBuilder(),
                new ProjectReferenceItemRemoverBuilder(), 
                new ProjectDefaultEventSourceBuilder(),
                new ProjectEventSourceBuilder(),
            };
            foreach (var builder in builders)
            {
                SetLoggers(builder as IWithLogging);
                builder.Build(project);
            }
            // Do this in step 2 as project extensions are not loaded until above.
            foreach (var builder in project.GetExtensions<IProjectBuilder>())
            {
                SetLoggers(builder as IWithLogging);
                builder.Build(project);
            }

            var renderers = new IProjectRenderer[]
            {
                new ProjectDefaultEventSourceDefinitionRenderer(),
                new ProjectEventSourceRenderer(), 
                new ProjectLoggerRenderer(),
                new ProjectRenderer(),
            };
            foreach (var renderer in renderers.Union(project.GetExtensions<IProjectRenderer>()))
            {
                SetLoggers(renderer as IWithLogging);
                renderer.Render(project);
            }

            return project;
        }
    }
}