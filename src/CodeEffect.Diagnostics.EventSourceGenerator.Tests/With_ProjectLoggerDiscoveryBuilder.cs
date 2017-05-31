using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;
using ConsoleApplication1.Loggers;
using FluentAssertions;
using NUnit.Framework;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Tests
{
    // ReSharper disable InconsistentNaming
    public class With_ProjectLoggerDiscoveryBuilder
    {
        public With_ProjectLoggerDiscoveryBuilder()
        {
        }

        [Test]
        public void should_build_logger_templates_and_discover_event_return_types()
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
                    //new ProjectItem<LoggerTemplateModel>(ProjectItemType.LoggerInterface, @"C:\\Code\\CodeEffect\\CodeEffect.Diagnostics.EventSourceGenerator\\src\\ConsoleApplication1\\Loggers\\IConsoleLogger.cs", null, @"Loggers\\IConsoleLogger.cs"),
                    //new ProjectItem<LoggerTemplateModel>(ProjectItemType.LoggerInterface, @"C:\\Code\\CodeEffect\\CodeEffect.Diagnostics.EventSourceGenerator\\src\\ConsoleApplication1\\Loggers\\IConsoleRunnerLogger.cs", null, @"Loggers\\IConsoleRunnerLogger.cs"),
                },
                DynamicAssembly = typeof(IDependencyLogger).Assembly,
            };
            projectLoggerDiscoverBuilder.Build(project);

            project.Loggers.Should().HaveCount(1);

            var dependencyLogger = project.Loggers[0];
            dependencyLogger.Should().NotBeNull();

            var loggerEvent = dependencyLogger.Events[0];
            loggerEvent.Should().NotBeNull();

            loggerEvent.ReturnType.Should().Be(typeof(IDisposable).GetFriendlyName());
        }
    }
    // ReSharper restore InconsistentNaming
}