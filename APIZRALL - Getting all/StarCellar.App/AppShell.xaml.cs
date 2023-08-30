using StarCellar.App.Views;

namespace StarCellar.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(WineDetailsPage), typeof(WineDetailsPage));
        Routing.RegisterRoute(nameof(WineEditPage), typeof(WineEditPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Default.Clear();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}