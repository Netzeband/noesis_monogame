using System;

namespace UI.Noesis.Renderer
{
    public interface IRenderStateStorage
    {
        /// <summary>
        /// Saves the current render-state.
        /// </summary>
        IDisposable Save();
        
        /// <summary>
        /// Restores the last saved render-state.
        /// </summary>
        void Restore();
    }
}