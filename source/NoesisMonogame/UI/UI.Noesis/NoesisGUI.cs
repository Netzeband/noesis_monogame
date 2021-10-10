using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoesisLib = Noesis;


namespace UI.Noesis
{
    public class NoesisGUI : IGUI
    {
        private readonly Logging.ILogger _logger;
        private readonly INoesisLicense _license;
        private readonly Provider.IProviderManager _providerManager;
        private readonly View.IViewFactory _viewFactory;
        private readonly string _theme;
        private readonly Renderer.IRenderDeviceFactory _renderDeviceFactory;
        private readonly Input.INoesisMouseInputHandler _mouseInputHandler = new Input.NullNoesisMouseInputHandler();
        private readonly Input.INoesisKeyboardHandler _keyboardInputHandler = new Input.NullNoesisKeyboardInputHandler();
        private readonly Input.INoesisKeyboardHandler _priorityKeyboardInputHandler = new Input.NullNoesisKeyboardInputHandler();
        
        private NoesisLib.View _guiView;
        private GraphicsDeviceManager _graphics;
        private Renderer.IRenderStateStorage _renderStateStorage;
        private bool _isInitialized;

        public UI.Input.IMouseInputHandler MouseInputHandler => _mouseInputHandler;
        public UI.Input.IKeyboardInputHandler KeyboardInputHandler => _keyboardInputHandler;
        public UI.Input.IKeyboardInputHandler PriorityKeyboardInputHandler => _priorityKeyboardInputHandler;
        
        public NoesisGUI(
            INoesisLicense license, 
            Logging.ILogger logger, 
            Provider.IProviderManager providerManager,
            View.IViewFactory viewFactory,
            Renderer.IRenderDeviceFactory renderDeviceFactory,
            Input.INoesisMouseInputHandler mouseInputHandler = null,
            Input.INoesisKeyboardHandler keyboardInputHandler = null,
            Input.INoesisKeyboardHandler priorityKeyboardInputHandler = null,
            string theme = ""
            )
        {
            _license = license;
            _logger = logger;
            _providerManager = providerManager;
            _viewFactory = viewFactory;
            _renderDeviceFactory = renderDeviceFactory;
            
            if (mouseInputHandler != null)
            {
                _mouseInputHandler = mouseInputHandler;
            }

            if (keyboardInputHandler != null)
            {
                _keyboardInputHandler = keyboardInputHandler;
            }

            if (priorityKeyboardInputHandler != null)
            {
                _priorityKeyboardInputHandler = priorityKeyboardInputHandler;                
            }
            
            _theme = theme;

            Debug.Assert(_license != null);
            Debug.Assert(_logger != null);
            Debug.Assert(_providerManager != null);
            Debug.Assert(_viewFactory != null);
            Debug.Assert(_renderDeviceFactory != null);
            Debug.Assert(_mouseInputHandler != null);
            Debug.Assert(_keyboardInputHandler != null);
            Debug.Assert(_priorityKeyboardInputHandler != null);
            Debug.Assert(_theme != null);
            
            _isInitialized = false;
        }
        
        public void Init()
        {
            NoesisLib.Log.SetLogCallback((l, s, m) => _logger.Log(l.ToString(), m));
            NoesisLib.GUI.Init();
            NoesisLib.GUI.SetLicense(_license.Name, _license.Key);
            _providerManager.Init();
            
            NoesisApp.Application.SetThemeProviders(
                xamlProvider: _providerManager.XamlProvider.NoesisProvider,
                fontProvider: _providerManager.FontProvider.NoesisProvider,
                textureProvider: _providerManager.TextureProvider.NoesisProvider
            );
            if (_theme != "")
            {
                NoesisLib.GUI.LoadApplicationResources(_theme);
            }
            _isInitialized = true;
        }

        public void Load(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            
            Debug.Assert(_isInitialized, $"{nameof(NoesisGUI)} is not initialized. Call Init() before!");
            Debug.Assert(graphics.GraphicsProfile == GraphicsProfile.HiDef, "Graphic profile must be HiDef to use Noesis GUI!");
            
            _guiView = NoesisLib.GUI.CreateView(_viewFactory.LoadView());
            RefreshGUISize();

            _renderStateStorage = _renderDeviceFactory.CreateRenderStateStorage(_graphics.GraphicsDevice);
            {
                using var renderState = _renderStateStorage.Save();
                
                _guiView.Renderer.Render();
                _guiView.Renderer.Init(_renderDeviceFactory.CreateNoesisRenderDevice(_graphics.GraphicsDevice));
                _guiView.SetFlags(NoesisLib.RenderFlags.PPAA | NoesisLib.RenderFlags.LCD);
            }

            _mouseInputHandler.Init(_guiView);
            _keyboardInputHandler.Init(_guiView);
            _priorityKeyboardInputHandler.Init(_guiView);
            
            // ToDo: register resize event
        }
        
        
        private void RefreshGUISize()
        {
            var viewport = _graphics.GraphicsDevice.Viewport;
            _guiView.SetSize((ushort)viewport.Width, (ushort)viewport.Height);
        }


        public void Unload()
        {
            _mouseInputHandler.UnInit();
            _keyboardInputHandler.UnInit();
            _priorityKeyboardInputHandler.UnInit();
             
            if (_guiView != null)
            {
                // ToDo: unsubscribe events (like resize)...
                
                _guiView.Renderer.Shutdown();
                _guiView = null;
            }

            _renderStateStorage = null;
            _graphics = null;
        }


        public bool IsLoaded()
        {
            return (_guiView != null) && (_renderStateStorage != null) && (_graphics != null);
        }
        
        public void Update(TimeSpan totalTime)
        {
            _providerManager.Update(totalTime);
            _guiView.Update(totalTime.TotalSeconds);
        }

        public void PreRender()
        {
            if (IsLoaded())
            {
                using var renderState = _renderStateStorage.Save();
            
                _guiView.Renderer.UpdateRenderTree();
                _guiView.Renderer.RenderOffscreen();
            }
        }

        public void Render()
        {
            if (IsLoaded())
            {
                using var renderState = _renderStateStorage.Save();

                _guiView.Renderer.Render();
            }
        }
    }
}