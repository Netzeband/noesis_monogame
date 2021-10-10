// This file is mainly derived from the MonoGame NoesisGUI integration written by
// AtomicTorchStudio.
// https://github.com/AtomicTorchStudio/NoesisGUI.MonoGameWrapper/blob/master/NoesisGUI.MonoGameWrapper/Helpers/DeviceState/DeviceStateHelperD3D11.cs
// The whole file is licensed under the MIT license. 

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;


namespace UI.Noesis.DirectX.Renderer
{
    /// <summary>
    /// This helper provide methods for saving and restoring D3D11 graphics device state
    /// with MonoGame. Provided by NoesisGUI team.
    /// </summary>
    internal class D3D11RenderState
    {
        /// <summary>
        /// Cached delegate instance to get the samplers from the shader stage.
        /// </summary>
        private static readonly ShaderStageGetStuff<SamplerState> ShaderStageGetSamplers
            = GetLambda<ShaderStageGetStuff<SamplerState>>("GetSamplers", typeof(SamplerState[]));

        /// <summary>
        /// Cached delegate instance to get the resources from the shader stage.
        /// </summary>
        private static readonly ShaderStageGetStuff<ShaderResourceView> ShaderStageGetResources
            = GetLambda<ShaderStageGetStuff<ShaderResourceView>>("GetShaderResources", typeof(ShaderResourceView[]));

        /// <summary>
        /// Cached delegate instance to get the constant buffers from the shader stage.
        /// </summary>
        private static readonly ShaderStageGetStuff<Buffer> ShaderStageGetConstantBuffers
            = GetLambda<ShaderStageGetStuff<Buffer>>("GetConstantBuffers", typeof(Buffer[]));

        /// <summary>
        /// Cached delegate instance to get the viewports count from the rasterizer state.
        /// </summary>
        private static readonly RasterizerStateGetViewportsCountDelegate RasterizerGetViewportsCount =
            CreateRasterizerGetViewportsCountLambda();

        private readonly Device _device;

        private readonly Buffer[] _psConstantBuffers = new Buffer[4];

        private readonly ShaderResourceView[] _psResources = new ShaderResourceView[4];

        private readonly SamplerState[] _psSamplers = new SamplerState[4];

        private readonly Buffer[] _vb = new Buffer[1];

        private readonly int[] _vbOffset = new int[1];

        private readonly int[] _vbStride = new int[1];

        private readonly Buffer[] _vsConstantBuffers = new Buffer[4];

        private readonly ShaderResourceView[] _vsResources = new ShaderResourceView[4];

        private readonly SamplerState[] _vsSamplers = new SamplerState[4];

        private RawColor4 _blendFactor;

        private BlendState _blendState;

        private DepthStencilState _depthState;

        private DepthStencilView _depthStencilView;

        private Buffer _ib;

        private Format _ibFormat;

        private int _ibOffset;

        private InputLayout _layout;

        private PixelShader _ps;

        private RasterizerState _rasterizerState;

        private RenderTargetView[] _renderTargetView;

        private int _sampleMaskRef;

        private int _stencilRefRef;

        private PrimitiveTopology _topology;

        private RawViewportF[] _viewports = new RawViewportF[0];

        private VertexShader _vs;

        public D3D11RenderState(Device device)
        {
            _device = device;
            
            Debug.Assert(_device != null);
        }

        private delegate int RasterizerStateGetViewportsCountDelegate(RasterizerStage rasterizerStage);

        private delegate void ShaderStageGetStuff<in T>(
            CommonShaderStage shaderStage,
            int startSlot,
            int count,
            T[] result
            );

        protected void Restore()
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

        protected void Save()
        {
            var context = _device.ImmediateContext;
            _topology = context.InputAssembler.PrimitiveTopology;
            _layout = context.InputAssembler.InputLayout;

            var rasterizer = context.Rasterizer;
            this.SaveViewports(rasterizer);

            _rasterizerState = rasterizer.State;
            _blendState = context.OutputMerger.GetBlendState(out _blendFactor, out _sampleMaskRef);
            _depthState = context.OutputMerger.GetDepthStencilState(out _stencilRefRef);
            _renderTargetView = context.OutputMerger.GetRenderTargets(1, out _depthStencilView);

            var pixelShaderStage = context.PixelShader;
            _ps = pixelShaderStage.Get();
            ShaderStageGetConstantBuffers(pixelShaderStage, 0, 4, _psConstantBuffers);
            ShaderStageGetSamplers(pixelShaderStage, 0, 4, _psSamplers);
            ShaderStageGetResources(pixelShaderStage, 0, 4, _psResources);

            var vertexShaderStage = context.VertexShader;
            _vs = vertexShaderStage.Get();
            ShaderStageGetConstantBuffers(vertexShaderStage, 0, 4, _vsConstantBuffers);
            ShaderStageGetSamplers(vertexShaderStage, 0, 4, _vsSamplers);
            ShaderStageGetResources(vertexShaderStage, 0, 4, _vsResources);

            context.InputAssembler.GetIndexBuffer(out _ib, out _ibFormat, out _ibOffset);
            context.InputAssembler.GetVertexBuffers(0, 1, _vb, _vbStride, _vbOffset);
        }

        // Helper method to get the viewports count from the rasterizer state.
        // Because the method is internal, we have to use Expression to call it via lambda
        // (method signature internal unsafe void GetViewports(ref int numViewportsRef, IntPtr viewportsRef)).
        private static RasterizerStateGetViewportsCountDelegate CreateRasterizerGetViewportsCountLambda()
        {
            var methodInfo = typeof(RasterizerStage)
                .GetMethod("GetViewports", BindingFlags.NonPublic | BindingFlags.Instance);

            var argInstance = Expression.Parameter(typeof(RasterizerStage), "rasterizerStage");
            var varNumViewports = Expression.Variable(typeof(int), "numViewports");
            var expCall = Expression.Call(argInstance,
                                          // ReSharper disable once AssignNullToNotNullAttribute
                                          methodInfo,
                                          varNumViewports,
                                          /*(viewportsRef*/
                                          Expression.Constant(IntPtr.Zero));

            var lambda = Expression.Lambda(typeof(RasterizerStateGetViewportsCountDelegate),
                                           Expression.Block(
                                               // variables
                                               new[] { varNumViewports },
                                               // call method
                                               expCall,
                                               // return num viewports
                                               varNumViewports),
                                           argInstance);

            return (RasterizerStateGetViewportsCountDelegate)lambda.Compile();
        }

        private static void DisposeArray<T>(T[] array)
            where T : IDisposable
        {
            foreach (var entry in array)
            {
                entry?.Dispose();
            }
        }

        // Helper method to get stuff from the shader stage.
        // Because many method are internal, we have to use Expression to call them via lambda
        // (method signature internal abstract void MethodName(int startSlot, int numBuffers, * someStuff)).
        private static T GetLambda<T>(string methodName, Type resultType)
            where T : Delegate
        {
            var methodInfo = typeof(CommonShaderStage)
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            var argInstance = Expression.Parameter(typeof(CommonShaderStage), "shaderStage");
            var argStartSlot = Expression.Parameter(typeof(int),              "startSlot");
            var argNumSamplers = Expression.Parameter(typeof(int),            "numSamplers");
            var argResult = Expression.Parameter(resultType,                  "result");

            var expCall = Expression.Call(argInstance,
                                          // ReSharper disable once AssignNullToNotNullAttribute
                                          methodInfo,
                                          argStartSlot,
                                          argNumSamplers,
                                          argResult);

            var lambda = Expression.Lambda(typeof(T),
                                           expCall,
                                           argInstance,
                                           argStartSlot,
                                           argNumSamplers,
                                           argResult);

            return (T)lambda.Compile();
        }

        private void SaveViewports(RasterizerStage rasterizer)
        {
            var count = RasterizerGetViewportsCount(rasterizer);
            if (_viewports.Length != count)
            {
                _viewports = new RawViewportF[count];
            }

            rasterizer.GetViewports(_viewports);
        }
    }
}