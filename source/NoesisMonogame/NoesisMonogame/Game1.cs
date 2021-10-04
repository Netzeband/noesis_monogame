using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace NoesisMonogame
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Noesis.View _guiView;
        private MouseState _lastMouseState;
        private bool _wasScrolledByMouse;
        private readonly Dictionary<Noesis.MouseButton, TimeSpan> _lastMouseClickTime = new Dictionary<Noesis.MouseButton, TimeSpan>();
        private readonly TimeSpan _doubleClickInterval = TimeSpan.FromMilliseconds(250);
        private UI.Provider.IReloadProvider _xamlProvider;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Data";
            IsMouseVisible = true;
            _wasScrolledByMouse = false;
        }

        private static void LogGUIMessage(Noesis.LogLevel level, string channel, string message)
        {
            Console.WriteLine($"[{level}] {message}");
        }
        
        protected override void Initialize()
        {
            Noesis.Log.SetLogCallback(LogGUIMessage);
            Noesis.GUI.Init();
            Noesis.GUI.SetLicense(NoesisLicense.Name, NoesisLicense.Key);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var directXDevice = (SharpDX.Direct3D11.Device) GraphicsDevice.Handle;
            var deviceContext = directXDevice.ImmediateContext;
            var guiDevice = new Noesis.RenderDeviceD3D11(deviceContext.NativePointer, sRGB: false);

            var rootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Data/UI"));

            var xamlProvider = new UI.Provider.XamlProvider(rootPath, new UI.Provider.ReloadProvider());
            var fontProvider = new UI.Provider.FontProvider(rootPath);
            NoesisApp.Application.SetThemeProviders(
                xamlProvider: xamlProvider,
                fontProvider: fontProvider,
                textureProvider: new NoesisApp.LocalTextureProvider(rootPath)
                );
            _xamlProvider = xamlProvider;
            Noesis.GUI.LoadApplicationResources("Theme/NoesisTheme.DarkBlue.xaml");
            
            var rootXaml = "Test.xaml";

            var rootElement = Noesis.GUI.LoadXaml(rootXaml) as Noesis.FrameworkElement;
            if (rootElement == null)
            {
                throw new FileNotFoundException($"Cannot find file '{rootXaml}' on path '{rootPath}'.");
            }
            
            _guiView = Noesis.GUI.CreateView(rootElement);
            RefreshGUISize();
            _guiView.Renderer.Init(guiDevice);
            _guiView.SetFlags(Noesis.RenderFlags.PPAA | Noesis.RenderFlags.LCD);
            
            // ToDo: register resize event
        }

        private void RefreshGUISize()
        {
            var viewport = GraphicsDevice.Viewport;
            _guiView.SetSize((ushort)viewport.Width, (ushort)viewport.Height);
        }
        
        protected override void UnloadContent()
        {
            if (_guiView != null)
            {
                // ToDo: unsubscribe events (like resize)...
                
                _guiView.Renderer.Shutdown();
                _guiView = null;
            }

            // unload content here if necessary ...
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                (Keyboard.GetState().IsKeyDown(Keys.Escape)) )
            {
                Exit();
            }
            
            // update your game state here ...

            _xamlProvider.Update(gameTime);

            if (IsActive)
            {
                var mouseState = Mouse.GetState();

                if (_wasScrolledByMouse || (_lastMouseState.X != mouseState.X) || (_lastMouseState.Y != mouseState.Y))
                {
                    _guiView.MouseMove(mouseState.X, mouseState.Y);
                    _wasScrolledByMouse = false;
                }

                if (_lastMouseState.ScrollWheelValue != mouseState.ScrollWheelValue)
                {
                    // ToDo: Only consider mouse wheel, when we are over a control element
                    _guiView.MouseWheel(
                        mouseState.X,
                        mouseState.Y,
                        mouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue
                    );
                    _wasScrolledByMouse = true;
                }

                ProcessMouseButton(
                    mouseState.X, 
                    mouseState.Y, 
                    Noesis.MouseButton.Left,  
                    mouseState.LeftButton,  
                    _lastMouseState.LeftButton, gameTime
                    );
                ProcessMouseButton(
                    mouseState.X, 
                    mouseState.Y, 
                    Noesis.MouseButton.Right,  
                    mouseState.LeftButton,  
                    _lastMouseState.LeftButton, gameTime
                    );
                ProcessMouseButton(
                    mouseState.X, 
                    mouseState.Y, 
                    Noesis.MouseButton.Middle,  
                    mouseState.LeftButton,  
                    _lastMouseState.LeftButton, gameTime
                    );
                
                _lastMouseState = mouseState;
                
                // ToDo: Add keyboard events for GUI here ...
            }
            
            _guiView.Update(gameTime.TotalGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        
        private void ProcessMouseButton(int x, int y, Noesis.MouseButton buttonType, ButtonState state, ButtonState lastState, GameTime gameTime)
        {
            if (state == lastState) return;

            if (state == ButtonState.Pressed)
            {
                if (RegisterAndCheckDoubleClick(buttonType, gameTime))
                {
                    _guiView.MouseDoubleClick(x, y, buttonType);
                }
                else
                {
                    _guiView.MouseButtonDown(x, y, buttonType);
                }
            }
            else if (state == ButtonState.Released)
            {
                _guiView.MouseButtonUp(x, y, buttonType);
            }
        }


        private bool RegisterAndCheckDoubleClick(Noesis.MouseButton buttonType, GameTime gameTime)
        {
            var currentTime = gameTime.TotalGameTime;

            if (_lastMouseClickTime.TryGetValue(buttonType, out var lastClickTime))
            {

                if (currentTime - lastClickTime < _doubleClickInterval)
                {
                    _lastMouseClickTime.Remove(buttonType);
                    return true;
                }
            }
            
            _lastMouseClickTime[buttonType] = currentTime;
            
            return false;
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            {
                using var renderState = new D3X11RenderState(GraphicsDevice);
                _guiView.Renderer.UpdateRenderTree();
                _guiView.Renderer.RenderOffscreen();
            }

            // draw your game state here ...
            
            _guiView.Renderer.Render();
            
            base.Draw(gameTime);
        }

    }
}