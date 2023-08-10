using Cellar.app.ViewModels;

namespace Cellar.app.Views;

public partial class WineDetailsPage : ContentPage
{
	public WineDetailsPage(WineDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}