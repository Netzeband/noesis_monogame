using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace NoesisMonogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
//            _graphics.GraphicsProfile = GraphicsProfile.HiDef; // ToDo: Why?

            Content.RootDirectory = "Data";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // initialize your code here ...
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // load any content here ...
        }

        protected override void UnloadContent()
        {
            // unload content here if necessary ...
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape)) )
            {
                Exit();
            }
            
            // update your game state here ...
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw your game state here ...
            
            base.Draw(gameTime);
        }

    }
}