using Microsoft.Xna.Framework;

namespace UI.Provider
{
    public interface IReloadProvider
    {
        /// <summary>
        /// Checks if a hot file reload is necessary.
        /// </summary>
        /// <param name="gameTime">The current game-time.</param>
        void Update(GameTime gameTime);
    }
}