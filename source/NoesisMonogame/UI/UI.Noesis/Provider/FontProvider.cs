using System;
using System.Diagnostics;
using System.IO;
using NoesisLib = Noesis;


namespace UI.Noesis.Provider
{
    public class FontProvider : IFontProvider
    {
        private class NoesisFontProvider : NoesisApp.LocalFontProvider
        {
            public NoesisFontProvider(string rootPath) : base(rootPath) {}
            
            public override Stream OpenFont(Uri uri, string filename)
            {
                try
                {
                    return base.OpenFont(uri, filename);
                }
                catch (DirectoryNotFoundException)
                {
                    return null;
                }
            }
        }
        
        public NoesisLib.FontProvider NoesisProvider { get; private set; }

        private readonly string _rootPath;

        public FontProvider(string rootPath)
        {
            _rootPath = rootPath;
            
            Debug.Assert(_rootPath != null);
        }

        public void Init()
        {
            NoesisProvider = new NoesisFontProvider(_rootPath);
        }
        
        public void Update(TimeSpan totalTime)
        {
            // no hot reload implemented
        }
        
    }
}