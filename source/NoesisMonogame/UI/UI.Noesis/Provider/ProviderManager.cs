using System;
using System.Diagnostics;


namespace UI.Noesis.Provider
{
    public class ProviderManager : IProviderManager
    {
        public IXamlProvider XamlProvider { get; }
        public IFontProvider FontProvider { get; }
        public ITextureProvider TextureProvider { get; }

        public ProviderManager(IXamlProvider xamlProvider, IFontProvider fontProvider, ITextureProvider textureProvider)
        {
            XamlProvider = xamlProvider;
            FontProvider = fontProvider;
            TextureProvider = textureProvider;
            
            Debug.Assert(XamlProvider != null);
            Debug.Assert(FontProvider != null);
            Debug.Assert(TextureProvider != null);
        }

        public void Init()
        {
            XamlProvider.Init();
            FontProvider.Init();
            TextureProvider.Init();
        }
        
        public void Update(TimeSpan totalTime)
        {
            XamlProvider.Update(totalTime);
            FontProvider.Update(totalTime);
            TextureProvider.Update(totalTime);
        }
    }
}