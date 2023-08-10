using Cellar.app.ViewModels;

namespace Cellar.app.Views;

public partial class WineEditPage : ContentPage
{
	public WineEditPage(WineEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}