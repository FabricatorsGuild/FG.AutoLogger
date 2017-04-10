using CommandLine;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Tool
{
    internal class ConsoleOptions
    {
        [Option('p', "projectFile", Required = true,
            HelpText = "CS Project file to be processed.")]
        public string ProjectFile { get; set; }

        [Option('v', "verbose", Default = true,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('o', "output", Default = false,
            HelpText = "Prints out all generated output.")]
        public bool DisplayOutput { get; set; }
    }
}