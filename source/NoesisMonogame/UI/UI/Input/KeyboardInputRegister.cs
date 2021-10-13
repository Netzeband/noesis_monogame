using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.XInput;

namespace UI.Input
{
    public class KeyboardInputRegister<T> : IKeyboardInputRegister<T> where T : Enum
    {
        private readonly Dictionary<T, Dictionary<Keys, IKeyboardInputRegister<T>.HandlerFunction>> _handlers = new();
        private Keys[] _pressedKeys;
        private Keys[] _lastPressedKeys = Array.Empty<Keys>();

        public void RegisterKey(Keys key, T state, IKeyboardInputRegister<T>.HandlerFunction function)
        {
            if (!_handlers.ContainsKey(state))
            {
                _handlers[state] = new Dictionary<Keys, IKeyboardInputRegister<T>.HandlerFunction>();
            }

            _handlers[state][key] = function;
        }
        
        public Keys[] ProcessKeys(Keys[] pressedKeys, T state, GameTime gameTime)
        {
            var unconsumedKeys = new List<Keys>();
            var consumedKeys = new List<Keys>();
            _pressedKeys = pressedKeys;

            foreach (var key in pressedKeys.Concat(_lastPressedKeys))
            {
                if (!consumedKeys.Contains(key))
                {
                    if (ProcessKey(key, state, gameTime))
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
        
        
        private bool ProcessKey(Keys key, T state, GameTime gameTime)
        {
            if (TryGetStateFunction(key, state, out var handler))
            {
                if (handler(key, state, gameTime))
                {
                    return _pressedKeys.Contains(key);
                }
            }

            return false;
        }

        private bool TryGetStateFunction(Keys key, T state, out IKeyboardInputRegister<T>.HandlerFunction handler)
        {
            if (_handlers.TryGetValue(state, out var keyFunctions))
            {
                if (keyFunctions.TryGetValue(key, out handler))
                {
                    if (handler != null)
                    {
                        return true;
                    }
                }
            }

            handler = null;
            return false;
        }
        
        private bool WaitForKeyUp(Keys key, T state, GameTime gameTime, IKeyboardInputRegister<T>.HandlerFunction handler)
        {
            if (!_pressedKeys.Contains(key) && _lastPressedKeys.Contains(key))
            {
                return handler(key, state, gameTime);
            }
            return true;
        }
        

        public IKeyboardInputRegister<T>.HandlerFunction AtKeyUp(IKeyboardInputRegister<T>.HandlerFunction handler)
        {
            return (k,s, t) => WaitForKeyUp(k, s, t, handler);
        }

        
        private bool WaitForKeyDown(Keys key, T state, GameTime gameTime, IKeyboardInputRegister<T>.HandlerFunction handler)
        {
            if (_pressedKeys.Contains(key) && !_lastPressedKeys.Contains(key))
            {
                return handler(key, state, gameTime);
            }
            return true;
        }
        

        public IKeyboardInputRegister<T>.HandlerFunction AtKeyDown(IKeyboardInputRegister<T>.HandlerFunction handler)
        {
            return (k,s, t) => WaitForKeyDown(k, s, t, handler);
        }

    }
}