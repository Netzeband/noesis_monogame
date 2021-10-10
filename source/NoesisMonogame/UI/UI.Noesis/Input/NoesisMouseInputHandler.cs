using System;
using System.Collections.Generic;
using System.Diagnostics;
using NoesisLib = Noesis;


namespace UI.Noesis.Input
{
    public class NoesisMouseInputHandler : INoesisMouseInputHandler
    {
        private class ButtonState
        {
            private readonly NoesisLib.MouseButton _noesisID;

            private bool _isPressed;
            private bool _lastIsPressed;
            private TimeSpan _releaseTime;

            public ButtonState(bool isPressed, NoesisLib.MouseButton noesisID)
            {
                _lastIsPressed = false;
                _isPressed = isPressed;
                _releaseTime = TimeSpan.Zero;
                _noesisID = noesisID;
            }
            
            public void Prepare()
            {
                _isPressed = false;
            }

            public void Press()
            {
                _isPressed = true;
            }
            
            public void Update(NoesisLib.View view, TimeSpan totalTime, TimeSpan doubleClickTime, int X, int Y)
            {
                if (_isPressed && !_lastIsPressed)
                {
                    if (totalTime - _releaseTime < doubleClickTime)
                    {
                        view?.MouseDoubleClick(X, Y, _noesisID);
                        _releaseTime = TimeSpan.Zero;
                    }
                    else
                    {
                        view?.MouseButtonDown(X, Y, _noesisID);
                    }
                }
                else if (!_isPressed && _lastIsPressed)
                {
                    view?.MouseButtonUp(X, Y, _noesisID);
                    _releaseTime = totalTime;
                }

                _lastIsPressed = _isPressed;
            }
        }

        private readonly TimeSpan _doubleClickTime = TimeSpan.FromMilliseconds(200);

        private NoesisLib.View _view;
        private int _lastX = 0;
        private int _lastY = 0;
        private int _newX = 0;
        private int _newY = 0;
        private int _newWheel = 0;
        private int _lastWheel = 0;
        private readonly Dictionary<UI.Input.MouseButtons, ButtonState> _mouseButtonStates = new();
        
        public void Init(NoesisLib.View view)
        {
            _view = view;
            
            Debug.Assert(_view != null);
        }

        public void UnInit()
        {
            _view = null;
        }

        public void PrepareProcessing()
        {
            _newX = _lastX;
            _newY = _lastY;
            _newWheel = _lastWheel;
            foreach (var state in _mouseButtonStates.Values)
            {
                state.Prepare();
            }
        }

        public bool ProcessMouseMove(int x, int y)
        {
            _newX = x;
            _newY = y;
            return false;
        }

        public bool ProcessMouseWheel(int wheel)
        {
            _newWheel = wheel;
            return false;
        }

        public bool ProcessButtonPressed(UI.Input.MouseButtons button)
        {
            if (_mouseButtonStates.TryGetValue(button, out var state))
            {
                state.Press();
            }
            else
            {
                _mouseButtonStates[button] = new ButtonState(true, GetNoesisID(button));
            }
            return false;
        }

        public void Update(TimeSpan totalTime)
        {
            if ((_newX != _lastX) || (_newY != _lastY))
            {
                _view?.MouseMove(_newX, _newY);
                _lastX = _newX;
                _lastY = _newY;
            }

            if (_lastWheel != _newWheel)
            {
                _view?.MouseWheel(_newX, _newY, _newWheel);
                _lastWheel = _newWheel;
            }

            foreach (var state in _mouseButtonStates.Values)
            {
                state.Update(_view, totalTime, _doubleClickTime, _newX, _newY);
            }
        }

        private static NoesisLib.MouseButton GetNoesisID(UI.Input.MouseButtons button)
        {
            switch (button)
            {
                case UI.Input.MouseButtons.Left: return NoesisLib.MouseButton.Left;
                case UI.Input.MouseButtons.Right: return NoesisLib.MouseButton.Right;
                case UI.Input.MouseButtons.Middle: return NoesisLib.MouseButton.Middle;
                default: return NoesisLib.MouseButton.Left;
            }
        }
    }
}