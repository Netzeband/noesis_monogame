using System;
using System.Diagnostics;
using NoesisLib = Noesis;


namespace UI.Noesis.Provider
{
    public class TextureProvider : ITextureProvider
    {
        public NoesisLib.TextureProvider NoesisProvider { get; private set; }

        private readonly string _rootPath;

        public TextureProvider(string rootPath)
        {
            _rootPath = rootPath;

            Debug.Assert(_rootPath != null);
        }

        public void Init()
        {
            NoesisProvider = new NoesisApp.LocalTextureProvider(_rootPath);
        }
        
        public void Update(TimeSpan totalTime) {}
    }
}