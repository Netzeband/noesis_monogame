using System;
using System.Collections.Generic;
using System.IO;
using Noesis;
using NoesisApp;
using Microsoft.Xna.Framework;


namespace UI.Provider
{
    public class XamlProvider : LocalXamlProvider, IReloadProvider
    {
        private readonly IReloadProviderSettings _reloadProvider;
        
        public XamlProvider(string rootPath, IReloadProviderSettings reloadProvider) : base(rootPath)
        {
            _reloadProvider = reloadProvider;
            _reloadProvider.Setup(rootPath, RaiseXamlChanged);
        }
        
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
        
        
        public void Update(GameTime gameTime) => _reloadProvider.Update(gameTime); 
    }
}