using System;
using System.IO;
using NoesisApp;

namespace UI.Provider
{
    public class XamlProvider : LocalXamlProvider
    {
        public XamlProvider(string rootPath) : base(rootPath) {}
        
        public override Stream LoadXaml(Uri uri)
        {
            try
            {
                return base.LoadXaml(uri);
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }
    }
}