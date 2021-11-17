using Microsoft.Xna.Framework.Input;

namespace UI.Input
{
    public interface IKeyboardStateReader
    {
        /// <returns>Returns the currently pressed keys.</returns>
        Keys[] GetState();
    }
}