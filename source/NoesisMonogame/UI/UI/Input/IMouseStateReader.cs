using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    public interface IMouseStateReader
    {
        /// <returns>Returns the current mouse state.</returns>
        MouseState GetState();
    }
}