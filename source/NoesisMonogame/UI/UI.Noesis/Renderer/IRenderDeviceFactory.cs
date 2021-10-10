using Microsoft.Xna.Framework.Graphics;
using Noesis;


namespace UI.Noesis.Renderer
{
    public interface IRenderDeviceFactory
    {
        /// <summary>
        /// Creates a storage for the render state.
        /// </summary>
        /// <param name="graphicsDevice">The graphic device where to read the state from.</param>
        /// <returns>Returns the storage object.</returns>
        IRenderStateStorage CreateRenderStateStorage(GraphicsDevice graphicsDevice);

        /// <summary>
        /// Create the render device for Noesis GUI.
        /// </summary>
        /// <param name="graphicsDevice">The MonoGames graphic device where to create the render device from.</param>
        /// <returns>Returns the Noesis GUI render device.</returns>
        RenderDevice CreateNoesisRenderDevice(GraphicsDevice graphicsDevice);
    }
}