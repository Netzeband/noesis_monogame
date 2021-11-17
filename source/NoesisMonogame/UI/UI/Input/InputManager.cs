using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    public class InputManager : IInputManager
    {
        readonly IMouseStateReader _mouseStateReader;
        readonly IKeyboardStateReader _keyboardStateReader;
        readonly IMouseInputHandler _mouseInputHandler;
        readonly IKeyboardInputHandler _keyboardInputHandler;
        
        public InputManager(
            IMouseStateReader mouseStateReader, 
            IKeyboardStateReader keyboardStateReader,
            IMouseInputHandler mouseInputHandler,
            IKeyboardInputHandler keyboardInputHandler
            )
        {
            _mouseStateReader = mouseStateReader;
            _keyboardStateReader = keyboardStateReader;
            _mouseInputHandler = mouseInputHandler;
            _keyboardInputHandler = keyboardInputHandler;
            
            Debug.Assert(_mouseStateReader != null);
            Debug.Assert(_keyboardStateReader != null);
            Debug.Assert(_mouseInputHandler != null);
            Debug.Assert(_keyboardInputHandler != null);
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

            _keyboardInputHandler.ProcessKeys(_keyboardStateReader.GetState(), gameTime);
        }
    }
}