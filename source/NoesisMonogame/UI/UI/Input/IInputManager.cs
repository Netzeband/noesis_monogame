using Microsoft.Xna.Framework;

namespace UI.Input
{
    public interface IInputManager
    {
        /// <summary>
        /// Processes the input from the game-window.
        /// </summary>
        /// <param name="gameTime">The current game-time.</param>
        void Update(GameTime gameTime);
    }
}