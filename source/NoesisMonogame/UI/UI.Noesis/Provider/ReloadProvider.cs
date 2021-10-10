using System;
using System.Collections.Generic;
using System.IO;
using Noesis;


namespace UI.Noesis.Provider
{
    public class ReloadProvider : IReloadProviderSettings
    {
        private string _rootPath;
        private IReloadProviderSettings.TriggerReload _triggerReload;
        private readonly Dictionary<Uri, int> _fileHashes = new();
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);
        private TimeSpan _lastCheck = TimeSpan.Zero;

        public void Setup(string rootPath, IReloadProviderSettings.TriggerReload triggerReload)
        {
            _rootPath = rootPath;
            _triggerReload = triggerReload;
        }

        public void Update(TimeSpan totalTime)
        {
#if  DEBUG
            var currentTime = totalTime;

            if ((currentTime - _lastCheck > _checkInterval) && (_triggerReload != null))
            {
                foreach (var item in _fileHashes)
                {
                    if (item.Value != GetFileHash(item.Key))
                    {
                        _triggerReload(item.Key);
                        _fileHashes[item.Key] = GetFileHash(item.Key);
                    }
                }
                
                _lastCheck = currentTime;
            }
#endif
        }
        
        private int GetFileHash(Uri uri)
        {
            var filePath = System.IO.Path.Combine(_rootPath, uri.GetPath());
            return File.GetLastWriteTime(filePath).GetHashCode();
        }
        
    }
}