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

    }
}