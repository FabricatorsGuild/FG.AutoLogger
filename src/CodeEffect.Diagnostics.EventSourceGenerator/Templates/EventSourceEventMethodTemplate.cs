namespace CodeEffect.Diagnostics.EventSourceGenerator.Templates
{
    public static class EventSourceEventMethodTemplate
    {
// ReSharper disable InconsistentNaming
        public const string Variable_EVENT_NAME = @"@@EVENT_NAME@@";
        public const string Variable_EVENT_ID = @"@@EVENT_ID@@";
        public const string Variable_EVENT_LEVEL = @"@@EVENT_LEVEL@@";
        public const string Variable_EVENT_KEYWORDS_DECLARATION = @"@@EVENT_KEYWORDS_DECLARATION@@";
        public const string Variable_EVENT_OPCODE_DECLARATION = @"@@EVENT_OPCODE_DECLARATION@@";
        public const string Variable_EVENT_TASK_DECLARATION = @"@@EVENT_TASK_DECLARATION@@";
        public const string Variable_EVENT_MESSAGE_FORMATTER = @"@@EVENT_MESSAGE_FORMATTER@@";
        public const string Variable_EVENT_METHOD_ACCESS = @"@@EVENT_METHOD_ACCESS@@";
        public const string Variable_EVENT_METHOD_ARGUMENTS = @"@@EVENT_METHOD_ARGUMENTS@@";
        public const string Template_EVENT_METHOD_ARGUMENT_DELIMITER = @", 
			";
        public const string Template_EVENT_METHOD_CALL_ARGUMENT_DELIMITER = @", 
				";
        public const string Variable_WRITEEVENT_CALL_ARGUMENTS = @"@@WRITEEVENT_CALL_ARGUMENTS@@";

        public const string Variable_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";
        public const string Variable_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        public const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";

        public const string Template_METHOD_CALL_PASSTHROUGH_ARGUMENT = @"@@ARGUMENT_NAME@@";
        public const string Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT = @"_@@ARGUMENT_NAME@@";


        public const string Template_EVENT_METHOD = @"
		private const int @@EVENT_NAME@@EventId = @@EVENT_ID@@;

		[Event(@@EVENT_NAME@@EventId, Level = EventLevel.@@EVENT_LEVEL@@, Message = ""@@EVENT_MESSAGE_FORMATTER@@""@@EVENT_KEYWORDS_DECLARATION@@@@EVENT_OPCODE_DECLARATION@@@@EVENT_TASK_DECLARATION@@)]
		@@EVENT_METHOD_ACCESS@@ void @@EVENT_NAME@@(
			@@EVENT_METHOD_ARGUMENTS@@)
		{
			WriteEvent(
				@@WRITEEVENT_CALL_ARGUMENTS@@);
		}";
    }
}