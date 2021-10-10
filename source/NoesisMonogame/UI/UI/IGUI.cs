using System;
using Microsoft.Xna.Framework;

namespace UI
{
    public interface IGUI
    {
        /// <summary>
        /// The input handler for the mouse.
        /// </summary>
        Input.IMouseInputHandler MouseInputHandler { get; }
        
        /// <summary>
        /// The input handler for the keyboard.
        /// </summary>
        Input.IKeyboardInputHandler KeyboardInputHandler { get; }

        /// <summary>
        /// The input handler for the keyboard. This keyboard handler processed priority keys, which always
        /// must be consumed before the game.
        /// </summary>
        Input.IKeyboardInputHandler PriorityKeyboardInputHandler { get; }
        
        /// <summary>
        /// Initializes the User Interface.
        /// </summary>
        void Init();

        /// <summary>
        /// Loads the GUI content.
        /// </summary>
        /// <param name="graphics">The MonoGame graphic device to use.</param>
        void Load(GraphicsDeviceManager graphics);

        /// <summary>
        /// Unloads the GUI content.
        /// </summary>
        void Unload();

        /// <summary>
        /// Can be used to check, if the GUI content is loaded.
        /// </summary>
        /// <returns>Returns true, if the GUI content is loaded.</returns>
        bool IsLoaded();
        
        /// <summary>
        /// Updated the gui. 
        /// </summary>
        /// <param name="totalTime">The total running time.</param>
        void Update(TimeSpan totalTime);

        /// <summary>
        /// This must be called before the game content is rendered.
        /// </summary>
        void PreRender();
        
        /// <summary>
        /// This must be called after the game content was rendered.
        /// </summary>
        void Render();
    }
}