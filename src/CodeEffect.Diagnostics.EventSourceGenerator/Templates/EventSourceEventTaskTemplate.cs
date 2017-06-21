namespace CodeEffect.Diagnostics.EventSourceGenerator.Templates
{
    public static class EventSourceEventTaskTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Template_EVENTTASK = @"			public const EventTask @@EVENTTASK_NAME@@ = (EventTask)0x@@EVENTTASK_INDEX@@L;";
        public const string Template_EVENTTASK_NAME = @"@@EVENTTASK_NAME@@";
        public const string Template_EVENTTASK_INDEX = @"@@EVENTTASK_INDEX@@";
        // ReSharper restore InconsistentNaming
    }
}