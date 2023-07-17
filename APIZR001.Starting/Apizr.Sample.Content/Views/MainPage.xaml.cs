using Apizr.Sample.Content.ViewModels;

namespace Apizr.Sample.Content.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}

