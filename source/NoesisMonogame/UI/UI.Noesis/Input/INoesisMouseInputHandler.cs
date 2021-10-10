using UI.Input;
using NoesisLib = Noesis;

namespace UI.Noesis.Input
{
    public interface INoesisMouseInputHandler : IMouseInputHandler
    {
        /// <summary>
        /// Sets the view for the input handler. 
        /// </summary>
        /// <param name="view">The view, where the input should be processed on.</param>
        void Init(NoesisLib.View view);

        /// <summary>
        /// Unsets the input handler.
        /// </summary>
        void UnInit();
    }
}