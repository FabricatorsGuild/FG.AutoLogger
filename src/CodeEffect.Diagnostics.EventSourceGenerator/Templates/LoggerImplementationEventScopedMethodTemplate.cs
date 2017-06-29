namespace FG.Diagnostics.AutoLogger.Generator.Templates
{
    public static class LoggerImplementationEventScopedMethodTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";

        public const string Template_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";
        public const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        public const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";

        public const string Variable_LOGGER_METHOD_NAME = @"@@LOGGER_METHOD_NAME@@";
        public const string Variable_LOGGER_METHOD_RETURNTYPE = @"@@LOGGER_METHOD_RETURNTYPE@@";
        public const string Variable_LOGGER_METHOD_ARGUMENTS = @"@@LOGGER_METHOD_ARGUMENTS@@";

        public const string Template_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION_DELIMITER = @",
			";

        public const string Variable_LOGGER_METHOD_IMPLEMENTATION_START_CALL = @"@@LOGGER_METHOD_IMPLEMENTATION_START_CALL@@";
        public const string Variable_LOGGER_METHOD_IMPLEMENTATION_STOP_CALL = @"@@LOGGER_METHOD_IMPLEMENTATION_STOP_CALL@@";
        public const string Template_LOGGER_METHOD_IMPLEMENTATION_WRAPPER_CALL = @"
		        ScopeWrapperWithAction.Wrap(() =>
		        {
@@LOGGER_METHOD_IMPLEMENTATION_START_CALL@@
		            return new ScopeWrapperWithAction(() =>
		            {
@@LOGGER_METHOD_IMPLEMENTATION_STOP_CALL@@
		            });
		        }),";

        public const string Variable_LOGGER_METHOD_SCOPEWRAPPER_CALLS = @"@@LOGGER_METHOD_SCOPEWRAPPER_CALLS@@";
        public const string Variable_LOGGER_METHOD_SCOPEWRAPPER_EXECUTION = @"@@LOGGER_METHOD_SCOPEWRAPPER_EXECUTION@@";
        public const string Template_LOGGER_METHOD_SCOPEWRAPPER_CALL = @"
		        ((Func<IDisposable>) (() =>
		        {
@@LOGGER_METHOD_SCOPEWRAPPER_EXECUTION@@
		        }))(),";

        public const string Template_LOGGER_METHOD = @"
		public @@LOGGER_METHOD_RETURNTYPE@@ @@LOGGER_METHOD_NAME@@(
			@@LOGGER_METHOD_ARGUMENTS@@)
		{
		    return new ScopeWrapper(new IDisposable[]
		    {
@@LOGGER_METHOD_SCOPEWRAPPER_CALLS@@
		    });
		}
";
        // ReSharper restore InconsistentNaming
    }
}