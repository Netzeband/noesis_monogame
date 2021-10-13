using Microsoft.Xna.Framework;
using NoesisLib = Noesis;


namespace UI.Noesis.Input
{
    public class NullNoesisMouseInputHandler : INoesisMouseInputHandler
    {
        public void Init(NoesisLib.View view)
        {
        }

        public void UnInit()
        {
        }

        public void PrepareProcessing()
        {
        }

        public bool ProcessMouseMove(int x, int y)
        {
            return false;
        }

        public bool ProcessMouseWheel(int wheel)
        {
            return false;
        }

        public bool ProcessButtonPressed(UI.Input.MouseButtons button)
        {
            return false;
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}