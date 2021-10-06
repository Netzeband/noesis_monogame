using System;
using System.ComponentModel;
using System.Windows.Input;
using Noesis;
using NoesisApp;

namespace Data.UI
{
    public class ViewModel : INotifyPropertyChanged
    {
        public delegate void ExitFunc();

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand Start { get; }
        public ICommand Exit { get; }
        public ICommand Pause { get; }
        public ICommand Resume { get; }
        public ICommand Stop { get; }
        
        public States State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    RaisePropertyChanged(nameof(State));
                }
            }
        }

        private readonly ExitFunc _exitFunc;
        private States _state = States.Menu;

        public ViewModel(ExitFunc exitFunc)
        {
            _exitFunc = exitFunc;
            
            Start = new DelegateCommand(OnStart);
            Exit = new DelegateCommand(OnExit);
            Pause = new DelegateCommand(OnPause);
            Resume = new DelegateCommand(OnResume);
            Stop = new DelegateCommand(OnStop);
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        private void OnStart(object parameter)
        {
            State = States.Game;
        }

        private void OnExit(object parameter)
        {
            _exitFunc();
        }
        
        private void OnPause(object parameter)
        {
            State = States.Pause;
        }

        private void OnResume(object parameter)
        {
            State = States.Game;
        }
        
        private void OnStop(object parameter)
        {
            State = States.Menu;
        }
    }
}