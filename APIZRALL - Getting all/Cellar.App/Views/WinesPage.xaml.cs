using Cellar.app.ViewModels;

namespace Cellar.app.Views;

public partial class WinesPage : ContentPage
{
	public WinesPage(WinesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

