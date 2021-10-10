using NoesisLib = Noesis;


namespace UI.Noesis.Provider
{
    public interface IFontProvider : IReloadProvider
    {
        /// <summary>
        /// Initializes the this provider.
        /// </summary>
        void Init();

        /// <summary>
        /// Returns the provider object for Noesis GUI.
        /// Note: Is only valid, when initialized.
        /// </summary>
        NoesisLib.FontProvider NoesisProvider { get; }
    }
}