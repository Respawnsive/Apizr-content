using Cellar.app.Views;

namespace Cellar.app;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(WineDetailsPage), typeof(WineDetailsPage));
        Routing.RegisterRoute(nameof(WineEditPage), typeof(WineEditPage));
}
}