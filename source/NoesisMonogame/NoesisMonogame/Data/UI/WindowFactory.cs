using System.Diagnostics;
using Noesis;
using UI.Noesis.View;

namespace Data.UI
{
    public class WindowFactory : IViewFactory
    {
        private readonly ViewModel _viewModel;

        public WindowFactory(ViewModel viewModel)
        {
            _viewModel = viewModel;
            
            Debug.Assert(_viewModel != null);
        }
        
        public FrameworkElement LoadView()
        {
            return new Window(_viewModel);
        }
    }
}