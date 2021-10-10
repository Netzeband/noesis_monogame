using System;

namespace UI.Input
{
    public interface IMouseInputHandler
    {
        /// <summary>
        /// This method must be called before any Process* method can be called.
        /// </summary>
        void PrepareProcessing();
        
        /// <summary>
        /// Processes a mouse movement.
        /// </summary>
        /// <param name="x">The current X position.</param>
        /// <param name="y">The current Y position.</param>
        /// <returns>Returns true, if the movement was consumed.</returns>
        bool ProcessMouseMove(int x, int y);

        /// <summary>
        /// Processes a mouse wheel change.
        /// </summary>
        /// <param name="wheel">The current wheel value.</param>
        /// <returns>Returns true, if the wheel value was consumed.</returns>
        bool ProcessMouseWheel(int wheel);

        /// <summary>
        /// Indicates, that a certain mouse button was pressed.
        /// </summary>
        /// <param name="button">The button type.</param>
        /// <returns>Returns ture, if the button down even was processed.</returns>
        bool ProcessButtonPressed(MouseButtons button);

        /// <summary>
        /// Updates the input handler calculation.
        /// </summary>
        /// <param name="totalTime">The total game-time.</param>
        void Update(TimeSpan totalTime);
    }
}