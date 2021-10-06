using Noesis;
using System;

namespace Data.UI
{
    public class Window : UserControl
    {
        public Window(ViewModel viewModel)
        {
            GUI.LoadComponent(this, "Window.xaml");

            var reference = new WeakReference(this);
            Loaded += (s, e) => (reference.Target as Window)?.OnLoaded(s, e, viewModel);
        }
        
        private void OnLoaded(object sender, Noesis.EventArgs e, ViewModel viewModel)
        {
            Console.WriteLine("[Info] Setup data context for main window.");
            DataContext = viewModel;
        }
    }
}