using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


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
        private readonly UI.IGUI _gui;
        
        private Texture2D _ballTexture;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly GameModel _model;
        private Data.UI.ViewModel _viewModel;
        private MouseState _lastMouseState;
        private bool _wasScrolledByMouse;
        private readonly Dictionary<Noesis.MouseButton, TimeSpan> _lastMouseClickTime = new();
        private readonly TimeSpan _doubleClickInterval = TimeSpan.FromMilliseconds(250);
        private readonly Dictionary<Keys, TimeSpan> _lastPressedKeys = new(KeysComparer.Instance);
        private readonly TimeSpan _keyRepeatInterval = TimeSpan.FromMilliseconds(250);
        
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 840;

            _model = new GameModel(
                new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)
            );
            _viewModel = new Data.UI.ViewModel(_model);

            var rootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Data/UI"));
            var providerManager = new UI.Noesis.Provider.ProviderManager(
                new UI.Noesis.Provider.XamlProvider(rootPath, new UI.Noesis.Provider.ReloadProvider()),
                new UI.Noesis.Provider.FontProvider(rootPath),
                new UI.Noesis.Provider.TextureProvider(rootPath)
                );
            _gui = new UI.Noesis.NoesisGUI(
                new NoesisLicense(),
                new UI.Logging.ConsoleLogger(),
                providerManager,
                new Data.UI.WindowFactory(_viewModel),
                new UI.Noesis.DirectX.Renderer.RenderDeviceFactory(),
                new UI.Noesis.Input.NoesisMouseInputHandler(),
                new UI.Noesis.Input.NullNoesisKeyboardInputHandler(),
                new UI.Noesis.Input.NoesisKeyboardInputHandler(),
                theme: "Theme/NoesisTheme.DarkBlue.xaml"
                );
            
            Content.RootDirectory = "Data";
            IsMouseVisible = true;
            _wasScrolledByMouse = false;
        }
        
        
        protected override void Initialize()
        {
            _gui.Init();
            
            base.Initialize();
        }
        

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ballTexture = Content.Load<Texture2D>("ball");
            
            _gui.Load(_graphics);
        }
        
        
        protected override void UnloadContent()
        {
            _gui.Unload();
            
            // unload content here if necessary ...
        }

        protected override void Update(GameTime gameTime)
        {
            // update your game state here ...

            if (IsActive)
            {
                var mouseState = Mouse.GetState();

                _gui.MouseInputHandler.PrepareProcessing();
                _gui.MouseInputHandler.ProcessMouseMove(mouseState.X, mouseState.Y);
                _gui.MouseInputHandler.ProcessMouseWheel(mouseState.ScrollWheelValue);
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _gui.MouseInputHandler.ProcessButtonPressed(UI.Input.MouseButtons.Left);
                }
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _gui.MouseInputHandler.ProcessButtonPressed(UI.Input.MouseButtons.Right);
                }
                if (mouseState.MiddleButton == ButtonState.Pressed)
                {
                    _gui.MouseInputHandler.ProcessButtonPressed(UI.Input.MouseButtons.Middle);
                }
                _gui.MouseInputHandler.Update(gameTime.TotalGameTime);
                
                // Consider Keyboard events 
                var pressedKeys = Keyboard.GetState().GetPressedKeys();
                pressedKeys = _gui.PriorityKeyboardInputHandler.ProcessKeys(pressedKeys, gameTime.TotalGameTime);
                if (pressedKeys.Length > 0)
                {
                    Console.WriteLine(pressedKeys);
                }
                _gui.KeyboardInputHandler.ProcessKeys(pressedKeys, gameTime.TotalGameTime);

            }

            if (_model.State == GameModel.States.Exit)
            {
                Exit();
            }

            _gui.Update(gameTime.TotalGameTime);
            _viewModel.Update(gameTime);
            
            base.Update(gameTime);
        }

        
        /*
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

        */
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _gui.PreRender();
            
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

            _gui.Render();
            base.Draw(gameTime);
        }

    }
}