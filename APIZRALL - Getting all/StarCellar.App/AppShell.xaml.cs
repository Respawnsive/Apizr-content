using StarCellar.App.Views;

namespace StarCellar.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(WineDetailsPage), typeof(WineDetailsPage));
        Routing.RegisterRoute(nameof(WineEditPage), typeof(WineEditPage));
}
}