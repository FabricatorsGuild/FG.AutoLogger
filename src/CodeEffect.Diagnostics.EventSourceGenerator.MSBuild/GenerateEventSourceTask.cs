namespace CodeEffect.Diagnostics.EventSourceGenerator.MSBuild
{
    public class GenerateEventSourceTask : Microsoft.Build.Utilities.Task
    {
        // ReSharper disable once MemberCanBePrivate.Global - Set by the target task
        public string ProjectFilePath { get; set; }
        public string CscToolPath { get; set; }

        public override bool Execute()
        {
            Log.LogMessage($"Executing {nameof(GenerateEventSourceTask)} for project {ProjectFilePath}");

            var projectEventSourceGenerator = new ProjectEventSourceGenerator();
            projectEventSourceGenerator.SetLogMessage(m => Log.LogMessage(m));
            projectEventSourceGenerator.SetLogWarning(w => Log.LogWarning(w));
            projectEventSourceGenerator.SetLogError(e => Log.LogError(e));

            var project = projectEventSourceGenerator.Run(ProjectFilePath, CscToolPath, saveChanges:true);

            Log.LogMessage($"Executed {nameof(GenerateEventSourceTask)} in {ProjectFilePath}");

            return true;
        }
    }
}
