using System.Collections.Generic;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using SharpDX.DXGI;

namespace NoesisMonogame
{
    public class D3X11RenderState : System.IDisposable
    {
        private SharpDX.Direct3D11.Device _device;
 
        private readonly Buffer[] _vb = new Buffer[1];
        private readonly int[] _vbOffset = new int[1];
        private readonly int[] _vbStride = new int[1];
        private RawColor4 _blendFactor;
        private BlendState _blendState;
        private DepthStencilState _depthState;
        private DepthStencilView _depthStencilView;
        private Buffer _ib;
        private Format _ibFormat;
        private int _ibOffset;
        private InputLayout _layout;
        private PixelShader _ps;
        private Buffer[] _psConstantBuffers;
        private ShaderResourceView[] _psResources;
        private SamplerState[] _psSamplers;
        private RasterizerState _rasterizerState;
        private RenderTargetView[] _renderTargetView;
        private int _sampleMaskRef;
        private int _stencilRefRef;
        private PrimitiveTopology _topology;
        private RawViewportF[] _viewports;
        private VertexShader _vs;
        private Buffer[] _vsConstantBuffers;
        private ShaderResourceView[] _vsResources;
        private SamplerState[] _vsSamplers;

        public D3X11RenderState(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        {
            _device = (SharpDX.Direct3D11.Device)graphicsDevice.Handle;
            
            var context = _device.ImmediateContext;
            _topology = context.InputAssembler.PrimitiveTopology;
            _layout = context.InputAssembler.InputLayout;
            _viewports = context.Rasterizer.GetViewports<RawViewportF>();
            _rasterizerState = context.Rasterizer.State;
            _blendState = context.OutputMerger.GetBlendState(out _blendFactor, out _sampleMaskRef);
            _depthState = context.OutputMerger.GetDepthStencilState(out _stencilRefRef);
            _renderTargetView = context.OutputMerger.GetRenderTargets(1, out _depthStencilView);

            _ps = context.PixelShader.Get();
            _psConstantBuffers = context.PixelShader.GetConstantBuffers(0, 4);
            _psSamplers = context.PixelShader.GetSamplers(0, 4);
            _psResources = context.PixelShader.GetShaderResources(0, 4);

            _vs = context.VertexShader.Get();
            _vsConstantBuffers = context.VertexShader.GetConstantBuffers(0, 4);
            _vsSamplers = context.VertexShader.GetSamplers(0, 4);
            _vsResources = context.VertexShader.GetShaderResources(0, 4);

            context.InputAssembler.GetIndexBuffer(out _ib, out _ibFormat, out _ibOffset);
            context.InputAssembler.GetVertexBuffers(0, 1, _vb, _vbStride, _vbOffset);
            
        }
        
        public void Dispose()
        {
            var context = _device.ImmediateContext;
            
            context.InputAssembler.PrimitiveTopology = _topology;
            context.InputAssembler.InputLayout = _layout;
            _layout?.Dispose();
            context.Rasterizer.SetViewports(_viewports);

            context.Rasterizer.State = _rasterizerState;
            _rasterizerState?.Dispose();

            context.OutputMerger.SetBlendState(_blendState, _blendFactor, _sampleMaskRef);
            _blendState?.Dispose();

            context.OutputMerger.SetDepthStencilState(_depthState, _stencilRefRef);
            _depthState?.Dispose();

            context.OutputMerger.SetRenderTargets(_depthStencilView, _renderTargetView[0]);
            _depthStencilView?.Dispose();
            _renderTargetView[0]?.Dispose();

            context.PixelShader.Set(_ps);
            context.PixelShader.SetConstantBuffers(0, _psConstantBuffers);
            context.PixelShader.SetSamplers(0, _psSamplers);
            context.PixelShader.SetShaderResources(0, _psResources);
            _ps?.Dispose();
            DisposeArray(_psConstantBuffers);
            DisposeArray(_psSamplers);
            DisposeArray(_psResources);

            context.VertexShader.Set(_vs);
            context.VertexShader.SetConstantBuffers(0, _vsConstantBuffers);
            context.VertexShader.SetSamplers(0, _vsSamplers);
            context.VertexShader.SetShaderResources(0, _vsResources);
            _vs?.Dispose();
            DisposeArray(_vsConstantBuffers);
            DisposeArray(_vsSamplers);
            DisposeArray(_vsResources);

            context.InputAssembler.SetIndexBuffer(_ib, _ibFormat, _ibOffset);
            _ib?.Dispose();

            context.InputAssembler.SetVertexBuffers(0, _vb, _vbStride, _vbOffset);
            DisposeArray(_vb);            
        }
        
        private static void DisposeArray<T>(IEnumerable<T> array) where T : System.IDisposable
        {
            foreach (var entry in array)
            {
                entry?.Dispose();
            }
        }
        
    }
}