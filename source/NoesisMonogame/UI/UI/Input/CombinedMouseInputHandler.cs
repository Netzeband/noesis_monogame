using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace UI.Input
{
    public class CombinedMouseInputHandler : IMouseInputHandler
    {
        readonly Dictionary<InputHandlerPriority, List<IMouseInputHandler>> _handlers = new();

        public void Add(InputHandlerPriority priority, IMouseInputHandler handler)
        {
            if (!_handlers.ContainsKey(priority))
            {
                _handlers[priority] = new List<IMouseInputHandler>();
            }
            
            _handlers[priority].Add(handler);
        }

        private void DoForAllHandler(Action<IMouseInputHandler> callback)
        {
            foreach (var priority in (InputHandlerPriority[])Enum.GetValues(typeof(InputHandlerPriority)))
            {
                foreach (var handler in _handlers[priority])
                {
                    callback(handler);
                }
            }
        }

        
        private bool DoUntilFirstConsumingHandler(Func<IMouseInputHandler, bool> callback)
        {
            foreach (var priority in (InputHandlerPriority[])Enum.GetValues(typeof(InputHandlerPriority)))
            {
                foreach (var handler in _handlers[priority])
                {
                    if (callback(handler))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void PrepareProcessing()
        {
            DoForAllHandler(h =>
            {
                h.PrepareProcessing();
            });
        }

        public bool ProcessMouseMove(int x, int y)
        {
            return DoUntilFirstConsumingHandler(h => h.ProcessMouseMove(x, y));
        }

        public bool ProcessMouseWheel(int wheel)
        {
            throw new System.NotImplementedException();
        }

        public bool ProcessButtonPressed(MouseButtons button)
        {
            throw new System.NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            DoForAllHandler(h =>
            {
                h.Update(gameTime);
            });
        }
    }
}