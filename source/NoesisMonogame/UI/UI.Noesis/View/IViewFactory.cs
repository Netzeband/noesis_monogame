using Noesis;


namespace UI.Noesis.View
{
    public interface IViewFactory
    {
        /// <summary>
        /// Loads the view element.
        /// </summary>
        /// <returns>Returns the loaded view element.</returns>
        FrameworkElement LoadView();
    }
}