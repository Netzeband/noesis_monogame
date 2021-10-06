using Noesis;

namespace Data.UI
{
    public class Game : UserControl
    {
        public Game()
        {
            GUI.LoadComponent(this, "Game.xaml");
        }
    }
}