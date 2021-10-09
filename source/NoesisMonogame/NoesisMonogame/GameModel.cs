using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace NoesisMonogame
{
    public class GameModel
    {
        public enum States
        {
            Setup,
            Running,
            Pause,
            Exit,
        }

        public interface IGameEvent {}
        
        public class Start : IGameEvent {}

        public class Exit : IGameEvent {}
        public class Pause : IGameEvent {}

        public class Move : IGameEvent
        {
            public enum Direction
            {
                Up,
                Down,
                Right,
                Left,
            }

            private readonly Vector2 _vector;
            private readonly GameTime _gameTime;
            
            public Move(Direction direction, GameTime gameTime)
            {
                switch (direction)
                {
                    case Direction.Up:
                        _vector = -Vector2.UnitY;
                        break;
                    case Direction.Down:
                        _vector = Vector2.UnitY;
                        break;
                    case Direction.Right:
                        _vector = Vector2.UnitX;
                        break;
                    case Direction.Left:
                        _vector = -Vector2.UnitX;
                        break;
                }
                _gameTime = gameTime;
            }

            public Vector2 GetMovementVector(float speed)
            {
                return _vector * speed * (float)_gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public States State { get; private set; }

        private Vector2 _ballPosition;
        public Vector2 BallPosition => _ballPosition;

        private Vector2 _screenSize;
            
        private delegate States? StateEntryFunc(States lastState, States newState, IGameEvent gameEvent);
        private delegate States? StateFunc(States state, IGameEvent gameEvent);

        private readonly Dictionary<States, StateEntryFunc> _entryFunctions = new();
        private readonly Dictionary<States, Dictionary<Type, StateFunc>> _stateFunctions = new();

        public GameModel(Vector2 screenSize)
        {
            // setup entry functions
            _entryFunctions[States.Running] = ResetGame;
            
            // setup state functions
            _stateFunctions[States.Setup] = new Dictionary<Type, StateFunc>
            {
                {typeof(Start), Goto(States.Running)},
                {typeof(Exit), Goto(States.Exit)},
            };
            _stateFunctions[States.Running] = new Dictionary<Type, StateFunc>
            {
                {typeof(Pause), Goto(States.Pause)},
                {typeof(Move), OnMove}
            };
            _stateFunctions[States.Pause] = new Dictionary<Type, StateFunc>
            {
                {typeof(Start), Goto(States.Running)},
                {typeof(Exit), Goto(States.Setup)},
            };

            _screenSize = screenSize;
            _ballPosition = new Vector2(
                screenSize.X / 2,
                screenSize.Y / 2
                );
            State = States.Setup;
        }

        public void Trigger(IGameEvent gameEvent)
        {
            Console.WriteLine($"[Info] Trigger event '{gameEvent.GetType()}' in state '{State}'.");
            var newState = ProcessStateHandler(State, gameEvent);

            while ((newState != null) && (newState != State))
            {
                var oldState = State;
                State = (States)newState;

                newState = ProcessEntryHandler(oldState, State, gameEvent);
            }
        }

        private States? ProcessStateHandler(States currentState, IGameEvent gameEvent)
        {
            StateFunc handler = null;
            if (_stateFunctions.TryGetValue(currentState, out var eventHandler))
            {
                eventHandler.TryGetValue(gameEvent.GetType(), out handler);
            }

            if (handler == null)
            {
                Console.WriteLine($"[Warning] Cannot find handler for state '{currentState}' and event '{gameEvent.GetType()}'.");
                return null;
            }
            
            return handler(currentState, gameEvent);
        }


        private States? ProcessEntryHandler(States lastState, States newState, IGameEvent gameEvent)
        {
            if (_entryFunctions.TryGetValue(newState, out var entryHandler))
            {
                return entryHandler(lastState, newState, gameEvent);
            }

            return null;
        }
        

        // entry functions
        private States? ResetGame(States lastState, States newState, IGameEvent gameEvent)
        {
            if (lastState == States.Setup)
            {
                _ballPosition.X = _screenSize.X / 2;
                _ballPosition.Y = _screenSize.Y / 2;
            }
            return null;
        }
        
        
        // state functions
        private static StateFunc Goto(States newState)
        {
            return (_, _) => newState;
        }


        private States? OnMove(States state, IGameEvent gameEvent)
        {
            Debug.Assert(gameEvent is Move);
            var e = (Move)gameEvent;

            _ballPosition += e.GetMovementVector(200f);
            if (_ballPosition.X < 0)
            {
                _ballPosition.X = 0;
            }
            if (_ballPosition.X > _screenSize.X)
            {
                _ballPosition.X = _screenSize.X;
            }
            if (_ballPosition.Y < 0)
            {
                _ballPosition.Y = 0;
            }
            if (_ballPosition.Y > _screenSize.Y)
            {
                _ballPosition.Y = _screenSize.Y;
            }
            
            return null;
        }
        
    }
}