using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using NoesisApp;
using NoesisMonogame;

namespace Data.UI
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand Start { get; }
        public ICommand Exit { get; }
        
        public Views View
        {
            get => _view;
            private set
            {
                if (_view != value)
                {
                    _view = value;
                    RaisePropertyChanged(nameof(View));
                }
            }
        }

        private readonly GameModel _gameModel;
        private Views _view = Views.Menu;

        public ViewModel(GameModel gameModel)
        {
            _gameModel = gameModel;
            
            Start = new DelegateCommand(OnStart);
            Exit = new DelegateCommand(OnExit);
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Update(GameTime gameTime)
        {
            switch (_gameModel.State)
            {
                case GameModel.States.Setup:
                {
                    View = Views.Menu;
                    break;
                }
                
                case GameModel.States.Running:
                {
                    View = Views.Game;
                    break;
                }
                
                case GameModel.States.Pause:
                {
                    View = Views.Pause;
                    break;
                }

                default:
                {
                    View = Views.None;
                    break;
                }
            }
        }
        
        private void OnStart(object parameter)
        {
            _gameModel.Trigger(new GameModel.Start());
        }

        private void OnExit(object parameter)
        {
            _gameModel.Trigger(new GameModel.Exit());
        }
        
    }
}