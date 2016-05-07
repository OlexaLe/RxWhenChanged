using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RxWhenChanged
{
    public class RxCommand<T> : ICommand, IDisposable
    {
        private readonly IDisposable _canExecuteSubscription;
        private readonly Func<object, Task<T>> _execute;
        private readonly Subject<bool> _isExecuting = new Subject<bool>();
        private readonly Subject<T> _results = new Subject<T>();
        private readonly Subject<Exception> _thrownExceptions = new Subject<Exception>();

        private bool _canExecuteLatest;
        private bool _isDisposed;

        public RxCommand(IObservable<bool> canExecute, Func<object, Task<T>> execute)
        {
            _execute = execute;
            _canExecuteSubscription = canExecute.Subscribe(RaiseCanExecuteChanged);
        }

        public IObservable<bool> IsExecuting => _isExecuting;

        public IObservable<T> Results => _results;

        public IObservable<Exception> ThrownExceptions => _thrownExceptions;

        public bool CanExecute(object parameter)
        {
            ThrowIfDisposed();
            return _canExecuteLatest;
        }

        public async void Execute(object parameter)
        {
            ThrowIfDisposed();
            _isExecuting.OnNext(true);
            try
            {
                var result = await _execute(parameter);
                _results.OnNext(result);
            }
            catch (Exception e)
            {
                _thrownExceptions.OnNext(e);
            }
            _isExecuting.OnNext(false);
        }

        public event EventHandler CanExecuteChanged;

        public void Dispose()
        {
            _isDisposed = true;
            _canExecuteSubscription?.Dispose();
            _isExecuting?.Dispose();
            _results?.Dispose();
            _thrownExceptions?.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void RaiseCanExecuteChanged(bool value)
        {
            _canExecuteLatest = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}