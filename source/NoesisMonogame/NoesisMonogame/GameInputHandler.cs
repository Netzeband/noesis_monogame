using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NoesisMonogame
{
    public class GameInputHandler : UI.Input.IKeyboardInputHandler
    {
        private delegate bool StateFunc(GameModel.States state, Keys key, GameTime gameTime);
            
        private readonly GameModel _model;
        private readonly Dictionary<GameModel.States, Dictionary<Keys, StateFunc>> _stateFunctions = new();
        private Keys[] _lastPressedKeys;
        private Keys[] _pressedKeys;

        public GameInputHandler(GameModel model)
        {
            _lastPressedKeys = Array.Empty<Keys>();
            _model = model;
            _stateFunctions[GameModel.States.Running] = new Dictionary<Keys, StateFunc>()
            {
                {Keys.Up, (_, _, t) => { _model.Trigger(new GameModel.Move(GameModel.Move.Direction.Up, t)); return true; }},
                {Keys.Down, (_, _, t) => { _model.Trigger(new GameModel.Move(GameModel.Move.Direction.Down, t)); return true; }},
                {Keys.Right, (_, _, t) => { _model.Trigger(new GameModel.Move(GameModel.Move.Direction.Right, t)); return true; }},
                {Keys.Left, (_, _, t) => { _model.Trigger(new GameModel.Move(GameModel.Move.Direction.Left, t)); return true; }},
                {Keys.Escape, AtKeyUp((_, _, t) => { _model.Trigger(new GameModel.Pause()); return true; })}
            };
            _stateFunctions[GameModel.States.Pause] = new Dictionary<Keys, StateFunc>()
            {
                {Keys.Escape, AtKeyUp((_, _, t) => { _model.Trigger(new GameModel.Start()); return true; })}
            };
            
            Debug.Assert(_model != null);
        }

        public Keys[] ProcessKeys(Keys[] pressedKeys, GameTime gameTime)
        {
            var unconsumedKeys = new List<Keys>();
            var consumedKeys = new List<Keys>();
            _pressedKeys = pressedKeys;

            foreach (var key in pressedKeys.Concat(_lastPressedKeys.ToArray()))
            {
                if (!consumedKeys.Contains(key))
                {
                    if (ProcessKey(_model.State, key, gameTime))
                    {
                        consumedKeys.Add(key);   
                    }
                    else if (_pressedKeys.Contains(key))
                    {
                        unconsumedKeys.Add(key);
                    }
                }
            }

            _lastPressedKeys = consumedKeys.ToArray();
            return unconsumedKeys.ToArray();
        }


        private bool ProcessKey(GameModel.States state, Keys key, GameTime gameTime)
        {
            if (TryGetStateFunction(state, key, out var function))
            {
                if (function(state, key, gameTime))
                {
                    return _pressedKeys.Contains(key);
                }
            }

            return false;
        }

        
        private bool TryGetStateFunction(GameModel.States state, Keys key, out StateFunc function)
        {
            if (_stateFunctions.TryGetValue(state, out var keyFunctions))
            {
                if (keyFunctions.TryGetValue(key, out function))
                {
                    if (function != null)
                    {
                        return true;
                    }
                }
            }

            function = null;
            return false;
        }


        private bool WaitForKeyUp(GameModel.States state, Keys key, GameTime gameTime, StateFunc stateFunc)
        {
            if (!_pressedKeys.Contains(key) && _lastPressedKeys.Contains(key))
            {
                return stateFunc(state, key, gameTime);
            }
            return true;
        }
        

        private StateFunc AtKeyUp(StateFunc stateFunction)
        {
            return (s, k, t) => WaitForKeyUp(s, k, t, stateFunction);
        }
        
    }
}