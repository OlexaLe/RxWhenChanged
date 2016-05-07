using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RxWhenChanged.Tests.Playground
{
    public class PlaygroundViewModel : INotifyPropertyChanged
    {
        private readonly RxCommand<string> _echo;
        private string _input;
        private string _output;

        public PlaygroundViewModel(IScheduler scheduler)
        {
            this.WhenChanged<string>(nameof(Input))
                .Delay(TimeSpan.FromMilliseconds(100), scheduler)
                .Subscribe(v => Output = v);

            var echoCanExecute = this.WhenChanged<string>(nameof(Input))
                .Select(x => !string.IsNullOrWhiteSpace(x));
            _echo = new RxCommand<string>(echoCanExecute, DoEchoAsync);
            _echo.Results.Subscribe(v => EchoOutput = v);
        }

        public ICommand Echo => _echo;

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }

        public string Output
        {
            get { return _output; }
            private set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

        public string EchoOutput { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private Task<string> DoEchoAsync(object commandParameter)
        {
            return Task.FromResult(Input);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}