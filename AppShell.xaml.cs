namespace FrontaPrace.Apps;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Views.FilterPage), typeof(Views.FilterPage));
        Routing.RegisterRoute(nameof(Views.RfidCheckPage), typeof(Views.RfidCheckPage));
    }
}