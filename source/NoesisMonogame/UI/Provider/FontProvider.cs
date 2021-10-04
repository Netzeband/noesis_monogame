using System;
using System.IO;
using NoesisApp;

namespace UI.Provider
{
    public class FontProvider : LocalFontProvider
    {
        public FontProvider(string rootPath) : base(rootPath) {}
        
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
}