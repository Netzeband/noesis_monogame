using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    public class InputManager : IInputManager
    {
        readonly IMouseStateReader _mouseStateReader;
        readonly IMouseInputHandler _mouseInputHandler;
        
        public InputManager(IMouseStateReader mouseStateReader, IMouseInputHandler mouseInputHandler)
        {
            _mouseStateReader = mouseStateReader;
            _mouseInputHandler = mouseInputHandler;
            
            Debug.Assert(_mouseStateReader != null);
            Debug.Assert(_mouseInputHandler != null);
        }
        
        public void Update(GameTime gameTime)
        {
            var mouseState = _mouseStateReader.GetState();
            
            _mouseInputHandler.PrepareProcessing();
            _mouseInputHandler.ProcessMouseMove(mouseState.X, mouseState.Y);
            _mouseInputHandler.ProcessMouseWheel(mouseState.ScrollWheelValue);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _mouseInputHandler.ProcessButtonPressed(MouseButtons.Left);
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                _mouseInputHandler.ProcessButtonPressed(MouseButtons.Right);
            }
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                _mouseInputHandler.ProcessButtonPressed(MouseButtons.Middle);
            }
            _mouseInputHandler.Update(gameTime);
        }
    }
}