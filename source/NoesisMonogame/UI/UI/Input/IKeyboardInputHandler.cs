using System;
using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    public interface IKeyboardInputHandler
    {
        /// <summary>
        /// Processes an array of keys.
        /// </summary>
        /// <param name="pressedKeys">The array of keys to process.</param>
        /// <param name="totalTime">The current total game-time.</param>
        /// <remarks>Returns a list of unprocessed keys.</remarks>
        Keys[] ProcessKeys(Keys[] pressedKeys, TimeSpan totalTime);
    }
}