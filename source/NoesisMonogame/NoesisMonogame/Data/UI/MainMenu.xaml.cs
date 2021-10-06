using System;
using Noesis;
using System.Windows.Input;
using NoesisApp;


namespace Data.UI
{
    public class MainMenu : UserControl
    {
        public MainMenu()
        {
            GUI.LoadComponent(this, "MainMenu.xaml");
        }
    }
}