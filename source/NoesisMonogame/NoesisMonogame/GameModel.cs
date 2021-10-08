using System;
using System.Collections.Generic;

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

        public States State { get; private set; }

        private delegate States? StateEntryFunc(States lastState, States newState, IGameEvent gameEvent);
        private delegate States? StateFunc(States state, IGameEvent gameEvent);

        private Dictionary<States, StateEntryFunc> _entryFunctions = new();
        private Dictionary<States, Dictionary<Type, StateFunc>> _stateFunctions = new();

        public GameModel()
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
            };
            _stateFunctions[States.Pause] = new Dictionary<Type, StateFunc>
            {
                {typeof(Start), Goto(States.Running)},
                {typeof(Exit), Goto(States.Setup)},
            };

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
            return null;
        }
        
        
        // state functions
        private static StateFunc Goto(States newState)
        {
            return (_, _) => newState;
        }
        
    }
}