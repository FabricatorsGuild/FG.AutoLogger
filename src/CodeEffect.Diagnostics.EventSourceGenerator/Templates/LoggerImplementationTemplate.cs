﻿namespace FG.Diagnostics.AutoLogger.Generator.Templates
{
    public static class LoggerImplementationTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Variable_LOGGER_SOURCE_FILE_NAME = @"@@LOGGER_SOURCE_FILE_NAME@@";
        public const string Variable_LOGGER_NAME = @"@@LOGGER_NAME@@";
        public const string Variable_LOGGER_CLASS_NAME = @"@@LOGGER_CLASS_NAME@@";
        public const string Variable_EVENTSOURCE_NAMESPACE = @"@@EVENTSOURCE_NAMESPACE@@";
        public const string Variable_NAMESPACE_DECLARATION = @"@@NAMESPACE_DECLARATION@@";
        public const string Variable_LOGGER_IMPLICIT_USING_DECLARATION = @"@@LOGGER_IMPLICIT_USING_DECLARATION@@";
        public const string Variable_LOGGER_USING_AUTOLOGGER_MODEL = @"@@LOGGER_USING_AUTOLOGGER_MODEL@@";

        public const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";
        public const string Template_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";

        public const string Template_PRIVATE_MEMBER_DECLARATION = @"private readonly @@ARGUMENT_CLR_TYPE@@ _@@ARGUMENT_NAME@@;";
        public const string Template_PRIVATE_MEMBER_ASSIGNMENT = @"_@@ARGUMENT_NAME@@ = @@ARGUMENT_NAME@@;";

        public const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";

        public const string Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT = @"@@LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT@@";
        public const string Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT_DELIMITER = @"
			";
        public const string Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION = @"@@LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION@@";
        public const string Template_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION_DELIMITER = @"
		";
        public const string Variable_LOGGER_IMPLICIT_ARGUMENTS_CONSTRUCTOR_DECLARATION = @"@@LOGGER_IMPLICIT_ARGUMENTS_CONSTRUCTOR_DECLARATION@@";
        public const string Variable_LOGGER_IMPLICIT_ARGUMENTS_METHOD_CONSTRUCTOR_DELIMITER = @",
			";
        public const string Variable_LOGGER_IMPLEMENTATION = @"@@LOGGER_IMPLEMENTATION@@";
        public const string Variable_LOGGER_IMPLEMENTATION_SCOPE_WRAPPERS = @"@@LOGGER_IMPLEMENTATION_SCOPE_WRAPPERS@@";
        public const string Template_LOGGER_IMPLEMENTATION_SCOPE_WRAPPERS = @"
        private sealed class ScopeWrapper : IDisposable
        {
            private readonly IEnumerable<IDisposable> _disposables;

            public ScopeWrapper(IEnumerable<IDisposable> disposables)
            {
                _disposables = disposables;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    foreach (var disposable in _disposables)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

	    private sealed class ScopeWrapperWithAction : IDisposable
        {
            private readonly Action _onStop;

            internal static IDisposable Wrap(Func<IDisposable> wrap)
            {
                return wrap();
            }

            public ScopeWrapperWithAction(Action onStop)
            {
                _onStop = onStop;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _onStop?.Invoke();
                }
            }
        }";


        public const string Template_LOGGER_CLASS_DECLARATION =
@"/*******************************************************************************************
*  This class is autogenerated from the class @@LOGGER_SOURCE_FILE_NAME@@
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Collections.Generic;
using @@NAMESPACE_DECLARATION@@;
@@LOGGER_IMPLICIT_USING_DECLARATION@@
@@LOGGER_USING_AUTOLOGGER_MODEL@@

namespace @@EVENTSOURCE_NAMESPACE@@
{
	internal sealed class @@LOGGER_CLASS_NAME@@ : @@LOGGER_NAME@@
	{
@@LOGGER_IMPLEMENTATION_SCOPE_WRAPPERS@@

		@@LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION@@

		public @@LOGGER_CLASS_NAME@@(
			@@LOGGER_IMPLICIT_ARGUMENTS_CONSTRUCTOR_DECLARATION@@)
		{
			@@LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT@@
		}
@@LOGGER_IMPLEMENTATION@@
	}
}
";
        // ReSharper restore InconsistentNaming
    }
}