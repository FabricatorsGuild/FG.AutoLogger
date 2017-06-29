using System;
using ConsoleApplication1.Loggers;
using FG.Diagnostics.AutoLogger.Generator.Builders;
using FG.Diagnostics.AutoLogger.Generator.Renderers;
using FG.Diagnostics.AutoLogger.Model;
using FluentAssertions;
using NUnit.Framework;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Tests
{
    // ReSharper disable InconsistentNaming
    public class With_LoggerImplementationEventMethodRenderer
    {
        [Test]
        public void should_render_return_types_for_logger_event_implementations()
        {
            var projectLoggerDiscoverBuilder = new ProjectLoggerDiscoverBuilder();
            projectLoggerDiscoverBuilder.SetLogMessage(Console.WriteLine);
            projectLoggerDiscoverBuilder.SetLogWarning(Console.WriteLine);
            projectLoggerDiscoverBuilder.SetLogError(Console.WriteLine);

            var project = new Project()
            {
                ProjectItems = new ProjectItem[]
                {
                    new ProjectItem<LoggerTemplateModel>(ProjectItemType.LoggerInterface, 
                    @"C:\\Code\\CodeEffect\\CodeEffect.Diagnostics.EventSourceGenerator\\src\\ConsoleApplication1\\Loggers\\IDependencyLogger.cs", null, @"Loggers\\IDependencyLogger.cs"),
                },
                DynamicAssembly = typeof(IDependencyLogger).Assembly,
                Extensions = new IExtension[0]
            };
            projectLoggerDiscoverBuilder.Build(project);

            var eventSourceProjectItem = new ProjectItem<EventSourceModel>(ProjectItemType.EventSource, "sample", new EventSourceModel());
            project.AddProjectItem(eventSourceProjectItem);

            var projectItem = new ProjectItem<LoggerModel>(ProjectItemType.LoggerImplementation,
                @"C:\\Code\\CodeEffect\\CodeEffect.Diagnostics.EventSourceGenerator\\src\\ConsoleApplication1\\Loggers\\DependencyLogger.cs", new LoggerModel() {ClassName = "DependencyLogger"},
                @"Loggers\\DependencyLogger.cs") {DependentUpon = eventSourceProjectItem };
            project.AddProjectItem(projectItem);

            var loggerImplementationRenderer = new LoggerImplementationRenderer();
            loggerImplementationRenderer.SetLogMessage(Console.WriteLine);
            loggerImplementationRenderer.SetLogWarning(Console.WriteLine);
            loggerImplementationRenderer.SetLogError(Console.WriteLine);

            loggerImplementationRenderer.Render(project, projectItem);

            projectItem.Output.Should().Be(@"");

            //loggerImplementationRenderer.Render(project, );

        }
    }
    // ReSharper restore InconsistentNaming
}