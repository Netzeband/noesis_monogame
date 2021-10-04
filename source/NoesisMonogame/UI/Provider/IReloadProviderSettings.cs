using System;

namespace UI.Provider
{
    public interface IReloadProviderSettings : IReloadProvider
    {
        /// <summary>
        /// Setups the reload provider with necessary information.
        /// </summary>
        /// <param name="rootPath">The base path to look at.</param>
        /// <param name="triggerReload">The function to call for triggering a reload.</param>
        void Setup(string rootPath, TriggerReload triggerReload);
        
        /// <summary>
        /// A delegate, which triggers the reload.
        /// </summary>
        delegate void TriggerReload(Uri uri);
    }
}