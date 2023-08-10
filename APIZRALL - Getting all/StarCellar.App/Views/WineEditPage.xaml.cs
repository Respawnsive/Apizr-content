using StarCellar.App.ViewModels;

namespace StarCellar.App.Views;

public partial class WineEditPage : ContentPage
{
	public WineEditPage(WineEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}