using System;
using Microsoft.Xna.Framework.Input;

namespace UI.Noesis.Input
{
    public class NullNoesisKeyboardInputHandler : INoesisKeyboardHandler
    {
        public void Init(global::Noesis.View view)
        {
        }

        public void UnInit()
        {
        }

        public Keys[] ProcessKeys(Keys[] pressedKeys, TimeSpan totalTime)
        {
            return pressedKeys;
        }
    }
}