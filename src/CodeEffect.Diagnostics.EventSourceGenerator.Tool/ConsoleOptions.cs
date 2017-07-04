using System.IO;
using CommandLine;

namespace FG.Diagnostics.AutoLogger.Tool
{
    internal class ConsoleOptions
    {
        [Option('p', "projectFile", Required = true,
            HelpText = "CS Project file to be processed.")]
        public string ProjectFile { get; set; }

        [Option('v', "verbose", DefaultValue = true,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('o', "output", DefaultValue = false,
            HelpText = "Prints out all generated output.")]
        public bool DisplayOutput { get; set; }


        [Option('s', "save", DefaultValue = false,
            HelpText = "Save all changes")]
        public bool SaveChanges { get; set; }

        [Option('g', "generateSchema", DefaultValue = false,
            HelpText = "Generates a JSON schema for the EventSource model")]
        public bool GenerateSchema { get; set; }

        [Option('f', "force", DefaultValue = false,
            HelpText = "Force update")]
        public bool ForceUpdate { get; set; }


        [Option('i', "interactive", DefaultValue = false,
            HelpText = "Interactive mode, requests and waits for user interaction")]
        public bool Interactive { get; set; }

        [Option('n', "interactive", DefaultValue = false,
            HelpText = "Ignores to check online for a newer version of the tool.")]
        public bool IgnoreVersionCheck { get; set; }
    }
}