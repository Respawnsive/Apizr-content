using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    [RelayCommand]
    public async Task LoginAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(CellarPage)}");
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}