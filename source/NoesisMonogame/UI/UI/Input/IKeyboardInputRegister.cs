using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    /// <summary>
    /// An keyboard input register, which helps to implement a keyboard input handler, where different keys and
    /// key-combinations can be registered to call different handlers in case of they are pressed.
    /// </summary>
    public interface IKeyboardInputRegister<T> where T : Enum
    {
        /// <summary>
        /// A handler function to call in case a key is pressed in a certain state.
        /// </summary>
        /// <returns>The handler must return true in order to consume the key.</returns>
        delegate bool HandlerFunction(Keys key, T state, GameTime gameTime);

        /// <summary>
        /// Registers a handler for a certain key and state.
        /// </summary>
        /// <param name="key">The key which should be handled.</param>
        /// <param name="state">The state in which this key should be handled.</param>
        /// <param name="function">The handler function to call.</param>
        void RegisterKey(Keys key, T state, HandlerFunction function);

        /// <summary>
        /// Processed a list of keys for a certain state.
        /// </summary>
        /// <param name="pressedKeys">An array of keys, that have been pressed.</param>
        /// <param name="state">The state of the application to consider.</param>
        /// <param name="gameTime">The game-time object.</param>
        /// <returns>Returns a list of unhandled keys.</returns>
        Keys[] ProcessKeys(Keys[] pressedKeys, T state, GameTime gameTime);

        /// <summary>
        /// A handler modificator, which considers a key only in case the key is released. 
        /// </summary>
        /// <param name="handler">The handler call in this case.</param>
        /// <returns>Returns a modified handler, which checks first if the key was released.</returns>
        HandlerFunction AtKeyUp(HandlerFunction handler);

        /// <summary>
        /// A handler modificator, which considers a key only in case the key is just pressed. 
        /// </summary>
        /// <param name="handler">The handler call in this case.</param>
        /// <returns>Returns a modified handler, which checks first if the key was just pressed.</returns>
        HandlerFunction AtKeyDown(HandlerFunction handler);
        
        /// <summary>
        /// A handler modificator, which recalls the handler only after a delay. 
        /// </summary>
        /// <param name="handler">The handler call in this case.</param>
        /// <param name="firstDelay">The delay, which mist be reached, before the handler is called a second time.</param>
        /// <param name="nextDelay">The delay, which mist be reached, before the handler is called a third or more times.
        /// If this is not defined, the delay from the first time is used again.</param>
        /// <returns>Returns a modified handler, which checks first if the key was just pressed.</returns>
        HandlerFunction WithRepeatDelay(HandlerFunction handler, TimeSpan firstDelay, TimeSpan? nextDelay = null);
    }
}