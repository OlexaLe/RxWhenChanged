# RxWhenChanged
Helpers for ViewModels

## WhenChanged
Extension for `INotifyPropertyChanged` interface. Use it to observe property changes in ViewModels.
Signature:
```c#
IObservable<TResult> WhenChanged<TResult>(this INotifyPropertyChanged self, string property)
```

Usage:
```c#
this.WhenChanged<string>(nameof(Input))
  .Delay(TimeSpan.FromMilliseconds(100), scheduler)
  .Subscribe(v => Output = v);
```

## RxCommand
ICommand implementation with Rx Observables.

Signature:
```c#
public RxCommand(IObservable<bool> canExecute, Func<object, Task<T>> execute)
```
canExecute parameter is mandatory. Use ```Observable.Return(true)``` in case if don't need it.

Usage:
```c#
var cmd = new RxCommand<string>(cmdCanExecute, DoSomethingAsync);
cmd.Results.Subscribe(...);
cmd.ThrownExceptions.Subscribe(...);
cmd.IsExecuting.Subscribe(...);
```
Do not forget to call Dispose() in real projects!

Please refer to unit tests for documentation and inspiration. Feel free to create Issues with bugs or propositions.
Thanks!
