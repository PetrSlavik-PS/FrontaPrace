using Microsoft.Maui.Controls;

namespace FrontaPrace.Apps;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}