using System;
using Microsoft.Xna.Framework.Graphics;
using UI.Noesis.Renderer;


namespace UI.Noesis.DirectX.Renderer
{
    public class RenderStateStorage : IRenderStateStorage
    {
        private class RenderState : D3D11RenderState, IDisposable
        {
            private bool _isStored;
            
            public RenderState(SharpDX.Direct3D11.Device device) : base(device)
            {
                _isStored = false;
            }

            public void Save()
            {
                base.Save();
                _isStored = true;
            }

            public void Restore()
            {
                base.Restore();
                _isStored = false;
            }

            public void Dispose()
            {
                if (_isStored)
                {
                    Restore();
                }
            }
        }

        private readonly RenderState _renderState;

        public RenderStateStorage(GraphicsDevice graphicsDevice)
        {
            _renderState = new RenderState(graphicsDevice.Handle as SharpDX.Direct3D11.Device);
        }
        
        public IDisposable Save()
        {
            _renderState.Save();
            return _renderState;
        }

        public void Restore()
        {
            _renderState.Restore();
        }
    }
}