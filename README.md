# AutoLogger by Fabricators Guild

* [Adding EventSources to a project](#adding-eventsources-to-a-project)
* [Adding Loggers to a project](#adding-loggers-to-a-project)
* [Modifying an .eventsource.json file](#modifying-an-eventsourcejson-file)
* [The Generated output](#the-generated-output)
* [Using the Eventsource](#using-the-eventsource)
* [Generating EventSources and loggers](#generating-eventsources-and-loggers)


AutoLogger (previously EventSourceGenerator) automatically generates ETW EventSources(and potentially other log sources) for C# .NET projects. It picks up [existing logger files](#adding-loggers-to-a-project) in the project and [metadata files](#modifying-an-eventsourcejson-file) describing the expected EventSource.

The purpose is to remove a lot of the manual writing and maintainance of EventSource implemenations in solutions. Since it accepts logger interfaces as well, implementations of these interfaces can be passed around to classes that we don't want having a reference to the typical ``XXXEventSource.Current`` singleton. This makes the code 

1) Easier to write
2) Easier to read
3) Easier to maintain
4) Easier to test

## Getting Started

### Tutorial - Setting up the Tool in Visual Studio
[![Setting up the Tool in Visual Studio](https://github.com/FabricatorsGuild/FG.AutoLogger/raw/master/docs/AutoLogger%20setup%20tool.PNG)](https://youtu.be/vl-eLHK7des)

There are currently two ways of generating concrete loggers and EventSource implementations in a project

* Using the MSBuild NuGet that triggers a pre-build of related files and then includes the generated result. **NOTE: This method is currenly not working well with larger projects and may end up in .dlls shared between multiple projects using the MSBuild target being locked by overlapping MSBuild processes**
* Using a separate Tool. Recommended is to add this tool to Visual Studio for easier access and use. The steps for doing that is included below

The easiest way to work with AutoLogger is to use it as an Visual Studio External Tool.

### Download the tool

To get started with AutoLogger as an External Tool for VS you need to download the executable for the tool from the [latest build](https://github.com/FabricatorsGuild/FG.AutoLogger/releases/latest), the file ``FG.Diagnostics.AutoLogger.Tool.zip`` contains the executable and related asseblies (Note: the tool uses .NET 4.5.2). Extract this tool to some folder where you can access it from Visual Studio later on, e.g. ``c:\tools\autologger``.

### Adding the AutoLogger tool as a Visual Studio External Tool

* Go to Tools > External Tools...
* Click Add
* Enter title "AutoLogger - Generate"
* Command: ``[path to downloaded tool, e.g. c:/tools/autologger]/FG.Diagnostics.AutoLogger.Tool.exe``
* Arguments: ``-o -s -p $(ProjectDir)$(ProjectFileName) -f``
* Initial Directory: ``$(ProjectDir)``

![Adding External Tool](https://github.com/FabricatorsGuild/FG.AutoLogger/raw/master/docs/Setting%20up%20Visual%20Studio%20External%20Tool.PNG)

After doing this you can add a Keyboard shortcut for activating the tool in the project you are currently working in. If the External Tool you added above was number 2 in the list of External Tools, then simply add a Keyboard shortcut Tools > Options > Keyboard > Tools.ExternalCommands2 -> <kbd>ALT</kbd>+<kbd>L</kbd>

### Create an EventSource in the project

* Create a new Visual Studio project. This could be a Console Application, a Web Applcation, really any type of project, AutoLogger doesn't (and shouldn't) care which. For this tutorial we create a Console App (.NET Framework)
* Add a logger interface to the project. This should be a simple c# interface contained in a single file named after the interface where the name ends with ``Logger``. The naming of the interface is the only thing that is important to the tool for it to work with the just-in-time pre-compilation and examination. As an example we can create the interface ``IConsoleLogger`` in ``IConsoleLogger.cs``:

```csharp
using System;

namespace Sample.ConsoleApp
{
	public interface IConsoleLogger
	{
		void StartRunMain(string[] args);
		void StopRunMain();
		void KeyPressed(ConsoleKey key);
	}
}
```
* Run the AutoLogger tool in the project <kbd>ALT</kbd>+<kbd>L</kbd>. This will generate a default eventsource definition file in you project. Visual Studio will therefore ask you to reload the project, this is normal as the tool modifies the .csproj file as it adds new files. There should now be a ``DefaultEventSource.eventsource.json`` in your project that looks like this:
```json
{
  "Loggers": [],
  "Name": "DefaultEventSource",
  "ProviderName": "AutoLogger-Samples-GettingStarted-Default",
  "Keywords": [],
  "Tasks": [],
  "TypeTemplates": [
    {
      "Name": "Exception",
      "CLRType": "System.Exception",
      "Arguments": [
        {
          "Name": "message",
          "Type": "string",
          "Assignment": "$this.Message"
        },
        {
          "Name": "source",
          "Type": "string",
          "Assignment": "$this.Source"
        },
        {
          "Name": "exceptionTypeName",
          "Type": "string",
          "Assignment": "$this.GetType().FullName"
        },
        {
          "Name": "exception",
          "Type": "string",
          "Assignment": "$this.AsJson()"
        }
      ]
    }
  ],
  "Settings": {
    "AutogenerateLoggerInterfaces": false
  }
}
```
* Add the logger interface to the eventsource definition. In the Loggers secion ``Loggers: []`` add a new logger entry:
```json
    {
      "Name": "IConsoleLogger",
      "StartId": 1000
    }
```
* Add the base NuGet for logging to your project ``FG.Diagnostics.AutoLogger.Model``. This includes some conveniece classes for the loggers.
* Run the tool again <kbd>ALT</kbd>+<kbd>L</kbd>. It should now have generated an number of additional files under the ``DefaultEventSource.eventsource.json`` in the project. These files contains the concrete loggers and ETW EventSource implementations based on the logger interface declared. You can now create a new instance of the concret logger directly in your code, where we instead of going to the singleton accessor of the EventSource, we can use the generated implementation of our logger interface. Add some logging code to your startup code:
```csharp
		static void Main(string[] args)
		{
			var logger = new ConsoleLogger();
			logger.StartRunMain(args);

			Console.WriteLine("Press any key");
			var key = Console.ReadKey();
			logger.KeyPressed(key);

			logger.StopRunMain();
		}
```

* Add some implicit arguments to each logged message (see below for a descritpion of implicit arguments), these will be included in each actual logged event but only set on the constructor of the concrete loggers. Let's say we wan't to log the machine name and process id of each of these console apps running. We can to that easily by injecting the current process object as an implicit argument to our logger:
```json
    {
      "Name": "IConsoleLogger",
      "StartId": 1000,
      "ImplicitArguments": [
        {
          "Name": "process",
          "Type": "Process"
        }
      ],
    }
```
For the AutoLogger to know how to logg this object we need to add a template for which arguments should be logged for this complex type. In the ``TypeTemplates`` section of the eventsource json, add a new entry directly after the first entry for ``System.Exception`` (that one is included by default to help you create more complex TypeTemplates):
```json
    {
      "Name": "Process",
      "CLRType": "System.Diagnostics.Process",
      "Arguments": [
        {
          "Name": "machineName",
          "Type": "string",
          "Assignment": "$this.MachineName"
        },
        {
          "Name": "processId",
          "Type": "int",
          "Assignment": "$this.Id"
        }
      ]
    }
```
* Run the tool again <kbd>ALT</kbd>+<kbd>L</kbd>. It will now add some additional arguments to the constructor of your concrete logger. Change the code in ``Program.Main()`` to add the implicit arguments at creation:
```csharp
			var logger = new ConsoleLogger(Process.GetCurrentProcess());
```

Below are details on how to use AutoLogger in more complex scenarios.

## Adding EventSources to a project

1) Add the NuGet package ``FG.Diagnostics.AutoLogger.Generator`` to a project in Visual Studio.  Install with ``Install-Package FG.Diagnostics.AutoLogger.Generator`` and ``Install-Package FG.Diagnostics.AutoLogger.AI`` (all packages listed at [https://www.nuget.org/profiles/Fabricators-Guild](https://www.nuget.org/profiles/Fabricators-Guild)
2) Add any number of loggers to the project [Adding Loggers to a project](#adding-loggers-to-a-project)
3) Build the project.
4) [Modify](#mofifying-an-eventsource-file) the ``DefaultEventSource.eventsource`` file -or- create a new ``*EventSource.eventsource.json`` file in your project

## Adding Loggers to a project

Add a new ``.cs`` file to the project. The file should be named ``I{Name of your logger}Logger.cs``. *E.g. to create a logger for Semantic events, name the logger file ``ISemanticEventsLogger``*

Add a new interface to the file, the interface shoud be named the same way as the file, i.e. ``I{Name of your logger}Logger``

```csharp
    public interface IConsoleLogger
    {
        void SayHello(string message);
        void Message(string message);
        void Error(Exception exception);
        void SayGoodbye(string goodbye, DateTime nightTime);
    }    
```

**Important**: The interface should be plain and only use C# 5 or below (*No Roslyn!*)

## Modifying an .eventsource.json file

The ``.eventsource.json`` is an json file that should have the following structure:

```json
{
  "Name": "",
  "Settings": {},
  "Keywords": [],
  "TypeTemplates": [],
  "Loggers": [],
  "Events": []
}
```

The json schema can be found [here](https://raw.githubusercontent.com/FredrikGoransson/CodeEffect.Diagnostics.EventSourceGenerator/master/src/CodeEffect.Diagnostics.EventSourceGenerator.Schema/CodeEffect.Diagnostics.EventSourceGenerator.Model.json).

The *.eventsource.json file defines *how* the new EventSource should be generated. It can control the following aspects:

* The naming of the ETW provider generated
* [EventSource Keywords](#eventsource-keywords)
* [Templates for how to log complex types (see supported types)](#type-templates)
* [Loggers to implement](#lgger-interfaces)
  * EventId series
  * Implicit arguments for use with loggers
* Custom events (not included in loggers)
  * EventIds generated *it is recommended to use loggers instead as this is a much cleaner way to define events*
  * Naming
  * Arguments



### Naming of ETW provider

The name of the ETW provider is the provider name that will be logged to:

```csharp
  [EventSource(Name = "CompanyName-ProjectNamespace")]
  internal sealed partial class DefaultEventSource : EventSource
  {
    public static readonly DefaultEventSource Current = new DefaultEventSource();

    static DefaultEventSource()
    {
      // A workaround for the problem where ETW activities do not 
      // get tracked until Tasks infrastructure is initialized.
      // This problem will be fixed in .NET Framework 4.6.2.
      Task.Run(() => { });
    }
  ...
```

If Name is set to ``CompanyName-ProjectNamespace`` in the ``.eventsource`` file it will be generated as above.

### EventSource keywords

Keywords can be explicitly defined in the  ``.eventsource.json`` file as a string list:

```json
{
  "Name": "",
  "Settings": {},
  "Keywords": ["Infrastructure", "Domain", "Process", "Debugging"],
  "TypeTemplates": [],
  "Loggers": [],
  "Events": []
}
```

These can then be referenced by events defined in the  ``.eventsource.json`` file.

For loggers, a new keyword is implicitly generated for each logger. This means that we can split loggers on functionality and get them logged to the same EventSource but with different keywords to separate them. Events emitted from a logger will automatically have the keyword generated from that logger set.

*E.g. if we have two loggers, ``IHostLogger`` and ``IDomainLogger`` then two keywords ``"Host"`` and ``"Domain"`` will be added to the EventSource.*

### Type Templates

Type templates allows complex types to be split into multiple logged arguments. Since ETW only allows for logging of simple types ((see supported types)[#supported-types]), complex arguments have to be split into relevant components to log. Type templates are defined as the type to split and the corresponding sub-arguments:

```json
    {
      "Name": "Exception",
      "CLRType": "System.Exception",
      "Arguments": [
        {
          "Assignment": "$this.Message",
          "Name": "message",
          "Type": "string"
        },
        {
          "Assignment": "$this.Source",
          "Name": "source",
          "Type": "string"
        },
        {
          "Assignment": "$this.GetType().FullName",
          "Name": "exceptionTypeName",
          "Type": "string"
        },
        {
          "Assignment": "$this.AsJson()",
          "Name": "exception",
          "Type": "string"
        }
      ]
    }
```

The ``Name`` of the Type Template can then be used in either Implicit Arguments, Override Arguments or in custom Event definitions. The ``CLRType`` should be the CLR FullName of the type that should be expanded. This is useful when a logger contains complex types (e.g. ``System.Exception``) that we should decide the logging behavior for in the eventsource definition, not the logger.

The array of arguments should countain all arguments that should be logged for this type. Each argument should have

* Name - The name of the argument as it is logged 
* Type - The type of the argument that is logged, only simple types are allowed ((see supported types)[#supported-types]). *Note, this is NOT the type of the complex type*
* Assignment - How the simple argument is assigned from the complex type.

Assignment should contain the constant ``$this`` that denotes the original complex type. Single line assignment can be used here and the assignment should always return an instance of the type specified by ``Type``

#### Extension methods

Assignments could use a number of simple extension methods to simplify logging. The available methods are currently:

* AsJson() - Serialized the entire object as json and returns it as a string
* GetContentDigest() - Creates a digest of a long string (that is naturally too long to be logged) on the format ``"[First 30 chars]... (length in chars) [MD5 hash of content]"``
* GetMD5Hash() - A string representation of the MD5 hash of the entire content

Other helper methods could be used but if they are defined in the code then they should be references not as static extension methods but as regular static methods, .e.g. ``MyHelperClass.GetValue($this)``.

### Logger interfaces

*Note: Logger interfaces should be implented in C# 5.0 or below*

In order for an interface to be included in an EventSource it should be included in the .eventsource file. Loggers are included in the ``Loggers`` section as such:

```json
  {
      "Name": "",
      "LoggerNamespace": "",
      "StartId": 0,
      "ImplicitArguments": [
        {
          "Name": "",
          "Type": "",
          "CLRType": ""
        }
      ],
      "OverrideArguments": []
    }
```

The ``Name`` property should match the name of the logger that is implemented somewhere in the code. It does not need to be the CLR FullName of the type, if more than one logger share the same name then use the CLR FullName to specify the logger name in the template. The ``LoggerNamespace`` could be omitted (should be?). ``StartId`` is the first EventId that is assigned to events/methods from this interface. The event ids are then incremented for each method in the interface.

*Note, it is important to keep the numbering of events in an EventSource. Previously assigned and used numbers (i.e. something has logged using that event id) should never be changed. Therefore it is recommended to always append new logger methods to the end of the interface as events are generated in exactly the order they occur in the interface declaration*

## The Generated output

The msbuild-task generates outputs in the form of ``.cs`` implementation files and adds them directly to the project. The following structure and files are generated for each ``.eventsource`` file found:

* MyEventSource.eventsource
  * MyEventSource.cs

The generated ``EventSource`` will contain any custom events defined in the .eventsource file and the boilerplate code to create the singleton accessor to the EventSource, setting up Keywords and any specific helper extension methods defined by the loggers.

### Concrete loggers

For each logger ``I*Logger`` it additionaly generates a partial EventSource file and a separate concrete logger. For a project containing ``MyEventSource.eventsource`` and ``IMyLogger.cs`` the following will be generated:

* MyEventSource.eventsource
  * MyEventSource.cs
  * MyEventSource.IMyLogger.cs
  * MyLogger.cs

``MyLogger`` is the concrete implemenatation of ``IMyLogger`` and it uses the partial of ``MyEventSource`` implemented in ``MyEventSource.IMyLogger.cs``.

```csharp
	internal sealed class MyLogger : IMyLogger
	{
		private System.Diagnostics.Process _process;

		public MyLogger(
			System.Diagnostics.Process process)
		{
			_process = process;
		}

		public void KeyPushed(
			ConsoleKeyInfo consoleKey)
		{

			DefaultEventSource.Current.KeyPushed(
				_process, 
				consoleKey
			);
		}
	}
```

This example shows the generated logger implementation for a logger with one method. This logger is additionaly defined in the .eventsource file as

```json
  {
      "Name": "IMyLogger",
      "LoggerNamespace": null,
      "StartId": 2000,
      "ImplicitArguments": [
        {
          "Name": "process",
          "Type": "Process",
          "CLRType": null
        }
      ],
      "OverrideArguments": null
    }
```

*Note, ``process`` should be defined as a Type Template since it is a complex type. 

```json
    {
      "Name": "Process",
      "CLRType": "System.Diagnostics.Process",
      "Arguments": [
        {
          "Assignment": "$this.Id",
          "Name": "processId",
          "Type": "int",
          "CLRType": "int"
        },
        {
          "Assignment": "$this.MachineName",
          "Name": "machineName",
          "Type": "string",
          "CLRType": "string"
        },
        {
          "Assignment": "$this.ProcessName",
          "Name": "processName",
          "Type": "string",
          "CLRType": "string"
        }
      ]
    }
```


## Using the Eventsource

The resulting EventSource should primarily be used through the generated, concrete implementation classes based on loggers. These can be instantiated within a class and passed around to other classed and methods without creating a strong dependency on the Singleton implementation of the EventSource. The concrete logger implementations in turn use the Singleton accessor for the EventSource, but this is opaque to the user of the classes.

## Generating EventSources and loggers

There are currently two ways of generating concrete loggers and EventSource implementations in a project

* Using the MSBuild NuGet that triggers a pre-build of related files and then includes the generated result. **NOTE: This method is currenly not working well with larger projects and may end up in .dlls shared between multiple projects using the MSBuild target being locked by overlapping MSBuild processes**
* Using a separate Tool. Recommended is to add this tool to Visual Studio for easier access and use. The steps for doing that is included below



