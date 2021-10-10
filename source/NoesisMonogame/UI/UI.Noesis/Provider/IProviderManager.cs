using System;


namespace UI.Noesis.Provider
{
    public interface IProviderManager
    {
        IXamlProvider XamlProvider { get; }
        IFontProvider FontProvider { get; }
        ITextureProvider TextureProvider { get; }

        /// <summary>
        /// Initializes the resource providers.
        /// </summary>
        void Init();
        
        /// <summary>
        /// Updates all providers.
        /// </summary>
        /// <param name="totalTime">The total running time.</param>
        void Update(TimeSpan totalTime);
    }
}