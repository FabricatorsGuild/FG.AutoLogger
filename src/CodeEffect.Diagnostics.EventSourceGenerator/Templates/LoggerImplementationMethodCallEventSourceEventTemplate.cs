namespace CodeEffect.Diagnostics.EventSourceGenerator.Templates
{
    public static class LoggerImplementationMethodCallEventSourceEventTemplate
    {
        // ReSharper disable InconsistentNaming

        public const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";

        public const string Variable_LOGGER_METHOD_NAME = @"@@LOGGER_METHOD_NAME@@";

        public const string Variable_LOGGER_METHOD_IMPLEMENTATION_CALL_ARGUMENTS = @"@@LOGGER_METHOD_IMPLEMENTATION_CALL_ARGUMENTS@@";

        public const string Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT = @"_@@ARGUMENT_NAME@@";
        public const string Template_METHOD_CALL_PASSTHROUGH_ARGUMENT = @"@@ARGUMENT_NAME@@";
        public const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        public const string Template_LOGGER_METHOD_CALL_EVENTSOURCE_EVENT = @"			@@EVENTSOURCE_CLASS_NAME@@.Current.@@LOGGER_METHOD_NAME@@(
				@@LOGGER_METHOD_IMPLEMENTATION_CALL_ARGUMENTS@@
			);
";
        public const string Template_LOGGER_CALL_ARGUMENTS_DELIMITER = @", 
				";

        // ReSharper restore InconsistentNaming
    }
}