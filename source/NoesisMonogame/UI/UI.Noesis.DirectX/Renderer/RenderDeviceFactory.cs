using Microsoft.Xna.Framework.Graphics;
using UI.Noesis.Renderer;
using NoesisLib = Noesis;


namespace UI.Noesis.DirectX.Renderer
{
    public class RenderDeviceFactory : IRenderDeviceFactory
    {
        public NoesisLib.RenderDevice CreateNoesisRenderDevice(GraphicsDevice graphicsDevice)
        {
            var directXDevice = (SharpDX.Direct3D11.Device)graphicsDevice.Handle;
            var deviceContext = directXDevice.ImmediateContext;
            return new NoesisLib.RenderDeviceD3D11(deviceContext.NativePointer, sRGB: false);
        }

        public IRenderStateStorage CreateRenderStateStorage(GraphicsDevice graphicsDevice)
        {
            return new RenderStateStorage(graphicsDevice);
        }
    }
}