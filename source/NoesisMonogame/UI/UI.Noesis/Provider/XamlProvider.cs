using System;
using System.IO;
using NoesisLib = Noesis;


namespace UI.Noesis.Provider
{
    public class XamlProvider : IXamlProvider
    {
        private class NoesisXamlProvider : NoesisApp.LocalXamlProvider
        {
            public NoesisXamlProvider(string rootPath): base(rootPath) {}
            
            public override Stream LoadXaml(Uri uri)
            {
                try
                {
                    var stream = base.LoadXaml(uri);
                    return stream;
                }
                catch (DirectoryNotFoundException)
                {
                    return null;
                }
            }
        }
        
        private readonly IReloadProviderSettings _reloadProvider;
        private readonly string _rootPath;
        public NoesisLib.XamlProvider NoesisProvider { get; private set; }
        
        public XamlProvider(string rootPath, IReloadProviderSettings reloadProvider)
        {
            _rootPath = rootPath;
            _reloadProvider = reloadProvider;
        }

        public void Init()
        {
            NoesisProvider = new NoesisXamlProvider(_rootPath);
            _reloadProvider.Setup(_rootPath, NoesisProvider.RaiseXamlChanged);
        }
        
        public void Update(TimeSpan totalTime) => _reloadProvider.Update(totalTime); 
    }
}