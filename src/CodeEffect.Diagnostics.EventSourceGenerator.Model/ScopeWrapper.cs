using System;
using System.Collections.Generic;

namespace FG.Diagnostics.AutoLogger.Model
{
    public sealed class ScopeWrapper : IDisposable
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

    public sealed class ScopeWrapperWithAction : IDisposable
    {
        private readonly Action _onStop;

        public static IDisposable Wrap(Func<IDisposable> wrap)
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
}