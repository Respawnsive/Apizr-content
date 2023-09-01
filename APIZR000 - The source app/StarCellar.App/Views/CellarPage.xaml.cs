using StarCellar.App.ViewModels;

namespace StarCellar.App.Views;

public partial class CellarPage : ContentPage
{
	public CellarPage(CellarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

