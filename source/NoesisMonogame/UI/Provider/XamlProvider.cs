using System;
using System.Collections.Generic;
using System.IO;
using Noesis;
using NoesisApp;
using Microsoft.Xna.Framework;


namespace UI.Provider
{
    public class XamlProvider : LocalXamlProvider
    {
        private readonly string _rootPath;
        private readonly Dictionary<Uri, int> _fileHashes = new Dictionary<Uri, int>();
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);
        private TimeSpan _lastCheck = TimeSpan.Zero;
        
        public XamlProvider(string rootPath) : base(rootPath)
        {
            _rootPath = rootPath;
        }
        
        public override Stream LoadXaml(Uri uri)
        {
            try
            {
                var stream = base.LoadXaml(uri);
                _fileHashes[uri] = GetFileHash(uri);
                return stream;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }
        
        private int GetFileHash(Uri uri)
        {
            var filePath = System.IO.Path.Combine(_rootPath, uri.GetPath());
            return File.GetLastWriteTime(filePath).GetHashCode();
        }

        public void Update(GameTime gameTime)
        {
#if  DEBUG
            var currentTime = gameTime.TotalGameTime;

            if (currentTime - _lastCheck > _checkInterval)
            {
                foreach (var item in _fileHashes)
                {
                    if (item.Value != GetFileHash(item.Key))
                    {
                        RaiseXamlChanged(item.Key);
                    }
                }
                
                _lastCheck = currentTime;
            }
#endif
        }
    }
}