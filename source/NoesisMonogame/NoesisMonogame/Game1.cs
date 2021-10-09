using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Game = Microsoft.Xna.Framework.Game;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using Path = System.IO.Path;


namespace NoesisMonogame
{
    internal class KeysComparer : IEqualityComparer<Keys>
    {
        public static readonly KeysComparer Instance = new KeysComparer();

        private KeysComparer()
        {
        }

        public bool Equals(Keys x, Keys y)
        {
            return x == y;
        }

        public int GetHashCode(Keys obj)
        {
            return (int)obj;
        }
    }
    
    public class Game1 : Game
    {
        private Texture2D _ballTexture;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Noesis.View _guiView;
        private readonly GameModel _model;
        private Data.UI.ViewModel _viewModel;
        private MouseState _lastMouseState;
        private bool _wasScrolledByMouse;
        private readonly Dictionary<Noesis.MouseButton, TimeSpan> _lastMouseClickTime = new();
        private readonly TimeSpan _doubleClickInterval = TimeSpan.FromMilliseconds(250);
        private UI.Provider.IReloadProvider _xamlProvider;
        private readonly Dictionary<Keys, TimeSpan> _lastPressedKeys = new(KeysComparer.Instance);
        private readonly TimeSpan _keyRepeatInterval = TimeSpan.FromMilliseconds(250);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 840;
            
            Content.RootDirectory = "Data";
            IsMouseVisible = true;
            _wasScrolledByMouse = false;
            
            _model = new GameModel(
                new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)
                );
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
            _ballTexture = Content.Load<Texture2D>("ball");
            
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

            _viewModel = new Data.UI.ViewModel(_model);
            var rootElement = new Data.UI.Window(_viewModel);
            _guiView = Noesis.GUI.CreateView(rootElement);
            RefreshGUISize();
            
            {
                using var renderState = new D3X11RenderState(GraphicsDevice);
                
                _guiView.Renderer.Render();
                var directXDevice = (SharpDX.Direct3D11.Device) GraphicsDevice.Handle;
                var deviceContext = directXDevice.ImmediateContext;
                var guiDevice = new Noesis.RenderDeviceD3D11(deviceContext.NativePointer, sRGB: false);
                _guiView.Renderer.Init(guiDevice);
            }

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
            /*
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                (Keyboard.GetState().IsKeyDown(Keys.Escape)) )
            {
                Exit();
            }
            */
            
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
                
                // Consider Keyboard events 
                var pressedKeys = Keyboard.GetState().GetPressedKeys();
                
                foreach (var key in pressedKeys.Concat(_lastPressedKeys.Keys))
                {
                    if (pressedKeys.Contains(key) && !_lastPressedKeys.ContainsKey(key))
                    {
                        ProcessKeyPressed(key, gameTime.TotalGameTime, gameTime);
                    }
                    else if (!pressedKeys.Contains(key) && _lastPressedKeys.ContainsKey(key))
                    {
                        ProcessKeyReleased(key);
                    }
                    else if ((gameTime.TotalGameTime - _lastPressedKeys[key]) > _keyRepeatInterval)
                    {
                        ProcessKeyPressed(key, gameTime.TotalGameTime, gameTime);
                    }
                }

            }

            if (_model.State == GameModel.States.Exit)
            {
                Exit();
            }
            
            _viewModel.Update(gameTime);
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


        private void ProcessKeyPressed(Keys key, TimeSpan keyProcessingTime, GameTime gameTime)
        {
            var processed = false;
            
            if (key is (Keys.Up or Keys.Down or Keys.Right or Keys.Left))
            {
                if (_model.State == GameModel.States.Running)
                {
                    GameModel.Move.Direction direction = GameModel.Move.Direction.Up;
                    switch (key)
                    {
                        case Keys.Up:
                            direction = GameModel.Move.Direction.Up;
                            break;
                        case Keys.Down:
                            direction = GameModel.Move.Direction.Down;
                            break;
                        case Keys.Right:
                            direction = GameModel.Move.Direction.Right;
                            break;
                        case Keys.Left:
                            direction = GameModel.Move.Direction.Left;
                            break;
                    }
                    _model.Trigger(new GameModel.Move(direction, gameTime));
                    processed = true;
                    _lastPressedKeys[key] = keyProcessingTime - _keyRepeatInterval;
                }
            }

            if (!processed)
            {
                var noesisKey = ConvertKey(key);
                if (noesisKey != Noesis.Key.None)
                {
                    Console.WriteLine($"{key}: {noesisKey}");
                    _guiView.KeyDown(noesisKey);
                    processed = true;
                    _lastPressedKeys[key] = keyProcessingTime;
                }
            }
        }

        
        private void ProcessKeyReleased(Keys key)
        {
            _lastPressedKeys.Remove(key);
            var processed = false;
            
            if (key == Keys.Escape)
            {
                if (_model.State == GameModel.States.Running)
                {
                    _model.Trigger(new GameModel.Pause());
                    processed = true;
                }
                else if (_model.State == GameModel.States.Pause)
                {
                    _model.Trigger(new GameModel.Start());
                    processed = true;
                }
            }

            if (!processed)
            {
                var noesisKey = ConvertKey(key);
                if (noesisKey != Noesis.Key.None)
                {
                    _guiView.KeyUp(noesisKey);
                }
            }
        }

        private static Noesis.Key ConvertKey(Keys key)
        {
            switch (key)
            {
                case Keys.Back: return Noesis.Key.Back;
                case Keys.Tab: return Noesis.Key.Tab;
                case Keys.Enter: return Noesis.Key.Enter;
                case Keys.Pause: return Noesis.Key.Pause;
                case Keys.CapsLock: return Noesis.Key.CapsLock;
                case Keys.Kana: return Noesis.Key.KanaMode;
                case Keys.Kanji: return Noesis.Key.KanjiMode;
                case Keys.Escape: return Noesis.Key.Escape;
                case Keys.ImeConvert: return Noesis.Key.ImeConvert;
                case Keys.ImeNoConvert: return Noesis.Key.ImeNonConvert;
                case Keys.Space: return Noesis.Key.Space;
                case Keys.PageUp: return Noesis.Key.PageUp;
                case Keys.PageDown: return Noesis.Key.PageDown;
                case Keys.End: return Noesis.Key.End;
                case Keys.Home: return Noesis.Key.Home;
                case Keys.Left: return Noesis.Key.Left;
                case Keys.Up: return Noesis.Key.Up;
                case Keys.Right: return Noesis.Key.Right;
                case Keys.Down: return Noesis.Key.Down;
                case Keys.Select: return Noesis.Key.Select;
                case Keys.Print: return Noesis.Key.Print;
                case Keys.Execute: return Noesis.Key.Execute;
                case Keys.PrintScreen: return Noesis.Key.PrintScreen;
                case Keys.Insert: return Noesis.Key.Insert;
                case Keys.Delete: return Noesis.Key.Delete;
                case Keys.Help: return Noesis.Key.Help;
                case Keys.D0: return Noesis.Key.D0;
                case Keys.D1: return Noesis.Key.D1;
                case Keys.D2: return Noesis.Key.D2;
                case Keys.D3: return Noesis.Key.D3;
                case Keys.D4: return Noesis.Key.D4;
                case Keys.D5: return Noesis.Key.D5;
                case Keys.D6: return Noesis.Key.D6;
                case Keys.D7: return Noesis.Key.D7;
                case Keys.D8: return Noesis.Key.D8;
                case Keys.D9: return Noesis.Key.D9;
                case Keys.A: return Noesis.Key.A;
                case Keys.B: return Noesis.Key.B;
                case Keys.C: return Noesis.Key.C;
                case Keys.D: return Noesis.Key.D;
                case Keys.E: return Noesis.Key.E;
                case Keys.F: return Noesis.Key.F;
                case Keys.G: return Noesis.Key.G;
                case Keys.H: return Noesis.Key.H;
                case Keys.I: return Noesis.Key.I;
                case Keys.J: return Noesis.Key.J;
                case Keys.K: return Noesis.Key.K;
                case Keys.L: return Noesis.Key.L;
                case Keys.M: return Noesis.Key.M;
                case Keys.N: return Noesis.Key.N;
                case Keys.O: return Noesis.Key.O;
                case Keys.P: return Noesis.Key.P;
                case Keys.Q: return Noesis.Key.Q;
                case Keys.R: return Noesis.Key.R;
                case Keys.S: return Noesis.Key.S;
                case Keys.T: return Noesis.Key.T;
                case Keys.U: return Noesis.Key.U;
                case Keys.V: return Noesis.Key.V;
                case Keys.W: return Noesis.Key.W;
                case Keys.X: return Noesis.Key.X;
                case Keys.Y: return Noesis.Key.Y;
                case Keys.Z: return Noesis.Key.Z;
                case Keys.LeftWindows: return Noesis.Key.LWin;
                case Keys.RightWindows: return Noesis.Key.RWin;
                case Keys.Apps: return Noesis.Key.Apps;
                case Keys.Sleep: return Noesis.Key.Sleep;
                case Keys.NumPad0: return Noesis.Key.NumPad0;
                case Keys.NumPad1: return Noesis.Key.NumPad1;
                case Keys.NumPad2: return Noesis.Key.NumPad2;
                case Keys.NumPad3: return Noesis.Key.NumPad3;
                case Keys.NumPad4: return Noesis.Key.NumPad4;
                case Keys.NumPad5: return Noesis.Key.NumPad5;
                case Keys.NumPad6: return Noesis.Key.NumPad6;
                case Keys.NumPad7: return Noesis.Key.NumPad7;
                case Keys.NumPad8: return Noesis.Key.NumPad8;
                case Keys.NumPad9: return Noesis.Key.NumPad9;
                case Keys.Multiply: return Noesis.Key.Multiply;
                case Keys.Add: return Noesis.Key.Add;
                case Keys.Separator: return Noesis.Key.Separator;
                case Keys.Subtract: return Noesis.Key.Subtract;
                case Keys.Decimal: return Noesis.Key.Decimal;
                case Keys.Divide: return Noesis.Key.Divide;
                case Keys.F1: return Noesis.Key.F1;
                case Keys.F2: return Noesis.Key.F2;
                case Keys.F3: return Noesis.Key.F3;
                case Keys.F4: return Noesis.Key.F4;
                case Keys.F5: return Noesis.Key.F5;
                case Keys.F6: return Noesis.Key.F6;
                case Keys.F7: return Noesis.Key.F7;
                case Keys.F8: return Noesis.Key.F8;
                case Keys.F9: return Noesis.Key.F9;
                case Keys.F10: return Noesis.Key.F10;
                case Keys.F11: return Noesis.Key.F11;
                case Keys.F12: return Noesis.Key.F12;
                case Keys.F13: return Noesis.Key.F13;
                case Keys.F14: return Noesis.Key.F14;
                case Keys.F15: return Noesis.Key.F15;
                case Keys.F16: return Noesis.Key.F16;
                case Keys.F17: return Noesis.Key.F17;
                case Keys.F18: return Noesis.Key.F18;
                case Keys.F19: return Noesis.Key.F19;
                case Keys.F20: return Noesis.Key.F20;
                case Keys.F21: return Noesis.Key.F21;
                case Keys.F22: return Noesis.Key.F22;
                case Keys.F23: return Noesis.Key.F23;
                case Keys.F24: return Noesis.Key.F24;
                case Keys.NumLock: return Noesis.Key.NumLock;
                case Keys.Scroll: return Noesis.Key.Scroll;
                case Keys.LeftShift: return Noesis.Key.LeftShift;
                case Keys.RightShift: return Noesis.Key.RightShift;
                case Keys.LeftControl: return Noesis.Key.LeftCtrl;
                case Keys.RightControl: return Noesis.Key.RightCtrl;
                case Keys.LeftAlt: return Noesis.Key.LeftAlt;
                case Keys.RightAlt: return Noesis.Key.RightAlt;
                case Keys.BrowserBack: return Noesis.Key.BrowserBack;
                case Keys.BrowserForward: return Noesis.Key.BrowserForward;
                case Keys.BrowserRefresh: return Noesis.Key.BrowserRefresh;
                case Keys.BrowserStop: return Noesis.Key.BrowserStop;
                case Keys.BrowserSearch: return Noesis.Key.BrowserSearch;
                case Keys.BrowserFavorites: return Noesis.Key.BrowserFavorites;
                case Keys.BrowserHome: return Noesis.Key.BrowserHome;
                case Keys.VolumeMute: return Noesis.Key.VolumeMute;
                case Keys.VolumeDown: return Noesis.Key.VolumeDown;
                case Keys.VolumeUp: return Noesis.Key.VolumeUp;
                case Keys.MediaNextTrack: return Noesis.Key.MediaNextTrack;
                case Keys.MediaPreviousTrack: return Noesis.Key.MediaPreviousTrack;
                case Keys.MediaStop: return Noesis.Key.MediaStop;
                case Keys.MediaPlayPause: return Noesis.Key.MediaPlayPause;
                case Keys.LaunchMail: return Noesis.Key.LaunchMail;
                case Keys.SelectMedia: return Noesis.Key.SelectMedia;
                case Keys.LaunchApplication1: return Noesis.Key.LaunchApplication1;
                case Keys.LaunchApplication2: return Noesis.Key.LaunchApplication2;
                case Keys.OemSemicolon: return Noesis.Key.OemSemicolon;
                case Keys.OemPlus: return Noesis.Key.OemPlus;
                case Keys.OemComma: return Noesis.Key.OemComma;
                case Keys.OemMinus: return Noesis.Key.OemMinus;
                case Keys.OemPeriod: return Noesis.Key.OemPeriod;
                case Keys.OemQuestion: return Noesis.Key.OemQuestion;
                case Keys.OemTilde: return Noesis.Key.OemTilde;
                case Keys.OemOpenBrackets: return Noesis.Key.OemOpenBrackets;
                case Keys.OemPipe: return Noesis.Key.OemPipe;
                case Keys.OemCloseBrackets: return Noesis.Key.OemCloseBrackets;
                case Keys.OemQuotes: return Noesis.Key.OemQuotes;
                case Keys.Oem8: return Noesis.Key.Oem8;
                case Keys.OemBackslash: return Noesis.Key.OemBackslash;
                case Keys.ProcessKey: return Noesis.Key.ImeProcessed;
                case Keys.OemCopy: return Noesis.Key.OemCopy;
                case Keys.OemAuto: return Noesis.Key.OemAuto;
                case Keys.OemEnlW: return Noesis.Key.OemEnlw;
                case Keys.Attn: return Noesis.Key.Attn;
                case Keys.Crsel: return Noesis.Key.CrSel;
                case Keys.Exsel: return Noesis.Key.ExSel;
                case Keys.EraseEof: return Noesis.Key.EraseEof;
                case Keys.Play: return Noesis.Key.Play;
                case Keys.Zoom: return Noesis.Key.Zoom;
                case Keys.Pa1: return Noesis.Key.Pa1;
                case Keys.OemClear: return Noesis.Key.OemClear;
                default: return Noesis.Key.None;
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            {
                using var renderState = new D3X11RenderState(GraphicsDevice);
                _guiView.Renderer.UpdateRenderTree();
                _guiView.Renderer.RenderOffscreen();
            }

            if (_model.State is (GameModel.States.Running or GameModel.States.Pause))
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    _ballTexture,
                    _model.BallPosition,
                    null,
                    Color.White,
                    0f,
                    new Vector2(_ballTexture.Width / 2f, _ballTexture.Height / 2f),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                    );
                _spriteBatch.End();
            }

            {
                using var renderState = new D3X11RenderState(GraphicsDevice);
                _guiView.Renderer.Render();
            }

            base.Draw(gameTime);
        }

    }
}