using System;
using Noesis;
using System.Windows.Input;
using NoesisApp;


namespace Data.UI
{
    public class PauseMenu : UserControl
    {
        public PauseMenu()
        {
            GUI.LoadComponent(this, "PauseMenu.xaml");
        }
    }
}