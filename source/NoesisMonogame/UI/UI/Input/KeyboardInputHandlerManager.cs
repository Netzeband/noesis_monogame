using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    public class KeyboardInputHandlerManager : IKeyboardInputHandler
    {
        readonly Dictionary<InputHandlerPriority, List<IKeyboardInputHandler>> _handlers = new();

        public void Add(InputHandlerPriority priority, IKeyboardInputHandler handler)
        {
            if (!_handlers.ContainsKey(priority))
            {
                _handlers[priority] = new List<IKeyboardInputHandler>();
            }
            
            _handlers[priority].Add(handler);
        }

        public void Remove(IKeyboardInputHandler handler)
        {
            foreach (var priority in (InputHandlerPriority[])Enum.GetValues(typeof(InputHandlerPriority)))
            {
                RemovePriorityHandler(priority, handler);
            }            
        }

        private void RemovePriorityHandler(InputHandlerPriority priority, IKeyboardInputHandler handler)
        {
            if (_handlers.TryGetValue(priority, out var handlers))
            {
                handlers.Remove(handler);
            }
        }
        
        public Keys[] ProcessKeys(Keys[] pressedKeys, GameTime gameTime)
        {
            foreach (var priority in (InputHandlerPriority[])Enum.GetValues(typeof(InputHandlerPriority)))
            {
                pressedKeys = ProcessPriorityKeys(priority, pressedKeys, gameTime);
            }
            
            return pressedKeys;
        }

        private Keys[] ProcessPriorityKeys(InputHandlerPriority priority, Keys[] pressedKeys, GameTime gameTime)
        {
            if (_handlers.TryGetValue(priority, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    pressedKeys = handler.ProcessKeys(pressedKeys, gameTime);
                }
            }
            return pressedKeys;
        }
    }
}