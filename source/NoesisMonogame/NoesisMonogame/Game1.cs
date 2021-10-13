using System;
using System.Collections.Generic;
using System.IO;
using Data.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game = Microsoft.Xna.Framework.Game;


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
        private readonly ViewModel _viewModel;
        private readonly GameInputHandler _inputHandler;
        
        
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

            _inputHandler = new GameInputHandler(_model);
            
            Content.RootDirectory = "Data";
            IsMouseVisible = true;
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
                _gui.MouseInputHandler.Update(gameTime);
                
                // Consider Keyboard events 
                var pressedKeys = Keyboard.GetState().GetPressedKeys();
                pressedKeys = _gui.PriorityKeyboardInputHandler.ProcessKeys(pressedKeys, gameTime);
                pressedKeys = _inputHandler.ProcessKeys(pressedKeys, gameTime);
                _gui.KeyboardInputHandler.ProcessKeys(pressedKeys, gameTime);

            }

            if (_model.State == GameModel.States.Exit)
            {
                Exit();
            }

            _viewModel.Update(gameTime);
            _gui.Update(gameTime.TotalGameTime);
            
            base.Update(gameTime);
        }

        
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