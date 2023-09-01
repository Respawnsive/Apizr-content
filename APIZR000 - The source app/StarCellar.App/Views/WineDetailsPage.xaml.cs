using StarCellar.App.ViewModels;

namespace StarCellar.App.Views;

public partial class WineDetailsPage : ContentPage
{
	public WineDetailsPage(WineDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}