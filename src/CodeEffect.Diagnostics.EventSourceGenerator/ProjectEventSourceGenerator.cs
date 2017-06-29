using System;
using System.Linq;
using FG.Diagnostics.AutoLogger.Generator.Builders;
using FG.Diagnostics.AutoLogger.Generator.Renderers;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator
{
    public class ProjectEventSourceGenerator : BaseWithLogging
    {
        public Project Run(string projectBasePath, string cscToolPath, bool saveChanges = false, bool forceUpdate = false)
        {

            var project = new Project() { ProjectFilePath = projectBasePath, CscToolPath = cscToolPath, ForceUpdate = forceUpdate};

            var builders = new IProjectBuilder[]
            {
                new ProjectBuilder(),
                new ProjectSummaryBuilder(),
                new ToolModuleReferenceBuilder(),
            };
            foreach (var builder in builders)
            {
                PassAlongLoggers(builder as IWithLogging);
                builder.Build(project);
            }

            // Do not continue if no changes are detected, saving time on compilation etc
            if ((!project.HasProjectChanges) && (!project.ForceUpdate))
            {
                LogMessage("Ignoring to build and render the project as no changes were detected");
                return project;
            }

            var referenceFiles = 
                project.ProjectItems.OfType(ProjectItemType.Reference).Select(r => r.Name)
                .Union(project.ProjectItems.OfType(ProjectItemType.ProjectReference).Select(r => r.Name))
                .Union(project.ToolModuleReferences)
                .ToArray();

            var assemblyResolver = new AssemblyResolver(referenceFiles);
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.CurrentDomainOnAssemblyResolve;

            builders = new IProjectBuilder[]
            {
                new ProjectRefenceCopyBuilder(), 
                new ProjectPrecompileBuilder(), 
                new ProjectExtensionsDiscoverBuilder(),
                new ProjectDefaultExtensionsBuilder(),
                new ProjectLoggerDiscoverBuilder(),
                new ProjectReferenceItemRemoverBuilder(), 
                new ProjectDefaultEventSourceBuilder(),
                new ProjectEventSourceBuilder(),
            };
            foreach (var builder in builders)
            {
                PassAlongLoggers(builder as IWithLogging);
                builder.Build(project);
            }

            // Do this in step 2 as project extensions are not loaded until above.
            foreach (var builder in project.GetExtensions<IProjectBuilder>())
            {
                PassAlongLoggers(builder as IWithLogging);
                builder.Build(project);
            }

            var renderers = new IProjectRenderer[]
            {
                new ProjectDefaultEventSourceDefinitionRenderer(),
                new ProjectEventSourceRenderer(),
                new ProjectLoggerRenderer(),
                new ProjectSummaryRenderer(),
                new ProjectRenderer() {SaveChanges = saveChanges},
                new ProjectFilesRenderer() {SaveChanges = saveChanges},
            };
            foreach (var renderer in renderers.Union(project.GetExtensions<IProjectRenderer>()))
            {
                PassAlongLoggers(renderer as IWithLogging);
                renderer.Render(project);
            }

            AppDomain.CurrentDomain.ResourceResolve -= assemblyResolver.CurrentDomainOnAssemblyResolve;

            return project;
        }

    }
}