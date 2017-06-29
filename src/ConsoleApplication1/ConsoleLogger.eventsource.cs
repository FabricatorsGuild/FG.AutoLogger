/*******************************************************************************************
*  This class is autogenerated from the class ConsoleLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Collections.Generic;
using ConsoleApplication1.Loggers;


namespace ConsoleApplication1
{
	internal sealed class ConsoleLogger : IConsoleLogger
	{
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
        }


		private readonly bool _autogenerated;
		private readonly string _machineName;

		public ConsoleLogger(
			bool autogenerated,
			string machineName)
		{
			_autogenerated = autogenerated;
			_machineName = machineName;
		}

		public void SayHello(
			string message)
		{
			ConsoleApplication1EventSource.Current.SayHello(
				_autogenerated, 
				_machineName, 
				message
			);
    
		}


		public void Message(
			string message)
		{
			ConsoleApplication1EventSource.Current.Message(
				_autogenerated, 
				_machineName, 
				message
			);
    
		}


		public void Error(
			System.Exception exception)
		{
			ConsoleApplication1EventSource.Current.Error(
				_autogenerated, 
				_machineName, 
				exception
			);
    
		}


		public void SayGoodbye(
			string goodbye,
			System.DateTime nightTime)
		{
			ConsoleApplication1EventSource.Current.SayGoodbye(
				_autogenerated, 
				_machineName, 
				goodbye, 
				nightTime
			);
    
		}


		public void Specially(
			ConsoleApplication1.Loggers.Special special)
		{
			ConsoleApplication1EventSource.Current.Specially(
				_autogenerated, 
				_machineName, 
				special
			);
    
		}


		public void StartHello(
			)
		{
			ConsoleApplication1EventSource.Current.StartHello(
				_autogenerated, 
				_machineName
			);
    
		}


		public void StopHello(
			)
		{
			ConsoleApplication1EventSource.Current.StopHello(
				_autogenerated, 
				_machineName
			);
    
		}


	}
}
