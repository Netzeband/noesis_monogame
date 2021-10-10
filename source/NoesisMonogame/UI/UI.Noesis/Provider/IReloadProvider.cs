using System;

namespace UI.Noesis.Provider
{
    public interface IReloadProvider
    {
        /// <summary>
        /// Checks if a hot file reload is necessary.
        /// </summary>
        /// <param name="totalTime">The total running time.</param>
        void Update(TimeSpan totalTime);
    }
}